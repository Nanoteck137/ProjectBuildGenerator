using System.Collections.Generic;
using System.Text;
using System;

using CodeGen.Batch;


namespace Batch
{
    public interface ICommand
    {
        void GenCode(StringBuilder builder);
    }

    public class CommandList
    {
        public List<ICommand> commands;

        public CommandList() 
        {
            this.commands = new List<ICommand>();
        }

        public CommandList(ICommand[] commands) 
        { 
            this.commands = new List<ICommand>();
            if(commands != null)
                this.commands.AddRange(commands);
        }

        public void AddCommand(ICommand command)
        {
            this.commands.Add(command);
        }

        public void AddCommandList(CommandList list)
        {
            this.commands.AddRange(list.commands);
        }

        public void GenCode(StringBuilder builder, string prefix = "", string suffix = "")
        {
            foreach(ICommand command in this.commands)
            {
                builder.Append(prefix);
                command.GenCode(builder);
                builder.Append(suffix);
            }
        }
    }

    public class PushDirectoryCommand : ICommand
    {
        private string dirPath;
        private CommandList commands;

        public PushDirectoryCommand(string dirPath, CommandList commands)
        {
            this.dirPath = dirPath;
            this.commands = commands;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.AppendFormat("pushd \"{0}\"\n", this.dirPath);
            commands.GenCode(builder, "\t");
            builder.Append("popd\n");
        }
    }

    public enum ExistCondition
    {
        If,
        IfNot,
    }

    public class ExistCommand : ICommand
    {
        private ExistCondition condition;
        private string path;
        private CommandList commands;

        public ExistCommand(ExistCondition condition, string path, CommandList commands)
        {
            this.condition = condition;
            this.path = path;
            this.commands = commands;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.Append("if ");
            if(condition == ExistCondition.IfNot)
                builder.Append("not ");
            builder.AppendFormat("exist \"{0}\" ", this.path);
            builder.Append("(\n");

            this.commands.GenCode(builder, "\t");

            builder.Append(")\n");
        }
    }

    public class CustomCommand : ICommand
    {
        private string command;
        private string[] arguments;

        public CustomCommand(string command, string[] args)
        {
            this.command = command;
            this.arguments = args;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.Append(command);
            foreach(string arg in this.arguments)
            {
                builder.Append(" " + arg);
            }

            builder.Append("\n");
        }
    }

    public class Generator
    {
        public CommandList commands;

        public Generator() 
        {
            commands = new CommandList();
        }

        public void AddCommand(ICommand command)
        {
            this.commands.AddCommand(command);
        }

        public void AddCommandList(CommandList list)
        {
            this.commands.AddCommandList(list);
        }

        public void CreateCustomCommand(string command, params string[] arguments)
        {
            AddCommand(new CustomCommand(command, arguments));
        }

        public void CreateExistCommand(string path, ExistCondition condition, params ICommand[] commands)
        {
            AddCommand(new ExistCommand(condition, path, new CommandList(commands)));
        }

        public string GenCode()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("@echo off\n\n");

            this.commands.GenCode(builder, "", "\n");
            return builder.ToString();
        }        
    }
}