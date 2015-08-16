namespace Server.API.Controllers
{
    using Server.Framework.Entities;
    using Server.Framework.Helpers;
    using System.Configuration;
    using System.Web.Http;

    /// <summary>
    /// Base controller for entity controllers.
    /// </summary>
    public class EntityController : ApiController
    {
        /// <summary>
        /// The table storage instance.
        /// </summary>
        protected static readonly AzureTable<MapEntity> TableStorage = new AzureTable<MapEntity>(MapEntity.MainKey, ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

        /// <summary>
        /// The blob storage instance.
        /// </summary>
        protected static readonly AzureBlob BlobStorage = new AzureBlob(AzureBlob.ImagesKey, ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
    }
}