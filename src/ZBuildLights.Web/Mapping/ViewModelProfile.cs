using AutoMapper;
using ZBuildLights.Core.CruiseControl;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Mapping
{
    public class ViewModelProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Project, AdminProjectViewModel>();
            CreateMap<LightGroup, AdminLightGroupViewModel>();
            CreateMap<Light, AdminLightViewModel>();
            CreateMap<CruiseServer, EditCruiseServerViewModel>();

            //Mapping from CCXML to View Model
            CreateMap<Projects, CcProjectCollectionViewModel>()
                .ForMember(x => x.Projects, opt => opt.Ignore());
            CreateMap<ProjectsProject, CcProjectViewModel>();
        }
    }
}