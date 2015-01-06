using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ZBuildLights.Core.CruiseControl
{
    [GeneratedCode("xsd", "4.0.30319.18020")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Projects
    {
        [XmlElement("Project", Form = XmlSchemaForm.Unqualified)] public ProjectsProject[] Items;
    }

    [GeneratedCode("xsd", "4.0.30319.18020")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ProjectsProject
    {
        [XmlAttribute] public string activity;
        [XmlAttribute] public string lastBuildLabel;
        [XmlAttribute] public string lastBuildStatus;
        [XmlAttribute] public string lastBuildTime;
        [XmlAttribute] public string name;
        [XmlAttribute] public string webUrl;
    }
}