using System;
using System.Linq;
using NUnit.Framework;
using Should;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.Results;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class CruiseServerManagerTests
    {
        [TestFixture]
        public class When_creating_new_server_with_unique_name
        {
            private MasterModel _savedModel;
            private CreationResult<CruiseServer> _result;
            private Guid _newId;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                existingMasterModel.CreateCruiseServer(x =>
                {
                    x.Name = "Existing";
                    x.Url = "foo";
                });

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new CruiseServerManager(repository);
                _result = creator.Create("My New Server", "http://example.com:8088/cc.xml");
                _newId = _result.Entity.Id;

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_the_creation_was_successful()
            {
                _result.IsSuccessful.ShouldBeTrue();
            }

            [Test]
            public void Should_save_the_existing_master_model_with_the_new_server()
            {
                _savedModel.ShouldNotBeNull();
                _savedModel.CruiseServers.Length.ShouldEqual(2);
                _savedModel.CruiseServers.Any(x => x.Name.Equals("Existing")).ShouldBeTrue();
                _savedModel.CruiseServers.Any(x => x.Id.Equals(_newId)).ShouldBeTrue();
            }

            [Test]
            public void Should_set_the_name()
            {
                _savedModel.CruiseServers.Any(x => x.Name.Equals("My New Server")).ShouldBeTrue();
            }

            [Test]
            public void Should_set_the_url()
            {
                _savedModel.CruiseServers.Any(x => x.Url.Equals("http://example.com:8088/cc.xml")).ShouldBeTrue();
            }
        }


        [TestFixture]
        public class When_creating_a_new_server_with_a_name_that_is_already_used
        {
            private CreationResult<CruiseServer> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                existingMasterModel.CreateCruiseServer(x =>
                {
                    x.Name = "Existing";
                    x.Url = "foo";
                });

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new CruiseServerManager(repository);
                _result = creator.Create("Existing", "http://example.com:8088/cc.xml");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_failure()
            {
                _result.IsSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual("A cruise server with this name already exists");
            }

            [Test]
            public void Should_not_save_the_master_model()
            {
                _savedModel.ShouldBeNull();
            }
        }
        [TestFixture]
        public class When_creating_a_new_server_without_a_name
        {
            private CreationResult<CruiseServer> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                existingMasterModel.CreateCruiseServer(x =>
                {
                    x.Name = "Foo";
                    x.Url = "foo";
                });

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new CruiseServerManager(repository);
                _result = creator.Create(string.Empty, "http://bar.example.com:8088/cc.xml");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_failure()
            {
                _result.IsSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual("'Name' is a required property for cruise server");
            }

            [Test]
            public void Should_not_save_the_master_model()
            {
                _savedModel.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_updating_a_group_with_unique_name
        {
            private MasterModel _savedModel;
            private EditResult<LightGroup> _result;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                var project = existingMasterModel.CreateProject(x => x.Name = "Existing Project");
                var existingGroup = project.CreateGroup(x => x.Name = "Existing Group");
                project.CreateGroup(x => x.Name = "Some Other Group");

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.Update(existingGroup.Id, "My New Name");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_the_update_was_successful()
            {
                _result.IsSuccessful.ShouldBeTrue();
            }

            [Test]
            public void Should_save_the_existing_master_model()
            {
                _savedModel.ShouldNotBeNull();
            }

            [Test]
            public void Should_not_add_any_groups()
            {
                _savedModel.Projects.Length.ShouldEqual(1);
                _savedModel.Projects[0].Groups.Length.ShouldEqual(2);
            }

            [Test]
            public void Should_set_the_group_name()
            {
                var id = _result.Entity.Id;
                var group = _savedModel.Projects.Single().Groups.Single(x => x.Id.Equals(id));
                group.Name.ShouldEqual("My New Name");
            }

            [Test]
            public void Should_not_change_the_id_of_the_project()
            {
                var id = _result.Entity.Id;
                _savedModel.Projects.Single().Groups.Count(x => x.Id.Equals(id)).ShouldEqual(1);
            }
        }

        [TestFixture]
        public class When_updating_a_group_with_a_name_that_is_already_used
        {
            private EditResult<LightGroup> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                var project = existingMasterModel.CreateProject(x => x.Name = "Existing Project");
                var existingGroup = project.CreateGroup(x => x.Name = "Existing Group");
                project.CreateGroup(x => x.Name = "Name Collision");

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.Update(existingGroup.Id, "Name Collision");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_that_update_was_not_successful()
            {
                _result.IsSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_not_save_the_master_model()
            {
                _savedModel.ShouldBeNull();
            }

            [Test]
            public void Should_include_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual(
                    "There is already a group named 'Name Collision' in project 'Existing Project'");
            }
        }

        [TestFixture]
        public class When_updating_a_group_and_setting_the_name_to_the_value_it_already_has
        {
            private EditResult<LightGroup> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();

                var project = existingMasterModel.CreateProject(x => x.Name = "Existing Project");
                var existingGroup = project.CreateGroup(x => x.Name = "Existing Group");

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.Update(existingGroup.Id, "Existing Group");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_that_update_was_successful()
            {
                _result.IsSuccessful.ShouldBeTrue();
            }

            [Test]
            public void Should_not_persist_the_master_model()
            {
                _savedModel.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_updating_a_group_that_does_not_exist
        {
            private EditResult<LightGroup> _result;
            private MasterModel _savedModel;
            private Guid _deleteId = Guid.NewGuid();
            private StubMasterModelRepository _repository;
            private Guid _groupIdDoesntExist;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();

                var project = existingMasterModel.CreateProject(x => x.Name = "Existing Project");
                project.CreateGroup(x => x.Name = "Existing Group");

                _groupIdDoesntExist = Guid.NewGuid();

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.Update(_groupIdDoesntExist, "My New Name");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_fail()
            {
                _result.IsSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual(string.Format("Could not locate a light group with ID '{0}'",
                    _groupIdDoesntExist));
            }

            [Test]
            public void Should_not_save_the_model()
            {
                _savedModel.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_deleting_group_that_exists
        {
            private EditResult<LightGroup> _result;
            private MasterModel _savedModel;
            private LightGroup _remainingGroup;
            private Project _parentProject;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();

                _parentProject = existingMasterModel.CreateProject(x => x.Name = "Existing Project");
                var existingGroup = _parentProject.CreateGroup(x => x.Name = "Existing Group");
                existingGroup.AddLight(new Light(new ZWaveIdentity(1, 1, 4234)));
                existingGroup.AddLight(new Light(new ZWaveIdentity(1, 2, 203984)));
                _remainingGroup = _parentProject.CreateGroup();
                _remainingGroup.AddLight(new Light(new ZWaveIdentity(1, 10, 239)));

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.Delete(existingGroup.Id);

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_save_the_updated_model()
            {
                _savedModel.ShouldNotBeNull();
            }

            [Test]
            public void Should_remove_project_from_the_model()
            {
                var remainingGroups = _parentProject.Groups;
                remainingGroups.Length.ShouldEqual(1);
                remainingGroups[0].ShouldBeSameAs(_remainingGroup);
            }

            [Test]
            public void Should_unassign_all_lights_in_the_group()
            {
                _savedModel.AllLights.Length.ShouldEqual(3);
                _remainingGroup.Lights.Length.ShouldEqual(1);
                var unassignedLights = _savedModel.UnassignedLights;
                unassignedLights.Length.ShouldEqual(2);
                unassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(1)).ShouldBeTrue();
                unassignedLights.Any(x => x.ZWaveIdentity.HomeId.Equals(1) && x.ZWaveIdentity.NodeId.Equals(2)).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_deleting_group_that_does_not_exist
        {
            private EditResult<LightGroup> _result;
            private MasterModel _savedModel;
            private StubMasterModelRepository _repository;
            private Guid _idDoesNotExist;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                var parentProject = existingMasterModel.CreateProject(x=>x.Name = "Existing Project");
                var existingGroup = parentProject.CreateGroup(x => x.Name = "Existing Group");

                _idDoesNotExist = Guid.NewGuid();

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.Delete(_idDoesNotExist);

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_fail()
            {
                _result.IsSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual(string.Format("Could not locate a light group with ID '{0}'",
                    _idDoesNotExist));
            }

            [Test]
            public void Should_not_save_the_model()
            {
                _savedModel.ShouldBeNull();
            }
        }
    }
}