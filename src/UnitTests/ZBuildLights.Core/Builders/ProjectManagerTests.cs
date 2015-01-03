using System;
using System.Linq;
using NUnit.Framework;
using Should;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Validation;

namespace UnitTests.ZBuildLights.Core.Builders
{
    public class ProjectManagerTests
    {
        [TestFixture]
        public class When_creating_new_project_with_unique_name
        {
            private MasterModel _savedModel;
            private CreationResult<Project> _result;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(new Project {Name = "Existing Project"});

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new ProjectManager(repository);
                _result = creator.CreateProject("My New Project");

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
            public void Should_add_the_project_to_the_existing_master_model()
            {
                _savedModel.Projects.Length.ShouldEqual(2);
                _savedModel.Projects.Count(x => ReferenceEquals(x, _result.Entity)).ShouldEqual(1);
            }

            [Test]
            public void Should_set_the_project_name()
            {
                _result.Entity.Name.ShouldEqual("My New Project");
            }

            [Test]
            public void Should_set_the_id_of_the_project()
            {
                _result.Entity.Id.ShouldNotEqual(Guid.Empty);
            }
        }

        [TestFixture]
        public class When_creating_a_new_project_with_a_name_that_is_already_used
        {
            private CreationResult<Project> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(new Project {Name = "Existing Project"});

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new ProjectManager(repository);
                _result = creator.CreateProject("Existing Project");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_that_creation_was_not_successful()
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
                _result.Message.ShouldEqual("There is already a project named 'Existing Project'");
            }
        }

        [TestFixture]
        public class When_updating_a_project_with_unique_name
        {
            private MasterModel _savedModel;
            private EditResult<Project> _result;

            [SetUp]
            public void ContextSetup()
            {
                var projectToEdit = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var project2 = new Project {Name = "Existing Project 2", Id = Guid.NewGuid()};
                var project3 = new Project {Name = "Existing Project 3", Id = Guid.NewGuid()};

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(projectToEdit);
                existingMasterModel.AddProject(project2);
                existingMasterModel.AddProject(project3);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new ProjectManager(repository);
                _result = creator.UpdateProject(projectToEdit.Id, "My New Name");

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
            public void Should_not_add_any_projects()
            {
                _savedModel.Projects.Length.ShouldEqual(3);
                _savedModel.Projects.Count(x => ReferenceEquals(x, _result.Entity)).ShouldEqual(1);
            }

            [Test]
            public void Should_set_the_project_name()
            {
                _result.Entity.Name.ShouldEqual("My New Name");
            }

            [Test]
            public void Should_not_change_the_id_of_the_project()
            {
                _result.Entity.Id.ShouldNotEqual(Guid.Empty);
            }
        }

        [TestFixture]
        public class When_updating_a_project_with_a_name_that_is_already_used
        {
            private EditResult<Project> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var project1 = new Project {Name = "Name Collision", Id = Guid.NewGuid()};
                var projectToEdit = new Project {Name = "Existing Project 2", Id = Guid.NewGuid()};
                var project3 = new Project {Name = "Existing Project 3", Id = Guid.NewGuid()};

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project1);
                existingMasterModel.AddProject(projectToEdit);
                existingMasterModel.AddProject(project3);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new ProjectManager(repository);
                _result = creator.UpdateProject(projectToEdit.Id, "Name Collision");

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
                _result.Message.ShouldEqual("There is already a project named 'Name Collision'");
            }
        }

        [TestFixture]
        public class When_updating_a_project_and_setting_the_name_to_the_value_it_already_has
        {
            private EditResult<Project> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var project1 = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var projectToEdit = new Project {Name = "Keep This", Id = Guid.NewGuid()};
                var project3 = new Project {Name = "Existing Project 3", Id = Guid.NewGuid()};

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project1);
                existingMasterModel.AddProject(projectToEdit);
                existingMasterModel.AddProject(project3);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new ProjectManager(repository);
                _result = creator.UpdateProject(projectToEdit.Id, "Keep This");

                _savedModel = repository.LastSaved;
            }

            [Test]
            public void Should_indicate_that_update_was_successful()
            {
                _result.WasSuccessful.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_updating_a_project_that_does_not_exist
        {
            private EditResult<Project> _result;
            private MasterModel _savedModel;
            private Guid _deleteId = Guid.NewGuid();
            private StubMasterModelRepository _repository;

            [SetUp]
            public void ContextSetup()
            {
                var project1 = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var project2 = new Project {Name = "Existing Project 2", Id = Guid.NewGuid()};
                var project3 = new Project {Name = "Existing Project 3", Id = Guid.NewGuid()};

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project1);
                existingMasterModel.AddProject(project2);
                existingMasterModel.AddProject(project3);

                _repository = new StubMasterModelRepository();
                _repository.UseCurrentModel(existingMasterModel);

                var manager = new ProjectManager(_repository);
                _result = manager.UpdateProject(_deleteId, "Value doesn't matter");

                _savedModel = _repository.LastSaved;
            }

            [Test]
            public void Should_fail()
            {
                _result.WasSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                var expectedMessage = string.Format("Could not locate a project with Id '{0}'", _deleteId);
                _result.Message.ShouldEqual(expectedMessage);
            }

            [Test]
            public void Should_not_save_the_model()
            {
                _repository.LastSaved.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_deleting_project_that_exists
        {
            private CreationResult<Project> _result;
            private MasterModel _savedModel;

            [SetUp]
            public void ContextSetup()
            {
                var project1 = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var project2 = new Project {Name = "Existing Project 2", Id = Guid.NewGuid()};
                var project3 = new Project {Name = "Existing Project 3", Id = Guid.NewGuid()};

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project1);
                existingMasterModel.AddProject(project2);
                existingMasterModel.AddProject(project3);

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var manager = new ProjectManager(repository);
                manager.DeleteProject(project2.Id);

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
                _savedModel.Projects.Length.ShouldEqual(2);
                _savedModel.Projects.Count(x => x.Name == "Existing Project").ShouldEqual(1);
                _savedModel.Projects.Count(x => x.Name == "Existing Project 3").ShouldEqual(1);
            }
        }

        [TestFixture]
        public class When_deleting_project_that_does_not_exist
        {
            private EditResult<Project> _result;
            private MasterModel _savedModel;
            private Guid _deleteId = Guid.NewGuid();
            private StubMasterModelRepository _repository;

            [SetUp]
            public void ContextSetup()
            {
                var project1 = new Project {Name = "Existing Project", Id = Guid.NewGuid()};
                var project2 = new Project {Name = "Existing Project 2", Id = Guid.NewGuid()};
                var project3 = new Project {Name = "Existing Project 3", Id = Guid.NewGuid()};

                var existingMasterModel = new MasterModel();
                existingMasterModel.AddProject(project1);
                existingMasterModel.AddProject(project2);
                existingMasterModel.AddProject(project3);

                _repository = new StubMasterModelRepository();
                _repository.UseCurrentModel(existingMasterModel);

                var manager = new ProjectManager(_repository);
                _result = manager.DeleteProject(_deleteId);

                _savedModel = _repository.LastSaved;
            }

            [Test]
            public void Should_fail()
            {
                _result.WasSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_have_a_reasonable_failure_message()
            {
                var expectedMessage = string.Format("Could not locate a project with Id '{0}'", _deleteId);
                _result.Message.ShouldEqual(expectedMessage);
            }

            [Test]
            public void Should_not_save_the_model()
            {
                _repository.LastSaved.ShouldBeNull();
            }
        }
    }
}