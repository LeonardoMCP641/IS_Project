using System.Linq;
using System.Web.Http;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Resources;

namespace WebAPI.Controllers
{
    [RoutePrefix("applications/{appName}/containers/{containerName}/subscriptions")]
    public class SubscriptionsController : ApiController
    {
        // GET /applications/{app}/containers/{container}/subscriptions
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetSubscriptions(string appName, string containerName)
        {
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            var subs = SubscriptionSQLHelper.GetSubscriptions(container.Id)
                .Select(s => new SubscriptionResource
                {
                    resource_name = s.ResourceName,
                    evt = s.Evt,
                    endpoint = s.Endpoint,
                    creation_datetime = s.CreationDateTime
                        .ToString("yyyy-MM-ddTHH:mm:ss")
                });

            return Ok(subs);
        }

        // POST /applications/{app}/containers/{container}/subscriptions
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateSubscription(
            string appName,
            string containerName,
            [FromBody] SubscriptionResource resource)
        {
            // 🔴 Enunciado: validar TODOS os campos
            if (resource == null ||
                string.IsNullOrWhiteSpace(resource.resource_name) ||
                string.IsNullOrWhiteSpace(resource.endpoint))
            {
                return BadRequest("resource_name and endpoint are required.");
            }

            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            var sub = SubscriptionSQLHelper.CreateSubscription(
                new Subscription
                {
                    ResourceName = resource.resource_name,
                    Evt = resource.evt,
                    Endpoint = resource.endpoint,
                    ContainerId = container.Id
                });

            if (sub == null)
                return BadRequest("Could not create subscription.");

            return Created("", new SubscriptionResource
            {
                resource_name = sub.ResourceName,
                evt = sub.Evt,
                endpoint = sub.Endpoint,
                creation_datetime = sub.CreationDateTime
                    .ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }

        // DELETE /applications/{app}/containers/{container}/subscriptions/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteSubscription(
            string appName,
            string containerName,
            int id)
        {
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            var deleted = SubscriptionSQLHelper.DeleteSubscription(id);
            if (deleted == null)
                return NotFound();

            return Ok();
        }
    }
}
