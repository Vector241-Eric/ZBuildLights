using System.Web.Mvc;
using BuildLightControl;
using ZBuildLights.Core.Models;
using ZBuildLights.Web.Models.Home;

namespace ZBuildLights.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var project1
                = new Project {StatusMode = StatusMode.Success, Name = "My Successful Project"}
                    .AddLight(new Light(LightColor.Green, LightStatus.On) {Description = "Project 1 Green"})
                    .AddLight(new Light(LightColor.Yellow, LightStatus.Off) {Description = "Project 1 Yellow"})
                    .AddLight(new Light(LightColor.Red, LightStatus.Off) {Description = "Project 1 Red"})
                ;

            var project2
                = new Project {StatusMode = StatusMode.BrokenAndBuilding, Name = "Trying To Fix This One"}
                    .AddLight(new Light(LightColor.Green, LightStatus.Off) {Description = "Project 2 Green"})
                    .AddLight(new Light(LightColor.Yellow, LightStatus.On) {Description = "Project 2 Yellow"})
                    .AddLight(new Light(LightColor.Red, LightStatus.On) {Description = "Project 2 Red"})
                ;

            var model = new ViewModel
            {
                Projects = new[] {project1, project2}
            };
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}