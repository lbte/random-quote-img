using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace random_image_fetcher;

public static class FetchDailyImage
{
    private static readonly HttpClient httpClient = new();

    [FunctionName("FetchDailyImage")]
    public static async Task Run(
        [TimerTrigger("0 0 0 * * *")] TimerInfo timer, // Runs daily at midnight
        ILogger log)
    {
        string unsplashAccessKey = Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_KEY");
        string blobConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(unsplashAccessKey))
        {
            log.LogError("Unsplash API Key is missing.");
            return;
        }

        if (string.IsNullOrEmpty(blobConnectionString))
        {
            log.LogError("Azure Storage Connection String is missing.");
            return;
        }

        // Fetch a random image from Unsplash
        var UNSPLASH_API_URL = "https://api.unsplash.com/photos/random";
        var response = await httpClient.GetAsync($"{UNSPLASH_API_URL}?collections=70076102&orientation=portrait&client_id={unsplashAccessKey}");

        if (!response.IsSuccessStatusCode)
        {
            log.LogError($"Unsplash API error: {response.StatusCode}");
            return;
        }

        var imageData = await response.Content.ReadAsAsync<dynamic>();
        string imageUrl = imageData.urls.regular;

        // Download the image
        var imageResponse = await httpClient.GetAsync(imageUrl);
        var imageContent = await imageResponse.Content.ReadAsByteArrayAsync();

        // Upload the image to Azure Blob Storage
        var blobServiceClient = new BlobServiceClient(blobConnectionString);
        var blobClient = blobServiceClient.GetBlobContainerClient("images").GetBlobClient($"{DateTime.Today:yyyy-MM-dd}.jpg");
        await blobClient.UploadAsync(new MemoryStream(imageContent), overwrite: true);

        log.LogInformation($"Uploaded daily image: {blobClient.Uri}");
    }
}