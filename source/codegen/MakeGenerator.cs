using System.Collections.Generic;
using System.Text;
using System;

namespace CodeGen.Make
{
    enum CommandType
    {
        None,
        CompileExe,
        CompileObj,
        PackStaticLib,
        Custom,
    }

    class Command
    {
        public CommandType Type { get; set; }
        public string[] Files { get; set; }
        public string OutputName { get; set; }
        public string Options { get; set; }

        public string CustomCommand { get; set; }

        public Command() { }

        public Command(CommandType type, string[] files, string outputName, string options)
        {
            this.Type = type;
            this.Files = files;
            this.OutputName = outputName;
            this.Options = options;
        }

        public Command(string customCommand)
        {
            this.Type = CommandType.Custom;
            this.CustomCommand = customCommand;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.Append("This is a command\n");
        }
    }

    class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Variable() { }

        public Variable(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.AppendFormat("{0}={1}\n", this.Name, this.Value);
        }
    }

    class Target
    {
        public string Name { get; set; }
        public string[] Dependencies { get; set; }
        public Command[] Commands { get; set; }

        public Target() { }

        public Target(string name, string[] dependencies, Command[] commands) 
        { 
            this.Name = name;
            this.Dependencies = dependencies;
            this.Commands = commands;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.Append(this.Name + ":");
            if(this.Dependencies != null)
            {
                foreach(string dep in this.Dependencies)
                {
                    builder.Append(" " + dep);
                }
            }
            builder.Append("\n");

            foreach(Command command in this.Commands)
            {
                builder.Append("\t");
                command.GenCode(builder);
            }
        }
    }

    class Generator
    {
        private List<Variable> variables;
        private List<Target> targets;

        public Generator()
        {
            variables = new List<Variable>();
            targets = new List<Target>();

            Target all = new Target("all", null, new Command[] { new Command("@echo Select a Target") });
            targets.Add(all);
        }

        public void AddVariable(Variable variable)
        {
            this.variables.Add(variable);
        }

        public void AddTarget(Target target)
        {
            targets.Add(target);
        }

        public string GenCode()
        {
            StringBuilder result = new StringBuilder();

            foreach(Variable variable in this.variables)
            {
                variable.GenCode(result);
            }
            result.Append("\n");

            foreach(Target target in this.targets)
            {
                target.GenCode(result);
                result.Append("\n");
            }

            return result.ToString();
        }

        public void GenCodeSaveToFile(string filePath)
        {
            //TODO: Generate code and save it to a file
        }
    }
}