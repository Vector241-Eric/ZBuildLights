using System.Collections.Generic;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Services.CruiseControl;

namespace UnitTests._Stubs
{
    public class StubCcReader : ICcReader
    {
        private IDictionary<string, int> _lookupCount = new Dictionary<string, int>();

        private IDictionary<string, NetworkResponse<Projects>> _stubbedResponses =
            new Dictionary<string, NetworkResponse<Projects>>();

        public NetworkResponse<Projects> GetStatus(string url)
        {
            if (!_lookupCount.ContainsKey(url))
                _lookupCount[url] = 0;
            _lookupCount[url] = _lookupCount[url] + 1;

            if (!_stubbedResponses.ContainsKey(url))
                return NetworkResponse.Fail<Projects>(string.Format("The response for url [{0}] must be stubbed.", url));
            return _stubbedResponses[url];
        }

        public StubCcReader WithResponse(string url, NetworkResponse<Projects> stubResponse)
        {
            _stubbedResponses[url] = stubResponse;
            return this;
        }

        public int GetLookupCountForUrl(string url)
        {
            if (!_lookupCount.ContainsKey(url))
                return 0;
            return _lookupCount[url];
        }
    }
}