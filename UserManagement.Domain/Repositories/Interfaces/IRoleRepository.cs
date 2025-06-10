using UserManagement.Domain.Models;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface IRoleRepository
{
    public Role GetRoleById(int roleId);
}
