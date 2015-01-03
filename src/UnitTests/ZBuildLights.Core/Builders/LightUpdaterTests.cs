using System;
using NUnit.Framework;
using Should;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Builders
{
    public class LightUpdaterTests
    {
        [TestFixture]
        public class HappyPath
        {
            private const int _zwaveHomeId = 1;
            private const int _zWaveDeviceId = 14;
            private MasterModel _saved;
            private Guid _destinationGroup;

            [SetUp]
            public void ContextSetup()
            {
                _destinationGroup = Guid.NewGuid();

                var existingMasterModel = new MasterModel();
                var project = existingMasterModel.AddProject(new Project {Name = "Existing Project"});
                project.AddGroup(new LightGroup()).AddLight(new Light(1, 11)).AddLight(new Light(1, 12));
                project.AddGroup(new LightGroup()).AddLight(new Light(1, 13)).AddLight(new Light(1, 14));

                project.AddGroup(new LightGroup {Id = _destinationGroup})
                    .AddLight(new Light(1, 15))
                    .AddLight(new Light(1, 16));

                project.AddGroup(new LightGroup()).AddLight(new Light(2, 13)).AddLight(new Light(2, 14));

                var project2 = existingMasterModel.AddProject(new Project {Name = "Existing Project 2"});
                project2.AddGroup(new LightGroup()).AddLight(new Light(1, 21)).AddLight(new Light(1, 22));

                var project3 = existingMasterModel.AddProject(new Project {Name = "Existing Project 3"});
                project3.AddGroup(new LightGroup()).AddLight(new Light(1, 31)).AddLight(new Light(1, 32));


                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var updater = new LightUpdater(repository);
                updater.Update(_zwaveHomeId, _zWaveDeviceId, _destinationGroup, LightColor.Red.Value);

                _saved = repository.LastSaved;
            }

            [Test]
            public void Should_move_the_light_to_the_indicated_group()
            {
                var light = _saved.FindLight(_zwaveHomeId, _zWaveDeviceId);
                light.ParentGroup.Id.ShouldEqual(_destinationGroup);
            }

            [Test]
            public void Should_update_the_light_color()
            {
                var light = _saved.FindLight(_zwaveHomeId, _zWaveDeviceId);
                light.Color.ShouldEqual(LightColor.Red);
            }

            [Test]
            public void Should_save_the_updated_model()
            {
                _saved.ShouldNotBeNull();
            }
        }
    }
}