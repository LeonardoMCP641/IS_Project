using System;
using System.Linq;
using System.Web.Http;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Resources;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing Subscriptions.
    /// </summary>
    [RoutePrefix("applications/{appName}/containers/{containerName}/subscriptions")]
    public class SubscriptionsController : ApiController
    {
        // GET /applications/{app}/containers/{container}/subscriptions
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetSubscriptions(string appName, string containerName)
        {
            // Validate application
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            // Validate container
            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            // Get subscriptions
            var subs = SubscriptionSQLHelper.GetSubscriptions(container.Id)
                .Select(s => new SubscriptionResource
                {
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
            // Validate body
            if (resource == null || string.IsNullOrWhiteSpace(resource.endpoint))
                return BadRequest("Subscription endpoint is required.");

            // Validate application
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            // Validate container
            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            // Create subscription
            var sub = SubscriptionSQLHelper.CreateSubscription(
                new Subscription
                {
                    Endpoint = resource.endpoint,
                    ContainerId = container.Id
                });

            if (sub == null)
                return BadRequest("Could not create subscription.");

            // Return resource
            return Created("", new SubscriptionResource
            {
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
            // Validate application
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            // Validate container
            var container = ContainerSQLHelper.GetContainer(app.Id, containerName);
            if (container == null)
                return NotFound();

            // Delete subscription
            var deleted = SubscriptionSQLHelper.DeleteSubscription(id);
            if (deleted == null)
                return NotFound();

            return Ok();
        }
    }
}
