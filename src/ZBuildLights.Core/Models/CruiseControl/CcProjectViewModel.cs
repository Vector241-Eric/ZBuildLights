using System;

namespace ZBuildLights.Core.Models.CruiseControl
{
    public class CcProjectViewModel
    {
        public string Name { get; set; }
        public string LastBuildStatus { get; set; }
        public string Activity { get; set; }

        public string Status
        {
            get
            {
                var building = "Building".Equals(Activity) ? " and Building" : string.Empty;
                return string.Format("{0}{1}", LastBuildStatus, building);
            }
        }

        public string ProjectAndName
        {
            get
            {
                var splits = Name.Split(new[] {" :: "}, StringSplitOptions.None);
                if (splits.Length == 1)
                    return splits[0];
                return string.Format("{0}.{1}", splits[splits.Length - 2], splits[splits.Length - 1]);
            }
        }
    }
}