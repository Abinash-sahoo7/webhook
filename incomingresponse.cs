/*
# Secure Webhook Receiver Implementation in C#

This code implements a secure webhook endpoint that:

1. Receives incoming webhook requests via HTTP POST
2. Verifies their authenticity using HMAC-SHA256 signatures
3. Rejects unauthorized requests with a 401 Unauthorized response
4. Accepts valid requests with a 200 OK response

## Key Components:

- Microsoft.AspNetCore.Mvc: For API controller functionality
- System.Security.Cryptography: For HMAC signature verification
- System.IO: For reading the request body

## Security Implementation:

The code uses a shared secret key between sender and receiver to verify
HMAC signatures, ensuring:
- Data integrity (detecting if data was modified in transit)
- Authentication (verifying the sender's identity)
- Protection against replay attacks and tampering

## Workflow:

1. Receives an HTTP POST request at the /webhook endpoint
2. Reads the entire request body as a string
3. Extracts the signature from the X-Signature header
4. Computes the expected HMAC-SHA256 signature using the shared secret key
5. Compares the received signature with the computed signature
6. If signatures match:
   - Logs success message
   - Returns 200 OK response
7. If signatures don't match:
   - Returns 401 Unauthorized response
   - Prevents processing potentially tampered data
*/

using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[Route("webhook")]
[ApiController]
public class WebhookController : ControllerBase
{
    private const string SecretKey = "your-secret-key"; // Same secret key used by sender

    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook()
    {
        using (var reader = new StreamReader(Request.Body))
        {
            string payload = await reader.ReadToEndAsync();
            string receivedSignature = Request.Headers["X-Signature"];

            string computedSignature = GenerateHmacSignature(payload, SecretKey);

            if (receivedSignature != computedSignature)
            {
                return Unauthorized("Invalid signature"); // Signature mismatch = possible tampering
            }

            Console.WriteLine("Webhook received and verified successfully");
            return Ok("Webhook processed");
        }
    }

    private string GenerateHmacSignature(string data, string secret)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
