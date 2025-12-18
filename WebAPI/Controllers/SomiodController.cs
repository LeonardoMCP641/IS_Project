using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SomiodController : ApiController
    {
        private readonly SomiodDbContext db = new SomiodDbContext();
        private readonly MqttNotificationService mqtt = new MqttNotificationService();

        // =========================================================
        // Helpers
        // =========================================================

        private string NowIso()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        private string GenerateUniqueAppName()
        {
            string name;
            do
            {
                name = $"app-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }
            while (db.Applications.Any(a => a.ResourceName == name));
            return name;
        }

        private string GenerateUniqueContainerName()
        {
            string name;
            do
            {
                name = $"cont-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }
            while (db.Containers.Any(c => c.ResourceName == name));
            return name;
        }

        private string GenerateUniqueCiName()
        {
            string name;
            do
            {
                name = $"ci-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }
            while (db.ContentInstances.Any(ci => ci.ResourceName == name));
            return name;
        }

        // =========================================================
        // DISCOVERY FROM ROOT
        // =========================================================

        [HttpGet, Route("")]
        public IHttpActionResult DiscoverFromRoot()
        {
            if (!Request.Headers.Contains("somiod-discovery"))
                return BadRequest();

            var value = Request.Headers.GetValues("somiod-discovery").First();

            if (value.Equals("application", StringComparison.OrdinalIgnoreCase))
            {
                var paths = db.Applications
                    .Select(a => "/api/somiod/" + a.ResourceName)
                    .ToList();

                return Ok(paths);
            }

            if (value.Equals("content-instance", StringComparison.OrdinalIgnoreCase))
            {
                var paths =
                    (from ci in db.ContentInstances
                     join c in db.Containers on ci.ContainerId equals c.Id
                     join a in db.Applications on c.ApplicationId equals a.Id
                     select new
                     {
                         AppRn = a.ResourceName,
                         ContRn = c.ResourceName,
                         CiRn = ci.ResourceName
                     })
                    .AsEnumerable()
                    .Select(x => $"/api/somiod/{x.AppRn}/{x.ContRn}/{x.CiRn}")
                    .ToList();

                return Ok(paths);
            }
            return BadRequest("Invalid somiod-discovery value");
        }

        // =========================================================
        // APPLICATIONS
        // =========================================================

        [HttpPost, Route("")]
        public IHttpActionResult CreateApplication([FromBody] ApplicationCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var name = string.IsNullOrWhiteSpace(dto.ResourceName)
                ? GenerateUniqueAppName()
                : dto.ResourceName.Trim();

            if (db.Applications.Any(a => a.ResourceName == name))
                return Conflict();

            var app = new Application
            {
                ResourceName = name,
                CreationDateTime = NowIso()
            };

            db.Applications.Add(app);
            db.SaveChanges();

            return Content(HttpStatusCode.Created, app);
        }

        [HttpGet, Route("{appRn}")]
        public IHttpActionResult GetApplicationOrDiscover(string appRn)
        {
            var app = db.Applications.FirstOrDefault(a => a.ResourceName == appRn);
            if (app == null) return NotFound();

            if (!Request.Headers.Contains("somiod-discovery"))
                return Ok(app);

            var value = Request.Headers.GetValues("somiod-discovery").First();

            if (value.Equals("container", StringComparison.OrdinalIgnoreCase))
            {
                var paths = db.Containers
                    .Where(c => c.ApplicationId == app.Id)
                    .Select(c => c.ResourceName)
                    .AsEnumerable()     
                    .Select(rn => $"/api/somiod/{appRn}/{rn}")
                    .ToList();

                return Ok(paths);
            }
            if (value.Equals("content-instance", StringComparison.OrdinalIgnoreCase))
            {
                var paths =
                    (from ci in db.ContentInstances
                     join c in db.Containers on ci.ContainerId equals c.Id
                     where c.ApplicationId == app.Id
                     select new
                     {
                         ContainerRn = c.ResourceName,
                         CiRn = ci.ResourceName
                     })
                    .AsEnumerable()
                    .Select(x => $"/api/somiod/{appRn}/{x.ContainerRn}/{x.CiRn}")
                    .ToList();

                return Ok(paths);
            }
            if (value.Equals("subscription", StringComparison.OrdinalIgnoreCase))
            {
                var paths =
                     (from s in db.Subscriptions
                      join c in db.Containers on s.ContainerId equals c.Id
                      join a in db.Applications on c.ApplicationId equals a.Id
                      where a.ResourceName == appRn
                      select new
                      {
                          AppRn = a.ResourceName,
                          ContRn = c.ResourceName,
                          SubRn = s.ResourceName
                      })
                     .AsEnumerable()  
                     .Select(x => $"/api/somiod/{x.AppRn}/{x.ContRn}/subs/{x.SubRn}")
                     .ToList();


                return Ok(paths);
            }



            return BadRequest("Invalid somiod-discovery value");
        }
        [HttpPut, Route("{appRn}")]
        public IHttpActionResult UpdateApplication(
            string appRn,
            [FromBody] ApplicationUpdateDto dto)
        {
            var app = db.Applications.FirstOrDefault(a => a.ResourceName == appRn);
            if (app == null) return NotFound();

            if (dto == null) return BadRequest("Body is required");

            if (!string.IsNullOrWhiteSpace(dto.ResType) &&
                !dto.ResType.Equals("application", StringComparison.OrdinalIgnoreCase))
                return BadRequest("res-type must be 'application'");

            if (!string.IsNullOrWhiteSpace(dto.ResourceName) &&
                dto.ResourceName != app.ResourceName)
            {
                if (db.Applications.Any(a => a.ResourceName == dto.ResourceName))
                    return Conflict();

                app.ResourceName = dto.ResourceName.Trim();
            }

            db.SaveChanges();
            return Ok(app);
        }

        [HttpDelete, Route("{appRn}")]
        public IHttpActionResult DeleteApplication(string appRn)
        {
            var app = db.Applications.FirstOrDefault(a => a.ResourceName == appRn);
            if (app == null) return NotFound();

            db.Applications.Remove(app);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }


        // =========================================================
        // CONTAINERS
        // =========================================================

        [HttpPost, Route("{appRn}")]
        public IHttpActionResult CreateContainer(string appRn, [FromBody] ContainerCreateDto dto)
        {
            var app = db.Applications.FirstOrDefault(a => a.ResourceName == appRn);
            if (app == null) return NotFound();

            var name = string.IsNullOrWhiteSpace(dto.ResourceName)
                ? GenerateUniqueContainerName()
                : dto.ResourceName.Trim();

            if (db.Containers.Any(c => c.ResourceName == name))
                return Conflict();

            var cont = new Container
            {
                ResourceName = name,
                CreationDateTime = NowIso(),
                ApplicationId = app.Id
            };

            db.Containers.Add(cont);
            db.SaveChanges();

            return Content(HttpStatusCode.Created, cont);
        }

        [HttpGet, Route("{appRn}/{contRn}")]
        public IHttpActionResult GetContainer(string appRn, string contRn)
        {
            var cont = db.Containers.FirstOrDefault(c =>
                c.ResourceName == contRn &&
                c.Application.ResourceName == appRn);

            if (cont == null) return NotFound();
            return Ok(cont);
        }

        // UPDATE CONTAINER
        // PUT /api/somiod/{appRn}/{contRn}
        [HttpPut, Route("{appRn}/{contRn}")]
        public IHttpActionResult UpdateContainer(
            string appRn,
            string contRn,
            [FromBody] ContainerUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Body is required");

            if (!string.IsNullOrWhiteSpace(dto.ResType) &&
                !dto.ResType.Equals("container", StringComparison.OrdinalIgnoreCase))
                return BadRequest("res-type must be 'container'");

            var cont =
                (from c in db.Containers
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn
                    && c.ResourceName == contRn
                 select c)
                .FirstOrDefault();

            if (cont == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.ResourceName) &&
                dto.ResourceName != cont.ResourceName)
            {
                if (db.Containers.Any(c => c.ResourceName == dto.ResourceName))
                    return Conflict();

                cont.ResourceName = dto.ResourceName.Trim();
            }

            db.SaveChanges();
            return Ok(cont);
        }

        // DELETE CONTAINER
        // DELETE /api/somiod/{appRn}/{contRn}
        [HttpDelete, Route("{appRn}/{contRn}")]
        public IHttpActionResult DeleteContainer(string appRn, string contRn)
        {
            var cont =
                (from c in db.Containers
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn
                    && c.ResourceName == contRn
                 select c)
                .FirstOrDefault();

            if (cont == null)
                return NotFound();

            db.Containers.Remove(cont);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }



        // =========================================================
        // CONTENT-INSTANCES (CORRIGIDO)
        // =========================================================

        [HttpPost, Route("{appRn}/{contRn}")]
        public IHttpActionResult CreateContentInstance(
            string appRn,
            string contRn,
            [FromBody] ContentInstanceCreateDto dto)
        {
            var cont =
                (from c in db.Containers
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn && c.ResourceName == contRn
                 select c)
                .FirstOrDefault();

            if (cont == null) return NotFound();

            var name = string.IsNullOrWhiteSpace(dto.ResourceName)
                ? GenerateUniqueCiName()
                : dto.ResourceName.Trim();

            var ci = new ContentInstance
            {
                ResourceName = name,
                ContentType = dto.ContentType,
                Content = dto.Content,
                CreationDateTime = NowIso(),
                ContainerId = cont.Id
            };

            db.ContentInstances.Add(ci);
            db.SaveChanges();

            mqtt.NotifyContentInstanceEvent(appRn,contRn,ci.ResourceName,ci.ContentType,ci.Content,1);


            return Content(HttpStatusCode.Created, ci);
        }

        [HttpGet, Route("{appRn}/{contRn}/{ciRn}")]
        public IHttpActionResult GetContentInstance(string appRn, string contRn, string ciRn)
        {
            var ci =
                (from x in db.ContentInstances
                 join c in db.Containers on x.ContainerId equals c.Id
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn
                    && c.ResourceName == contRn
                    && x.ResourceName == ciRn
                 select x)
                .FirstOrDefault();

            if (ci == null) return NotFound();
            return Ok(ci);
        }

        [HttpDelete, Route("{appRn}/{contRn}/{ciRn}")]
        public IHttpActionResult DeleteContentInstance(string appRn, string contRn, string ciRn)
        {
            var ci =
                (from x in db.ContentInstances
                 join c in db.Containers on x.ContainerId equals c.Id
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn
                    && c.ResourceName == contRn
                    && x.ResourceName == ciRn
                 select x)
                .FirstOrDefault();

            if (ci == null) return NotFound();

            db.ContentInstances.Remove(ci);
            db.SaveChanges();

            mqtt.NotifyContentInstanceEvent(appRn,contRn,ci.ResourceName,ci.ContentType,ci.Content,2);


            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost, Route("{appRn}/{contRn}/subs")]
        public IHttpActionResult CreateSubscription(string appRn,string contRn,[FromBody] SubscriptionCreateDto dto)
        {
            if (dto == null) return BadRequest();

            if (!dto.ResType.Equals("subscription", StringComparison.OrdinalIgnoreCase))
                return BadRequest("res-type must be subscription");

            if (dto.Evt != 1 && dto.Evt != 2)
                return BadRequest("evt must be 1 or 2");

            var cont =
                (from c in db.Containers
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn && c.ResourceName == contRn
                 select c).FirstOrDefault();

            if (cont == null) return NotFound();

            var name = string.IsNullOrWhiteSpace(dto.ResourceName)
                ? $"sub-{Guid.NewGuid():N}".Substring(0, 12)
                : dto.ResourceName.Trim();

            if (db.Subscriptions.Any(s => s.ResourceName == name))
                return Conflict();

            var sub = new Subscription
            {
                ResourceName = name,
                Evt = dto.Evt,
                Endpoint = dto.Endpoint,
                CreationDateTime = NowIso(),
                ContainerId = cont.Id
            };

            db.Subscriptions.Add(sub);
            db.SaveChanges();

            return Content(HttpStatusCode.Created, sub);
        }

        [HttpGet, Route("{appRn}/{contRn}/subs/{subRn}")]
        public IHttpActionResult GetSubscription(string appRn, string contRn, string subRn)
        {
            var sub =
                (from s in db.Subscriptions
                 join c in db.Containers on s.ContainerId equals c.Id
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn
                    && c.ResourceName == contRn
                    && s.ResourceName == subRn
                 select s).FirstOrDefault();

            if (sub == null) return NotFound();
            return Ok(sub);
        }

        [HttpDelete, Route("{appRn}/{contRn}/subs/{subRn}")]
        public IHttpActionResult DeleteSubscription(string appRn, string contRn, string subRn)
        {
            var sub =
                (from s in db.Subscriptions
                 join c in db.Containers on s.ContainerId equals c.Id
                 join a in db.Applications on c.ApplicationId equals a.Id
                 where a.ResourceName == appRn
                    && c.ResourceName == contRn
                    && s.ResourceName == subRn
                 select s).FirstOrDefault();

            if (sub == null) return NotFound();

            db.Subscriptions.Remove(sub);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
