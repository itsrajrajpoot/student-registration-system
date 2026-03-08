using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult AccessDenied()
    {
        return View();
    }
}