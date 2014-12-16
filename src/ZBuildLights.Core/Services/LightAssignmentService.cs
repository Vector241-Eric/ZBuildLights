using BuildLightControl;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class LightAssignmentService : ILightAssignmentService
    {
        public LightGroup GetUnassignedLights()
        {
            var lightGroup = new LightGroup {Name = "Unassigned"};
            lightGroup
                .AddLight(new Light{ZWaveHomeId = 1, ZWaveDeviceId = 50})
                .AddLight(new Light{ZWaveHomeId = 1, ZWaveDeviceId = 51, SwitchState = SwitchState.Off})
                .AddLight(new Light{ZWaveHomeId = 1, ZWaveDeviceId = 52, SwitchState = SwitchState.Off})
                .AddLight(new Light{ZWaveHomeId = 1, ZWaveDeviceId = 53, SwitchState = SwitchState.On})
                ;
            return lightGroup;
        }
    }
}