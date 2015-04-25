using System;
using System.Linq;
using OpenZWaveDotNet;
using ZBuildLights.Core.Models;
using ZWaveControl;

namespace ZBuildLightsConsole
{
    public class ConsoleCommandWatcher
    {
        private readonly ZWaveNodeList _nodeList = new ZWaveNodeList();

        public void Run()
        {
            var manager = new ZWaveManagerFactory(new ZWaveSettings(), _nodeList).GetManager();

            Console.ReadKey();
            Console.WriteLine("NODES:");
            Console.WriteLine("------");

            Console.WriteLine("------");
            var homeId = _nodeList.AllNodes[0].HomeId;
            ZWaveNodeIdentity activeNode = new ZWaveNodeIdentity(homeId, 2);
            ulong activeValueId = default(ulong);
            while (true)
            {
                Console.WriteLine("Waiting for input (Current node: {0})...", activeNode.NodeId);
                var input = ReadKey();
                if (input.Equals("q"))
                    break;
                if (input.Equals("h"))
                {
                    manager.HealNetwork(homeId, true);
                }
                else if (input.Equals("n"))
                {
                    Console.Write("Enter node number: ");
                    var numberString = Console.ReadLine();
                    byte value;
                    if (Byte.TryParse(numberString, out value))
                    {
                        activeNode = new ZWaveNodeIdentity(homeId, value);
                    }
                }
                else if (input.Equals("v"))
                {
                    Console.Write("Enter value ID: ");
                    var numberString = Console.ReadLine();
                    ulong value;
                    if (UInt64.TryParse(numberString, out value))
                    {
                        activeValueId = value;
                    }
                }
                else if (input.Equals("d"))
                {
                    var node = _nodeList.GetNode(activeNode);
                    if (node == null)
                        continue;
                    DumpNodeValues(node, manager);
                }
                else if (input.Equals("s"))
                {
                    var node = _nodeList.GetNode(activeNode);
                    if (node == null)
                        continue;
                    var switchZwValue = node.Values.FirstOrDefault(x => x.GetId().Equals(activeValueId));
                    if (switchZwValue == null)
                        Console.WriteLine("Could not locate switch with value ({0})", activeValueId);
                    else
                    {
                        bool value;
                        manager.GetValueAsBool(switchZwValue, out value);
                        var newValue = !value;
                        manager.SetValue(switchZwValue, newValue);
                    }
                }
            }
        }

        private static Node GetNode(Node[] nodes, int nodeId)
        {
            var node = nodes.SingleOrDefault(x => x.NodeId == nodeId);
            if (node == null)
            {
                Console.WriteLine("Could not locate node {0}.", nodeId);
                return null;
            }
            return node;
        }

        private static void DumpNodeValues(Node node, ZWManager manager)
        {
            foreach (var value in node.Values)
            {
                var valueLabel = manager.GetValueLabel(value);
                if (valueLabel.ToLowerInvariant().Equals("switch"))
                    Console.WriteLine("***** Value ID:({0}) label:({1}) index:({2}) type:({3})", value.GetId(),
                        valueLabel, value.GetIndex(), value.GetType());
            }
        }

        private static string ReadKey()
        {
            var keyInfo = Console.ReadKey();
            var key = keyInfo.Key;
            var input = key.ToString().ToLowerInvariant();
            return input;
        }
    }
}