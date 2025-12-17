using System.Linq;
using System.Web.Http;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Resources;

namespace WebAPI.Controllers
{
    [RoutePrefix("applications/{appName}/containers")]
    public class ContainersController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateContainer(
            string appName,
            [FromBody] ContainerResource resource)
        {
            if (resource == null || string.IsNullOrWhiteSpace(resource.resource_name))
                return BadRequest("resource_name is required");

            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null)
                return NotFound();

            var container = ContainerSQLHelper.CreateContainer(
                new Container
                {
                    ResourceName = resource.resource_name,
                    ApplicationId = app.Id
                });

            if (container == null)
                return BadRequest();

            return Created("", new ContainerResource
            {
                resource_name = container.ResourceName,
                creation_datetime = container.CreationDateTime
                    .ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }
    }
}
