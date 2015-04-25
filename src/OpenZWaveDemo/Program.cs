using System;
using System.Linq;
using System.Threading;
using OpenZWaveDotNet;

namespace OpenZWaveDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var lowerArgs = args.Select(x => x.ToLowerInvariant()).ToArray();
            if (lowerArgs.Contains("--nodes"))
                DumpZWaveNodes();
            else if (lowerArgs.Contains("--values"))
                DumpZWaveValues();
            else if (lowerArgs.Contains("--switch"))
                ToggleSwitch(args[1]);
        }

        private static void DumpZWaveNodes()
        {
            var manager = CreateOpenZWaveManager();

            bool allNodesQueried = false;
            manager.OnNotification += notification =>
            {
                switch (notification.GetType())
                {
                    case ZWNotification.Type.AllNodesQueried:
                    case ZWNotification.Type.AllNodesQueriedSomeDead:
                        allNodesQueried = true;
                        break;
                    case ZWNotification.Type.NodeAdded:
                        Console.WriteLine("Added node --> Home:{0}  Node:{1}", notification.GetHomeId(),
                            notification.GetNodeId());
                        break;
                }
            };
            Console.WriteLine("Initializing ZWave Manager");
            manager.AddDriver(@"\\.\COM3");
            while (!allNodesQueried)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("Initialization Complete");
            manager.Destroy();
        }

        private static void DumpZWaveValues()
        {
            var manager = CreateOpenZWaveManager();

            bool allNodesQueried = false;
            manager.OnNotification += notification =>
            {
                switch (notification.GetType())
                {
                    case ZWNotification.Type.AllNodesQueried:
                    case ZWNotification.Type.AllNodesQueriedSomeDead:
                        allNodesQueried = true;
                        break;
                    case ZWNotification.Type.NodeAdded:
                        Console.WriteLine("Added node --> Home:{0}  Node:{1}", notification.GetHomeId(),
                            notification.GetNodeId());
                        break;
                    case ZWNotification.Type.ValueAdded:
                        Console.WriteLine("Added value --> Home:{0}  Node:{1}  Value:{2}  Label: {3}",
                            notification.GetHomeId(), notification.GetNodeId(), notification.GetValueID().GetId(),
                            manager.GetValueLabel(notification.GetValueID()));
                        break;
                }
            };
            Console.WriteLine("Initializing ZWave Manager");
            manager.AddDriver(@"\\.\COM3");
            while (!allNodesQueried)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("Initialization Complete");
            manager.Destroy();
        }

        private static void ToggleSwitch(string switchId)
        {
            var manager = CreateOpenZWaveManager();
            ZWValueID valueId = null;

            bool allNodesQueried = false;
            manager.OnNotification += notification =>
            {
                switch (notification.GetType())
                {
                    case ZWNotification.Type.AllNodesQueried:
                    case ZWNotification.Type.AllNodesQueriedSomeDead:
                        allNodesQueried = true;
                        break;
                    case ZWNotification.Type.ValueAdded:
                        if (notification.GetValueID().GetId().ToString().Equals(switchId))
                        {
                            valueId = notification.GetValueID();
                        }
                        break;
                }
            };
            Console.WriteLine("Initializing ZWave Manager");
            manager.AddDriver(@"\\.\COM3");
            while (!allNodesQueried)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("Initialization Complete");

            if (valueId != null)
            {
                ToggleSwitchValue(manager, valueId);
            }
            
            manager.Destroy();
        }

        private static void ToggleSwitchValue(ZWManager manager, ZWValueID valueId)
        {
            bool switchValue;
            manager.GetValueAsBool(valueId, out switchValue);
            manager.SetValue(valueId, !switchValue);
        }

        private static ZWManager CreateOpenZWaveManager()
        {
            SetOptions();

            var manager = new ZWManager();
            manager.Create();
            return manager;
        }

        private static void SetOptions()
        {
            var options = new ZWOptions();
            options.Create(@"C:\work\OSS\ZBuildLights\lib\OpenZWave_1-3-Release\config", @"c:\temp\OzwDemo\UserData",
                string.Empty);
            options.AddOptionInt("SaveLogLevel", (int) ZWLogLevel.None);
            options.Lock();
        }
    }
}