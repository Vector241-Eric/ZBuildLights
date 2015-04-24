namespace ZBuildLights.Core.Models
{
    public class ZWaveNodeIdentity
    {
        public ZWaveNodeIdentity(uint homeId, byte nodeId)
        {
            HomeId = homeId;
            NodeId = nodeId;
        }

        public uint HomeId { get; private set; }
        public byte NodeId { get; private set; }

        protected bool Equals(ZWaveNodeIdentity other)
        {
            return HomeId == other.HomeId && NodeId == other.NodeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ZWaveNodeIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) HomeId*397) ^ NodeId.GetHashCode();
            }
        }
    }
}