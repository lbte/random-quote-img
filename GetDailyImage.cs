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

        string imageUrl = $"{blobClient.Uri}?v={DateTime.UtcNow.Ticks}"; // Cache-busting URL

        // Return a full HTML page for better embedding support
        string htmlPage = $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Daily Quote Image</title>
            <style>
                body {{
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    margin: 0;
                    background-color: #f8f9fa;
                }}
                img {{
                    max-width: 100%;
                    height: auto;
                    border-radius: 10px;
                }}
            </style>
        </head>
        <body>
            <img src='{imageUrl}' alt='Daily Quote Image' />
        </body>
        </html>";


        return new ContentResult
        {
            Content = htmlPage,
            ContentType = "text/html"
        };
    }
}
