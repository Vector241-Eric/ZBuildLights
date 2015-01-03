namespace ZBuildLights.Core.Models
{
    public class Light
    {
        public byte ZWaveDeviceId { get; private set; }
        public uint ZWaveHomeId { get; private set; }
        public LightGroup ParentGroup { get; set; }

        public Light(uint zwaveHomeId, byte zWaveDeviceId)
        {
            ZWaveHomeId = zwaveHomeId;
            ZWaveDeviceId = zWaveDeviceId;
            SwitchState = SwitchState.Unknown;
            Color = LightColor.Unknown;
        }

        public LightColor Color { get; set; }
        public SwitchState SwitchState { get; set; }

        public override string ToString()
        {
            return string.Format("Home: {0} Id:{1}, State: {2}", ZWaveHomeId, ZWaveDeviceId, SwitchState);
        }

        public void MoveTo(LightGroup group)
        {
            if (ParentGroup != null)
                this.ParentGroup.RemoveLight(this);
            group.AddLight(this);
        }

        public void RemoveFromGroup()
        {
            ParentGroup.RemoveLight(this);
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
    }
}