namespace ZBuildLights.Core.Models
{
    public class Light : IHasZWaveIdentity
    {
        public LightGroup ParentGroup { get; set; }
        public ZWaveValueIdentity ZWaveIdentity { get; private set; }

        public Light(ZWaveValueIdentity identity)
        {
            SwitchState = SwitchState.Unknown;
            Color = LightColor.Unknown;
            ZWaveIdentity = identity;
        }

//        public Light(uint homeId, byte nodeId, ulong valueId) : this(new ZWaveIdentity(homeId, nodeId, valueId))
//        {
//        }

        public LightColor Color { get; set; }
        public SwitchState SwitchState { get; set; }

        public bool IsInGroup
        {
            get { return ParentGroup != null; }
        }

        public override string ToString()
        {
            return string.Format("Light: {0}", ZWaveIdentity);
        }

        protected bool Equals(Light other)
        {
            return Equals(ZWaveIdentity, other.ZWaveIdentity);
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
            return (ZWaveIdentity != null ? ZWaveIdentity.GetHashCode() : 0);
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