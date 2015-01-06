using ZBuildLights.Core.CruiseControl;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public interface ICruiseProjectModelProvider
    {
        CcProjectCollection GetProjects(string url);
    }

    public class CruiseProjectModelProvider : ICruiseProjectModelProvider
    {
        private readonly ICcReader _ccReader;
        private readonly IMapper _mapper;

        public CruiseProjectModelProvider(ICcReader ccReader, IMapper mapper)
        {
            _ccReader = ccReader;
            _mapper = mapper;
        }

        public CcProjectCollection GetProjects(string url)
        {
            var ccProjects = _ccReader.GetStatus(url);
            var viewModel = _mapper.Map<Projects, CcProjectCollection>(ccProjects);
            return viewModel;
        }
    }
}