using Swashbuckle.Application;
using System;
using System.IO;
using System.Web;
using System.Web.Http;
using WebActivatorEx;
using WebAPI.App_Start;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebAPI.App_Start.SwaggerConfig), "Register")]

namespace WebAPI.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "SOMIOD Middleware API");

                    var xmlFilePath = GetXmlCommentsPath();
                    if (File.Exists(xmlFilePath))
                    {
                        c.IncludeXmlComments(xmlFilePath);
                    }
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("SOMIOD");
                });
        }

        protected static string GetXmlCommentsPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "WebAPI.xml");
        }
    }
}