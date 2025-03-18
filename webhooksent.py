# This code demonstrates secure webhook implementation with HMAC signatures

# Generate a secure random key using the secrets module
import secrets 
secret_key = secrets.token_hex(32)  # 32-byte secure key
print(secret_key)  # Prints the generated secure random hexadecimal key

# Import necessary libraries for webhook implementation
import hmac      # For creating HMAC signatures
import hashlib   # For cryptographic hash functions
import requests  # For making HTTP requests
import json      # For JSON handling

# The commented code below shows a complete webhook implementation:

# 1. A secret key would be shared between sender and recipient for verification
SECRET_KEY = "your-secret-key" 

# 2. Example webhook data payload (in this case for a healthcare application)
webhook_data = {
    "event": "patient_updated",
    "patient_id": "123456",
    "updated_fields": ["diagnosis", "treatment"],
    "timestamp": "2025-03-14T12:34:56Z"
}

# 3. Convert the payload to a minified JSON string and encode to bytes
payload = json.dumps(webhook_data, separators=(',', ':')).encode("utf-8")  

# 4. Generate HMAC signature using SHA-256 for security verification
signature = hmac.new(SECRET_KEY.encode(), payload, hashlib.sha256).hexdigest()  

# 5. Prepare HTTP headers including the signature for verification
headers = {
    "X-Signature": signature,  # HMAC signature
    "Content-Type": "application/json" 
}  

# 6. Send the webhook POST request to the recipient server
webhook_url = "https://recipient-server.com/webhook" 
response = requests.post(webhook_url, headers=headers, data=payload) 
print(f"Webhook sent, response: {response.status_code}")

