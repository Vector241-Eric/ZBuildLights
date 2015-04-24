namespace ZBuildLights.Core.Models
{
    public class ZWaveSwitch : IHasZWaveIdentity
    {
        public ZWaveSwitch(ZWaveValueIdentity identity)
        {
            ZWaveIdentity = identity;
        }

        public SwitchState SwitchState { get; set; }
        public string SwitchStateDisplayText { get { return SwitchState.ToString(); } }
        public ZWaveValueIdentity ZWaveIdentity { get; private set; }
    }
}