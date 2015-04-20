using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class StatusLightConfiguration
    {
        private readonly Dictionary<LightColor, SwitchState> _states = new Dictionary<LightColor, SwitchState>();

        private LightSetting[] AsSettings()
        {
            return new[]
            {
                new LightSetting(LightColor.Green, this[LightColor.Green]),
                new LightSetting(LightColor.Yellow, this[LightColor.Yellow]),
                new LightSetting(LightColor.Red, this[LightColor.Red])
            };
        }

        public SwitchState this[LightColor color]
        {
            get { return _states[color]; }
            set { _states[color] = value; }
        }

        public override string ToString()
        {
            return string.Join(", ", AsSettings().Select(x => x.ToString()));
        }
    }
}