using System.Diagnostics;
using System.IO;
using System;

public class Helper
{
    private Helper() { }

    public static string[] GetAllFilesWithExt(string dirPath, string ext)
    {
        return Directory.GetFiles(dirPath, ext, SearchOption.AllDirectories);
    }

    public static void ErrorExit(string prefix, string message)
    {
        string errorMessage = string.Format("{0} - {1} error: {2}", AppDomain.CurrentDomain.FriendlyName, prefix, message);
        Console.WriteLine(errorMessage);
        Debug.Assert(false, errorMessage);
        Environment.Exit(-1);
    }

    public static void ErrorExit(string message)
    {
        string errorMessage = string.Format("{0} - error: {1}", AppDomain.CurrentDomain.FriendlyName, message);
        Console.WriteLine(errorMessage);
        Debug.Assert(false, errorMessage);
        Environment.Exit(-1);
    }

    public static void Exit()
    {
        Debug.Assert(false);
        Environment.Exit(-1);
    }

}