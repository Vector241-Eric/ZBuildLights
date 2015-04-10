using System;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public interface IAdminViewModelProvider
    {
        AdminViewModel GetIndexViewModel();
        EditProjectMasterViewModel GetEditProjectViewModel(Guid? id);
        EditCruiseServerViewModel[] GetCruiseServerViewModels();
    }
}