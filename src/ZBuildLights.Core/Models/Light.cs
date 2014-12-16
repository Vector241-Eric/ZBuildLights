using System;
using System.Web.Script.Serialization;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    [Serializable]
    public class Light
    {
        public byte ZWaveDeviceId { get; set; }
        public uint ZWaveHomeId { get; set; }
        public Guid Id { get; internal set; }

        public Light()
        {
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