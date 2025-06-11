using UserManagement.Domain.Models;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface ITokenRepository
{
    public bool SaveToken(Refreshtoken token);
    public Refreshtoken GetRefreshtoken(string token);
}
