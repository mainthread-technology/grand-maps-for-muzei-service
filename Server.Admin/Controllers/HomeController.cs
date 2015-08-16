namespace Server.Admin.Controllers
{
    using Server.Admin.Attributes;
    using Server.Admin.Models;
    using Server.Framework.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The home controller.
    /// Handles login, logout.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The index route, redirects the user to a better page.
        /// </summary>
        /// <returns>A redirect.</returns>
        public ActionResult Index()
        {
            if (Request.Cookies[AuthenticateAttribute.AuthenticationCookieName] != null)
            {
                return this.RedirectToAction("Index", "Main");
            }

            return this.RedirectToAction("Login");
        }

        /// <summary>
        /// The stats route.
        /// </summary>
        /// <returns>The stats page.</returns>
        [Authenticate]
        public ActionResult Analytics()
        {
            return this.View();
        }

        /// <summary>
        /// Displays the login form.
        /// </summary>
        /// <returns>The login form.</returns>
        public ActionResult Login()
        {
            return this.View();
        }

        /// <summary>
        /// Processes a login request.
        /// </summary>
        /// <param name="model">The form data.</param>
        /// <returns>Either a login or form error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Throttle(Name = "LoginThrottle", Seconds = 1)]
        public ActionResult Login(FormCollection model)
        {
            string password = model.Get("Password");

            if (string.IsNullOrEmpty(password))
            {
                ViewBag.Alerts = new List<AlertModel> { new AlertModel(AlertType.Info, "A pre-shared key is required.") };

                return this.View();
            }

            string hash = Utils.HashPassword(password, ConfigurationManager.AppSettings.Get("TokenSalt"));

            string validToken = ConfigurationManager.AppSettings.Get("AuthToken");

            if (!string.IsNullOrEmpty(validToken) && validToken == hash)
            {
                var cookie = new HttpCookie(AuthenticateAttribute.AuthenticationCookieName, hash);

                cookie.HttpOnly = true;
                cookie.Secure   = true;

                Response.SetCookie(cookie);

                return this.RedirectToAction("Index", "Main");
            }
            else
            {
                ViewBag.Alerts = new List<AlertModel> { new AlertModel(AlertType.Danger, "The pre-shared key was incorrect.") };
            }

            return this.View();
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <returns>A redirect to the index.</returns>
        [Authenticate]
        public ActionResult Logout()
        {
            var cookie = new HttpCookie(AuthenticateAttribute.AuthenticationCookieName);

            cookie.Expires = DateTime.Now.AddDays(-1);

            Response.SetCookie(cookie);

            this.TempData["Alerts"] = new List<AlertModel> { new AlertModel(AlertType.Success, "You have been logged out.") };

            return this.RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Displays the details of an error.
        /// </summary>
        /// <returns>The error view.</returns>
        public ActionResult Error()
        {
            if (this.ViewData.Model == null)
            {
                return this.RedirectToAction("Index");
            }

            return this.View();
        }
    }
}