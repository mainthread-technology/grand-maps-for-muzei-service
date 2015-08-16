namespace Server.Framework.Helpers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Helper class to access an Azure Storage Blob Container.
    /// </summary>
    public class AzureBlob
    {
        /// <summary>
        /// Key for the images 
        /// </summary>
        public const string ImagesKey = "images";

        /// <summary>
        /// The blob container.
        /// </summary>
        public readonly CloudBlobContainer BlobContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlob" /> class.
        /// </summary>
        /// <param name="containerName">The name of the container to access.</param>
        /// <param name="connectionString">The storage connection string.</param>
        public AzureBlob(string containerName, string connectionString)
        {
            Guard.NotNullOrEmpty(() => containerName);
            Guard.NotNullOrEmpty(() => connectionString);

            var client = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();

            this.BlobContainer = client.GetContainerReference(containerName);
        }

        /// <summary>
        /// Creates a new blob from the given URL.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="fileURL">The URL to copy the file from.</param>
        /// <returns>The URL of the new blob.</returns>
        public async Task<Uri> SaveFromURL(string fileName, string fileURL)
        {
            Guard.NotNullOrEmpty(() => fileName);
            Guard.NotNullOrEmpty(() => fileURL);

            var blob = this.BlobContainer.GetBlockBlobReference(fileName);

            blob.Properties.ContentType = MimeMapping.GetMimeMapping(Path.GetFileName(fileURL));

            await blob.StartCopyFromBlobAsync(new Uri(fileURL));

            // Check the blob could load the url and delete itself and throw if it couldn't.
            if (blob.Properties.Length == 0)
            {
                await blob.DeleteAsync();

                throw new StorageException("Blob failed to upload correctly.");
            }

            return blob.Uri;
        }
    }
}
