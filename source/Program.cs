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

class Program
{
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

        LuaScript script = new LuaScript();
        script.AddEnumType("Mode", typeof(Mode));
        script.AddEnumType("ProjectType", typeof(ProjectType));
        
        script.RunScript("test.lua");
        Console.Read();

        //new Program(mode);
    }
}
