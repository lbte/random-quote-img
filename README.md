# **Daily Quote Image**  
A **serverless API** that delivers a **random quote image** daily, fetched from Unsplash and cached in Azure Blob Storage. The API provides an **embeddable HTML snippet**, allowing seamless integration into tools like Notion.  

## 🚀 **Features**
- **Automated Daily Image Fetching**: A scheduled Azure Function retrieves a **new quote image** from Unsplash every day.  
- **Cached Image Delivery**: The API serves the **stored image**, reducing API calls to Unsplash.  
- **Embeddable HTML Snippet**: Easily **integrate the image into Notion, websites, or other platforms**.  
- **CI/CD Deployment**: Uses **GitHub Actions** to automatically deploy to Azure.  

---

## 🛠 **Technology Stack**
- **Azure Functions** (Serverless backend)
- **Azure Blob Storage** (Caching images)
- **Unsplash API** (Image source)
- **GitHub Actions** (CI/CD for deployment)
- **.NET 8** (Backend implementation)

---

## 📌 **Architecture**
### **1️⃣ Timer-Triggered Function (`FetchDailyImage`)**  
- Runs **once per day** (CRON schedule)  
- Fetches a **random quote image** from Unsplash  
- Saves it to **Azure Blob Storage**  

### **2️⃣ HTTP-Triggered Function (`GetDailyImage`)**  
- Responds to **HTTP requests**  
- Retrieves the **cached image** from Blob Storage  
- Returns an **HTML snippet** for embedding  

---

## 🔧 **Setup & Installation**
### **Prerequisites**
- **Azure Account**  
- **Azure Functions Core Tools**  
- **.NET 8 SDK**  
- **GitHub CLI** (optional for CI/CD)  
- **Unsplash API Access Key**  

### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/lbte/random-quote-img.git
cd random-quote-img
```

### **2️⃣ Configure Environment Variables**
Create a local `local.settings.json` file inside the DailyImageFunctionApp project:

``` json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "UNSPLASH_ACCESS_KEY": "<YourUnsplashAccessKey>",
    "AZURE_STORAGE_CONNECTION_STRING": "<YourBlobStorageConnectionString>"
  }
}
```

### **3️⃣ Run Locally**
```sh
func start
```

The HTTP function will be available at:

👉 http://localhost:7071/api/daily-image

## 🎯 Usage
Get the Daily Quote Image
* Endpoint:
  ```http
  GET /api/daily-image
  ```
* Response: (Embeddable HTML snippet)
  ```html
  <img src="https://your-storage-url/daily-image.jpg" alt="Daily Quote Image">
  ```

## 🛠 CI/CD with GitHub Actions
The project uses GitHub Actions for automated deployment.
The workflow:

1. Builds & publishes the .NET app
2. Deploys to Azure Functions
3. Manages secrets using Azure Entra ID

To trigger deployment, push changes to main:

```sh
git push origin main
```