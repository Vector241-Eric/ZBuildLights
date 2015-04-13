using System;
using System.Linq;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public class ConsoleCommandWatcher
    {
        public void Run()
        {
            var manager = ZWaveManagerFactory.GetInstance();
            Console.ReadKey();
            Console.WriteLine("NODES:");
            Console.WriteLine("------");

            var nodes = ZWaveNotificationHandler.GetNodes();
            Console.WriteLine("------");
            var homeId = nodes[0].HomeId;
            var activeNodeId = 2;
            ulong activeValueId = default(ulong);
            while (true)
            {
                Console.WriteLine("Waiting for input (Current node: {0})...", activeNodeId);
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
                    int value;
                    if (Int32.TryParse(numberString, out value))
                    {
                        activeNodeId = value;
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
                    var node = GetNode(nodes, activeNodeId);
                    if (node == null)
                        continue;
                    DumpNodeValues(node, manager);
                }
                else if (input.Equals("s"))
                {
                    var node = GetNode(nodes, activeNodeId);
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
            var node = nodes.SingleOrDefault(x => x.Id == nodeId);
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