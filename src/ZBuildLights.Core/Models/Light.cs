namespace ZBuildLights.Core.Models
{
    public class Light
    {
        public byte ZWaveDeviceId { get; private set; }
        public uint ZWaveHomeId { get; private set; }
        public ulong ZWaveValueId { get; private set; }
        public LightGroup ParentGroup { get; set; }

        public Light(uint zwaveHomeId, byte zWaveDeviceId, ulong valueId)
        {
            ZWaveHomeId = zwaveHomeId;
            ZWaveDeviceId = zWaveDeviceId;
            ZWaveValueId = valueId;
            SwitchState = SwitchState.Unknown;
            Color = LightColor.Unknown;
        }

        public LightColor Color { get; set; }
        public SwitchState SwitchState { get; set; }
        public bool IsInGroup { get { return ParentGroup != null; } }

        public override string ToString()
        {
            return string.Format("Home: {0} Id:{1}, State: {2}", ZWaveHomeId, ZWaveDeviceId, SwitchState);
        }

        protected bool Equals(Light other)
        {
            return ZWaveDeviceId == other.ZWaveDeviceId && ZWaveHomeId == other.ZWaveHomeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Light) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ZWaveDeviceId.GetHashCode()*397) ^ (int) ZWaveHomeId;
            }
        }

        public void Unassign()
        {
            if (ParentGroup == null)
                return;

            ParentGroup.RemoveLight(this);
            ParentGroup = null;
        }
    }
}