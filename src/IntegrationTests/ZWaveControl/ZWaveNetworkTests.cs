﻿using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Models;
using ZBuildLights.Web.DependencyResolution;
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
                    var container = IoC.Initialize();
                    var network = container.GetInstance<ZWaveNetwork>();

                    var switches = network.GetAllSwitches();
                    Console.WriteLine(JsonConvert.SerializeObject(switches, Formatting.Indented));
                }
                finally
                {
                    ZWaveManagerFactory.Destroy();
                }
            }

            [Test, Explicit]
            public void Should_set_switch_state_on_device()
            {
                try
                {
                    var container = IoC.Initialize();
                    var network = container.GetInstance<ZWaveNetwork>();

                    var result = network.SetSwitchState(new ZWaveValueIdentity(25479126, 2, 72057594076282880), SwitchState.Off);
                    result.IsSuccessful.ShouldBeTrue();
                }
                finally
                {
                    ZWaveManagerFactory.Destroy();
                }
            }
        }
    }
}