namespace Server.API.Controllers
{
    using Keen.Core;
    using Server.API.Attributes;
    using Server.API.Extensions;
    using Server.API.Models;
    using Server.Framework.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// The MapController class.
    /// </summary>
    [RoutePrefix("v1/maps")]
    public class MapPublicController : EntityController
    {
        private static KeenClient Analytics = new KeenClient(new ProjectSettingsProvider(ConfigurationManager.AppSettings["keenProjectId"], writeKey: ConfigurationManager.AppSettings["keenWriteKey"]));

        protected string ClientId
        {
            get
            {
                if (Request.Headers.Contains("X-Client-ID"))
                {
                    return Request.Headers.GetValues("X-Client-ID").FirstOrDefault();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the currently featured Map.
        /// </summary>
        /// <returns>The currently featured Map.</returns>
        [HttpGet]
        [Route("featured")]
        [Authenticate(AuthenticationType.Public)]
        public async Task<MapModel> GetFeaturedMap()
        {
            MapModel model = MapModel.FromEntity(await TableStorage.GetFeaturedMap(DateTimeOffset.UtcNow));

            // If no featured map is set return a random one and set a short update check.
            if (model == null)
            {
                model = MapModel.FromEntity(await TableStorage.GetRandomMap());

                var nextHour =  DateTimeOffset.UtcNow.AddHours(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                model.NextUpdate = (long)nextHour.TotalSeconds;

                Analytics.AddEventAsync("api.public.get.featured", new
                {
                    client_id = this.ClientId,
                    map_id = model.Id,
                    map_title = model.Title,
                    map_author = model.Author,
                    map_year = model.Year,
                    map_image_url = model.ImageAddress,
                    map_reference_url = model.ReferenceAddress,
                    next_update = model.NextUpdate
                });

                return model;
            }

            model.NextUpdate = Utils.NextFeaturedUpdateTime();

            Analytics.AddEventAsync("api.public.get.featured", new
            {
                client_id = this.ClientId,
                map_id = model.Id,
                map_title = model.Title,
                map_author = model.Author,
                map_year = model.Year,
                map_image_url = model.ImageAddress,
                map_reference_url = model.ReferenceAddress,
                next_update = model.NextUpdate
            });

            return model;
        }

        /// <summary>
        /// Gets a Map that doesn't have an id matching the previous parameter.
        /// </summary>
        /// <param name="previous">The id of the parameter to avoid.</param>
        /// <returns>A random MapModel from storage.</returns>
        [HttpGet]
        [Route("random/{previous:length(22)?}")]
        [Authenticate(AuthenticationType.Public)]
        public async Task<MapModel> GetRandomMap(string previous = "")
        {
            var model = MapModel.FromEntity(await TableStorage.GetRandomMap(previous));

            Analytics.AddEventAsync("api.public.get.random", new
            {
                client_id = this.ClientId,
                request_param_previous = previous,
                map_id = model.Id,
                map_title = model.Title,
                map_author = model.Author,
                map_year = model.Year,
                map_image_url = model.ImageAddress,
                map_reference_url = model.ReferenceAddress
            });

            return model;
        }
    }
}
