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
        private IConfiguration _configuration;

        private string _blobAccountKey;

        private string _storageAccountKey;

        public BlobHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            _blobAccountKey = _configuration["blobAccountKey"].ToString();

            _storageAccountKey = _configuration["storageAccount"].ToString();

        }

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

        public async Task<bool> DeleteReportBlobAsync(string reportName)
        {
            string blobConnectionString = CreateConnectionString();
            CloudStorageAccount account = CloudStorageAccount.Parse(blobConnectionString);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(Constants.BlobContainerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(reportName);
            return await blob.DeleteIfExistsAsync();
            
        }

        private string CreateConnectionString()
        {
            return string.Format(Constants.BlobConnectionString, _storageAccountKey, _blobAccountKey);
        }


    }
}