using System;
using System.Linq;
using NUnit.Framework;
using Should;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Validation;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class LightGroupManagerTests
    {
        [TestFixture]
        public class When_creating_new_group_with_unique_name
        {
            private MasterModel _savedModel;
            private CreationResult<LightGroup> _result;

            [SetUp]
            public void ContextSetup()
            {
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                project.AddGroup(new LightGroup {Name = "Existing Group"});

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.CreateLightGroup(project.Id, "My New Group");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_the_creation_was_successful()
            {
                _result.WasSuccessful.ShouldBeTrue();
            }

            [Test]
            public void Should_save_the_existing_master_model()
            {
                _savedModel.ShouldNotBeNull();
            }

            [Test]
            public void Should_add_the_group_to_the_project()
            {
                _savedModel.Projects.Single().Groups.Length.ShouldEqual(2);
            }

            [Test]
            public void Should_set_the_project_name()
            {
                _savedModel.Projects.Single().Groups.Count(x => x.Name.Equals("My New Group")).ShouldEqual(1);
            }

            [Test]
            public void Should_set_the_id_of_the_project()
            {
                var group = _savedModel.Projects.Single().Groups.Single(x => x.Name.Equals("My New Group"));
                group.Id.ShouldNotEqual(Guid.Empty);
            }
        }

        [TestFixture]
        public class When_creating_a_group_for_a_project_that_doesnt_exist
        {
            private MasterModel _savedModel;
            private CreationResult<LightGroup> _result;
            private Guid _projectIdDoesntExist;

            [SetUp]
            public void ContextSetup()
            {
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                project.AddGroup(new LightGroup {Name = "Existing Group"});

                _projectIdDoesntExist = Guid.NewGuid();

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.CreateLightGroup(_projectIdDoesntExist, "My New Group");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_failure()
            {
                _result.WasSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual(string.Format("Cannot create group for project '{0}' that doesn't exist",
                    _projectIdDoesntExist));
            }

            [Test]
            public void Should_not_save_the_master_model()
            {
                _savedModel.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_creating_a_new_group_with_a_name_that_is_already_used
        {
            private CreationResult<LightGroup> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                project.AddGroup(new LightGroup {Name = "Existing Group"});

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.CreateLightGroup(project.Id, "Existing Group");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_failure()
            {
                _result.WasSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                _result.Message.ShouldEqual("A group with this name already exists");
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
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var existingGroup = new LightGroup {Name = "Existing Group", Id = Guid.NewGuid()};
                project.AddGroup(existingGroup);
                project.AddGroup(new LightGroup {Id = Guid.NewGuid()});

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.UpdateLightGroup(existingGroup.Id, "My New Name");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_the_update_was_successful()
            {
                _result.WasSuccessful.ShouldBeTrue();
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
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var existingGroup = new LightGroup {Name = "Existing Group", Id = Guid.NewGuid()};
                project.AddGroup(existingGroup);
                project.AddGroup(new LightGroup {Id = Guid.NewGuid(), Name = "Name Collision"});

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.UpdateLightGroup(existingGroup.Id, "Name Collision");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_that_update_was_not_successful()
            {
                _result.WasSuccessful.ShouldBeFalse();
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
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var existingGroup = new LightGroup {Name = "Existing Group", Id = Guid.NewGuid()};
                project.AddGroup(existingGroup);
                project.AddGroup(new LightGroup {Id = Guid.NewGuid()});

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.UpdateLightGroup(existingGroup.Id, "Existing Group");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_that_update_was_successful()
            {
                _result.WasSuccessful.ShouldBeTrue();
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
                var project = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var existingGroup = new LightGroup {Name = "Existing Group", Id = Guid.NewGuid()};
                project.AddGroup(existingGroup);
                project.AddGroup(new LightGroup {Id = Guid.NewGuid()});

                _groupIdDoesntExist = Guid.NewGuid();

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.UpdateLightGroup(_groupIdDoesntExist, "My New Name");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_fail()
            {
                _result.WasSuccessful.ShouldBeFalse();
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
                _parentProject = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var existingGroup = new LightGroup {Name = "Existing Group", Id = Guid.NewGuid()};
                existingGroup.AddLight(new Light(1, 1));
                existingGroup.AddLight(new Light(1, 2));
                _parentProject.AddGroup(existingGroup);
                _remainingGroup = new LightGroup {Id = Guid.NewGuid()};
                _remainingGroup.AddLight(new Light(1, 10));
                _parentProject.AddGroup(_remainingGroup);

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(_parentProject);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.DeleteLightGroup(existingGroup.Id);

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
                var unassignedLights = _savedModel.GetUnassignedGroup().Lights;
                unassignedLights.Length.ShouldEqual(2);
                unassignedLights.Any(x => x.ZWaveHomeId.Equals(1) && x.ZWaveDeviceId.Equals(1)).ShouldBeTrue();
                unassignedLights.Any(x => x.ZWaveHomeId.Equals(1) && x.ZWaveDeviceId.Equals(2)).ShouldBeTrue();
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
                var parentProject = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var existingGroup = new LightGroup {Name = "Existing Group", Id = Guid.NewGuid()};
                parentProject.AddGroup(existingGroup);

                _idDoesNotExist = Guid.NewGuid();

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(parentProject);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new LightGroupManager(repository);
                _result = creator.DeleteLightGroup(_idDoesNotExist);

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_fail()
            {
                _result.WasSuccessful.ShouldBeFalse();
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