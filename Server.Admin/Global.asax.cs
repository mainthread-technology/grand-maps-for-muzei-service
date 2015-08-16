namespace Server.Admin
{
    using Server.Admin.Controllers;
    using SharpRaven;
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    /// <summary>
    /// The base application class.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The method to run at the start of the applications life.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcHandler.DisableMvcResponseHeader = true;
        }

        /// <summary>
        /// Stuff to run at the beginning of requests.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            switch (Request.Url.Scheme)
            {
                case "https":
                    Response.AddHeader("Strict-Transport-Security", "max-age=31536000");
                    break;
                case "http":
                    var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", path);
                    break;
            }
        }

        /// <summary>
        /// Stuff to run before returning the response headers.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Remove the "Server" HTTP Header from response
            HttpApplication app = sender as HttpApplication;
            if (null != app && null != app.Request && !app.Request.IsLocal &&
                null != app.Context && null != app.Context.Response)
            {
                NameValueCollection headers = app.Context.Response.Headers;
                if (null != headers)
                {
                    headers.Remove("Server");
                }
            }
        }

        /// <summary>
        /// Handles application errors.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData  = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = string.Empty;
            var currentAction     = string.Empty;

                if (currentRouteData != null
                    && currentRouteData.Values["controller"] != null
                    && !string.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData != null
                    && currentRouteData.Values["action"] != null
                    && !string.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }

            var exception = Server.GetLastError();

            Task.Run(() => new RavenClient("https://ad4f18731da44964b0a9273ab4e17e38:bb11af4154474781a68a591bf13a596f@app.getsentry.com/26993").CaptureException(exception));

            var controller = new HomeController();
            var routeData  = new RouteData();

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = exception is HttpException ? ((HttpException)exception).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "Error";

            controller.ViewData.Model = new HandleErrorInfo(exception, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }
    }
}
