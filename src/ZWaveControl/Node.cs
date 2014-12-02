using System.Collections.Generic;
using OpenZWaveDotNet;

namespace BuildLightControl.ZWave
{
    public class Node
    {
        public Node()
        {
            Values = new List<ZWValueID>();
        }

        public byte Id { get; set; }
        public uint HomeId { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }
        public string Label { get; set; }

        public string Manufacturer { get; set; }
        public string Product { get; set; }

        public ICollection<ZWValueID> Values { get; set; }
    }
}