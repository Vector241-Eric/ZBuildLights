using AutoMapper;
using NUnit.Framework;
using ZBuildLights.Web;

namespace IntegrationTests.Mapping
{
    public class AutoMapperConfigurationTests
    {
        [TestFixture]
        public class When_automapper_is_configured
        {
            [SetUp]
            public void ContextSetup()
            {
                AutoMapperConfig.Initialize();
            }

            [Test]
            public void Should_map()
            {
                Mapper.AssertConfigurationIsValid();
            } 
        } 
    }
}