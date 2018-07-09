using System.Collections.Generic;
using System;

/*this.script = new LuaScript();

this.script.AddEnumType("ProjectType", typeof(Project.Type));

ProjectInterface projectInterface = new ProjectInterface();
this.script.AddInterface("Project", projectInterface);

this.script.RunScript(luaConfigPath);
this.script.CallFunction("Init");*/

public abstract class Program
{
    //TODO: We want the lua script to live here
    //TODO: We want a list of projects
    protected List<Project> projects;

    protected Program(string luaConfigPath)
    {
        projects = new List<Project>();
    }

    public void AddProject(Project project)
    {
        projects.Add(project);
    }

    //TODO: A method to setup some generic lua initialization
    //TODO: A method to create the project or add it (im not sure yet)
}