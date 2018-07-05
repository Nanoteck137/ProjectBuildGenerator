using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

using Newtonsoft.Json;

class Program
{
    public Program()
    {
        Console.ReadLine();
    }

    public static string[] GetAllFilesWithExt(string dirPaths, string ext)
    {
        return Directory.GetFiles(dirPaths, ext, SearchOption.AllDirectories);
    }

    public static void Main(string[] args)
    {
        new Program();
    }
}
