using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using UserManagement.Domain.ViewModels;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _config;

    public PaymentController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("create-order")]
    public IActionResult CreateOrder([FromBody] PaymentRequest orderRequest)
    {
        // Get RazorPay Keys from appsettings.json
        string keyId = _config["RazorPay:KeyId"];
        string keySecret = _config["RazorPay:KeySecret"];

        RazorpayClient client = new RazorpayClient(keyId, keySecret);

        // Create Order
        Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", orderRequest.Amount * 100 }, // RazorPay expects amount in paise (1 INR = 100 paise)
            { "currency", "INR" },
            { "receipt", "order_" + DateTime.Now.Ticks },
            { "payment_capture", 1 } // Auto-capture payment
        };

        Order order = client.Order.Create(options);

        return Ok(new { orderId = order["id"] });
    }

    [HttpPost("verify-payment")]
    public IActionResult VerifyPayment([FromBody] PaymentVerificationRequest request)
    {
        string keySecret = _config["RazorPay:KeySecret"];
        
        bool isValid = VerifySignature(
            keySecret, 
            request.OrderId, 
            request.PaymentId, 
            request.Signature
        );

    if (!isValid)
    {
        return BadRequest("Payment verification failed: Invalid signature.");
    }

    // If signature is valid, update your database (e.g., mark order as paid)
    // Example: _orderService.MarkOrderAsPaid(request.OrderId);
    
    return Ok(new { success = true, message = "Payment verified successfully!" });
    }

private bool VerifySignature(string keySecret, string orderId, string paymentId, string razorpaySignature)
{
    string payload = $"{orderId}|{paymentId}";
    string secret = keySecret;
    
    byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
    byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
    
    using (var hmac = new HMACSHA256(secretBytes))
    {
        byte[] hash = hmac.ComputeHash(payloadBytes);
        string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
        
        return generatedSignature == razorpaySignature;
    }
}

}
