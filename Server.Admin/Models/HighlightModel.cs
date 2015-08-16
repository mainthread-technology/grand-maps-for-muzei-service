namespace Server.Admin.Models
{
    /// <summary>
    /// Model for the table row to highlight.
    /// </summary>
    public class HighlightModel
    {
        /// <summary>
        /// Gets or sets the bootstrap class.
        /// </summary>
        public readonly AlertType Style;

        /// <summary>
        /// Gets or sets the partition of the item to highlight.
        /// </summary>
        public readonly string Partition;

        /// <summary>
        /// Gets or sets the row key of the item to highlight.
        /// </summary>
        public readonly string Row;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightModel" /> class.
        /// </summary>
        /// <param name="style">The row style.</param>
        /// <param name="partition">The items partition.</param>
        /// <param name="row">The items row key.</param>
        public HighlightModel(AlertType style, string partition, string row)
        {
            Guard.NotNull(() => style);
            Guard.NotNullOrEmpty(() => row);
            Guard.NotNullOrEmpty(() => partition);

            this.Style = style;
            this.Partition = partition;
            this.Row = row;
        }
    }
}