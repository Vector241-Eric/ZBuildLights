using System;
using System.Linq;
using ZBuildLights.Core.CruiseControl;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Wrappers;

namespace ZBuildLights.Core.Services.CruiseControl
{
    public class CruiseProjectModelProvider : ICruiseProjectModelProvider
    {
        private readonly ICcReader _ccReader;
        private readonly IMapper _mapper;
        private readonly ISystemStatusProvider _statusProvider;

        public CruiseProjectModelProvider(ICcReader ccReader, IMapper mapper, ISystemStatusProvider statusProvider)
        {
            _ccReader = ccReader;
            _mapper = mapper;
            _statusProvider = statusProvider;
        }

        public NetworkResponse<CcProjectCollectionViewModel> GetProjects(Guid serverId)
        {
            var server = _statusProvider.GetSystemStatus().CruiseServers.Single(x => x.Id.Equals(serverId));
            var ccResult = _ccReader.GetStatus(server.Url);
            if (ccResult.IsSuccessful)
            {
                var ccProjects = ccResult.Data;
                var viewModel = _mapper.Map<Projects, CcProjectCollectionViewModel>(ccProjects);
                return NetworkResponse.Success(viewModel);
            }
            return NetworkResponse.Fail<CcProjectCollectionViewModel>(string.Format("Could not reach cruise server at URL: [{0}]", server.Url), ccResult.Exception);
        }
    }
}