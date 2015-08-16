namespace Server.Admin.Extensions
{
    using Server.Admin.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Extension methods for use in views.
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Displays any alerts.
        /// </summary>
        /// <param name="html">Extends the HtmlHelper for use in the view.</param>
        /// <returns>A formatted collection of alerts.</returns>
        public static MvcHtmlString DisplayAlerts(this HtmlHelper html)
        {
            Guard.NotNull(() => html);

            List<AlertModel> alerts = new List<AlertModel>();

            if (html.ViewBag.Alerts != null)
            {
                alerts.AddRange(html.ViewBag.Alerts);
            }

            if (html.ViewContext.TempData["Alerts"] != null)
            {
                alerts.AddRange(html.ViewContext.TempData["Alerts"] as List<AlertModel>);
            }

            var shortText = new Dictionary<AlertType, string>
            {
                { AlertType.Success, "Success!" },
                { AlertType.Info, "Heads up!" },
                { AlertType.Warning, "Warning!" },
                { AlertType.Danger, "Oh snap!" }
            };

            if (alerts.Count > 0)
            {
                string format = "<div class=\"alert alert-{0} alert-dismissible\" role=\"alert\"><strong>{1}</strong> {2}";

                format += "<button type=\"button\" class=\"close\" data-dismiss=\"alert\"><span aria-hidden=\"true\">&times;</span><span class=\"sr-only\">Close</span></button></div>";

                string result = alerts.Select(a => string.Format(format, a.Type.ToString().ToLower(), shortText[a.Type], a.Message)).Aggregate((a, b) => a + b);

                return MvcHtmlString.Create(result);
            }

            return null;
        }
    }
}