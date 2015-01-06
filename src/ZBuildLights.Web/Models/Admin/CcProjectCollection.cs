namespace ZBuildLights.Web.Models.Admin
{
    public class CcProjectCollection
    {
        public Project[] Projects { get; set; }

        public class Project
        {
            public string Name { get; set; }
        }
    }
}