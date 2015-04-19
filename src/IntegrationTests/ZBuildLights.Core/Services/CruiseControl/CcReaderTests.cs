using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Services.CruiseControl;

namespace IntegrationTests.ZBuildLights.Core.Services.CruiseControl
{
    public class CcReaderTests
    {
        [TestFixture]
        public class When_happy_path
        {
            [Test, Explicit]
            public void Should_read_ccXml()
            {
                var reader = new CcReader();
                var result =
                    reader.GetStatus(
                        "http://csbeap-ci1.oldev.preol.dell.com:8080/guestAuth/app/rest/cctray/projects.xml");
                result.IsSuccessful.ShouldBeTrue();
                Console.WriteLine(JsonConvert.SerializeObject(result.Data, Formatting.Indented));
            } 
        } 
    }
}