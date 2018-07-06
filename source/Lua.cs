using System.IO;
using System;

using MoonSharp.Interpreter;

[MoonSharpUserData]
class LuaSystemLibrary
{
    public const int Hello = 4284; 

    private LuaSystemLibrary() { }

    public static void Print(string str)
    {
        Console.Write(str);
    }

    public static void PrintLine(string str)
    {
        Console.WriteLine(str);
    }
}

class LuaScript
{
    private Script script;

    public LuaScript()
    {
        script = new Script();
        script.Globals["System"] = typeof(LuaSystemLibrary);
        script.Globals["Project"] = typeof(LuaMakeProject);
    }

    public void RunScript(string luaFilePath)
    {
        string fileData = File.ReadAllText(luaFilePath);
        try
        {
            script.DoString(fileData, null, "Testing.lua");
        }
        catch (ScriptRuntimeException e)
        {
            Console.WriteLine("LUA Error: " + e.DecoratedMessage);
            System.Diagnostics.Debugger.Break();
        }
    }

    public DynValue CallFunction(string function, params object[] parameters)
    {
        object functionPtr = script.Globals.Get(function);
        if(functionPtr == null)
        {
            throw new Exception("Cant find function: " + function);
        }

        DynValue result = script.Call(functionPtr, parameters);

        return result;
    }

    public void SetGlobalVariable(string name, object value)
    {
        script.Globals[name] = value;
    }

    public void AddFunction(System.Delegate function)
    {
        Console.WriteLine("Name: {0}", function.Method.Name);
    }
}
