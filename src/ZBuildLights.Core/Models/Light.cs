using System;
using System.Web.Script.Serialization;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    [Serializable]
    public class Light
    {
        public byte ZWaveDeviceId { get; private set; }
        public uint ZWaveHomeId { get; private set; }
        public Guid Id { get; internal set; }

        public Light(uint zwaveHomeId, byte zWaveDeviceId)
        {
            ZWaveHomeId = zwaveHomeId;
            ZWaveDeviceId = zWaveDeviceId;
            SwitchState = SwitchState.Unknown;
            Color = LightColor.Unknown;
        }

        public LightColor Color { get; set; }
        [ScriptIgnore]
        public SwitchState SwitchState { get; set; }

        public override string ToString()
        {
            return string.Format("Home: {0} Id:{1}, State: {2}", ZWaveHomeId, ZWaveDeviceId, SwitchState);
        }
    }
}