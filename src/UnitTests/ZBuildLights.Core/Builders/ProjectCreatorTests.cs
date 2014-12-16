using System;
using System.Linq;
using NUnit.Framework;
using Should;
using UnitTests._Stubs;
using ZBuildLights.Core.Builders;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace UnitTests.ZBuildLights.Core.Builders
{
    public class ProjectCreatorTests
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

                var creator = new ProjectCreator(repository);
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
                existingMasterModel.AddProject(new Project { Name = "Existing Project" });

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(existingMasterModel);

                var creator = new ProjectCreator(repository);
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
    }
}