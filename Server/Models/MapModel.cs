namespace Server.API.Models
{
    using Server.Framework.Entities;
    using System;

    /// <summary>
    /// A serializable Map to be used by the API controllers.
    /// </summary>
    public class MapModel
    {
        /// <summary>
        /// Gets or sets the models Id, which will be a <see cref="ShortGuid"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the models Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the models Author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the models Year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the models ImageAddress.
        /// </summary>
        public string ImageAddress { get; set; }

        /// <summary>
        /// Gets or sets the models ReferenceAddress.
        /// </summary>
        public string ReferenceAddress { get; set; }

        /// <summary>
        /// Gets or sets the models NextUpdate.
        /// </summary>
        public long? NextUpdate { get; set; }

        /// <summary>
        /// Converts a MapEntity into a MapModel.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>A populated MapModel.</returns>
        public static MapModel FromEntity(MapEntity entity)
        {
            Guard.NotNull(() => entity);

            return new MapModel
            {
                Id = entity.RowKey,
                Title = entity.Title,
                Author = entity.Author,
                Year = entity.Year,
                ImageAddress = entity.ImageAddress,
                ReferenceAddress = entity.ReferenceAddress
            };
        }
    }
}