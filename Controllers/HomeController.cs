using Microsoft.AspNetCore.Mvc;

namespace MunicipalityApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
