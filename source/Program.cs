using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using Newtonsoft.Json;

class Program
{
    public Program(Make.Mode mode)
    {
        Config.Data data = GetConfig("Test2.json");

        //MakeProgram program = CreateMakeProgramFromConfig(data);

        Console.ReadLine();
    }

/*    public string GetMakeNameFromMode(MakeMode mode)
    {
        switch(mode)
        {
            case MakeMode.Win32: return "Makefile.win32";
            case MakeMode.Linux: return "Makefile.linux";
            default:
                throw new NotSupportedException("Unkown mode");
        }
    }

    public void GetTargets(List<MakeTarget> targets, ConfigTarget target)
    {
        string dirPath = Path.Combine(Directory.GetCurrentDirectory(), target.name);
        if(!Directory.Exists(dirPath))
        {
            throw new DirectoryNotFoundException("Could not find directory: " + dirPath);
        }

        string[] files = GetAllFilesWithExt(dirPath, "*.cpp");
        foreach(string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            
            string[] dependencies = new string[] { Path.GetFileName(file) };
            MakeCommand[] commands = new MakeCommand[] {   };

            MakeTarget result = new MakeTarget();
        }
    }

    public List<MakeTarget> CreateMakeTargetsFromConfig(List<ConfigTarget> targets)
    {
        List<MakeTarget> result = new List<MakeTarget>();

        return result;
    }

    public MakeProgram CreateMakeProgramFromConfig(MakeMode mode, ConfigData config)
    {
        string name = GetMakeNameFromMode(mode);

        MakeProgram result = new MakeProgram(name, mode, );

        return result;
    }*/

    public string[] GetAllFilesWithExt(string dirPaths, string ext)
    {
        return Directory.GetFiles(dirPaths, ext, SearchOption.AllDirectories);
    }

    public Config.Data GetConfig(string configFilePath)
    {
        string fileContent = File.ReadAllText(configFilePath);
        Config.Data result = JsonConvert.DeserializeObject<Config.Data>(fileContent);

        return result;
    }

    public static void Main(string[] args)
    {
        Make.Mode mode = Make.Mode.Unknown;

        if(args.Length == 1)
        {
            if(args[0] == "-win32")
                mode = Make.Mode.Win32;
            else if(args[0] == "-linux")
                mode = Make.Mode.Linux;
            else
                throw new ArgumentException("Unknown mode");
        }
        else
        {
            throw new ArgumentException("Need to specify arguments");
        }


        new Program(mode);
    }
}
