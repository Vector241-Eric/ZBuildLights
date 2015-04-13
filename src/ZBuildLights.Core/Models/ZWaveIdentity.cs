namespace ZBuildLights.Core.Models
{
    public class ZWaveIdentity
    {
        public uint HomeId { get; private set; }
        public byte NodeId { get; private set; }
        public ulong ValueId { get; private set; }

        public ZWaveIdentity(uint homeId, byte nodeId, ulong valueId)
        {
            HomeId = homeId;
            NodeId = nodeId;
            ValueId = valueId;
        }

        protected bool Equals(ZWaveIdentity other)
        {
            return HomeId == other.HomeId && NodeId == other.NodeId && ValueId == other.ValueId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ZWaveIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) HomeId;
                hashCode = (hashCode*397) ^ NodeId.GetHashCode();
                hashCode = (hashCode*397) ^ ValueId.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Home: {0} Id: {1} Value: {2}", HomeId, NodeId, ValueId);
        }
    }
}