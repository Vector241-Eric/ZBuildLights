using System;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using ZBuildLights.Core.CruiseControl;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.CruiseControl;
using ZBuildLights.Core.Wrappers;

namespace UnitTests.ZBuildLights.Core.Services.CruiseControl
{
    public class CruiseProjectModelProviderTests
    {
        [TestFixture]
        public class HappyPath : TestBase
        {
            private NetworkRequest<CcProjectCollection> _result;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var server1 = masterModel.CreateCruiseServer(x => x.Url = "http://www.example.com/1");
                var server2 = masterModel.CreateCruiseServer(x => x.Url = "http://www.example.com/2");
                var ccReaderData = new Projects {Items = new[] {new ProjectsProject(), new ProjectsProject()}};
                var mappedData = new CcProjectCollection
                {
                    Items = new[] {new CcProjectCollection.CcProject {Name = "Homer Wuz Here"}}
                };

                var ccReaderResponse =
                    NetworkRequest.Success(ccReaderData);

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus()).Return(masterModel);

                var ccReader = S<ICcReader>();
                ccReader.Stub(x => x.GetStatus("http://www.example.com/1")).Return(ccReaderResponse);

                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<Projects, CcProjectCollection>(ccReaderData)).Return(mappedData);

                var provider = new CruiseProjectModelProvider(ccReader, mapper, statusProvider);
                _result = provider.GetProjects(server1.Id);
            }

            [Test]
            public void Should_indicate_success()
            {
                _result.IsSuccessful.ShouldBeTrue();
            }

            [Test]
            public void Should_get_cc_projects_for_server()
            {
                _result.Data.Projects.Length.ShouldEqual(1);
                _result.Data.Projects[0].Name.ShouldEqual("Homer Wuz Here");
            }
        }

        [TestFixture]
        public class When_network_request_fails : TestBase
        {
            private NetworkRequest<CcProjectCollection> _result;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var server1 = masterModel.CreateCruiseServer(x => x.Url = "http://www.example.com/1");
                var server2 = masterModel.CreateCruiseServer(x => x.Url = "http://www.example.com/2");

                var ccReaderResponse = NetworkRequest.Fail<Projects>("Network request failed.", new Exception("I don't wanna"));

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus()).Return(masterModel);

                var ccReader = S<ICcReader>();
                ccReader.Stub(x => x.GetStatus("http://www.example.com/1")).Return(ccReaderResponse);

                IMapper doNotUseMapper = null;

                var provider = new CruiseProjectModelProvider(ccReader, doNotUseMapper, statusProvider);
                _result = provider.GetProjects(server1.Id);
            }

            [Test]
            public void Should_return_an_error_result()
            {
                _result.IsSuccessful.ShouldBeFalse();
            }

            [Test]
            public void Should_include_the_exception()
            {
                _result.Exception.Message.ShouldEqual("I don't wanna");
            }

            [Test]
            public void Should_include_a_failure_message_indicating_that_the_server_is_unavailable()
            {
                _result.Message.ShouldEqual("Could not reach cruise server at URL: [http://www.example.com/1]");
            }
        }
    }
}