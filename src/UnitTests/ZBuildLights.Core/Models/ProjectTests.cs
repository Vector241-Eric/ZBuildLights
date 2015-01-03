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
                var project = new Project();
                var group = new LightGroup();
                project.AddGroup(group);

                group.ParentProject.ShouldBeSameAs(project);
            }
        }
    }
}