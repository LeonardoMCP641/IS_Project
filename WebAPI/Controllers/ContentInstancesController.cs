using System;
using System.Linq;
using System.Web.Http;
using WebAPI.Helpers;
using WebAPI.Resources;

namespace WebAPI.Controllers
{
    [RoutePrefix("applications/{appName}/containers/{containerName}/contentInstances")]
    public class ContentInstancesController : ApiController
    {
        // POST /applications/{app}/containers/{container}/contentInstances
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateContentInstance(string appName, string containerName, [FromBody] string content)
        {
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null) return NotFound();

            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null) return NotFound();

            var ci = ContentInstanceSQLHelper.CreateContentInstance(
                new Models.ContentInstance
                {
                    Content = content,
                    ContainerId = container.Id
                });

            if (ci == null) return BadRequest();

            var resource = new ContentInstanceResource
            {
                content = ci.Content,
                creation_datetime = ci.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            // MQTT NOTIFICATION
            var subs = SubscriptionSQLHelper.GetSubscriptions(container.Id);
            foreach (var s in subs)
            {
                MosquittoHelper.PublishContentInstance(
                    s.Endpoint,
                    $"{appName}/{containerName}",
                    resource
                );
            }

            return Created("", resource);
        }
    }
}
