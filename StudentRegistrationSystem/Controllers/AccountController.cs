using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationSystem.ViewModels;
using StudentRegistrationSystem.Models;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    
   
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

 
    [HttpPost]
    [ValidateAntiForgeryToken]
    
    public async Task<IActionResult> Login(string email, string password)
    {
        
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            ModelState.AddModelError("", "User not found");
            return View();
        }

       
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName, password, false, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ModelState.AddModelError("", "Invalid login attempt");
        return View();
    }

    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string name, string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = name
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "User");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }
}