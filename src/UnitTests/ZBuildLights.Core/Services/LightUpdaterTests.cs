using System;
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
    public class LightUpdaterTests
    {
        [TestFixture]
        public class HappyPath : TestBase
        {
            private MasterModel _saved;
            private Guid _destinationGroupId;
            private ZWaveIdentity _zWaveIdentity;

            [SetUp]
            public void ContextSetup()
            {
                _destinationGroupId = Guid.NewGuid();
                _zWaveIdentity = new ZWaveIdentity(1, 14, 123);

                var existingMasterModel = new MasterModel();
                var project = existingMasterModel.CreateProject(x => x.Name = "Existing Project");
                project.CreateGroup(x => x.Id = _destinationGroupId)
                    .AddLight(new Light(new ZWaveIdentity(1, 11, 123)))
                    .AddLight(new Light(new ZWaveIdentity(1, 12, 123)));
                project.CreateGroup()
                    .AddLight(new Light(new ZWaveIdentity(1, 13, 123)))
                    .AddLight(new Light(_zWaveIdentity));

                project.CreateGroup()
                    .AddLight(new Light(new ZWaveIdentity(1, 15, 123)))
                    .AddLight(new Light(new ZWaveIdentity(1, 16, 123)));

                project.CreateGroup()
                    .AddLight(new Light(new ZWaveIdentity(2, 13, 123)))
                    .AddLight(new Light(new ZWaveIdentity(2, 14, 123)));

                var project2 = existingMasterModel.CreateProject(x => x.Name = "Existing Project 2");
                project2.CreateGroup()
                    .AddLight(new Light(new ZWaveIdentity(1, 21, 123)))
                    .AddLight(new Light(new ZWaveIdentity(1, 22, 123)));

                var project3 = existingMasterModel.CreateProject(x => x.Name = "Existing Project 3");
                project3.CreateGroup()
                    .AddLight(new Light(new ZWaveIdentity(1, 31, 123)))
                    .AddLight(new Light(new ZWaveIdentity(1, 32, 123)));


                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var updater = new LightModelUpdater(repository);
                updater.Update(_zWaveIdentity, _destinationGroupId, LightColor.Red.Value);

                _saved = repository.LastSaved;
            }

            [Test]
            public void Should_move_the_light_to_the_indicated_group()
            {
                var light = _saved.FindLight(_zWaveIdentity);
                light.ParentGroup.Id.ShouldEqual(_destinationGroupId);
            }

            [Test]
            public void Should_update_the_light_color()
            {
                var light = _saved.FindLight(_zWaveIdentity);
                light.Color.ShouldEqual(LightColor.Red);
            }

            [Test]
            public void Should_save_the_updated_model()
            {
                _saved.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_assigning_an_unassigned_light : TestBase
        {
            private MasterModel _masterModel;
            private ZWaveIdentity _zWaveIdentity;

            [SetUp]
            public void ContextSetup()
            {
                var groupId = Guid.NewGuid();

                _masterModel = new MasterModel();
                var project = _masterModel.CreateProject(x => x.Name = "Existing Project");
                project.CreateGroup(x => x.Id = groupId)
                    .AddLight(new Light(new ZWaveIdentity(1, 11, 123)))
                    .AddLight(new Light(new ZWaveIdentity(1, 12, 123)));

                _zWaveIdentity = new ZWaveIdentity(1, 14, 123);
                var unassignedLights = new[]
                {
                    new Light(new ZWaveIdentity(1, 51, 123)),
                    new Light(_zWaveIdentity),
                    new Light(new ZWaveIdentity(1, 53, 123))
                };

                _masterModel.AddUnassignedLights(unassignedLights);

                var repository = S<IMasterModelRepository>();
                repository.Stub(x => x.GetCurrent()).Return(_masterModel);

                var updater = new LightModelUpdater(repository);
                updater.Update(_zWaveIdentity, groupId, LightColor.Red.Value);
            }

            [Test]
            public void Should_move_the_light_to_the_indicated_group()
            {
                _masterModel.AllGroups.Single()
                    .Lights.Any(x => x.ZWaveIdentity.Equals(_zWaveIdentity))
                    .ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_unassigning_an_assigned_light : TestBase
        {
            private MasterModel _lastSaved;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var project = masterModel.CreateProject(x => x.Name = "Existing Project");

                var zWaveIdentity = new ZWaveIdentity(1, 11, 5555);
                project.CreateGroup().AddLight(new Light(zWaveIdentity) {Color = LightColor.Yellow});

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var updater = new LightModelUpdater(repository);
                updater.Update(zWaveIdentity, Guid.Empty, LightColor.Red.Value);

                _lastSaved = repository.LastSaved;
            }

            [Test]
            public void Should_unassign_the_light()
            {
                _lastSaved.UnassignedLights.Length.ShouldEqual(1);
                _lastSaved.UnassignedLights[0].Color.ShouldEqual(LightColor.Red);
            }
        }

        [TestFixture]
        public class When_indicated_light_is_not_in_master_model_and_not_unassigned_in_the_network : TestBase
        {
            private InvalidOperationException _thrown;

            [SetUp]
            public void ContextSetup()
            {
                var groupId = Guid.NewGuid();

                var _masterModel = new MasterModel();
                var project = _masterModel.CreateProject(x => x.Name = "Existing Project");
                project.CreateGroup(x => x.Id = groupId);

                var repository = S<IMasterModelRepository>();
                repository.Stub(x => x.GetCurrent()).Return(_masterModel);

                var updater = new LightModelUpdater(repository);
                _thrown = ExpectException<InvalidOperationException>(
                    () => updater.Update(new ZWaveIdentity(12, 42, 444), groupId, LightColor.Red.Value));
            }

            [Test]
            public void Should_throw_an_exception_when_searching_for_a_light()
            {
                _thrown.Message.ShouldEqual("Could not find light with identity: Home: 12 Id: 42 Value: 444");
            }
        }
    }
}