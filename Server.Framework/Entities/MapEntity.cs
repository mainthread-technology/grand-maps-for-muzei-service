namespace Server.Framework.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The MapEntity class.
    /// </summary>
    public class MapEntity : TableEntity
    {
        /// <summary>
        /// The value of the main partition key/table.
        /// </summary>
        public const string MainKey = "Main";

        /// <summary>
        /// The value of the featured partition key/table.
        /// </summary>
        public const string FeaturedKey = "Featured";

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEntity" /> class.
        /// </summary>
        public MapEntity()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEntity" /> class.
        /// </summary>
        /// <param name="id">The row key.</param>
        /// <param name="source">The partition key.</param>
        public MapEntity(string id, string source)
        {
            this.PartitionKey = source;
            this.RowKey = id;
        }

        /// <summary>
        /// Gets or sets the entities Title.
        /// </summary>
        [Required(ErrorMessage = "A title is required.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the entities Author.
        /// </summary>
        [Required(ErrorMessage = "An author is required.")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the entities Year.
        /// </summary>
        [DisplayName("Published")]
        [Required(ErrorMessage = "A publication year is required.")]
        [Range(0, 3000, ErrorMessage = "The publication year must be a positive number.")]
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the entities ImageAddress.
        /// </summary>
        [DisplayName("Image URL")]
        [Required(ErrorMessage = "An image URL is required.")]
        [DataType(DataType.Url)]
        public string ImageAddress { get; set; }

        /// <summary>
        /// Gets or sets the entities ReferenceAddress.
        /// </summary>
        [DisplayName("Reference URL")]
        [Required(ErrorMessage = "A reference URL is required.")]
        [DataType(DataType.Url)]
        public string ReferenceAddress { get; set; }

        /// <summary>
        /// Gets or sets the entities FeaturedDate.
        /// </summary>
        [DisplayName("Featured")]
        [DataType(DataType.Date)]
        public string FeaturedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity should be returned by the API.
        /// </summary>
        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}
