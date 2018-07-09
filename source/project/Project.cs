using System.Collections.Generic;
using System;

public enum ProjectType
{
    Executable,
    StaticLibrary,
    DynamicLibrary,
}

public class Project
{
    public string Name { get; set; }
    public ProjectType Type { get; set; }
    public string[] Files { get; set; }
    public string[] ProjectDependencies { get; set; }

    public Project() { }

    public Project(string name, ProjectType type, string[] files, string[] projectDependencies)
    {
        this.Name = name;
        this.Type = type;
        this.Files = files;
        this.ProjectDependencies = projectDependencies;
    }
}