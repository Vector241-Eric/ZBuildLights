using System;
using System.Linq;

namespace BuildLightControl.ZWave
{
    public class ConsoleCommandWatcher
    {
        public void Run()
        {
            var manager = ZWaveManagerFactory.GetInstance();

            Console.ReadKey();
            Console.WriteLine("NODES:");
            Console.WriteLine("------");

            foreach (var node in ZWaveNotificationHandler.GetNodes())
            {
                Console.WriteLine("Node Name: " + node.Name);
            }

            while (true)
            {
                var keyInfo = Console.ReadKey();
                var key = keyInfo.Key;
                var input = key.ToString().ToLowerInvariant();
                if (input.Equals("q"))
                    break;

                var node = ZWaveNotificationHandler.GetNodes()[1];
                var switchZwValue = node.Values.FirstOrDefault(x => manager.GetValueLabel(x).Equals("Switch"));
                if (switchZwValue == null)
                    Console.WriteLine("Could not locate switch");
                else
                {
                    var currentValue = bool.Parse(ZWaveNotificationHandler.GetValue(switchZwValue, manager));
                    Console.WriteLine("Current switch: {0}", currentValue);

                    if (input.Equals("s"))
                    {
                        var newValue = !currentValue;
                        manager.SetValue(switchZwValue, newValue);
                    }
                }

                foreach (var value in node.Values)
                {
                    Console.WriteLine("Value label: {0}", manager.GetValueLabel(value));
                }
            }
        }
    }
}