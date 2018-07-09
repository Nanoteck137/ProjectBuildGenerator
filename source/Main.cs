﻿using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using MoonSharp.Interpreter;

using MakeGen = CodeGen.Make;
using BatchGen = CodeGen.Batch;

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

    public static string GetStaticLibraryFileExt(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return ".lib";
            case Mode.Linux: return ".a";

            default: throw new Exception();
        }
    }

    public static string GetDynamicLibraryFileExt(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return ".dll";
            case Mode.Linux: return ".so";

            default: throw new Exception();
        }
    }

    public static string GetDependencyFileExt(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return ".lib";
            case Mode.Linux: return ".so";

            default: throw new Exception();
        }
    }

    public static string GetDependencyFilePrefix(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return "";
            case Mode.Linux: return "lib";

            default: throw new Exception();
        }
    }

    public static string GetDependencyFileName(string dep, Mode mode)
    {
        switch (mode)
        {
            case Mode.Windows: return dep + ".lib";
            case Mode.Linux: return "-l"+ dep;

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
    private BatchGen.Generator batchGenerator;

    public Program(Mode mode)
    {
        this.script = new LuaScript();
        this.configs = new Dictionary<Mode, Config>();

        makeGenerator = new MakeGen.Generator();
        /*batchGenerator = new BatchGen.Generator();

        string dirTestPath = Path.Combine(Directory.GetCurrentDirectory(), "dirTest");
        string buildDirPath = Path.Combine(Directory.GetCurrentDirectory(), "build");

        BatchGen.CommandList commandList = new BatchGen.CommandList();
        commandList.AddCommand(new BatchGen.CustomCommand("mkdir", new string[] { dirTestPath }));

        BatchGen.CommandList buildCommandList = BatchHelper.CreateCommandList(null);
        buildCommandList.AddCommand(BatchHelper.CreateCustomCommand("make", "-f", Directory.GetCurrentDirectory()));

        batchGenerator.AddCommand(new BatchGen.PushDirectoryCommand(buildDirPath, buildCommandList));

        batchGenerator.AddCommand(new BatchGen.ExistCommand(
            BatchGen.ExistCondition.IfNot, dirTestPath, commandList
        ));*/

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

        string makeCode = makeGenerator.GenCode();
        File.WriteAllText("Makefile.gen.win32", makeCode);

        string buildDir = Path.Combine(Directory.GetCurrentDirectory(), "build");
        string makeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Makefile.win32.gen");

        ProjectBatch batch = new ProjectBatch(projects);
        string batchCode = batch.CreateBatchCode(buildDir, makeFilePath);
        File.WriteAllText("build.gen.bat", batchCode);

        Console.WriteLine(makeCode);
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(batchCode);

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

        switch(project.Type)
        {
            case Project.Type.Executable: {
                generator.AddTarget(CreateTargetForExecutable(objFiles, generator, project, config));
            } break;

            case Project.Type.StaticLibrary:
                generator.AddTarget(CreateTargetForStaticLibrary(objFiles, generator, project, config));
                break;
            case Project.Type.DynamicLibrary:
                break;

            default: throw new Exception();
        }
    }

    private MakeGen.Target CreateTargetForStaticLibrary(List<string> objFiles, MakeGen.Generator generator, Project.Project project, Config config)
    {
        MakeGen.Target target = new MakeGen.Target();

        string name = project.Name + Helper.GetStaticLibraryFileExt(config.Mode);

        List<string> arguments = new List<string>();
        arguments.Add("/out:" + name);
        arguments.AddRange(objFiles);

        target.Name = name;
        target.Dependencies = objFiles.ToArray();
        target.Commands = new MakeGen.Command[] {
            new MakeGen.Command("lib", arguments.ToArray())
        };

        return target;
    }

    private MakeGen.Target CreateTargetForExecutable(List<string> objFiles, MakeGen.Generator generator, Project.Project project, Config config)
    {
        MakeGen.Target target = new MakeGen.Target();

        string name = project.Name + Helper.GetExecutableFileExt(config.Mode);

        List<string> arguments = new List<string>();

        if (project.ProjectDependencies != null)
        {
            foreach (string dep in project.ProjectDependencies)
            {
                arguments.Add(Helper.GetDependencyFileName(dep, config.Mode));
            }
        }

        arguments.AddRange(objFiles);
        arguments.Add(config.OutputExeSwitch);
        arguments.Add(name);

        if(project.ProjectDependencies != null)
        {
            foreach(string dep in project.ProjectDependencies)
            {
                objFiles.Add(Helper.GetDependencyFilePrefix(config.Mode) + dep + Helper.GetDependencyFileExt(config.Mode));
            }
        }

        target.Name = name;
        target.Dependencies = objFiles.ToArray();
        target.Commands = new MakeGen.Command[] {
            new MakeGen.Command(config.Compiler, arguments.ToArray())
        };

        return target;
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