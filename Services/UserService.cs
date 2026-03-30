using Microsoft.AspNetCore.Identity;
using Ledgr.Models;

namespace Ledgr.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetCurrentUserAsync(string userId);
    Task UpdateLastLoginAsync(string userId);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
}

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task UpdateLastLoginAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
    {
        await _userManager.UpdateAsync(user);
        return user;
    }
}