using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using Newtonsoft.Json;

using MoonSharp.Interpreter;

enum Mode
{
    Unknown,
    Windows,
    Linux,
}

enum ProjectType
{
    Unknown,
    Executable,
    StaticLibrary,
    DynamicLibrary,
}

class Project
{
    public string Name { get; set; }
    public ProjectType Type { get; set; }

    public Project() { }
}

class Program
{
    private LuaScript script;

    private List<Project> projects;

    public Program(Mode mode)
    {
        this.script = new LuaScript();
        this.projects = new List<Project>();

        this.script.AddEnumType("Mode", typeof(Mode));
        this.script.AddEnumType("ProjectType", typeof(ProjectType));

        LuaInterface.ProjectInterface projectInterface = new LuaInterface.ProjectInterface(this);
        this.script.AddInterface("Project", projectInterface);

        this.script.RunScript("test.lua");

        this.script.CallFunction("Init", mode);

        Console.Read();
    }

    public void AddProject(Project project)
    {
        this.projects.Add(project);
    }

    public static void Main(string[] args)
    {
        Mode mode = Mode.Unknown;
        if(args.Length == 1)
        {
            if(args[0] == "-win32")
                mode = Mode.Windows;
            else if(args[0] == "-linux")
                mode = Mode.Linux;
            else 
                throw new Exception();
        }

        UserData.RegisterAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        new Program(mode);
    }
}
