using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace StudentRegistrationSystem.ViewModels;

[Authorize(Roles = "SystemAdmin")]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();

        var allowedRoles = new List<string>
    {
        "SystemAdmin",
        "ViewStudent",
        "AddStudent",
        "EditStudent",
        "DeleteStudent"
    };

        var model = new List<UserWithRolesViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var filteredRoles = roles
                .Where(r => allowedRoles.Contains(r))
                .ToList();

            model.Add(new UserWithRolesViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Roles = filteredRoles
            });
        }

        return View(model);
    }


    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var model = new List<UserRoleViewModel>();

        string[] allowedRoles =
        {
            "ViewStudent",
            "AddStudent",
            "EditStudent",
            "DeleteStudent"
        };

        foreach (var role in allowedRoles)
        {
            model.Add(new UserRoleViewModel
            {
                RoleName = role,
                IsSelected = await _userManager.IsInRoleAsync(user, role)
            });
        }

        ViewBag.UserId = userId;
        ViewBag.Email = user.Email;
        
        ViewBag.UserName = user.FullName;


        return View(model);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoles(
    string userId,
    List<UserRoleViewModel> model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        
        var existingRoles = await _userManager.GetRolesAsync(user);


        await _userManager.RemoveFromRolesAsync(user, existingRoles);

        
        var selectedRoles = model
            .Where(r => r.IsSelected)
            .Select(r => r.RoleName)
            .ToList();

        
        if (selectedRoles.Contains("EditStudent") ||
            selectedRoles.Contains("DeleteStudent"))
        {
            if (!selectedRoles.Contains("ViewStudent"))
            {
                selectedRoles.Add("ViewStudent");
            }
        }

        await _userManager.AddToRolesAsync(user, selectedRoles);

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        await _userManager.DeleteAsync(user);

        TempData["Success"] = "User removed successfully.";

        return RedirectToAction(nameof(Index));
    }


}