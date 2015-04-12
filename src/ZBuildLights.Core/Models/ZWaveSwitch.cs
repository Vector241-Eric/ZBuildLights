namespace ZBuildLights.Core.Models
{
    public class ZWaveSwitch
    {
        public byte DeviceId { get; set; }
        public uint HomeId { get; set; }
        public ulong ValueId { get; set; }
        public SwitchState SwitchState { get; set; }
    }
}