using System.Collections.Generic;
using System;

using MoonSharp.Interpreter;

namespace LuaInterface
{
    class LuaHelper
    {
        private LuaHelper() { }

        public static string GetStringFromValue(DynValue value)
        {
            if(value.IsNil())
                throw new Exception("Value cant be nil"); //TODO: Better error handling
            if(value.Type != DataType.String)
                throw new Exception("Value is not a string"); //TODO: Better error handling
            
            return value.String;
        }        
    }

    [MoonSharpUserData]
    class ConfigInterface
    {
        private Program program;

        public ConfigInterface(Program program)
        {
            this.program = program;
        }

        public void AddConfig(Mode mode, Table configData)
        {
            Config result = new Config();

            DynValue compiler = configData.Get("Compiler");
            DynValue compilerOnlySwitch = configData.Get("CompilerOnlySwitch");
            DynValue outputObjSwitch = configData.Get("OutputObjSwitch");
            DynValue outputExeSwitch = configData.Get("OutputExeSwitch");

            DynValue packer = configData.Get("Packer");

            result.Compiler = compiler.String;
            result.CompilerOnlySwitch = compilerOnlySwitch.String;
            result.OutputObjSwitch = outputObjSwitch.String;
            result.OutputExeSwitch = outputExeSwitch.String;
            result.Packer = packer.String;

            result.Mode = mode;

            program.AddConfig(mode, result);
        }
    }

    [MoonSharpUserData]
    class ProjectInterface
    {
        private Program program;

        public ProjectInterface(Program program)
        {
            this.program = program;
        }

        private string[] GetStringArray(string name, DynValue filesTable)
        {
            if (filesTable.IsNil())
                return null;
            if (filesTable.Type != DataType.Table)
                throw new Exception(name + " is not a table");

            Table table = filesTable.Table;
            if(table.Length == 0)
                return null;

            List<string> result = new List<string>();

            for (int i = 0; i < table.Length; i++)
            {
                string value = table.Get(i + 1).String;
                if (value != null)
                    result.Add(value);
            }

            return result.ToArray();
        }

        public void AddProject(Table projectData)
        {
            Project.Project result = new Project.Project();

            DynValue name = projectData.Get("Name");
            DynValue type = projectData.Get("Type");
            DynValue files = projectData.Get("Files");
            DynValue projectDependencies = projectData.Get("ProjectDependencies");

            result.Name = LuaHelper.GetStringFromValue(name);

            if (!type.IsNil())
            {
                if (type.Type != DataType.Number)
                    throw new Exception("Type needs to be a number/enum value");
                result.Type = (Project.Type)type.Number;
            }
            else
            {
                result.Type = Project.Type.Executable;
            }

            result.Files = GetStringArray("Files", files);
            if(result.Files == null)
                throw new Exception("'Files' table is nil");

            result.ProjectDependencies = GetStringArray("ProjectDependencies", projectDependencies);

            Project.ProjectManager.AddProject(result);
        }

    }
}