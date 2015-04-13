namespace ZBuildLights.Core.Models
{
    public class ZWaveSwitch : IHasZWaveIdentity
    {
        public ZWaveSwitch(ZWaveIdentity identity)
        {
            ZWaveIdentity = identity;
        }

        public SwitchState SwitchState { get; set; }
        public string SwitchStateDisplayText { get { return SwitchState.ToString(); } }
        public ZWaveIdentity ZWaveIdentity { get; private set; }
    }
}