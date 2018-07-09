using System.Collections.Generic;
using System;

using MoonSharp.Interpreter;

//TODO: Revisit this when i want the lua integration to work

namespace Lua.Interface
{
/*    class LuaHelper
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
            DynValue nameLuaValue = projectData.Get("Name");
            DynValue typeLuaValue = projectData.Get("Type");
            DynValue filesLuaValue = projectData.Get("Files");
            DynValue projectDependenciesLuaValue = projectData.Get("ProjectDependencies");

            result.Name = LuaHelper.GetStringFromValue(nameLuaValue);

            if (!typeLuaValue.IsNil())
            {
                if (typeLuaValue.Type != DataType.Number)
                    throw new Exception("Type needs to be a number/enum value");
                result.Type = (Project.Type)typeLuaValue.Number;
            }
            else
            {
                result.Type = Project.Type.Executable;
            }

            result.Files = GetStringArray("Files", filesLuaValue);
            if(result.Files == null)
                throw new Exception("'Files' table is nil");

            result.ProjectDependencies = GetStringArray("ProjectDependencies", projectDependenciesLuaValue);

            //TODO: Add the project to the program
            //this.Program.AddProject(result);
            //Project.ProjectManager.AddProject(result);
        }

    }*/
}