namespace Server.Admin
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Applies the various routes.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The route collection to add to.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{row}",
                defaults: new
                { 
                    controller = "Home",
                    action = "Index",
                    row = UrlParameter.Optional
                });
        }
    }
}
