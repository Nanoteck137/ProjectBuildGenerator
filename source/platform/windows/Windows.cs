using System.Collections.Generic;
using System.IO;
using System;

using Lua;
using Lua.Interface;

class WindowsProgram : Program
{
    private Make.Generator makeGenerator;
    
    public WindowsProgram(string luaConfigPath)
        : base(luaConfigPath)
    {
        makeGenerator = new Make.Generator();

        string[] testFiles = LuaSystemLibrary.GetAllFilesWithExt(Path.Combine(Directory.GetCurrentDirectory(), "test"), "*.cpp");
        string[] sourceFiles = LuaSystemLibrary.GetAllFilesWithExt(Path.Combine(Directory.GetCurrentDirectory(), "source"), "*.cpp");

        Project testProject = new Project("test", ProjectType.StaticLibrary, testFiles, null);
        AddProject(testProject);

        Project programProject = new Project("program", ProjectType.Executable, sourceFiles, new string[] { testProject.Name });
        AddProject(programProject);

        CreateMakeTarget();

        Console.WriteLine(makeGenerator.GenCode());
    }

    public void CreateMakeTarget()
    {
        foreach(Project project in this.projects)
        {
            WindowsMake.CreateMakeTargetsFromProject(this.makeGenerator, project);
        }
    }
}

class WindowsMake
{
    private WindowsMake() { }

    public static void CreateMakeTargetsFromProject(Make.Generator generator, Project project)
    {
        List<string> objFiles = new List<string>();
        foreach (string file in project.Files)
        {
            string targetName = project.Name + "_" + Path.GetFileNameWithoutExtension(file) + ".obj";
            objFiles.Add(targetName);

            Make.Target targetRes = new Make.Target();
            targetRes.Name = targetName;
            targetRes.Dependencies = new string[] { file };
            targetRes.Commands = new Make.Command[] {
                new Make.Command("cl", new string[] { "/c", file, "/Fo:", targetName})
            };

            generator.AddTarget(targetRes);
        }

        switch (project.Type)
        {
            case ProjectType.Executable:
                generator.AddTarget(CreateTargetForExecutable(objFiles, generator, project));
                break;

            case ProjectType.StaticLibrary:
                generator.AddTarget(CreateTargetForStaticLibrary(objFiles, generator, project));
                break;
            case ProjectType.DynamicLibrary:
                break;

            default: throw new Exception();
        }
    }

    public static Make.Target CreateTargetForExecutable(List<string> objFiles, Make.Generator generator, Project project)
    {
        Make.Target target = new Make.Target();

        string name = project.Name + ".exe";

        List<string> arguments = new List<string>();
        arguments.AddRange(objFiles);

        if (project.ProjectDependencies != null)
        {
            foreach (string dep in project.ProjectDependencies)
            {
                arguments.Add(dep + ".lib");
                objFiles.Add(dep + ".lib");
            }
        }

        arguments.Add("/Fe:");
        arguments.Add(name);

        target.Name = name;
        target.Dependencies = objFiles.ToArray();
        target.Commands = new Make.Command[] { new Make.Command("cl", arguments.ToArray()) };

        return target;
    }

    public static Make.Target CreateTargetForStaticLibrary(List<string> objFiles, Make.Generator generator, Project project)
    {
        Make.Target target = new Make.Target();

        string name = project.Name + ".lib";

        List<string> arguments = new List<string>();
        arguments.Add("/out:" + name);
        arguments.AddRange(objFiles);

        target.Name = name;
        target.Dependencies = objFiles.ToArray();
        target.Commands = new Make.Command[] {
            new Make.Command("lib", arguments.ToArray())
        };

        return target;
    }
}

class WindowsBatch
{
    private WindowsBatch() { }
}