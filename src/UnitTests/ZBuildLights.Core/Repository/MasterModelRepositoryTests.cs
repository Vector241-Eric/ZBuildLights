using System;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.Storage;

namespace UnitTests.ZBuildLights.Core.Repository
{
    public class MasterModelRepositoryTests
    {
        [TestFixture]
        public class When_file_does_not_exist_on_disk_and_we_retrieve_the_current_model : TestBase
        {
            private MasterModel _result;
            private DateTime _now;

            [SetUp]
            public void ContextSetup()
            {
                _now = new DateTime(2011, 1, 2, 3, 4, 5);
                SetSystemClock(_now);

                var fileStorage = S<IFileSystemStorage>();
                fileStorage.Stub(x => x.ReadMasterModel()).Return(null);

                var unassignedLightService = new StubUnassignedLightService().WithZeroUnassignedLights();

                var repository = new MasterModelRepository(fileStorage, unassignedLightService);
                _result = repository.GetCurrent();
            }

            [Test]
            public void Should_initialize_an_empty_model()
            {
                _result.ShouldNotBeNull();
            }

            [Test]
            public void Should_set_the_last_updated_date_to_now()
            {
                _result.LastUpdatedDate.ShouldEqual(_now);
            }
        }

        [TestFixture]
        public class When_file_exists_on_disk_and_we_retrieve_the_current_model : TestBase
        {
            private MasterModel _result;
            private MasterModel _existingModel;

            [SetUp]
            public void ContextSetup()
            {
                SetSystemClock(new DateTime(2011, 1, 2, 3, 4, 5));

                _existingModel = new MasterModel {LastUpdatedDate = new DateTime(2014, 1, 1)};

                var fileStorage = S<IFileSystemStorage>();
                fileStorage.Stub(x => x.ReadMasterModel()).Return(_existingModel);

                var unassignedLightService = new StubUnassignedLightService()
                    .WithUnassignedLight(new Light(new ZWaveValueIdentity(1, 3, 123)));

                var repository = new MasterModelRepository(fileStorage, unassignedLightService);
                _result = repository.GetCurrent();
            }

            [Test]
            public void Should_return_the_model_from_disk()
            {
                _result.ShouldBeSameAs(_existingModel);
            }

            [Test]
            public void Should_add_the_unassigned_lights_from_the_network()
            {
                _result.UnassignedLights.Length.ShouldEqual(1);
                _result.UnassignedLights[0].ZWaveIdentity.HomeId.ShouldEqual((uint)1);
                _result.UnassignedLights[0].ZWaveIdentity.NodeId.ShouldEqual((byte)3);
            }
        }

        [TestFixture]
        public class When_model_is_saved : TestBase
        {
            private MasterModel _model;
            private DateTime _now;
            private MasterModel _lastSaved;

            [SetUp]
            public void ContextSetup()
            {
                _model = new MasterModel {LastUpdatedDate = DateTime.Now};
                _now = new DateTime(2010, 4, 5);

                SetSystemClock(_now);

                IUnassignedLightService doNotUseUnassignedLightService = null;

                var fileStorage = new StorageStub();
                var respository = new MasterModelRepository(fileStorage, doNotUseUnassignedLightService);
                respository.Save(_model);

                _lastSaved = fileStorage.LastSaved;
            }

            [Test]
            public void Should_persist_model_to_disk()
            {
                _lastSaved.ShouldBeSameAs(_model);
            }

            [Test]
            public void Should_set_the_last_updated_date_to_now()
            {
                _lastSaved.LastUpdatedDate.ShouldEqual(_now);
            }
        }
    }
}