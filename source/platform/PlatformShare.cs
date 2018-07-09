using System.Collections.Generic;
using System;

/*this.script = new LuaScript();



*/

public abstract class Program
{
    //TODO: We want the lua script to live here
    //TODO: We want a list of projects
    private string luaFilePath;
    private LuaScript script;
    protected List<Project> projects;

    protected Program(string luaConfigPath)
    {
        this.luaFilePath = luaConfigPath;

        projects = new List<Project>();
        script = new LuaScript();
    }

    protected void SetupLua()
    {
        this.script.AddEnumType("ProjectType", typeof(ProjectType));

        LuaProjectInterface projectInterface = new LuaProjectInterface(this);
        this.script.AddInterface("Project", projectInterface);
    }

    protected void InitLuaScript()
    {
        this.script.RunScript(luaFilePath);
        this.script.CallFunction("Init");
    }

    public void AddProject(Project project)
    {
        projects.Add(project);
    }

    //TODO: A method to setup some generic lua initialization
    //TODO: A method to create the project or add it (im not sure yet)
}