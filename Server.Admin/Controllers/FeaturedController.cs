namespace Server.Admin.Controllers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Server.Admin.Attributes;
    using Server.Admin.Models;
    using Server.Framework.Entities;
    using Server.Framework.Structs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// Controller to manage featured maps.
    /// </summary>
    [Authenticate]
    public class FeaturedController : EntityController
    {
        /// <summary>
        /// The default action.
        /// </summary>
        /// <returns>The list of maps in the system.</returns>
        public async Task<ActionResult> Index()
        {
            var query = new TableQuery<MapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, MapEntity.FeaturedKey));

            IEnumerable<MapEntity> model = (await TableStorage.GetFromQuery(query)).OrderByDescending(e => e.FeaturedDate);

            return this.View(model);
        }

        /// <summary>
        /// Sets a weeks worth of random featured maps.
        /// </summary>
        /// <returns>Redirection back to the list of featured maps.</returns>
        public async Task<ActionResult> SetRandom()
        {
            HttpClient http = new HttpClient();

            http.DefaultRequestHeaders.Add("Authentication", "BVcmBLiKVLSwg1YWAKPjV0utRHe2y6sLgxIBZjAfWtrMqm5okZcM9EjERLHoJ6tf");

            await http.GetAsync("https://grand-maps.azurewebsites.net/v1/maps/featured/set/7");

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// The new map form, pre filled from an existing map.
        /// </summary>
        /// <param name="row">The row to copy from.</param>
        /// <returns>The creation form.</returns>
        public async Task<ActionResult> Create(string row = "")
        {
            MapEntity model = new MapEntity();

            if (!string.IsNullOrEmpty(row))
            {
                model = await TableStorage.Get(MapEntity.MainKey, row);
            }

            model.PartitionKey = MapEntity.FeaturedKey;
            model.RowKey = string.Empty;

            model.FeaturedDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

            return this.View(model);
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
            if (string.IsNullOrEmpty(model.FeaturedDate))
            {
                ModelState.AddModelError("FeaturedDate", "A featured date is required.");
            }

            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            model.PartitionKey = MapEntity.FeaturedKey;
            model.RowKey = ShortGuid.NewGuid().Value;

            try
            {
                await TableStorage.Insert(model);
            }
            catch (StorageException error)
            {
                Task.Run(() => RavenClient.CaptureException(error));

                ViewBag.Alerts = new List<AlertModel> { new AlertModel(AlertType.Danger, error.Message) };

                return this.View(model);
            }

            string alertMessage = string.Format("{0} by {1} for {2} was created.", model.Title, model.Author, model.FeaturedDate);

            this.TempData["Alerts"] = AlertModel.CreateSingle(AlertType.Success, alertMessage);

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
            MapEntity entity = await TableStorage.Get(MapEntity.FeaturedKey, row);

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
            if (string.IsNullOrEmpty(model.FeaturedDate))
            {
                ModelState.AddModelError("FeaturedDate", "A featured date is required.");
            }

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

                return this.AzureStorageErrorResponse(row, MapEntity.FeaturedKey, error);
            }

            string alertMessage = string.Format("The featured map for {0} was updated.", model.FeaturedDate);

            this.TempData["Alerts"] = AlertModel.CreateSingle(AlertType.Success, alertMessage);

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
            MapEntity entity = await TableStorage.Get(MapEntity.FeaturedKey, row);

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
            MapEntity entity = await TableStorage.Get(MapEntity.FeaturedKey, row);

            try
            {
                await TableStorage.Delete(entity);
            }
            catch (StorageException error)
            {
                Task.Run(() => RavenClient.CaptureException(error));

                return this.AzureStorageErrorResponse(row, MapEntity.FeaturedKey, error);
            }

            string alertMessage = string.Format("{0} by {1} for {2} was deleted.", entity.Title, entity.Author, model.FeaturedDate);

            this.TempData["Alerts"] = AlertModel.CreateSingle(AlertType.Info, alertMessage);

            return this.RedirectToAction("Index");
        }
    }
}