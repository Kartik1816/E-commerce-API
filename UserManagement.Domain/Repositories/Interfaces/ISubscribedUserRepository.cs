using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Interfaces;

public interface ISubscribedUserRepository
{
    public Task<IActionResult> SubscribeUser(string email);

    public Task<SubscribedUsersModel> GetAllSubScribedUsers();
}
