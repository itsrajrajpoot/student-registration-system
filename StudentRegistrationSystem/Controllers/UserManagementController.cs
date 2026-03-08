using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace StudentRegistrationSystem.Controllers;

[Authorize(Roles = "Admin")]
public class UserManagementController(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    public IActionResult Register()
    {
        ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string role)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
            TempData["Success"] = "User created successfully!";
            return RedirectToAction("Register");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();
        return View();
    }
}