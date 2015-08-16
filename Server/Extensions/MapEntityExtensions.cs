namespace Server.API.Extensions
{
    using Microsoft.WindowsAzure.Storage.Table;
    using Server.Framework.Entities;
    using Server.Framework.Helpers;
    using Server.Framework.Structs;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Set of extensions for the map entity and its Azure table.
    /// </summary>
    public static class MapEntityExtensions
    {
        /// <summary>
        /// A static random instance.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Gets a single random Map from Azure storage.
        /// </summary>
        /// <param name="table">The Azure table to use.</param>
        /// <param name="previous">A Map row key to avoid selecting.</param>
        /// <returns>A random Map.</returns>
        public static async Task<MapEntity> GetRandomMap(this AzureTable<MapEntity> table, string previous = "")
        {
            Guard.NotNull(() => table);
            Guard.NotNull(() => previous);

            var queryConditions = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, MapEntity.MainKey),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForBool("IsActive", QueryComparisons.Equal, true));

            var query = new TableQuery<MapEntity>().Where(queryConditions);

            var maps = await table.GetFromQuery(query);

            if (maps.Count() == 0)
            {
                return default(MapEntity);
            }

            int randomIndex = random.Next(maps.Count());

            var storedMap = maps.ElementAt(randomIndex);

            if (!string.IsNullOrEmpty(previous) && storedMap.RowKey == previous)
            {
                storedMap = maps.ElementAt(random.Next(maps.Count()));
            }

            return storedMap;
        }

        /// <summary>
        /// Gets the currently featured map, if any.
        /// </summary>
        /// <param name="table">The Azure table to use.</param>
        /// <param name="featuredDate">The featured date to use.</param>
        /// <returns>Returns the currently featured map.</returns>
        public static async Task<MapEntity> GetFeaturedMap(this AzureTable<MapEntity> table, DateTimeOffset featuredDate)
        {
            Guard.NotNull(() => table);
            Guard.NotNull(() => featuredDate);

            TableQuery<MapEntity> query = new TableQuery<MapEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, MapEntity.FeaturedKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("FeaturedDate", QueryComparisons.Equal, featuredDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))));

            return (await table.GetFromQuery(query)).FirstOrDefault();
        }

        /// <summary>
        /// Sets the featured Maps cache.
        /// </summary>
        /// <param name="table">The Azure table to use.</param>
        /// <param name="featuredEntity">The currently featured Map to cache.</param>
        /// <param name="featuredDate">The date to save the featured map for.</param>
        /// <returns>The saved entity.</returns>
        public static async Task<MapEntity> SetFeaturedMap(this AzureTable<MapEntity> table, MapEntity featuredEntity, DateTimeOffset featuredDate)
        {
            Guard.NotNull(() => table);
            Guard.NotNull(() => featuredEntity);
            Guard.NotNull(() => featuredDate);

            featuredEntity.RowKey = ShortGuid.NewGuid().Value;
            featuredEntity.PartitionKey = MapEntity.FeaturedKey;

            featuredEntity.FeaturedDate = featuredDate.ToString("yyyy-MM-dd");

            return await table.Insert(featuredEntity);
        }
    }
}
