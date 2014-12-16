using System;
using System.Linq;
using ZBuildLights.Core.Extensions;

namespace ZBuildLights.Core.Models
{
    public class MasterModel
    {
        public MasterModel()
        {
            Projects = new Project[0];
        }

        public Project[] Projects { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public void AddProject(Project project)
        {
            Projects = Projects.AddToEnd(project);
        }

        public void RemoveProject(Guid projectId)
        {
            Projects = Projects.Except(Projects.Where(x => x.Id.Equals(projectId))).ToArray();
        }
    }
}