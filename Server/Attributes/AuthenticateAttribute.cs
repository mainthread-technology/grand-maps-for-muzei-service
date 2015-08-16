namespace Server.API.Attributes
{
    using Newtonsoft.Json;
    using Server.API.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// The type of method being authenticated.
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// A safe default.
        /// </summary>
        None,

        /// <summary>
        /// A public endpoint.
        /// </summary>
        Public,

        /// <summary>
        /// A private admin endpoint.
        /// </summary>
        Admin
    }

    /// <summary>
    /// Indicates that a controller or method should pass authentication before running.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AuthenticateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The HTML header to look for.
        /// </summary>
        private const string AuthenticationHeaderName = "Authentication";

        /// <summary>
        /// The type of authentication being made.
        /// </summary>
        private readonly AuthenticationType authType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateAttribute" /> class.
        /// </summary>
        /// <param name="authType">The method or classes authentication type.</param>
        public AuthenticateAttribute(AuthenticationType authType)
        {
            this.authType = authType;
        }

        /// <summary>
        /// Checks if the request is authenticated.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public override void OnActionExecuting(HttpActionContext context)
        {
            if (!this.IsAuthenticated(context))
            {
                string jsonResponse = JsonConvert.SerializeObject(new ResponseModel("Unauthorized.", false));

                context.Response.StatusCode = HttpStatusCode.Unauthorized;
                context.Response.Content = new StringContent(jsonResponse);

                return;
            }
        }

        /// <summary>
        /// Checks the authentication header for a valid key.
        /// </summary>
        /// <param name="context">The HTTP context containing the request.</param>
        /// <returns>True if the request contains a valid key.</returns>
        private bool IsAuthenticated(HttpActionContext context)
        {
            string settingsKey;

            switch (this.authType)
            {
                case AuthenticationType.Public:
                    settingsKey = "PreSharedKey";
                    break;

                case AuthenticationType.Admin:
                    settingsKey = "UploadKey";
                    break;

                default:
                    return false;
            }

            IEnumerable<string> authValues;

            var headers = context.Request.Headers;

            if (!headers.TryGetValues(AuthenticationHeaderName, out authValues) || authValues.Count() != 1)
            {
                return false;
            }

            if (authValues.Single() == ConfigurationManager.AppSettings.Get(settingsKey))
            {
                return true;
            }

            return false;
        }
    }
}