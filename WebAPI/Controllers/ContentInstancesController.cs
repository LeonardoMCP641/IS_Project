using System;
using System.Linq;
using System.Web.Http;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Resources;

namespace WebAPI.Controllers
{
    [RoutePrefix("applications/{appName}/containers/{containerName}/contentInstances")]
    public class ContentInstancesController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateContentInstance(
            string appName,
            string containerName,
            [FromBody] ContentInstanceResource resource)
        {
            // Validate body (ENUNCIADO)
            if (resource == null ||
                string.IsNullOrWhiteSpace(resource.resource_name) ||
                string.IsNullOrWhiteSpace(resource.content_type) ||
                string.IsNullOrWhiteSpace(resource.content))
            {
                return BadRequest("resource_name, content_type and content are required");
            }

            // Validate application
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            // Validate container
            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            // Persist content instance
            var ci = ContentInstanceSQLHelper.CreateContentInstance(
                new ContentInstance
                {
                    ResourceName = resource.resource_name,
                    ContentType = resource.content_type,
                    Content = resource.content,
                    ContainerId = container.Id
                });

            if (ci == null)
                return BadRequest();

            // Resource to return + MQTT
            var ciResource = new ContentInstanceResource
            {
                resource_name = ci.ResourceName,
                content_type = ci.ContentType,
                content = ci.Content,
                creation_datetime = ci.CreationDateTime
                    .ToString("yyyy-MM-ddTHH:mm:ss")
            };

            // 🔔 MQTT notification (ENUNCIADO)
            var subs = SubscriptionSQLHelper.GetSubscriptions(container.Id);
            foreach (var s in subs)
            {
                MosquittoHelper.PublishContentInstance(
                    s.Endpoint,
                    $"{appName}/{containerName}",
                    ciResource
                );
            }

            return Created("", ciResource);
        }
    }
}
