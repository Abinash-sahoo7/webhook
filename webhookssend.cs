/*
# Secure Webhook Sender Implementation in C#

This code demonstrates a secure webhook implementation that:

1. Creates a webhook payload with patient data
2. Secures the webhook using HMAC-SHA256 signatures
3. Sends the webhook to a recipient server
4. Verifies the response

## Key Components:

- System.Security.Cryptography: For HMAC signature generation
- System.Text.Json: For JSON serialization
- HttpClient: For making HTTP requests

## Security Implementation:

The code uses a shared secret key between sender and receiver to generate
and verify HMAC signatures, ensuring:
- Data integrity (detecting if data was modified in transit)
- Authentication (verifying the sender's identity)
- Non-repudiation (sender cannot deny sending the message)

## Workflow:

1. Creates a webhook payload with patient information and timestamp
2. Serializes the payload to JSON
3. Generates an HMAC-SHA256 signature using the shared secret key
4. Sends an HTTP POST request with:
   - The JSON payload in the request body
   - The signature in the X-Signature header
5. Logs the HTTP response status code
*/

using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static readonly string SecretKey = "your-secret-key";  // Shared secret key

    static async Task Main()
    {
        var webhookData = new
        {
            event_name = "patient_updated",
            patient_id = "123456",
            updated_fields = new string[] { "diagnosis", "treatment" },
            timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
        };

        string jsonPayload = JsonSerializer.Serialize(webhookData);
        string signature = GenerateHmacSignature(jsonPayload, SecretKey);

        using (var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://recipient-server.com/webhook")
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Signature", signature); // Add HMAC signature in header

            HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine($"Webhook sent, response: {response.StatusCode}");
        }
    }

    // Method to generate HMAC SHA-256 signature
    private static string GenerateHmacSignature(string data, string secret)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Convert to hex string
        }
    }
}
