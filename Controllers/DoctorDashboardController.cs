using Microsoft.AspNetCore.Mvc;

public class DoctorDashboardController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Doctor")
            return RedirectToAction("LoginDoctor", "Login");

        return View();
    }
}
