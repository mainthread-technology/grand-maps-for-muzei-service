namespace Server.Admin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Indicates that a controller or method should pass authentication before running.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AuthenticateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The HTML header to look for.
        /// </summary>
        public const string AuthenticationCookieName = "Authentication";

        /// <summary>
        /// Checks if the request is authenticated.
        /// </summary>
        /// <param name="actionContext">The current HTTP context.</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            Guard.NotNull(() => actionContext);

            if (!this.IsAuthenticated(actionContext))
            {
                actionContext.HttpContext.Response.Redirect("/Home/Login", true);
            }
        }

        /// <summary>
        /// Checks the authentication header for a valid key.
        /// </summary>
        /// <param name="actionContext">The HTTP context containing the request.</param>
        /// <returns>True if the request contains a valid key.</returns>
        private bool IsAuthenticated(ActionExecutingContext actionContext)
        {
            Guard.NotNull(() => actionContext);

            var authCookie = actionContext.HttpContext.Request.Cookies.Get(AuthenticationCookieName);

            if (authCookie == null)
            {
                return false;
            }

            IEnumerable<string> validTokens = ConfigurationManager.AppSettings.GetValues("AuthToken");

            if (validTokens != null && validTokens.Contains(authCookie.Value))
            {
                return true;
            }

            return false;
        }
    }
}