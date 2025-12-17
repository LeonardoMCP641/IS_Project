using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.Helpers;
using WebAPI.Resources;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [RoutePrefix("applications")]
    public class ApplicationsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetApplications()
        {
            var apps = ApplicationSQLHelper.GetApplications()
                .Select(a => new ApplicationResource
                {
                    resource_name = a.ResourceName,
                    creation_datetime = a.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss")
                });

            return Ok(apps);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateApplication([FromBody] ApplicationResource resource)
        {
            if (resource == null || string.IsNullOrWhiteSpace(resource.resource_name))
                return BadRequest("resource_name is required");

            var app = ApplicationSQLHelper.CreateApplication(
                new Application
                {
                    ResourceName = resource.resource_name
                });

            if (app == null)
                return BadRequest();

            return Created("", new ApplicationResource
            {
                resource_name = app.ResourceName,
                creation_datetime = app.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }
    }

}
