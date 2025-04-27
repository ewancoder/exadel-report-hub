using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Auth.ServiceHost.Controllers
{
    public class ACLController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
