using System.Collections.Generic;
using System.Text;
using System;

namespace CodeGen.Batch 
{
    interface ICommand
    {
        void GenCode(StringBuilder builder);
    }

    class PushDirectoryCommand : ICommand
    {
        private ICommand[] commands;

        public PushDirectoryCommand(ICommand[] commands)
        {
            this.commands = commands;
        }

        public void GenCode(StringBuilder builder)
        {
            //pushd
            // Gen code for commands
            //popd
        }
    }

    enum ExistCondition
    {
        If,
        IfNot,
    }

    class ExistCommand : ICommand
    {
        private ExistCondition condition;
        private string path;
        private ICommand[] commands;

        public ExistCommand(ExistCondition condition, string path, ICommand[] commands)
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

            foreach(ICommand command in this.commands)
            {
                builder.Append("\t");
                command.GenCode(builder);
            }

            builder.Append(")\n");
        }
    }

    class MakeDirectoryCommand : ICommand
    {
        private string path;

        public MakeDirectoryCommand(string path)
        {
            this.path = path;
        }

        public void GenCode(StringBuilder builder)
        {
            builder.AppendFormat("mkdir {0}\n", path);
        }
    }

    class Generator
    {
        public List<ICommand> commands;

        public Generator() 
        {
            commands = new List<ICommand>();
        }

        public void AddCommand(ICommand command)
        {
            commands.Add(command);
        }

        public string GenCode()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("@echo off\n\n");

            foreach(ICommand command in commands)
            {
                command.GenCode(builder);
            }

            return builder.ToString();
        }        
    }
}