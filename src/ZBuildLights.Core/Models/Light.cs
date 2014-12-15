using System;
using System.Web.Script.Serialization;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    [Serializable]
    public class Light
    {
        public byte Id { get; private set; }
        public uint HomeId { get; private set; }

        public Light(uint homeId, byte id)
        {
            HomeId = homeId;
            Id = id;
            SwitchState = SwitchState.Unknown;
        }

        public LightColor Color { get; set; }
        [ScriptIgnore]
        public SwitchState SwitchState { get; set; }

        public override string ToString()
        {
            return string.Format("Home: {0} Id:{1}, State: {2}", HomeId, Id, SwitchState);
        }
    }
}