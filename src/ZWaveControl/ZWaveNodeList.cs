using System.Collections.Generic;
using System.Linq;
using ZBuildLights.Core.Models;

namespace ZWaveControl
{
    public class ZWaveNodeList : IZWaveNodeList
    {
        private readonly List<Node> _nodes = new List<Node>();

        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        public Node[] AllNodes
        {
            get { return _nodes.ToArray(); }
        }

        public Node GetNode(ZWaveNodeIdentity identity)
        {
            return _nodes.SingleOrDefault(x => x.NodeIdentity.Equals(identity));
        }
    }
}