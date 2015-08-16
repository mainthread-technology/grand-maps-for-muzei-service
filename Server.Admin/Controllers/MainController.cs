namespace Server.Admin.Controllers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Server.Admin.Attributes;
    using Server.Admin.Models;
    using Server.Framework.Entities;
    using Server.Framework.Structs;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// The main controller for our collection of maps.
    /// </summary>
    [Authenticate]
    public class MainController : EntityController
    {
        /// <summary>
        /// The default action.
        /// </summary>
        /// <returns>The list of maps in the system.</returns>
        public async Task<ActionResult> Index()
        {
            var query = new TableQuery<MapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, MapEntity.MainKey));

            IEnumerable<MapEntity> entities = (await TableStorage.GetFromQuery(query)).OrderBy(e => e.Author).ThenBy(e => e.Title);

            PagedModel<MapEntity> model = new PagedModel<MapEntity>
            {
                Items = entities,
                ItemsTotal = entities.Count()
            };

            return this.View(model);
        }

        /// <summary>
        /// The new map form.
        /// </summary>
        /// <returns>The creation form.</returns>
        public ActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        /// Creates a new map.
        /// </summary>
        /// <param name="model">The new map data.</param>
        /// <returns>A blank form if successful otherwise an alert.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MapEntity model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            model.PartitionKey = MapEntity.MainKey;
            model.RowKey = ShortGuid.NewGuid().Value;

            try
            {
                // upload image to blob storage.
                model.ImageAddress = await UploadImageToStorage(model.RowKey, model.ImageAddress);

                await TableStorage.Insert(model);
            }
            catch (StorageException error)
            {
                Task.Run(() => RavenClient.CaptureException(error));

                ViewBag.Alerts = new List<AlertModel> { new AlertModel(AlertType.Danger, error.Message) };

                return this.View(model);
            }

            this.TempData["Alerts"] = AlertModel.CreateSingle(AlertType.Success, string.Format("{0} by {1} was created.", model.Title, model.Author));

            this.TempData["Highlight"] = new HighlightModel(AlertType.Success, model.PartitionKey, model.RowKey);

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// The edit a map method.
        /// </summary>
        /// <param name="row">The maps row key.</param>
        /// <returns>A populated edit form for the given map.</returns>
        public async Task<ActionResult> Edit(string row)
        {
            MapEntity entity = await TableStorage.Get(MapEntity.MainKey, row);

            return this.View(entity);
        }

        /// <summary>
        /// Updates a maps data.
        /// </summary>
        /// <param name="row">The maps row key.</param>
        /// <param name="model">The new map information.</param>
        /// <returns>The update form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string row, MapEntity model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            try
            {
                await TableStorage.Replace(model);
            }
            catch (StorageException error)
            {
                Task.Run(() => RavenClient.CaptureException(error));

                return this.AzureStorageErrorResponse(row, MapEntity.MainKey, error);
            }

            this.TempData["Alerts"] = AlertModel.CreateSingle(AlertType.Success, string.Format("{0} by {1} was updated.", model.Title, model.Author));

            this.TempData["Highlight"] = new HighlightModel(AlertType.Info, model.PartitionKey, model.RowKey);

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Confirms the deletion of a map.
        /// </summary>
        /// <param name="row">The maps row key.</param>
        /// <returns>The maps information.</returns>
        [HttpGet]
        public async Task<ActionResult> Delete(string row)
        {
            MapEntity entity = await TableStorage.Get(MapEntity.MainKey, row);

            return this.View(entity);
        }

        /// <summary>
        /// Deletes a map.
        /// </summary>
        /// <param name="row">The maps row key.</param>
        /// <param name="model">Useless data.</param>
        /// <returns>A redirect if successful.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string row, MapEntity model)
        {
            MapEntity entity = await TableStorage.Get(MapEntity.MainKey, row);

            try
            {
                await TableStorage.Delete(entity);
            }
            catch (StorageException error)
            {
                Task.Run(() => RavenClient.CaptureException(error));

                return this.AzureStorageErrorResponse(row, MapEntity.MainKey, error);
            }

            this.TempData["Alerts"] = AlertModel.CreateSingle(AlertType.Info, string.Format("{0} by {1} was deleted.", entity.Title, entity.Author));

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Uploads a given image to Azure Blob Storage.
        /// </summary>
        /// <param name="fileName">The name for the blob, no extension.</param>
        /// <param name="imageUrl">The url of the image to upload.</param>
        /// <returns>The public url of the uploaded blob.</returns>
        private static async Task<string> UploadImageToStorage(string fileName, string imageUrl)
        {
            Guard.NotNullOrEmpty(() => fileName);
            Guard.NotNullOrEmpty(() => imageUrl);

            return (await BlobStorage.SaveFromURL(fileName + Path.GetExtension(imageUrl), imageUrl)).ToString();
        }
    }
}
