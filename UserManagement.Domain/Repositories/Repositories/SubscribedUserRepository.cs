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

            SubscribedUser? subscribedUser = await _userDbContext.SubscribedUsers.FirstOrDefaultAsync(su => su.Email == email);
            if (subscribedUser == null)
            {
                SubscribedUser newSubscribedUser = new()
                {
                    Email = email
                };
                _userDbContext.SubscribedUsers.Add(newSubscribedUser);
                await _userDbContext.SaveChangesAsync();
            }
            return new JsonResult(new { success = true, message = "User Subscribed successfully" });
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
