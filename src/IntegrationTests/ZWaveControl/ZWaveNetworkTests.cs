using System;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using ZWaveControl;

namespace IntegrationTests.ZWaveControl
{
    public class ZWaveNetworkTests
    {
        [TestFixture]
        public class When_getting_all_switches
        {
            [Test, Explicit]
            public void Should_dump_switch_information()
            {
                try
                {
                    var network = new ZWaveNetwork();
                    var switches = network.GetAllSwitches();
                    Console.WriteLine(JsonConvert.SerializeObject(switches, Formatting.Indented));
                }
                finally
                {
                    ZWaveManagerFactory.Destroy();
                }
            } 
        } 
    }
}