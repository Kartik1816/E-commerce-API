using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Models;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.utils;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.Repositories.Repositories;

public class SubscribedUserRepository : ISubscribedUserRepository
{
    private readonly UserDbContext _userDbContext;
    private readonly ResponseHandler _responseHandler;
    public SubscribedUserRepository(UserDbContext userDbContext, ResponseHandler responseHandler)
    {
        _userDbContext = userDbContext;
        _responseHandler = responseHandler;
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

            return new OkObjectResult(_responseHandler.Success(CustomErrorMessage.SubscribeUserSuccess, null));
        }
        catch (Exception e)
        {
            throw new Exception(CustomErrorMessage.SubscribeUserError + e);
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
            throw new Exception(CustomErrorMessage.FetchSubscribedUsersError + e);
        }
    }
}
