using System;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public interface IAdminViewModelProvider
    {
        AdminViewModel GetIndexViewModel();
        EditProjectViewModel GetEditProjectViewModel(Guid? id);
        EditCruiseServerViewModel[] GetCruiseServerViewModels();
    }
}