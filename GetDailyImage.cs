using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace random_image_fetcher;

public static class GetDailyImage
{
    [FunctionName("GetDailyImage")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        string blobConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        // Get the latest image from Azure Blob Storage
        var blobServiceClient = new BlobServiceClient(blobConnectionString);
        var blobClient = blobServiceClient.GetBlobContainerClient("images").GetBlobClient($"{DateTime.Today:yyyy-MM-dd}.jpg");

        // Generate the embeddable HTML snippet
        string imageUrl = blobClient.Uri.ToString();
        string htmlSnippet = $"<img src=\"{imageUrl}\" alt=\"Daily Quote Image\" />";

        return new ContentResult
        {
            Content = htmlSnippet,
            ContentType = "text/html"
        };
    }
}
