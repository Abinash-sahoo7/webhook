/*
This code creates a payment invoice using the Cryptomus payment API. Here's what it does:

1. Sets up the payment endpoint URL for the Cryptomus API.

2. Creates a payment data object with:
   - The order amount (converted to string)
   - Currency set to USD
   - Order ID from the finalOrder object
   - Return URLs for different payment scenarios (success, return, callback)

3. Generates a signature for API authentication:
   - Converts payment data to JSON and encodes it with btoa()
   - Appends the Cryptomus API key
   - Creates an MD5 hash of this combined string for security

4. Sets up request headers with:
   - The merchant ID from environment variables
   - The generated signature

5. Makes an API request to Cryptomus:
   - Sends a POST request with axios
   - Includes the payment data and authentication headers
   - Logs the response for debugging

6. Returns the payment URL from the response if successful

7. Error handling:
   - Catches and logs any errors during the process
   - Returns a failure message
   - Includes a TODO comment about implementing retry logic

Note: There are several TypeScript errors in this code related to missing type definitions
and variables that need to be addressed.
*/

// create invoice
const paymentUrl = "https://api.cryptomus.com/v1/payment";

const paymentData = {
  amount: String(finalOrder.price),
  currency: "USD",
  order_id: String(finalOrder.id),
  url_return: `${process.env.APP_BASE_URL}/payment/return`,
  url_success: `${process.env.APP_BASE_URL}/payment/success`,
  url_callback: `${process.env.APP_BASE_URL}/api/payment/callback`,
};

const stringData =
  btoa(JSON.stringify(paymentData)) + process.env.CRYPTOMUS_API_KEY;
const sign = crypto.createHash("md5").update(stringData).digest("hex");

const headers = {
  merchant: process.env.CRYPTOMUS_MERCHANT_ID,
  sign,
};

try {
  const response = await axios.post(paymentUrl, paymentData, {
    headers,
  });
  console.log("response", response.data);

  return Response.json({ paymentUrl: response.data.result.url });
} catch (err) {
  console.log("error while creating an invoice", err);
  // todo: 1. retry if not then we have undo the order.
  return Response.json({
    message: "Failed to create an invoice",
  });
}
