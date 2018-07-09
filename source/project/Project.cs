using System.Collections.Generic;
using System;

namespace Project
{
    enum Type
    {
        Executable,
        StaticLibrary,
        DynamicLibrary,
    }

    class Project
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string[] Files { get; set; }
        public string[] ProjectDependencies { get; set; }

        public Project() { }

        public Project(string name, Type type, string[] files, string[] projectDependencies)
        {
            this.Name = name;
            this.Type = type;
            this.Files = files;
            this.ProjectDependencies = projectDependencies;
        }
    }
}