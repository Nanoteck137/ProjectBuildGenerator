using System.Collections.Generic;
using System;

namespace Platform
{
    /*this.script = new LuaScript();

    this.script.AddEnumType("ProjectType", typeof(Project.Type));

    ProjectInterface projectInterface = new ProjectInterface();
    this.script.AddInterface("Project", projectInterface);

    this.script.RunScript(luaConfigPath);
    this.script.CallFunction("Init");*/

    abstract class Program
    {
        //TODO: We want the lua script to live here
        //TODO: We want a list of projects

        //TODO: A method to setup some generic lua initialization
        //TODO: A method to create the project or add it (im not sure yet)
    }
}