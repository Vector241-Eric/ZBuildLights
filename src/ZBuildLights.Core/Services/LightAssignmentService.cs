using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class LightAssignmentService : ILightAssignmentService
    {
        public LightGroup GetUnassignedLights()
        {
            var lightGroup = new LightGroup {Name = "Unassigned"};
            lightGroup
                .AddLight(new Light(1, 50))
                .AddLight(new Light(1, 51) {SwitchState = SwitchState.Off})
                .AddLight(new Light(1, 52) {SwitchState = SwitchState.Off})
                .AddLight(new Light(1, 53) {SwitchState = SwitchState.On})
                ;
            return lightGroup;
        }
    }
}