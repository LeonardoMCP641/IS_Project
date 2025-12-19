using System.Web.Http;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Habilita Attribute Routing
            config.MapHttpAttributeRoutes();

            // Fallback (opcional)
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Mantém JSON e XML
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(
                new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain")); // opcional: trata texto simples como JSON para leitura

            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
