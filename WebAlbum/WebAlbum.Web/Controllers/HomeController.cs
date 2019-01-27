using System.Web.Mvc;

namespace WebAlbum.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            TempData["error"] = "Your file exceeds the maximum upload size (4MB).";
            return View("Error");
        }
    }
}