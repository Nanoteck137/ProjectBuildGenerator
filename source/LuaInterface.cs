using System.Collections.Generic;
using System;

using MoonSharp.Interpreter;

namespace LuaInterface
{
    [MoonSharpUserData]
    class ProjectInterface
    {
        private Program program;

        public ProjectInterface(Program program)
        {
            this.program = program;
        }

        private string[] GetFiles(DynValue filesTable)
        {
            if(filesTable.IsNil())
                throw new Exception();
            if(filesTable.Type != DataType.Table)
                throw new Exception();

            Table table = filesTable.Table;
            List<string> result = new List<string>();

            for(int i = 0; i < table.Length; i++)
            {
                string value = table.Get(i + 1).String;
                if(value != null)
                    result.Add(value);
            }

            return result.ToArray();
        }

        public void AddProject(Table projectData)
        {
            Project result = new Project();

            DynValue name = projectData.Get("Name");
            DynValue type = projectData.Get("Type");
            DynValue files = projectData.Get("Files");

            result.Files = GetFiles(files);

            if(name.IsNil())
                throw new Exception("Theres need to be a Name in project table");
            if(name.Type != DataType.String)
                throw new Exception("Name needs to be a string");

            result.Name = name.String;

            if(!type.IsNil())
            {
                if(type.Type != DataType.Number)
                    throw new Exception("Type needs to be a number/enum value");
                result.Type = (ProjectType)type.Number;
            }
            else
            {
                result.Type = ProjectType.Executable;
            }

            this.program.AddProject(result);
        }

    }
}