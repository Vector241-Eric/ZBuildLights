using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    public class LightSetting
    {
        public LightColor Color { get; private set; }
        public SwitchState State { get; private set; }

        public LightSetting(LightColor color, SwitchState state)
        {
            Color = color;
            State = state;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Color, State);
        }
    }
}