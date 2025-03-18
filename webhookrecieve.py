# Webhook Receiver Implementation with HMAC Signature Verification
#
# This code demonstrates a secure webhook endpoint implementation that:
# 1. Receives incoming webhook requests
# 2. Verifies their authenticity using HMAC signatures
# 3. Rejects unauthorized requests with a 403 Forbidden response
# 4. Accepts valid requests with a 200 OK response
#
# The implementation uses:
# - hmac: For creating and comparing HMAC signatures
# - hashlib: For cryptographic hash functions (SHA-256)
# - Flask: Web framework for handling HTTP requests
# - request: For accessing request data and headers
# - abort: For returning HTTP error responses

import hmac 
import hashlib
from flask import Flask, request, abort

# Secret key shared between webhook sender and receiver
# This must match the key used by the sender to generate signatures
SECRET_KEY = "your-secret-key"  
# Shared key from Altura Health  

# Flask application setup
app = Flask(__name__)

@app.route('/webhook', methods=['POST'])
def webhook():
    # Get signature from request headers
    received_signature = request.headers.get("X-Signature")
    
    # Get raw request body
    payload = request.data
    
    # Compute expected HMAC signature
    expected_signature = hmac.new(SECRET_KEY.encode(), payload, hashlib.sha256).hexdigest()
    
    # Compare received signature with expected signature
    if not hmac.compare_digest(received_signature, expected_signature):
        abort(403)  # Reject unauthorized request
        
    # Process the webhook data here
    return "Webhook received securely", 200

# Security summary:
# ✅ Only valid webhooks with the correct signature will be accepted.
# ❌ Invalid or tampered requests will be rejected (403 Forbidden).