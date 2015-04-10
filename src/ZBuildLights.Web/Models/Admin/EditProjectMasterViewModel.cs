using System;

namespace ZBuildLights.Web.Models.Admin
{
    public class EditProjectMasterViewModel
    {
        public EditProjectViewModel Project { get; set; }
        public EditProjectCruiseServerViewModel[] CruiseServers { get; set; }
        public string HeaderText { get; set; }
        public bool ShowDelete { get; set; }
        public string ErrorMessage { get; set; }
        public bool ShowErrorMessage { get { return !string.IsNullOrEmpty(ErrorMessage); }}
    }

    public class EditProjectViewModel
    {
        public EditProjectViewModel()
        {
            CruiseProjects = new AdminCruiseProjectViewModel[0];
        }

        public string Name { get; set; }
        public Guid? Id { get; set; }
        public AdminCruiseProjectViewModel[] CruiseProjects { get; set; }
    }
}