using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class LightStatusSetterTests
    {
        [TestFixture]
        public class When_updating_light_status_and_all_lights_are_in_the_network : TestBase
        {
            private Light[] _lights;
            private IZWaveNetwork _network;

            [SetUp]
            public void ContextSetup()
            {
                var switches = new[]
                {
                    new ZWaveSwitch(new ZWaveValueIdentity(1, 1, 345)) {SwitchState = SwitchState.On},
                    new ZWaveSwitch(new ZWaveValueIdentity(1, 2, 345)) {SwitchState = SwitchState.Off},
                    new ZWaveSwitch(new ZWaveValueIdentity(1, 3, 345)) {SwitchState = SwitchState.On},
                    new ZWaveSwitch(new ZWaveValueIdentity(1, 4, 345)) {SwitchState = SwitchState.Off}
                };

                //Reordered
                _lights = new[]
                {
                    new Light(switches[3].ZWaveIdentity),
                    new Light(switches[0].ZWaveIdentity),
                    new Light(switches[1].ZWaveIdentity),
                    new Light(switches[2].ZWaveIdentity)
                };

                _network = S<IZWaveNetwork>();
                _network.Stub(x => x.GetAllSwitches()).Return(switches);

                var statusProvider = new SetModelStatusFromNetworkSwitches(_network);
                statusProvider.SetLightStatus(_lights);
            }

            [Test]
            public void Should_set_light_status()
            {
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(1)).SwitchState.ShouldEqual(SwitchState.On);
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(2)).SwitchState.ShouldEqual(SwitchState.Off);
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(3)).SwitchState.ShouldEqual(SwitchState.On);
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(4)).SwitchState.ShouldEqual(SwitchState.Off);
            }

            [Test]
            public void Should_only_query_the_network_once()
            {
                _network.AssertWasCalled(x => x.GetAllSwitches(), opt => opt.Repeat.Times(1));
            }
        }

        [TestFixture]
        public class When_some_lights_are_not_in_the_network_and_some_are : TestBase
        {
            private Light[] _lights;

            [SetUp]
            public void ContextSetup()
            {
                var switches = new[]
                {
                    new ZWaveSwitch(new ZWaveValueIdentity(1, 1, 111)) {SwitchState = SwitchState.On},
                    new ZWaveSwitch(new ZWaveValueIdentity(1, 4, 111)) {SwitchState = SwitchState.Off}
                };

                //Reordered
                _lights = new[]
                {
                    new Light(new ZWaveValueIdentity(1, 2, 123)),
                    new Light(switches[0].ZWaveIdentity),
                    new Light(switches[1].ZWaveIdentity),
                    new Light(new ZWaveValueIdentity(2, 3, 4))
                };

                var network = S<IZWaveNetwork>();
                network.Stub(x => x.GetAllSwitches()).Return(switches);

                var statusProvider = new SetModelStatusFromNetworkSwitches(network);
                statusProvider.SetLightStatus(_lights);
            }

            [Test]
            public void Should_set_light_status_for_the_lights_in_the_network()
            {
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(1)).SwitchState.ShouldEqual(SwitchState.On);
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(4) && x.ZWaveIdentity.ValueId == 111)
                    .SwitchState.ShouldEqual(SwitchState.Off);
            }

            [Test]
            public void Should_set_the_status_to_unknown_for_lights_not_in_the_network()
            {
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(2)).SwitchState.ShouldEqual(SwitchState.Unknown);
                _lights.Single(x => x.ZWaveIdentity.NodeId.Equals(3) && x.ZWaveIdentity.ValueId == 4)
                    .SwitchState.ShouldEqual(SwitchState.Unknown);
            }
        }
    }
}