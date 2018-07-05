using System.Collections.Generic;
using System;

namespace Make {

enum Mode
{
    Unknown,
    Win32,
    Linux,
}

enum CommandType
{
    Unknown, // Unkown
    Compile, // Compile a file
    Packs // Make a file a static library
}

class Command
{
    public CommandType Type { get; private set; }

    public Command(CommandType type)
    {
        this.Type = type;
    }
}

class Target
{
    public string Name { get; private set; }
    public string[] Dependencies { get; private set; }
    public Command[] Commands { get; private set; }

    public Target(string name, string[] dependencies, Command[] commands)
    {
        this.Name = name;
        this.Dependencies = dependencies;
        this.Commands = commands;
    }
}

class Variable
{
    public string Name { get; private set; }
    public string Value { get; private set; }

    public Variable(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }
}

class Program
{
    public string Name { get; private set; }
    public Mode Mode { get; private set; }
    public Target[] Targets { get; private set; }
    public Variable[] GlobalVariables { get; private set; }

    public Program(string name, Mode Mode,
                    Target[] targets, Variable[] globalVariables)
    {
        this.Name = name;
        this.Targets = targets;
        this.GlobalVariables = globalVariables;
    }

    public void GenerateCode()
    {

    }

    public void SaveToDisk(string filePath)
    {

    }
}

}