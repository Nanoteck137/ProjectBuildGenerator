using System;

using MoonSharp.Interpreter;

namespace LuaInterface
{
    [MoonSharpUserData]
    class ProjectInterface
    {
        private ProjectInterface() { }

        public static void AddProject(Table projectData)
        {
            DynValue value = projectData.Get("Name");
        }

    }
}