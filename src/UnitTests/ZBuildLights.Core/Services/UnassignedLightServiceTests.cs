using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class UnassignedLightServiceTests
    {
        [TestFixture]
        public class When_some_lights_in_the_master_model_are_missing_from_the_network_and_some_lights_are_new_in_the_network :
                TestBase
        {
            private MasterModel _masterModel;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                var group = _masterModel.CreateProject().CreateGroup();
                group.AddLight(new Light(new ZWaveIdentity(1, 11, 1)));
                group.AddLight(new Light(new ZWaveIdentity(1, 22, 4)));
                group.AddLight(new Light(new ZWaveIdentity(1, 33, 5)));

                _masterModel.AddUnassignedLight(new Light(new ZWaveIdentity(1, 44, 123)) { Color = LightColor.Red });

                var allSwitches = new[]
                {
                    new ZWaveSwitch(new ZWaveIdentity(1, 11, 1)), //In a group
                    new ZWaveSwitch(new ZWaveIdentity(1, 15, 2)), //New
                    new ZWaveSwitch(new ZWaveIdentity(1, 16, 3)), //New
                    new ZWaveSwitch(new ZWaveIdentity(1, 22, 4)), //In a group
                    new ZWaveSwitch(new ZWaveIdentity(1, 22, 5)), //New (different value ID)
                    new ZWaveSwitch(new ZWaveIdentity(1, 33, 5)), //In a group
                    new ZWaveSwitch(new ZWaveIdentity(2, 11, 6)), //New (different home ID)
                    new ZWaveSwitch(new ZWaveIdentity(2, 22, 7)) //New (different home ID)
                };

                var network = S<IZWaveNetwork>();
                network.Stub(x => x.GetAllSwitches()).Return(allSwitches);

                var repository = S<IMasterModelRepository>();
                repository.Stub(x => x.GetCurrent()).Return(_masterModel);

                var service = new UnassignedLightService(network);
                service.SetUnassignedLights(_masterModel);
            }

            [Test]
            public void Should_find_the_new_lights_in_the_network()
            {
                _masterModel.UnassignedLights.Length.ShouldEqual(6);
                _masterModel.UnassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(15)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(16)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(44)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(2) && x.ZWaveIdentity.NodeId.Equals(11)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(2) && x.ZWaveIdentity.NodeId.Equals(22)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(22) && x.ZWaveIdentity.ValueId.Equals(5)).ShouldBeTrue();
            }

            [Test]
            public void Should_add_the_new_lights_without_overwriting_the_state_on_the_existing_lights()
            {
                var light = _masterModel.UnassignedLights.Single(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(44));
                light.Color.ShouldEqual(LightColor.Red);
            }
        }
    }
}