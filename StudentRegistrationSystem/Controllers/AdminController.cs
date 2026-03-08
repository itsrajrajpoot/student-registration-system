using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace StudentRegistrationSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Users()
    {
        return View(_userManager.Users.ToList());
    }

    public async Task<IActionResult> ManageRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        ViewBag.User = user;

        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        ViewBag.Roles = roles;
        ViewBag.UserRoles = userRoles;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ManageRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (!await _userManager.IsInRoleAsync(user, role))
            await _userManager.AddToRoleAsync(user, role);

        TempData["Success"] = "Role Assigned!";
        return RedirectToAction("Users");
    }
}