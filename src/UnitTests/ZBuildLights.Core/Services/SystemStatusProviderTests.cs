using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class SystemStatusProviderTests
    {
        [TestFixture]
        public class Always : TestBase
        {
            private MasterModel _model;
            private MasterModel _result;
            private StubSetModelStatusFromNetworkSwitches _setModelStatusFromNetworkSwitches;
            private ZWaveIdentity _identityForOffSwitch;

            [SetUp]
            public void ContextSetup()
            {
                var repo = S<IMasterModelRepository>();
                _model = new MasterModel();
                var group = _model.CreateProject().CreateGroup();
                _identityForOffSwitch = new ZWaveIdentity(3, 11, 123);
                group.AddLight(new Light(_identityForOffSwitch));
                group.AddLight(new Light(new ZWaveIdentity(3, 22, 123)));
                group.AddLight(new Light(new ZWaveIdentity(3, 33, 123)));
                group.AddLight(new Light(new ZWaveIdentity(3, 44, 123)));
                repo.Stub(x => x.GetCurrent()).Return(_model);

                _setModelStatusFromNetworkSwitches = new StubSetModelStatusFromNetworkSwitches().DefaultStatus(SwitchState.On).StubStatus(_identityForOffSwitch, SwitchState.Off);

                var statusProvider = new SystemStatusProvider(repo, _setModelStatusFromNetworkSwitches);
                _result = statusProvider.GetSystemStatus();
            }

            [Test]
            public void Should_provide_the_persisted_master_model()
            {
                _result.ShouldEqual(_model);
            }

            [Test]
            public void Should_set_light_status_from_the_network()
            {
                _result.AllLights.Count(x => x.SwitchState.Equals(SwitchState.On)).ShouldEqual(3);
                _result.AllLights.Count(x => x.SwitchState.Equals(SwitchState.Off)).ShouldEqual(1);
                _setModelStatusFromNetworkSwitches.LightsThatHadStatusSet.Length.ShouldEqual(4);
                _setModelStatusFromNetworkSwitches.LightsThatHadStatusSet.Any(x => x.ZWaveIdentity.NodeId.Equals(11)).ShouldBeTrue();
                _setModelStatusFromNetworkSwitches.LightsThatHadStatusSet.Any(x => x.ZWaveIdentity.NodeId.Equals(22)).ShouldBeTrue();
                _setModelStatusFromNetworkSwitches.LightsThatHadStatusSet.Any(x => x.ZWaveIdentity.NodeId.Equals(33)).ShouldBeTrue();
                _setModelStatusFromNetworkSwitches.LightsThatHadStatusSet.Any(x => x.ZWaveIdentity.NodeId.Equals(44)).ShouldBeTrue();
            }
        }
    }
}