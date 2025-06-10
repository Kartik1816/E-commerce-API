using UserManagement.Domain.Models;

namespace UserManagement.Services.JWT;

public interface IGenerateJwt
{
    string GenerateJwtToken(User user,string role);
    int GetUserIdFromJwtToken(string token);
}
