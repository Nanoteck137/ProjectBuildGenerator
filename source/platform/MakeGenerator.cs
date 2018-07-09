using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace Make 
{
    class Command
    {
        public string Instruction { get; set; }
        public string[] Arguments { get; set; }

        public Command() { }

        public Command(string instruction, string[] arguments)
        {
            this.Instruction = instruction;
            this.Arguments = arguments;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.Append(this.Instruction);
            foreach(string arg in this.Arguments)
            {
                builder.Append(" " + arg);
            }
            builder.Append("\n");
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

            Target all = new Target("all", null, new Command[] { new Command("@echo", new string[] { "Select a Target" }) });
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
    }
}