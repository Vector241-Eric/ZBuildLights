using System;

namespace ZBuildLights.Core.Models
{
    public class EditProject
    {
        public Guid? ProjectId { get; set; }
        public string Name { get; set; }
        public EditProjectCruiseProject[] CruiseProjects { get; set; }

        public EditProjectCruiseProject[] SafeProjects
        {
            get { return CruiseProjects ?? new EditProjectCruiseProject[0]; }
        }
    }

    public class EditProjectCruiseProject
    {
        public string Project { get; set; }
        public Guid Server { get; set; }
    }
}