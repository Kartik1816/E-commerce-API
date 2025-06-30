using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class SubscribedUserRepository : ISubscribedUserRepository
{
    private readonly UserDbContext _userDbContext;

    public SubscribedUserRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> SubscribeUser(string email)
    {
        try
        {

            bool alreadyExists = await _userDbContext.SubscribedUsers.AnyAsync(su => su.Email == email);

            if (!alreadyExists)
            {
                _userDbContext.SubscribedUsers.Add(new SubscribedUser { Email = email });
                await _userDbContext.SaveChangesAsync();
            }

        return new JsonResult(new { success = true, message = "Subscription processed successfully" });
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while saving subscribed user" + e);
        }
    }

    public async Task<SubscribedUsersModel> GetAllSubScribedUsers()
    {
        try
        {
            return new SubscribedUsersModel
            {
                SubscribedUsers = await _userDbContext.SubscribedUsers.Select(su => su.Email).ToListAsync()
            };
        }
        catch (Exception e)
        {
            throw new Exception("An Exception occured while fetching subscribed users"+e);
        }
    }
}
