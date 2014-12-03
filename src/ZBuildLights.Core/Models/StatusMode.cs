using System.Linq;
using BuildLightControl;
using Headspring;

namespace ZBuildLights.Core.Models
{
    public class StatusMode : Enumeration<StatusMode, string>
    {
        public static readonly StatusMode Success = new StatusMode("Success", LightColor.Green);
        public static readonly StatusMode SuccessAndBuilding = new StatusMode("SuccessAndBuilding", LightColor.Green, LightColor.Yellow);
        public static readonly StatusMode Broken = new StatusMode("Broken", LightColor.Red);
        public static readonly StatusMode BrokenAndBuilding = new StatusMode("BrokenAndBuilding", LightColor.Red, LightColor.Yellow);
        public static readonly StatusMode NotConnected = new StatusMode("NotConnected", LightColor.Green, LightColor.Yellow, LightColor.Red);
        public static readonly StatusMode Off = new StatusMode("Off");

        private StatusMode(string displayName, params LightColor[] lights)
            : base(displayName, displayName)
        {
            LightConfiguration = new LightConfiguration
            {
                Green = lights.Contains(LightColor.Green) ? LightStatus.On : LightStatus.Off,
                Yellow = lights.Contains(LightColor.Yellow) ? LightStatus.On : LightStatus.Off,
                Red = lights.Contains(LightColor.Red) ? LightStatus.On : LightStatus.Off
            };
        }

        public LightConfiguration LightConfiguration { get; private set; }
    }
}