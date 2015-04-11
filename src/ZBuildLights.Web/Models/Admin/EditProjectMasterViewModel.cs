using System;
using System.Linq;
using Newtonsoft.Json;

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

        public string CruiseServerJson { get { return JsonConvert.SerializeObject(CruiseServers.Select(x => new {x.Name, x.Id, Projects = x.ProjectsByProjectAndName}).ToArray()); } }
    }

    public class EditProjectViewModel
    {
        public EditProjectViewModel()
        {
            CruiseProjectAssociations = new AdminCruiseProjectViewModel[0];
        }

        public string Name { get; set; }
        public Guid? Id { get; set; }
        public AdminCruiseProjectViewModel[] CruiseProjectAssociations { get; set; }
    }
}