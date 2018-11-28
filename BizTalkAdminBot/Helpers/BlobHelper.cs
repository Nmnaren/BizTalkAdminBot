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

namespace BizTalkAdminBot.Helpers
{
    public class BlobHelper
    {
        private IConfiguration _configuration;
        public BlobHelper(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public bool UploadReportToBlob(string report, string reportName, out string blobName)
        {
            string blobAccountKey = _configuration["blobAccountKey"].ToString();

            string blobConnectionString = string.Format(Constants.BlobConnectionString, blobAccountKey);

            CloudStorageAccount account = CloudStorageAccount.Parse(blobConnectionString);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(Constants.BlobContainerName);
            container.CreateIfNotExistsAsync();

            blobName = string.Format("{0}_{1}", reportName, Guid.NewGuid().ToString());

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            
            byte[] byteArray = Encoding.ASCII.GetBytes( report );
            MemoryStream stream = new MemoryStream( byteArray );

            return true;

            

        }


    }
}