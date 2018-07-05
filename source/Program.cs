using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using Newtonsoft.Json;

using MoonSharp.Interpreter;

class ConfigSection
{
    public string Name { get; set; }
    public string SourceDir { get; set; }
    public string Output { get; set; }
    public string Type { get; set; }
}

class Config
{
    public ConfigSection Section { get; set; }
}

enum Mode
{
    Unknown,
    Windows,
    Linux,
}

enum MakeTargetType
{
    Unknown,
    Empty,
    Executable,
    Object,
}

class MakeTarget
{
    public string Name { get; private set; }
    public string[] Dependencies { get; private set; }
    public MakeTargetType Type { get; private set; }

    public MakeTarget(string name, string[] dependencies, MakeTargetType type)
    {
        this.Name = name;
        this.Dependencies = dependencies;
        this.Type = type;
    }

    public void GenerateCode(StringBuilder result)
    {
        result.AppendFormat("{0}:", this.Name);
        foreach(string dependency in this.Dependencies)
        {
            result.AppendFormat(" {0}", dependency);
        }
        result.Append("\n\t");

        switch(this.Type)
        {
            case MakeTargetType.Executable:
                result.Append("$(compiler)");
                foreach (string dependency in this.Dependencies)
                {
                    result.AppendFormat(" {0}", dependency);
                }

                result.AppendFormat(" /Fe{0}", this.Name);
                break;
            case MakeTargetType.Object:
                result.Append("$(compiler) /c");
                foreach (string dependency in this.Dependencies)
                {
                    result.AppendFormat(" {0}", dependency);
                }
                break;

            case MakeTargetType.Empty:
                break;

            default:
                throw new Exception();
        }
        result.Append("\n\n");
    }
}

class MakeVariable
{
    public string Name { get; private set; }
    public string Value { get; private set; }

    public MakeVariable(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }

    public void GenerateCode(StringBuilder result)
    {
        result.AppendFormat("{0}={1}\n", this.Name, this.Value);
    }
}

class MakeProgram
{
    public MakeVariable[] Variables { get; private set; }
    public MakeTarget[] Targets { get; private set; }

    public MakeProgram(MakeVariable[] variables, MakeTarget[] targets)
    {
        this.Variables = variables;
        this.Targets = targets;
    }

    public void GenerateCode(StringBuilder result)
    {
        foreach(MakeVariable var in this.Variables)
        {
            var.GenerateCode(result);
        }

        result.Append("\n");

        result.Append("all:\n");
        result.Append("\t@ehco Select a build target\n");

        result.Append("\n");

        foreach(MakeTarget target in this.Targets)
        {
            target.GenerateCode(result);
        }
    }
}


class Program
{
    public Program(Mode mode)
    {
        Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Test3.json"));

        MakeTarget program = new MakeTarget("program.exe", new string[] { "main.obj", "test1.obj" }, MakeTargetType.Executable);

        List<MakeVariable> variables = new List<MakeVariable> {
            new MakeVariable("compiler", "cl"),
        };

        string dirPath = Path.Combine(Directory.GetCurrentDirectory(), config.Section.SourceDir);
        MakeTarget[] targets = CreateMakeTargets(dirPath, mode);

        MakeProgram prog = new MakeProgram(variables.ToArray(), targets);

        StringBuilder builder = new StringBuilder();
        prog.GenerateCode(builder);
        Console.WriteLine(builder.ToString());

        File.WriteAllText("Result.txt", builder.ToString());

        Console.ReadLine();
    }

    public MakeTarget[] CreateMakeTargets(string sourceDir, Mode mode)
    {
        List<MakeTarget> result = new List<MakeTarget>();
        string[] sourceFiles = GetAllFilesWithExt(sourceDir, "*.cpp");

        List<string> exeObjects = new List<string>();

        foreach(string file in sourceFiles)
        {
            string targetName = Path.GetFileNameWithoutExtension(file);
            targetName = targetName + GetObjectExtention(mode);

            exeObjects.Add(targetName);

            string sourceFileName = Path.GetFileName(file);

            result.Add(new MakeTarget(targetName, new string[] { "$(srcDir)\\" + sourceFileName }, MakeTargetType.Object));
        }

        result.Add(new MakeTarget("program.exe", exeObjects.ToArray(), MakeTargetType.Executable));

        return result.ToArray();
    }

    public string GetObjectExtention(Mode mode)
    {
        switch(mode)
        {
            case Mode.Windows: return ".obj";
            case Mode.Linux: return ".o";

            default: throw new Exception();
        }
    }

    public static string[] GetAllFilesWithExt(string dirPaths, string ext)
    {
        return Directory.GetFiles(dirPaths, ext, SearchOption.AllDirectories);
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

        LuaScript script = new LuaScript("test.lua");

        DynValue res = script.CallFunction("test", 4, 8);
        Console.WriteLine("Value: {0}", res.Number);

        Console.Read();

        //new Program(mode);
    }
}
