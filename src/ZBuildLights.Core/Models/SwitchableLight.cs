using System;
using System.Web.Script.Serialization;

namespace BuildLightControl
{
    [Serializable]
    public class SwitchableLight
    {
        public LightColor Color { get; private set; }
        [ScriptIgnore]
        public SwitchState SwitchState { get; private set; }

        public SwitchableLight(LightColor color)
        {
            Color = color;
            SwitchState = SwitchState.Unknown;
        }

        public SwitchableLight SetState(SwitchState state)
        {
            SwitchState = state;
            return this;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Color, SwitchState);
        }
    }
}