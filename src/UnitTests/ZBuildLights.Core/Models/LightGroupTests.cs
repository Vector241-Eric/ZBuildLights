﻿using System;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Models;

namespace UnitTests.ZBuildLights.Core.Models
{
    public class LightGroupTests
    {
        [TestFixture]
        public class When_getting_the_full_name_of_a_group
        {
            [Test]
            public void Should_include_the_name_of_the_parent_project()
            {
                var project = new Project {Name = "Foo"};
                var group = new LightGroup {Name = "Bar"};
                project.AddGroup(group);

                group.FullName.ShouldEqual("Foo.Bar");
            } 
        }

        [TestFixture]
        public class When_getting_the_full_name_of_a_group_that_isnt_in_a_project
        {
            [Test]
            public void Should_simply_provide_the_name_of_the_group()
            {
                var group = new LightGroup {Name = "Bar"};

                group.FullName.ShouldEqual("Bar");
            } 
        }

        [TestFixture]
        public class When_adding_a_light_that_isnt_in_a_group
        {
            private LightGroup _group;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _group = new LightGroup();
                _light = new Light(1, 2);
                _group.AddLight(_light);
            }

            [Test]
            public void Should_set_the_group_referene_to_the_containing_group()
            {
                _light.ParentGroup.ShouldBeSameAs(_group);
            }

            [Test]
            public void Should_add_the_light_to_the_light_collection_on_the_group()
            {
                _group.Lights.Length.ShouldEqual(1);
            }
        }

        [TestFixture]
        public class When_adding_a_light_that_is_already_in_a_group
        {
            private Exception _thrown;

            [SetUp]
            public void ContextSetup()
            {
                var group = new LightGroup {Name = "Foo", ParentProject = new Project {Name = "FooDaddy"}};
                var light = new Light(1, 2);
                group.AddLight(light);

                var newGroup = new LightGroup {Name = "Bar", ParentProject = new Project {Name = "BarDaddy"}};
                try
                {
                    newGroup.AddLight(light);
                }
                catch (Exception e)
                {
                    _thrown = e;
                }
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _thrown.ShouldNotBeNull();
                _thrown.GetType().ShouldEqual(typeof(InvalidOperationException));
                _thrown.Message.ShouldEqual(
                    "Cannot add light to group BarDaddy.Bar because it already belongs to group FooDaddy.Foo");
            }
        }

        [TestFixture]
        public class When_removing_a_light_from_a_group
        {
            private LightGroup _group;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _group = new LightGroup();
                _light = new Light(1, 2);

                _group.AddLight(_light);
                _group.RemoveLight(_light);
            }

            [Test]
            public void Should_remove_the_light_from_the_groups_collection()
            {
                _group.Lights.Length.ShouldEqual(0);
            }

            [Test]
            public void Should_remove_the_parent_reference_to_the_group()
            {
                _light.ParentGroup.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_attempting_to_remove_a_light_that_is_in_a_different_group
        {
            private LightGroup _bar;
            private Light _light;
            private Exception _thrown;

            [SetUp]
            public void ContextSetup()
            {
                _bar = new LightGroup{Name = "Bar"};
                var foo = new LightGroup{Name = "Foo"};
                _light = new Light(1,2);

                new Project { Name = "BarDaddy" }.AddGroup(_bar);
                new Project { Name = "FooDaddy" }.AddGroup(foo);

                _bar.AddLight(_light);

                try
                {
                    foo.RemoveLight(_light);
                }
                catch (Exception e)
                {
                    _thrown = e;
                }
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _thrown.GetType().ShouldEqual(typeof(InvalidOperationException));
                _thrown.Message.ShouldEqual("Cannot remove light from group FooDaddy.Foo because it belongs to group BarDaddy.Bar");
            }

            [Test]
            public void Should_not_change_the_parent_reference_on_the_light()
            {
                _light.ParentGroup.ShouldBeSameAs(_bar);
            }
        }
    }
}