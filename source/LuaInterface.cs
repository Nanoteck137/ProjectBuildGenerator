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

        public void AddProject(Table projectData)
        {
            Project result = new Project();

            DynValue name = projectData.Get("Name");
            DynValue type = projectData.Get("Type");

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