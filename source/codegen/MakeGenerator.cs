using System.Collections.Generic;
using System;

namespace CodeGen.Make
{
    enum CommandType
    {
        None,
        CompileExe,
        CompileObj,
        PackStaticLib,
    }

    class Command
    {
        public CommandType Type { get; set; }
        public string[] Files { get; set; }
        public string OutputName { get; set; }
        public string Options { get; set; }
    }

    class Target
    {
        public string Name { get; set; }
        public string[] Dependencies { get; set; }
        public Command[] Commands { get; set; }
    }

    class Generator
    {
        private List<Target> targets;

        public Generator()
        {
            targets = new List<Target>();
        }

        public void AddTarget(Target target)
        {
            targets.Add(target);
        }

        public string GenCode()
        {
            //TODO: Generate code
            return null;
        }

        public void GenCodeSaveToFile(string filePath)
        {
            //TODO: Generate code and save it to a file
        }
    }
}