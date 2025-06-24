using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using UserManagement.Domain.ViewModels;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _config;

    private readonly IOrderService _orderService;

    public PaymentController(IConfiguration config, IOrderService orderService)
    {
        _config = config;
        _orderService = orderService;
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] PaymentRequest orderRequest)
    {
        if (orderRequest.UserId <= 0)
        {
            return BadRequest("User not found");
        }

        // Get RazorPay Keys from appsettings.json
        string keyId = _config["RazorPay:KeyId"];
        string keySecret = _config["RazorPay:KeySecret"];

        if (orderRequest.Amount < 1)
        {
            return BadRequest("Amount must be at least 1 INR");
        }

        RazorpayClient client = new RazorpayClient(keyId, keySecret);

        // Create Order
        Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", (int)(orderRequest.Amount * 100) }, // RazorPay expects amount in paise (1 INR = 100 paise)
            { "currency", "INR" },
            { "receipt", "order_" + DateTime.Now.Ticks },
            { "payment_capture", 1 } // Auto-capture payment
        };

        Order order = client.Order.Create(options);

        int newOrderId = await _orderService.CreateNewOrder(orderRequest.UserId, orderRequest.Amount);
        if (newOrderId <= 0)
        {
            return new JsonResult(new { success = false, message = "Invalid userId or Amount" });
        }

        return Ok(new { orderId = order["id"], orderModelId = newOrderId });
    }

    [HttpPost("verify-payment")]
    public IActionResult VerifyPayment([FromBody] PaymentVerificationRequest request)
    {
        string keySecret = _config["RazorPay:KeySecret"];
        
    //     bool isValid = VerifySignature(
    //         keySecret, 
    //         request.OrderId, 
    //         request.PaymentId, 
    //         request.Signature
    //     );

    // if (!isValid)
    // {
    //     return BadRequest("Payment verification failed: Invalid signature.");
    // }

      return  _orderService.UpdateOrderStatus(request.OrderModelId);
    
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
