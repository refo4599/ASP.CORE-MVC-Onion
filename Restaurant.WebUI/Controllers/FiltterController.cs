using Microsoft.AspNetCore.Mvc;
using Restaurant.WebUI.Filleter;


namespace Restaurant.WebUI.Controllers
{
    public class FiltterController : Controller
    {

        [ValidateNameFilter]
        public IActionResult Index(string name) 
        {
            return View("Success", name);
        }

        [HandleError] 
        public IActionResult Error()
        {
            throw new Exception("Error    !");
        }
    }
}
