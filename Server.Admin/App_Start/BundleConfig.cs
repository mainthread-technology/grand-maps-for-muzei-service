namespace Server.Admin
{
    using System.Web;
    using System.Web.Optimization;

    /// <summary>
    /// Configures any bundling of static content.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registers any bundles.
        /// </summary>
        /// <see cref="http://go.microsoft.com/fwlink/?LinkId=301862"/>
        /// <param name="bundles">The bundle collection.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // bundles.Add(new StyleBundle("~/Content/Style").Include("~/Content/Styles/Bootstrap/bootstrap.less"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            // BundleTable.EnableOptimizations = true;
        }
    }
}
