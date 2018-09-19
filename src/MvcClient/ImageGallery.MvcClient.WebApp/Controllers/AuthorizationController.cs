using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.MvcClient.WebApp.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
