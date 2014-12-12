using System;
using System.Web.Script.Serialization;

namespace BuildLightControl
{
    [Serializable]
    public class LightSwitch
    {
        [ScriptIgnore]
        public SwitchState SwitchState { get; private set; }
        public byte Id { get; private set; }
        public uint HomeId { get; private set; }

        public LightSwitch(uint homeId, byte id)
        {
            HomeId = homeId;
            Id = id;
            SwitchState = SwitchState.Unknown;
        }

        public LightSwitch SetState(SwitchState state)
        {
            SwitchState = state;
            return this;
        }

        public override string ToString()
        {
            return string.Format("Home: {0} Id:{1}, State: {2}", HomeId, Id, SwitchState);
        }
    }
}