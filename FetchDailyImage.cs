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
    private static readonly HttpClient httpClient = new HttpClient();

    [FunctionName("FetchDailyImage")]
    public static async Task Run(
        [TimerTrigger("0 8 21 * * *")] TimerInfo myTimer, // Runs daily at midnight
        ILogger log)
    {
        string unsplashAccessKey = Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_KEY");
        string blobConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        // Fetch a random image from Unsplash
        var UNSPLASH_API_URL = "https://api.unsplash.com/photos/random";
        var response = await httpClient.GetAsync($"{UNSPLASH_API_URL}?collections=70076102&orientation=portrait&client_id={unsplashAccessKey}");
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