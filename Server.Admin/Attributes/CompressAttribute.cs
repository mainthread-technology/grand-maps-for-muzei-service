namespace Server.Admin.Attributes
{
    using System.IO.Compression;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Compresses the output before sending to the client.
    /// </summary>
    public class CompressAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Hooks onto the on executing event.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            Guard.NotNull(() => actionContext);

            HttpRequestBase request = actionContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding))
            {
                return;
            }

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = actionContext.HttpContext.Response;

            Compress(acceptEncoding, response);
        }

        /// <summary>
        /// Compress the response.
        /// </summary>
        /// <param name="acceptEncoding">The type of accepted compression.</param>
        /// <param name="response">The actions response.</param>
        private static void Compress(string acceptEncoding, HttpResponseBase response)
        {
            Guard.NotNullOrEmpty(() => acceptEncoding);
            Guard.NotNull(() => response);

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}