using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable attribute routing (required by SOMIOD)
            config.MapHttpAttributeRoutes();

            // Optional: force XML as default (recommended for SOMIOD)
            config.Formatters.Remove(config.Formatters.JsonFormatter);

            // Enable detailed errors during development
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }
    }
}
