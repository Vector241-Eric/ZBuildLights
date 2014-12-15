using System.Web.Mvc;

namespace ZBuildLights.Web.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(string groupName)
        {
            return Json(new {Success = true});
        }
    }
}