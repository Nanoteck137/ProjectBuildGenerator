using System.IO;
using System;

public class Helper
{
    private Helper() { }

    public static string[] GetAllFilesWithExt(string dirPath, string ext)
    {
        return Directory.GetFiles(dirPath, ext, SearchOption.AllDirectories);
    }
}