namespace Server.Admin.Controllers
{
    using Microsoft.WindowsAzure.Storage;
    using Server.Admin.Models;
    using Server.Framework.Entities;
    using Server.Framework.Helpers;
    using SharpRaven;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// Base class for entity related controllers.
    /// </summary>
    public class EntityController : Controller
    {
        /// <summary>
        /// The table storage instance.
        /// </summary>
        public static readonly AzureTable<MapEntity> TableStorage = new AzureTable<MapEntity>(MapEntity.MainKey, ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

        /// <summary>
        /// The blob storage instance.
        /// </summary>
        public static readonly AzureBlob BlobStorage = new AzureBlob(AzureBlob.ImagesKey, ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

        /// <summary>
        /// Error logging.
        /// </summary>
        public static readonly RavenClient RavenClient = new RavenClient("https://ad4f18731da44964b0a9273ab4e17e38:bb11af4154474781a68a591bf13a596f@app.getsentry.com/26993");

        /// <summary>
        /// Returns the view with an updated model in the event of a storage error.
        /// </summary>
        /// <param name="row">The maps row key.</param>
        /// <param name="partition">The maps partition.</param>
        /// <param name="error">The storage error.</param>
        /// <returns>An updated view.</returns>
        protected ActionResult AzureStorageErrorResponse(string row, string partition, StorageException error)
        {
            Guard.NotNullOrEmpty(() => row);
            Guard.NotNullOrEmpty(() => partition);
            Guard.NotNull(() => error);

            ViewBag.Alerts = AlertModel.CreateSingle(AlertType.Danger, error.Message);

            var entityTask = TableStorage.Get(partition, row);

            Task.WaitAll(entityTask);

            return this.View(entityTask.Result);
        }
    }
}