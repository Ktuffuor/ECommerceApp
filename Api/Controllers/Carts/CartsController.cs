using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Carts;

public class CartsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}