namespace Server.API
{
    using System.Web.Http;

    /// <summary>
    /// The API configuration class.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the API routes.
        /// </summary>
        /// <param name="config">The http configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
