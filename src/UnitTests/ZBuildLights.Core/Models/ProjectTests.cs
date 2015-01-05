using System;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Models;

namespace UnitTests.ZBuildLights.Core.Models
{
    public class ProjectTests
    {
        [TestFixture]
        public class When_adding_a_lightGroup
        {
            [Test]
            public void Should_set_parentProject()
            {
                var project = new MasterModel().CreateProject();
                var group = project.CreateGroup();
                group.ParentProject.ShouldBeSameAs(project);
            }
        }

        [TestFixture]
        public class When_creating_a_lightGroup_without_an_initializer
        {
            private LightGroup _group;
            private Project _project;
            private MasterModel _masterModel;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                _project = _masterModel.CreateProject();
                _group = _project.CreateGroup();
            }

            [Test]
            public void Should_set_parent_project()
            {
                _group.ParentProject.ShouldBeSameAs(_project);
            }

            [Test]
            public void Should_add_the_group_to_the_parent_project()
            {
                _project.Groups.Length.ShouldEqual(1);
                _project.Groups[0].ShouldBeSameAs(_group);
            }

            [Test]
            public void Should_provide_master_model()
            {
                _group.MasterModel.ShouldBeSameAs(_masterModel);
            }

            [Test]
            public void Should_set_light_group_id()
            {
                _group.Id.ShouldNotEqual(Guid.Empty);
            }
        }

        [TestFixture]
        public class When_creating_a_lightGroup_with_an_initializer_that_doesnt_set_the_ID
        {
            private LightGroup _group;
            private Project _project;
            private MasterModel _masterModel;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                _project = _masterModel.CreateProject();
                _group = _project.CreateGroup(x => x.Name = "Billy Bob");
            }

            [Test]
            public void Should_initialize()
            {
                _group.Name.ShouldEqual("Billy Bob");
            }

            [Test]
            public void Should_set_parent_project()
            {
                _group.ParentProject.ShouldBeSameAs(_project);
            }

            [Test]
            public void Should_provide_master_model()
            {
                _group.MasterModel.ShouldBeSameAs(_masterModel);
            }

            [Test]
            public void Should_set_light_group_id()
            {
                _group.Id.ShouldNotEqual(Guid.Empty);
            }
        }

        [TestFixture]
        public class When_creating_a_lightGroup_with_an_existing_id
        {
            private LightGroup _group;
            private Project _project;
            private MasterModel _masterModel;
            private Guid _id;

            [SetUp]
            public void ContextSetup()
            {
                _id = Guid.NewGuid();
                _masterModel = new MasterModel();
                _project = _masterModel.CreateProject();
                _group = _project.CreateGroup(x => x.Id = _id);

            }

            [Test]
            public void Should_set_parent_project()
            {
                _group.ParentProject.ShouldBeSameAs(_project);
            }

            [Test]
            public void Should_provide_master_model()
            {
                _group.MasterModel.ShouldBeSameAs(_masterModel);
            }

            [Test]
            public void Should_not_change_light_group_id()
            {
                _group.Id.ShouldEqual(_id);
            }
        }
    }
}