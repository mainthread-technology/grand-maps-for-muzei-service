namespace Server.Admin.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the type of alert.
    /// </summary>
    public enum AlertType
    {
        /// <summary>
        /// A successful alert.
        /// </summary>
        Success,

        /// <summary>
        /// An informative alert.
        /// </summary>
        Info,

        /// <summary>
        /// A warning alert.
        /// </summary>
        Warning,

        /// <summary>
        /// A danger alert.
        /// </summary>
        Danger
    }

    /// <summary>
    /// Represents a basic alert to display to the user.
    /// </summary>
    public class AlertModel
    {
        /// <summary>
        /// The type of alert.
        /// </summary>
        public readonly AlertType Type;

        /// <summary>
        /// The message to include.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertModel" /> class.
        /// </summary>
        /// <param name="type">The type of the alert.</param>
        /// <param name="message">The message.</param>
        public AlertModel(AlertType type, string message)
        {
            Guard.NotNull(() => type);
            Guard.NotNullOrEmpty(() => message);

            this.Type = type;
            this.Message = message;
        }

        /// <summary>
        /// Creates a list of AlertModels with a single item.
        /// </summary>
        /// <param name="type">The alert type.</param>
        /// <param name="message">The message.</param>
        /// <returns>A single item list of an AlertModel.</returns>
        public static List<AlertModel> CreateSingle(AlertType type, string message)
        {
            Guard.NotNull(() => type);
            Guard.NotNullOrEmpty(() => message);

            return new List<AlertModel> { new AlertModel(type, message) };
        }
    }
}