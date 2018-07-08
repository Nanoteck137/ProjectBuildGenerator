using System.Collections.Generic;
using System;

namespace Project
{
    enum Type
    {
        Unknown,
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
    }

    class ProjectManager
    {
        private static List<Project> projects = new List<Project>();

        private ProjectManager() { }

        public static void AddProject(Project project)
        {
            projects.Add(project);
        }

        public static Project[] GetProjects() 
        {
            return projects.ToArray();
        }
    }
}