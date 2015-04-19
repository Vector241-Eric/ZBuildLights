using ZBuildLights.Core.Models.CruiseControl;

namespace UnitTests._TestData
{
    public class ProjectsProjectBuilder
    {
        private readonly ProjectsProject _entity = new ProjectsProject();

        public static implicit operator ProjectsProject(ProjectsProjectBuilder builder)
        {
            return builder._entity;
        }

        public ProjectsProjectBuilder Activity(CcBuildActivity activity)
        {
            _entity.activity = activity.ToString();
            return this;
        }

        public ProjectsProjectBuilder Status(CcBuildStatus status)
        {
            _entity.lastBuildStatus = status.ToString();
            return this;
        }

        public ProjectsProjectBuilder StatusString(string status)
        {
            _entity.lastBuildStatus = status;
            return this;
        }

        public ProjectsProjectBuilder Name(string name)
        {
            _entity.name = name;
            return this;
        }
    }
}