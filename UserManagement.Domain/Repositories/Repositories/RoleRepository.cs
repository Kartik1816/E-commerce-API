using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;

namespace UserManagement.Domain.Repositories.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly UserDbContext _userDbContext;
    
    public RoleRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    
    public Role GetRoleById(int roleId)
    {
        try
        {
            Role role = _userDbContext.Roles.FirstOrDefault(r => r.Id == roleId) ?? new Role();
            if (role.Name != null)
            {
                return role;
            }
            else
            {
                return null!;
            }
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching Role" + e);
        }
    }
}
