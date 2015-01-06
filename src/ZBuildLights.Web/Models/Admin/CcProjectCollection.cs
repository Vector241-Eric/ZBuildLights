using System;
using System.Linq;

namespace ZBuildLights.Web.Models.Admin
{
    public class CcProjectCollection
    {
        public Project[] Items { private get; set; }

        public Project[] Projects
        {
            get { return Items.OrderBy(x => x.Name).ToArray(); }
        }

        public class Project
        {
            public string Name { get; set; }

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
}