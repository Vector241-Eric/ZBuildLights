using System.Linq;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    public class StatusLightConfiguration
    {
        public SwitchState GreenState { get; set; }
        public SwitchState YellowState { get; set; }
        public SwitchState RedState { get; set; }

        private LightSetting[] AsSettings()
        {
            return new[]
            {
                new LightSetting(LightColor.Green, GreenState),
                new LightSetting(LightColor.Yellow, YellowState),
                new LightSetting(LightColor.Red, RedState)
            };
        }

        public override string ToString()
        {
            return string.Join(", ", AsSettings().Select(x => x.ToString()));
        }
    }
}