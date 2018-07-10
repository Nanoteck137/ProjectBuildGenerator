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
     - Give all the object compile result a diffrent name like: ProjectName_InputName.obj
 */

enum Mode
{
    Unknown,
    Windows,
    Linux,
}

/*class Program
{
    public Program(Mode mode)
    {
        foreach(Project.Project project in projects)
        {
            CreateMakeTargetsFromProject(makeGenerator, project, config);
        }

        string makeCode = makeGenerator.GenCode();
        File.WriteAllText("Makefile.gen.win32", makeCode);

        string buildDir = Path.Combine(Directory.GetCurrentDirectory(), "build");
        string makeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Makefile.gen.win32");

        ProjectBatch batch = new ProjectBatch(projects);
        string batchCode = batch.CreateBatchCode(buildDir, makeFilePath);
        File.WriteAllText("build.gen.bat", batchCode);

        Console.WriteLine(makeCode);
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(batchCode);
    }

/*    private void CreateMakeTargetsFromProject(MakeGen.Generator generator, Project.Project project, Config config)
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

    public static void Main(string[] args)
    {
        //TODO: Redesign how the command arguments are passed
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


        new Program(mode);
    }
}*/

public class MainClass
{
    /*
        Structure of the Command line arguments
        ProjectBuildGenerator.exe
        [options]: --windows, --linux and other options
                 : --buildDirPath=path - This is the path to where the build files is gonna live, this can be a absolute path or relative.
                 : --workspaceDirPath=path - This is the path where lua's Working directory is, 
                    this can be a absolute path or relative 
                    this is gonna get resolved later with Path.GetFullPath().
        luaFilePath: C:\CSharp\ProjectBuildGenerator\Test2\test.lua
     */

    public static void PrintUsage()
    {
        Console.WriteLine("Usage: {0} [options] LuaFilePath", AppDomain.CurrentDomain.FriendlyName);
        Console.WriteLine("Options:");
        Console.WriteLine("  --windows              - Switches the windows code generator on");
        Console.WriteLine("  --linux                - Switches the linux code generator on");
        Console.WriteLine("  --buildDirPath:        - Sets where the build directory should be");
        Console.WriteLine("  --workspaceDirPath     - Sets where lua should start looking for projects");
    }

    public static void Main(string[] commandLine)
    {
        string luaFilePath = "";
        string buildDirPath = "";
        string workspaceDirPath = "";

        bool IsWindows = false;
        bool IsLinux = false;

        if(commandLine.Length <= 0)
            throw new Exception();

        for(int index = 0; 
            index < commandLine.Length; 
            index++)
        {
            if(index == commandLine.Length - 1)
            {
                //Last argument
                string filePath = Path.GetFullPath(commandLine[index]);

                if(File.Exists(filePath))
                    luaFilePath = filePath;
                else
                    throw new Exception();

                break;
            }

            string arg = commandLine[index];
            if(arg[0] == '-' && arg[1] == '-')
            {
                arg = arg.Remove(0, 2).ToLower();
                string[] splittedArgs = arg.Split('=');
                arg = splittedArgs[0];
                switch(arg)
                {
                    case "windows":
                        if(IsLinux)
                            throw new Exception();
                        IsWindows = true;
                        break;

                    case "linux":
                        if(IsWindows)
                            throw new Exception();
                        IsLinux = true;
                        break;

                    case "builddirpath":
                        if(splittedArgs.Length == 2)
                            buildDirPath = Path.GetFullPath(splittedArgs[1]);
                        else
                            throw new Exception();
                        break;

                    case "workspacedirpath":
                        if (splittedArgs.Length == 2)
                            workspaceDirPath = Path.GetFullPath(splittedArgs[1]);
                        else
                            throw new Exception();
                        break;

                    default:
                        PrintUsage();
                        Environment.Exit(-1);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Unknown argument: {0}", arg);
                PrintUsage();
                Environment.Exit(-1);
            }
        }

        if(luaFilePath == "")
            throw new Exception();
        if(buildDirPath == "")
            throw new Exception();
        if(workspaceDirPath == "")
            throw new Exception();

        if(!IsWindows && !IsLinux)
            throw new Exception();

        UserData.RegisterAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        new WindowsProgram(luaFilePath, workspaceDirPath);
        Console.ReadLine();
    }
}