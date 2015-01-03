using NUnit.Framework;

namespace UnitTests.ZBuildLights.Core.Builders
{
    public class SystemStatusProviderTests
    {
        [TestFixture]
        public class When_updating_light_status_and_all_lights_are_in_the_network
        {
            [Test]
            public void Should_set_light_status()
            {
                throw new System.NotImplementedException("Not yet implemented");
            } 
        }

        [TestFixture]
        public class When_some_lights_are_not_in_the_network_and_some_are
        {
            [Test]
            public void Should_set_light_status_for_the_lights_in_the_network()
            {
                throw new System.NotImplementedException("Not yet implemented");
            }

            [Test]
            public void Should_set_the_status_to_unknown_for_lights_not_in_the_network()
            {
                throw new System.NotImplementedException("Not yet implemented");
            }
        }
    }
}