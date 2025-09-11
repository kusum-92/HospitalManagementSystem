using Microsoft.AspNetCore.Mvc;

public class PatientDashboardController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Patient")
            return RedirectToAction("LoginPatient", "Login");

        return View();
    }
}

