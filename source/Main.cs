using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using MoonSharp.Interpreter;

using MakeGen = CodeGen.Make;

/*
    TODO:
     - Need to redo the code generator for make files
     - Need to implement the code generator 
        for batch and shell files
     - Give all the object compile result a diffrent name like: ProjectName_InputName.obj
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
    public string OutputObjSwitch { get; set; }
    public string OutputExeSwitch { get; set; }

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

    public static string GetDefaultCompiler(Mode mode)
    {
        switch(mode)
        {
            case Mode.Windows: return "cl";
            case Mode.Linux: return "clang++";
            default: throw new Exception();
        }
    }

    public static string GetDefaultCompilerOnlySwitch(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return "/c";
            case Mode.Linux: return "-c";
            default: throw new Exception();
        }
    }

    public static string GetDefaultOutputObjSwitch(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return "/Fo:";
            case Mode.Linux: return "-o";
            default: throw new Exception();
        }
    }

    public static string GetDefaultOutputExeSwitch(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return "/Fe:";
            case Mode.Linux: return "-o";
            default: throw new Exception();
        }
    }

    public static string GetDefaultPacker(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return "lib";
            case Mode.Linux: return "ar";
            default: throw new Exception();
        }
    }
}

class Program
{
    private LuaScript script;

    private Dictionary<Mode, Config> configs;

    private MakeGen.Generator makeGenerator;

    public Program(Mode mode)
    {
        this.script = new LuaScript();
        this.configs = new Dictionary<Mode, Config>();

        makeGenerator = new MakeGen.Generator();

        SetupLua();

        this.script.RunScript("test.lua");

        this.script.CallFunction("Init", mode);

        ProcessConfig(mode);

        Config config = this.configs[mode];

        Project.Project[] projects = Project.ProjectManager.GetProjects();

        foreach(Project.Project project in projects)
        {
            CreateMakeTargetsFromProject(makeGenerator, project, config);
        }

        string code = makeGenerator.GenCode();
        Console.WriteLine(code);
        File.WriteAllText("GeneratedMake.txt", code);
        Console.Read();
    }

    private void ProcessConfig(Mode mode)
    {
        if(!this.configs.ContainsKey(mode))
        {
            Config config = new Config();
            config.Compiler = Helper.GetDefaultCompiler(mode);
            config.CompilerOnlySwitch = Helper.GetDefaultCompilerOnlySwitch(mode);
            config.OutputObjSwitch = Helper.GetDefaultOutputObjSwitch(mode);
            config.OutputExeSwitch = Helper.GetDefaultOutputExeSwitch(mode);
            config.Packer = Helper.GetDefaultPacker(mode);

            config.Mode = mode;

            this.configs.Add(mode, config);
        }

        Config currentConfig = this.configs[mode];
        if(currentConfig.Compiler == null || currentConfig.Compiler == String.Empty)
        {
            currentConfig.Compiler = Helper.GetDefaultCompiler(mode);
        }

        if(currentConfig.CompilerOnlySwitch == null || currentConfig.CompilerOnlySwitch == String.Empty)
        {
            currentConfig.CompilerOnlySwitch = Helper.GetDefaultCompilerOnlySwitch(mode);
        }

        if(currentConfig.OutputObjSwitch == null || currentConfig.OutputObjSwitch == String.Empty)
        {
            currentConfig.OutputObjSwitch = Helper.GetDefaultOutputObjSwitch(mode);
        }

        if(currentConfig.OutputExeSwitch == null || currentConfig.OutputExeSwitch == String.Empty)
        {
            currentConfig.OutputExeSwitch = Helper.GetDefaultOutputExeSwitch(mode);
        }

        if(currentConfig.Packer == null || currentConfig.Packer == String.Empty)
        {
            currentConfig.Packer = Helper.GetDefaultPacker(mode);
        }

    }

    private void CreateMakeTargetsFromProject(MakeGen.Generator generator, Project.Project project, Config config)
    {
        List<string> objFiles = new List<string>();
        foreach(string file in project.Files)
        {
            string targetName = project.Name + "_" + Helper.GetObjFileName(config, file);
            objFiles.Add(targetName);

            MakeGen.Target targetRes = new MakeGen.Target(); 
            targetRes.Name = targetName;
            targetRes.Dependencies = new string[] { file };
            targetRes.Commands = new MakeGen.Command[] {
                new MakeGen.Command(config.Compiler, new string[] { config.CompilerOnlySwitch, file, config.OutputObjSwitch, targetName})
            };

            generator.AddTarget(targetRes);
        }

        MakeGen.Target projTarget = new MakeGen.Target();
        switch(project.Type)
        {
            case Project.Type.Executable: {
                string name = project.Name + Helper.GetExecutableFileExt(config.Mode);

                List<string> arguments = new List<string>();
                arguments.AddRange(objFiles);
                arguments.Add(config.OutputExeSwitch);
                arguments.Add(name);
                
                projTarget.Name = name;
                projTarget.Dependencies = objFiles.ToArray();
                projTarget.Commands = new MakeGen.Command[] {
                    new MakeGen.Command(config.Compiler, arguments.ToArray())
                    //new MakeGen.Command(MakeGen.CommandType.CompileExe, objFiles.ToArray(), name, "")
                };
            } break;

            case Project.Type.StaticLibrary:
                break;
            case Project.Type.DynamicLibrary:
                break;

            default: throw new Exception();
        }

        generator.AddTarget(projTarget);
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