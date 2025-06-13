using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactUsController : ControllerBase
{
    private readonly IAuthService _authService;
    public ContactUsController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("contact")]
    public bool ContactUs([FromBody] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }
        return _authService.IsUserPresent(email);
    }
}
