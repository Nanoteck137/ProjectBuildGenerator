using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class Command
{
    public string command;
    public List<string> paramaters;

    public Command(string command, List<string> paramaters)
    {
        this.command = command;
        this.paramaters = paramaters;
    }

    public void GenerateCode(StringBuilder str)
    {
        str.Append(this.command);
        foreach(string param in paramaters)
        {
            str.Append(" " + param);
        }
    }
}

public class Section
{
    public string name;
    public List<Command> commands;

    public Section(string name, List<Command> commands)
    {
        this.name = name;
        this.commands = commands;
    }

    public void GenerateCode(StringBuilder str)
    {
        str.Append(this.name + ":\n");
        foreach(Command command in this.commands)
        {
            str.Append("\t");
            command.GenerateCode(str);
        }

        str.Append("\n");
    }
}

public class Variable
{
    public string name;
    public string value;

    public Variable(string name, string value)
    {
        this.name = name;
        this.value = value;
    }

    public void GenerateCode(StringBuilder str)
    {
        str.Append(string.Format("{0}={1}\n", this.name, this.value));
    }
}

public class BuildInfo
{
    public string name;
    public List<Variable> globalVariables;
    public List<Section> sections;

    public BuildInfo(string name, List<Variable> globalVariables, List<Section> sections)
    {
        this.name = name;
        this.globalVariables = globalVariables;
        this.sections = sections;
    }

    public string GenerateCode()
    {
        StringBuilder result = new StringBuilder();

        foreach(Variable var in this.globalVariables)
        {
            var.GenerateCode(result);
        }

        result.Append("\n");

        foreach(Section section in this.sections)
        {
            section.GenerateCode(result);
        }

        return result.ToString();
    }
}

class Program
{
    static void Main(string[] args)
    {
        /*std::vector<Command> commands = {
            { "cl", { "-Zi", "%dir%\\main.cpp" } },
        };*/

        List<Command> commands = new List<Command>() {
            new Command("cl", new List<string> { "-Zi", "%dir%\\main.cpp" } ),
        };

        Section section = new Section("program", commands);

        List<Variable> globalVariables = new List<Variable> {
            new Variable("CC", "cl"),
            new Variable("DIR", "{CURDIR}")
        };

        BuildInfo buildInfo = new BuildInfo("make.win32", globalVariables, new List<Section> { section });

        Console.WriteLine(buildInfo.GenerateCode());
        Console.ReadLine();
    }
}
