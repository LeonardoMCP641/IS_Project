using System.Web.Http;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ATTRIBUTE ROUTING (OBRIGATÓRIO)
            config.MapHttpAttributeRoutes();

            // Fallback (opcional)
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // JSON only (opcional mas recomendado)
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
