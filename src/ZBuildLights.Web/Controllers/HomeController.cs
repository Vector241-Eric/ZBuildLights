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
                    .AddLight(new SwitchableLight(LightColor.Green, SwitchState.On) {Description = "Project 1 Green"})
                    .AddLight(new SwitchableLight(LightColor.Yellow, SwitchState.Off) {Description = "Project 1 Yellow"})
                    .AddLight(new SwitchableLight(LightColor.Red, SwitchState.Off) {Description = "Project 1 Red"})
                ;

            var project2
                = new Project {StatusMode = StatusMode.BrokenAndBuilding, Name = "Trying To Fix This One"}
                    .AddLight(new SwitchableLight(LightColor.Green, SwitchState.Off) {Description = "Project 2 Green"})
                    .AddLight(new SwitchableLight(LightColor.Yellow, SwitchState.On) {Description = "Project 2 Yellow"})
                    .AddLight(new SwitchableLight(LightColor.Red, SwitchState.On) {Description = "Project 2 Red"})
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