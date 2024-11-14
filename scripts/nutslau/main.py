import requests
import csv
import logging

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

def fetch_postal_data(filename="postal_data.csv"):
    base_url = "https://api.basisregisters.staging-vlaanderen.be/v2/postinfo"
    is_header_written = False  # Track if CSV header has been written

    # Loop through all pages
    url = base_url
    page = 1
    while url:
        logging.info(f"Fetching page {page} data from {url}")
        response = requests.get(url)
        response_data = response.json()

        # Process each postal code in the current page
        postal_info_page = []
        for post_info_obj in response_data.get("postInfoObjecten", []):
            detail_url = post_info_obj["detail"]
            postal_data = fetch_postal_detail(detail_url)
            if postal_data:  # Only add if both nuts3 and gemeente information is available
                postal_info_page.append(postal_data)

        # Save the current page to CSV
        save_to_csv(postal_info_page, filename, write_header=not is_header_written)
        is_header_written = True  # Header has been written after the first page

        # Get the next page URL, if available
        url = response_data.get("volgende")
        page += 1

    logging.info("Completed fetching all postal data")

def fetch_postal_detail(detail_url):
    """Fetch postal code and concatenated nuts3+gemeente_objectId if both are available."""
    logging.info(f"Fetching detail data from {detail_url}")
    response = requests.get(detail_url)
    data = response.json()

    postal_code = data["identificator"]["objectId"]
    nuts3 = data.get("nuts3", None)
    
    # Check if both nuts3 code and gemeente objectId are available
    gemeente_object_id = data.get("gemeente", {}).get("objectId", None)
    if not nuts3 or not gemeente_object_id:
        logging.warning(f"Skipping entry for postal code {postal_code}: Missing nuts3 or gemeente_objectId")
        return None

    # Concatenate nuts3 and gemeente_object_id
    nuts3_objectid = f"{nuts3}{gemeente_object_id}"
    logging.debug(f"Fetched data for postal code {postal_code}: nuts3_objectid={nuts3_objectid}")

    return {
        "postal_code": postal_code,
        "nuts3_objectid": nuts3_objectid
    }

def save_to_csv(postal_data, filename, write_header=False):
    """Save a batch of postal data to a CSV file."""
    logging.info(f"Saving data to {filename}")
    fieldnames = ["postal_code", "nuts3_objectid"]
    mode = "a" if write_header else "a"  # Always append after the first header write

    with open(filename, mode=mode, newline="") as file:
        writer = csv.DictWriter(file, fieldnames=fieldnames)
        if write_header:
            writer.writeheader()
        writer.writerows(postal_data)
    logging.info(f"Page data saved to {filename}")

if __name__ == "__main__":
    fetch_postal_data()
    logging.info("Script completed")
