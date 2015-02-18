using System;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class CcProjectCollection
    {
        public CcProject[] Items { private get; set; }

        public CcProject[] Projects
        {
            get { return Items.OrderBy(x => x.Name).ToArray(); }
        }

        public class CcProject
        {
            public string Name { get; set; }
            public string LastBuildStatus { get; set; }

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