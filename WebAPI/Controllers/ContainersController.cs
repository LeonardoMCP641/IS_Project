using System;
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
        public IHttpActionResult CreateContainer(string appName)
        {
            var app = ApplicationSQLHelper.GetApplication(appName);
            if (app == null) return NotFound();

            var container = ContainerSQLHelper.CreateContainer(
                new Container { ApplicationId = app.Id }
            );

            if (container == null) return BadRequest();

            return Created("", new ContainerResource
            {
                resource_name = container.ResourceName,
                creation_datetime = container.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }
    }

}
