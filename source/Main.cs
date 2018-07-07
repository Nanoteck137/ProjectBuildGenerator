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

enum ProjectType
{
    Unknown,
    Executable,
    StaticLibrary,
    DynamicLibrary,
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

class Project
{
    public string Name { get; set; }
    public ProjectType Type { get; set; }
    public string[] Files { get; set; }

    public Project() { }

    public void GenerateCode(Config config, StringBuilder result)
    {
        //TODO: Redo code generation
        string objFiles = "";

        foreach(string file in this.Files)
        {
            string name = Path.GetFileNameWithoutExtension(file);

            result.Append(Helper.CreateTargetCode(config, name, true, file));
            result.Append("\n");

            result.Append("\t");
            result.Append(Helper.CreateCompilerCode(config, true, file));

            result.Append("\n\n");

            objFiles = objFiles + " " + Helper.GetObjFileName(config, file);
        }

        result.Append(Helper.CreateTargetCode(config, this.Name, false, objFiles));
        result.Append("\n");

        result.Append("\t");
        result.Append(Helper.CreateCompilerCode(config, false, objFiles));

        result.Append(" /Fe" + this.Name + ".exe");
        result.Append("\n");
    }
}

class Program
{
    private LuaScript script;

    private Dictionary<Mode, Config> configs;
    private List<Project> projects;

    public Program(Mode mode)
    {
        this.script = new LuaScript();
        this.configs = new Dictionary<Mode, Config>();
        this.projects = new List<Project>();

        this.script.AddEnumType("Mode", typeof(Mode));
        this.script.AddEnumType("ProjectType", typeof(ProjectType));

        LuaInterface.ProjectInterface projectInterface = new LuaInterface.ProjectInterface(this);
        this.script.AddInterface("Project", projectInterface);

        LuaInterface.ConfigInterface configInterface = new LuaInterface.ConfigInterface(this);
        this.script.AddInterface("Config", configInterface);

        this.script.RunScript("test.lua");

        this.script.CallFunction("Init", mode);

        Console.WriteLine(GenerateCode(configs[mode]));
        Console.Read();
    }

    public string GenerateCode(Config config)
    {
        StringBuilder result = new StringBuilder();

        result.Append("all:\n\t@echo Select a target\n\n");

        this.projects[0].GenerateCode(config, result);
        return result.ToString();
    }

    public void AddConfig(Mode mode, Config config)
    {
        this.configs.Add(mode, config);
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
