using BuildLightControl;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class StubStatusProvider : IStatusProvider
    {
        public Project[] GetCurrentProjects()
        {
            var core = new Project {StatusMode = StatusMode.Success, Name = "Core"};
            core.AddGroup(new LightGroup {Name = "SnP Square"})
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 1, Color = LightColor.Green, SwitchState = SwitchState.On })
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 2, Color = LightColor.Yellow, SwitchState = SwitchState.Off })
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 3, Color = LightColor.Red, SwitchState = SwitchState.Off })
                ;
            core.AddGroup(new LightGroup {Name = "SnP Near Matt"})
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 4, Color = LightColor.Green, SwitchState = SwitchState.On })
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 5, Color = LightColor.Yellow, SwitchState = SwitchState.Off })
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 6, Color = LightColor.Red, SwitchState = SwitchState.Off })
                ;

            var apps = new Project {StatusMode = StatusMode.BrokenAndBuilding, Name = "Apps"};
            apps.AddGroup(new LightGroup {Name = "Near Window"})
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 7, Color = LightColor.Green, SwitchState = SwitchState.Off })
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 8, Color = LightColor.Yellow, SwitchState = SwitchState.On })
                .AddLight(new Light { ZWaveHomeId = 1, ZWaveDeviceId = 9, Color = LightColor.Red, SwitchState = SwitchState.On })
                ;

            return new[] {core, apps};
        }
    }
}