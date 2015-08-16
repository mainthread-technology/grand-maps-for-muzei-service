namespace Server.API.Controllers
{
    using Server.API.Attributes;
    using Server.API.Extensions;
    using Server.API.Models;
    using Server.Framework.Entities;
    using Server.Framework.Helpers;
    using Server.Framework.Structs;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// The MapAdminController class.
    /// </summary>
    [RoutePrefix("v1/maps")]
    public class MapAdminController : EntityController
    {        
        /// <summary>
        /// Sets the featured maps for the next several days.
        /// </summary>
        /// <param name="days">The number of days to set.</param>
        /// <returns>A success message if it saves.</returns>
        [HttpGet]
        [Route("featured/set/{days:int:min(1)}")]
        [Authenticate(AuthenticationType.Admin)]
        public async Task<ResponseModel> SetFeaturedMaps(int days)
        {
            var dates = Enumerable.Range(1, days).Select(i => DateTimeOffset.UtcNow.Date.AddDays(i));

            string previous = string.Empty;

            foreach (var date in dates)
            {
                if (await TableStorage.GetFeaturedMap(date) == default(MapEntity))
                {
                    var map = await TableStorage.SetFeaturedMap(await TableStorage.GetRandomMap(previous), date);

                    previous = map.RowKey;
                }
            }

            return new ResponseModel("Featured maps set.", true);
        }
    }
}
