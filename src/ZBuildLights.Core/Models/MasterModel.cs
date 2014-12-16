using System;
using ZBuildLights.Core.Extensions;

namespace ZBuildLights.Core.Models
{
    public class MasterModel
    {
        public Project[] Projects { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public void AddProject(Project project)
        {
            Projects = Projects.AddToEnd(project);
        }
    }
}