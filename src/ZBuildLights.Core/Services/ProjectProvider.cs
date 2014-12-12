using BuildLightControl;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class ProjectProvider : IProjectProvider
    {
        public Project[] GetCurrentProjects()
        {
            var project1 = new Project { StatusMode = StatusMode.Success, Name = "My Successful Project" };
            project1.AddLight(new LightSwitch(101, 1).SetState(SwitchState.On)).SetColor(LightColor.Green);
            project1.AddLight(new LightSwitch(101, 2).SetState(SwitchState.Off)).SetColor(LightColor.Yellow);
            project1.AddLight(new LightSwitch(101, 3).SetState(SwitchState.Off)).SetColor(LightColor.Red);

            var project2 = new Project { StatusMode = StatusMode.BrokenAndBuilding, Name = "Trying To Fix This One" };
            project2.AddLight(new LightSwitch(101, 4).SetState(SwitchState.Off)).SetColor(LightColor.Green);
            project2.AddLight(new LightSwitch(101, 5).SetState(SwitchState.On)).SetColor(LightColor.Yellow);
            project2.AddLight(new LightSwitch(101, 6).SetState(SwitchState.On)).SetColor(LightColor.Red);

            return new[] {project1, project2};
        }
    }
}