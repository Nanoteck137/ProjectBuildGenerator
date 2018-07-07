using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using MoonSharp.Interpreter;

/*
    TODO:
     - Need to redo the code generator for make files
     - Need to implement the code generator 
        for batch and shell files
     - 
 */

enum Mode
{
    Unknown,
    Windows,
    Linux,
}

class Config
{
    public string Compiler { get; set; }
    public string CompilerOnlySwitch { get; set; }
    public string Packer { get; set; }
    public Mode Mode { get; set; }

    public Config() { }
}

class Helper
{
    private Helper() { }

    public static string GetExecutableFileExt(Mode mode)
    {
        switch(mode)
        {
            case Mode.Windows: return ".exe";
            case Mode.Linux: return "";

            default: throw new Exception();
        }
    }

    public static string GetObjectFileExt(Mode mode)
    {
        switch(mode)
        {
            case Mode.Windows: return ".obj";
            case Mode.Linux: return ".o";

            default: throw new Exception();
        }
    }

    public static string GetObjFileName(Config config, string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName) + GetObjectFileExt(config.Mode);
    }

    public static string CreateTargetCode(Config config, string name, bool objectCompile, string dependencies)
    {
        if(objectCompile)
            return string.Format("{0}{1}: {2}", name, GetObjectFileExt(config.Mode), dependencies);
        return string.Format("{0}{1}: {2}", name, GetExecutableFileExt(config.Mode), dependencies);
    }

    public static string CreateCompilerCode(Config config, bool compilerOnly, string files)
    {
        if(compilerOnly)
            return string.Format("{0} {1} {2}", config.Compiler, config.CompilerOnlySwitch, files);
        return string.Format("{0}{1}", config.Compiler, files);
    }
}



class Program
{
    private LuaScript script;

    private Dictionary<Mode, Config> configs;
    private List<Project.Project> projects;

    private CodeGen.Make.Generator makeGenerator;

    public Program(Mode mode)
    {
        this.script = new LuaScript();
        this.configs = new Dictionary<Mode, Config>();
        this.projects = new List<Project.Project>();

        makeGenerator = new CodeGen.Make.Generator();

        SetupLua();

        this.script.RunScript("test.lua");

        this.script.CallFunction("Init", mode);

        Project.Project[] projects = Project.ProjectManager.GetProjects();

        Console.Read();
    }

    private void CreateMakeTargetsFromProject(CodeGen.Make.Generator generator, Project.Project project, Config config)
    {
        foreach(string file in project.Files)
        {
            string targetName = Helper.GetObjFileName(config, file);
            
        }
    }

    private void SetupLua()
    {
        this.script.AddEnumType("Mode", typeof(Mode));
        this.script.AddEnumType("ProjectType", typeof(Project.Type));

        LuaInterface.ProjectInterface projectInterface = new LuaInterface.ProjectInterface(this);
        this.script.AddInterface("Project", projectInterface);

        LuaInterface.ConfigInterface configInterface = new LuaInterface.ConfigInterface(this);
        this.script.AddInterface("Config", configInterface);
    }

    public void AddConfig(Mode mode, Config config)
    {
        this.configs.Add(mode, config);
    }

    public void AddProject(Project.Project project)
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
