namespace Server.API
{
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;

    /// <summary>
    /// The Web API utility class.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// Stuff to run on application start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            var formatters = GlobalConfiguration.Configuration.Formatters;

            formatters.XmlFormatter.SupportedMediaTypes.Clear();
            formatters.JsonFormatter.Indent = true;

            MvcHandler.DisableMvcResponseHeader = true;
        }

        /// <summary>
        /// Stuff to run at the beginning of requests.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
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
    }
}
