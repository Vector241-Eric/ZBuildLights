namespace ZBuildLights.Web.Models.Admin
{
    public class EditProjectViewModel
    {
        public AdminProjectViewModel Project { get; set; }
        public EditCruiseServerViewModel[] CruiseServers { get; set; }
    }
}