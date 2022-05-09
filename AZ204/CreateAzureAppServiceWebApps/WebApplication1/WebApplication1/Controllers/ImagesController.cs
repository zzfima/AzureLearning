using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication1
{
    [ApiController]
    [Route("/")]
    public class ImagesController : ControllerBase
    {
        private readonly Options _options;

        public ImagesController(Options options)
        {
            _options = options;
        }

        private async Task<BlobContainerClient> GetCloudBlobContainer(string containerName)
        {
            BlobServiceClient serviceClient = new BlobServiceClient(_options.StorageConnectionString);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }

        [Route("/")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            BlobContainerClient containerClient = await GetCloudBlobContainer(_options.FullImageContainerName);
            List<string> results = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                results.Add(
                    Flurl.Url.Combine(
                        containerClient.Uri.AbsoluteUri,
                        blobItem.Name
                    )
                );
            }

            Console.WriteLine("Got Images");
            return Ok(results);
        }

        [Route("/")]
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            Stream image = Request.Body;
            BlobContainerClient containerClient = await GetCloudBlobContainer(_options.FullImageContainerName);
            string blobName = Guid.NewGuid().ToString().ToLower().Replace("-", String.Empty);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(image);
            return Created(blobClient.Uri, null);
        }
    }
}