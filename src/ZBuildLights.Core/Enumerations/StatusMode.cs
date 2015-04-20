using System.Linq;
using Headspring;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Enumerations
{
    public class StatusMode : Enumeration<StatusMode, int>
    {
        public static readonly StatusMode Success = new StatusMode(1, "Success", LightColor.Green);
        public static readonly StatusMode SuccessAndBuilding = new StatusMode(2, "SuccessAndBuilding", LightColor.Green, LightColor.Yellow);
        public static readonly StatusMode Broken = new StatusMode(3, "Broken", LightColor.Red);
        public static readonly StatusMode BrokenAndBuilding = new StatusMode(4, "BrokenAndBuilding", LightColor.Red, LightColor.Yellow);
        public static readonly StatusMode NotConnected = new StatusMode(50, "NotConnected", LightColor.Green, LightColor.Yellow, LightColor.Red);
        public static readonly StatusMode Off = new StatusMode(100, "Off");

        private StatusMode(int id, string displayName, params LightColor[] lights)
            : base(id, displayName)
        {
            StatusLightConfiguration = new StatusLightConfiguration();
            StatusLightConfiguration[LightColor.Green] = lights.Contains(LightColor.Green) ? SwitchState.On: SwitchState.Off;
            StatusLightConfiguration[LightColor.Yellow] = lights.Contains(LightColor.Yellow) ? SwitchState.On: SwitchState.Off;
            StatusLightConfiguration[LightColor.Red] = lights.Contains(LightColor.Red) ? SwitchState.On: SwitchState.Off;
        }

        public StatusLightConfiguration StatusLightConfiguration { get; private set; }

        public int Id { get { return this.Value; } }
    }
}