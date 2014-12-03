using System.Linq;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    public class StatusLightConfiguration
    {
        public SwitchState GreenSwitchState { get; set; }
        public SwitchState YellowSwitchState { get; set; }
        public SwitchState RedSwitchState { get; set; }

        public SwitchableLight[] SwitchableLights
        {
            get
            {
                return new[]
                {
                    new SwitchableLight(LightColor.Green).SetState(GreenSwitchState),
                    new SwitchableLight(LightColor.Yellow).SetState(YellowSwitchState),
                    new SwitchableLight(LightColor.Red).SetState(RedSwitchState)
                };
            }
        }

        public override string ToString()
        {
            return string.Join(", ", SwitchableLights.Select(x => x.ToString()));
        }
    }
}