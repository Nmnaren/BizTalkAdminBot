using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Helpers
{
    public class BlobHelper
    {
        /// <summary>
        /// Configuration property which stores the configuration injected from the statrtup
        /// </summary>
        private IConfiguration _configuration;

        /// <summary>
        /// Key for the storage account where the blob will be uploaded
        /// </summary>
        private string _blobAccountKey;

        /// <summary>
        /// Name of the storage account
        /// </summary>
        private string _storageAccountKey;

        /// <summary>
        /// Constructor to Inject the Configuration object using Dependency Injection
        /// </summary>
        /// <param name="configuration"></param>
        public BlobHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            _blobAccountKey = _configuration["blobAccountKey"].ToString();

            _storageAccountKey = _configuration["storageAccount"].ToString();

        }

        /// <summary>
        /// Upload the Report to the storage account as a blob
        /// </summary>
        /// <param name="report">Report Html</param>
        /// <param name="reportName">Name of the report</param>
        /// <returns>Name of the blob created in the the storage account</returns>
        public async Task<string> UploadReportToBlobAsync(string report, string reportName)
        {
            string blobConnectionString = CreateConnectionString();

            CloudStorageAccount account = CloudStorageAccount.Parse(blobConnectionString);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(Constants.BlobContainerName);
            await container.CreateIfNotExistsAsync();

            string blobName = string.Format("{0}_{1}", reportName, Guid.NewGuid().ToString());

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = "text/html";
            
            await blob.UploadTextAsync(report);

            return blobName;
        }

        /// <summary>
        /// Delete the blob in the storage account
        /// </summary>
        /// <param name="reportName">Name of the blob</param>
        /// <returns>Status of the Delete operation</returns>
        public async Task<bool> DeleteReportBlobAsync(string reportName)
        {
            string blobConnectionString = CreateConnectionString();
            CloudStorageAccount account = CloudStorageAccount.Parse(blobConnectionString);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(Constants.BlobContainerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(reportName);
            return await blob.DeleteIfExistsAsync();
            
        }

        /// <summary>
        /// Parse the Connection String for the Azure Storage account
        /// </summary>
        /// <returns>Parsed Connection String</returns>
        private string CreateConnectionString()
        {
            return string.Format(Constants.BlobConnectionString, _storageAccountKey, _blobAccountKey);
        }


    }
}