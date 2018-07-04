using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string[] files = Directory.GetFiles("obj/");
        foreach(string file in files)
        {
            string newPath = Path.GetFullPath(file);
            Console.WriteLine("File: " + newPath);
        }
        Console.WriteLine("Hello World!");
        Console.WriteLine("This is a test for fun!");

        Console.ReadLine();
    }
}
