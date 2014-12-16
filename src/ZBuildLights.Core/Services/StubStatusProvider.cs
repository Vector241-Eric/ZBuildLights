using System;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class StubStatusProvider : IStatusProvider
    {
        public Project[] GetCurrentProjects()
        {
            var core = new Project {StatusMode = StatusMode.Success, Name = "Core", Id = Guid.NewGuid()};
            core.AddGroup(new LightGroup { Name = "SnP Square", Id = Guid.NewGuid() })
                .AddLight(new Light(1, 1) { Color = LightColor.Green, SwitchState = SwitchState.On, Id = Guid.NewGuid() })
                .AddLight(new Light(1, 2) { Color = LightColor.Yellow, SwitchState = SwitchState.Off, Id = Guid.NewGuid() })
                .AddLight(new Light(1, 3) { Color = LightColor.Red, SwitchState = SwitchState.Off, Id = Guid.NewGuid() })
                ;
            core.AddGroup(new LightGroup { Name = "SnP Near Matt", Id = Guid.NewGuid() })
                .AddLight(new Light(1, 4) { Color = LightColor.Green, SwitchState = SwitchState.On, Id = Guid.NewGuid() })
                .AddLight(new Light(1, 5) { Color = LightColor.Yellow, SwitchState = SwitchState.Off, Id = Guid.NewGuid() })
                .AddLight(new Light(1, 6) { Color = LightColor.Red, SwitchState = SwitchState.Off, Id = Guid.NewGuid() })
                ;

            var apps = new Project { StatusMode = StatusMode.BrokenAndBuilding, Name = "Apps", Id = Guid.NewGuid() };
            apps.AddGroup(new LightGroup { Name = "Near Window", Id = Guid.NewGuid() })
                .AddLight(new Light(1, 7) { Color = LightColor.Green, SwitchState = SwitchState.Off, Id = Guid.NewGuid() })
                .AddLight(new Light(1, 8) { Color = LightColor.Yellow, SwitchState = SwitchState.On, Id = Guid.NewGuid() })
                .AddLight(new Light(1, 9) { Color = LightColor.Red, SwitchState = SwitchState.On, Id = Guid.NewGuid() })
                ;

            return new[] {core, apps};
        }
    }
}