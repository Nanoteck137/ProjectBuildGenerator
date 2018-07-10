using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System;

using MoonSharp.Interpreter;

public class MainClass
{
    private static void PrintUsage()
    {
        Console.WriteLine("Usage: {0} [options] LuaFilePath", AppDomain.CurrentDomain.FriendlyName);
        Console.WriteLine("Options:");
        Console.WriteLine("  --windows              - (Default on Windows) Switches the windows code generator on");
        Console.WriteLine("  --linux                - (Default on Linux) Switches the linux code generator on");
        Console.WriteLine("  --workspaceDirPath     - Sets where lua should start looking for projects,");
        Console.WriteLine("                            if not set the workspace path is");
        Console.WriteLine("                            set to the same directory where the lua files lives");
    }

    public static void Main(string[] commandLine)
    {
        string luaFilePath = "";
        string workspaceDirPath = "";

        bool IsWindows = false;
        bool IsLinux = false;

        if(commandLine.Length <= 0)
        {
            Console.WriteLine("You need to specify the lua file path");
            PrintUsage();
            Helper.Exit();
        }

        if(commandLine.Length == 1 && commandLine[0] == "--help")
        {
            PrintUsage();
            Helper.Exit();
        }

        for(int index = 0; 
            index < commandLine.Length; 
            index++)
        {
            if (index == commandLine.Length - 1)
            {
                //Last argument
                string filePath = Path.GetFullPath(commandLine[index]);

                if (File.Exists(filePath))
                    luaFilePath = filePath;
                else
                    Helper.ErrorExit("The Last argument needs to be a path to the lua file");

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

                    case "workspacedirpath":
                        if (splittedArgs.Length == 2)
                            workspaceDirPath = Path.GetFullPath(splittedArgs[1]);
                        else
                            throw new Exception();
                        break;

                    default:
                        PrintUsage();
                        Helper.Exit();
                        break;
                }
            }
            else
            {
                Helper.ErrorExit(string.Format("Unknown argument: {0}", arg));
            }
        }

        if(workspaceDirPath == "")
            workspaceDirPath = Path.GetDirectoryName(luaFilePath);

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            IsWindows = true;
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            IsLinux = true;
        else
            Helper.ErrorExit("Windows and Linux are the only platform supported");

        UserData.RegisterAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        if(IsLinux)
            Helper.ErrorExit("Linux not implemented yet");
        else if(IsWindows)
            new WindowsProgram(luaFilePath, workspaceDirPath);
    }
}