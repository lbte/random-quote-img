from flask import Flask, jsonify
from flask_cors import CORS
import requests
import os
from dotenv import load_dotenv
from apscheduler.schedulers.background import BackgroundScheduler
from datetime import datetime

# Load environment variables from .env file
load_dotenv()

app = Flask(__name__)

# Enable CORS for all routes
# CORS(app, origins=["http://frontend-domain.com"]) -> limit the access
CORS(app)

# Unsplash API configuration
UNSPLASH_ACCESS_KEY = os.getenv("UNSPLASH_ACCESS_KEY")
if not UNSPLASH_ACCESS_KEY:
    raise ValueError("Unsplash API key is missing. Set it in the .env file.")

UNSPLASH_API_URL = "https://api.unsplash.com/photos/random"

# Cache for the daily image
cached_image = {"url": None, "last_updated": None}

def fetch_new_image():
    """
    Fetch a new random image from the Unsplash API and update the cache.
    """
    global cached_image
    try:
        response = requests.get(UNSPLASH_API_URL, params={
            "client_id": UNSPLASH_ACCESS_KEY,
            "collections": "70076102",
            "orientation": "portrait"
        })
        response.raise_for_status()
        data = response.json()
        cached_image["url"] = data["urls"]["regular"]
        cached_image["last_updated"] = datetime.now()
        print(f"Image updated at {cached_image['last_updated']}")
    except requests.exceptions.RequestException as e:
        print(f"Error fetching image: {e}")

# Schedule the image fetching task
scheduler = BackgroundScheduler()
# Schedule the fetch to run daily at 8:00 AM
scheduler.add_job(fetch_new_image, 'cron', hour=8, minute=0, second=0)
scheduler.start()

@app.route('/random-image', methods=['GET'])
def get_random_image():
    """
    Serve the cached image.
    """
    if cached_image["url"] is None:
        return jsonify({"error": "Image not available yet. Please try again later."}), 503

    return jsonify({
        "image_url": cached_image["url"],
        "last_updated": cached_image["last_updated"]
    })

if __name__ == '__main__':
    # Fetch an image immediately on startup
    fetch_new_image()
    app.run()