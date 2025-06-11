using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;

namespace UserManagement.Domain.Repositories.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly UserDbContext _userDbContext;
    public TokenRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public bool SaveToken(Refreshtoken token)
    {
        try
        {
            if (token.Id > 0)
            {
                _userDbContext.Refreshtokens.Update(token);
                _userDbContext.SaveChanges();
                return true;
            }
            else
            {
                _userDbContext.Refreshtokens.Add(token);
                _userDbContext.SaveChanges();
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
    public Refreshtoken GetRefreshtoken(string token)
    {
        try
        {
            return _userDbContext.Refreshtokens.Where(t=>t.Token == token).Include(t=>t.User).FirstOrDefault() ?? new Refreshtoken();
        }
        catch (Exception)
        {
            return new Refreshtoken();
        }
    }
}
