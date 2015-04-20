using System;
using System.Linq;
using NUnit.Framework;
using Should;
using UnitTests._Bases;
using UnitTests._Stubs;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class LightDisplayUpdaterTests
    {
        [TestFixture]
        public class Always : TestBase
        {
            private MasterModel _lastSaved;
            private Guid _projectId1;
            private Guid _projectId2;
            private ZWaveSwitch[] _switches;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var project1 = masterModel.CreateProject(x => { x.Name = "Project 1"; });
                _projectId1 = project1.Id;

                var lightGroup1 = project1.CreateGroup();
                lightGroup1.AddLight(new Light(new ZWaveIdentity(1, 1, 1)) {Color = LightColor.Green});
                lightGroup1.AddLight(new Light(new ZWaveIdentity(1, 2, 1)) {Color = LightColor.Yellow});
                lightGroup1.AddLight(new Light(new ZWaveIdentity(1, 3, 1)) {Color = LightColor.Red});

                var project2 = masterModel.CreateProject(x => { x.Name = "Project 1"; });
                _projectId2 = project2.Id;

                var lightGroup2 = project2.CreateGroup();
                lightGroup2.AddLight(new Light(new ZWaveIdentity(1, 4, 1)) {Color = LightColor.Green});
                lightGroup2.AddLight(new Light(new ZWaveIdentity(1, 5, 1)) {Color = LightColor.Yellow});
                lightGroup2.AddLight(new Light(new ZWaveIdentity(1, 6, 1)) {Color = LightColor.Red});

                var lightGroup3 = project2.CreateGroup();
                lightGroup3.AddLight(new Light(new ZWaveIdentity(1, 7, 1)) {Color = LightColor.Green});
                lightGroup3.AddLight(new Light(new ZWaveIdentity(1, 8, 1)) {Color = LightColor.Yellow});
                lightGroup3.AddLight(new Light(new ZWaveIdentity(1, 9, 1)) {Color = LightColor.Red});

                var projectStatusUpdater = new StubProjectStatusUpdater()
                    .WithStubStatus(project1, StatusMode.Broken)
                    .WithStubStatus(project2, StatusMode.SuccessAndBuilding)
                    ;

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var zWaveNetwork = new StubZWaveNetwork();

                var updater = new LightDisplayUpdater(repository, zWaveNetwork, projectStatusUpdater);
                updater.Update();

                _switches = zWaveNetwork.GetAllSwitches();
                _lastSaved = repository.LastSaved;
            }

            [Test]
            public void Should_set_appropriate_switch_state_for_each_light_on_each_project()
            {
                VerifySwitchState(new ZWaveIdentity(1, 1, 1), SwitchState.Off);
                VerifySwitchState(new ZWaveIdentity(1, 2, 1), SwitchState.Off);
                VerifySwitchState(new ZWaveIdentity(1, 3, 1), SwitchState.On);

                VerifySwitchState(new ZWaveIdentity(1, 4, 1), SwitchState.On);
                VerifySwitchState(new ZWaveIdentity(1, 5, 1), SwitchState.On);
                VerifySwitchState(new ZWaveIdentity(1, 6, 1), SwitchState.Off);

                VerifySwitchState(new ZWaveIdentity(1, 7, 1), SwitchState.On);
                VerifySwitchState(new ZWaveIdentity(1, 8, 1), SwitchState.On);
                VerifySwitchState(new ZWaveIdentity(1, 9, 1), SwitchState.Off);
            }

            private void VerifySwitchState(ZWaveIdentity zWaveIdentity, SwitchState switchState)
            {
                _switches.Single(x => x.ZWaveIdentity.Equals(zWaveIdentity)).SwitchState.ShouldEqual(switchState);
            }

            [Test]
            public void Should_save_master_model()
            {
                _lastSaved.ShouldNotBeNull();
                _lastSaved.Projects.Single(x => x.Id.Equals(_projectId1)).StatusMode.ShouldEqual(StatusMode.Broken);
                _lastSaved.Projects.Single(x => x.Id.Equals(_projectId2))
                    .StatusMode.ShouldEqual(StatusMode.SuccessAndBuilding);
            }
        }
    }
}