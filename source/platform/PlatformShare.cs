using System.Collections.Generic;
using System;

/*this.script = new LuaScript();



*/

public abstract class Program
{
    public string WorkspaceDir { get; private set; }

    protected string luaFilePath;
    protected LuaScript script;

    protected List<Project> projects;

    protected Program(string luaFilePath, string workspaceDir)
    {
        this.luaFilePath = luaFilePath;
        this.WorkspaceDir = workspaceDir;

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