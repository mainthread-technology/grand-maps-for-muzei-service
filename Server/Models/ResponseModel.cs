namespace Server.API.Models
{
    using Server.Framework.Entities;
    using System;

    /// <summary>
    /// A serializable Map to be used by the API controllers.
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// Gets the value indicating if the response is a success.
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// Gets the message for the response.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseModel" /> class.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="isSuccess">Indicated if the call was a success.</param>
        public ResponseModel(string message, bool isSuccess)
        {
            this.Success = isSuccess;
            this.Message = message;
        }
    }
}