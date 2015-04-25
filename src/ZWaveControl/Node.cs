using System.Collections.Generic;
using OpenZWaveDotNet;
using ZBuildLights.Core.Models;

namespace ZWaveControl
{
    public class Node
    {
        public Node()
        {
            Values = new List<ZWValueID>();
        }

        public byte NodeId { get; set; }
        public uint HomeId { get; set; }

        public string Name { get; set; }

        public ZWaveNodeIdentity NodeIdentity
        {
            get { return new ZWaveNodeIdentity(HomeId, NodeId); }
        }

        public ICollection<ZWValueID> Values { get; set; }
    }
}