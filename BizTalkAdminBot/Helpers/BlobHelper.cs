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
using System.Threading;
using System.Threading.Tasks;

namespace BizTalkAdminBot.Helpers
{
    public class BlobHelper
    {
        private IConfiguration _configuration;

        public BlobHelper(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public async Task<string> UploadReportToBlob(string report, string reportName)
        {
        
            string blobAccountKey = _configuration["blobAccountKey"].ToString();
            string blobAccount = _configuration["storageAccount"].ToString();

            string blobConnectionString = string.Format(Constants.BlobConnectionString, blobAccount, blobAccountKey);

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


    }
}