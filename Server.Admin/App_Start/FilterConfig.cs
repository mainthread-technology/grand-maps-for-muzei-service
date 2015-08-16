namespace Server.Admin
{
    using Server.Admin.Attributes;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Applies any filters.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Adds any global filters.
        /// </summary>
        /// <param name="filters">The collection of filters to add to.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CompressAttribute());
            filters.Add(new HandleErrorAttribute
            {
                View = "Error"
            });
        }
    }
}
