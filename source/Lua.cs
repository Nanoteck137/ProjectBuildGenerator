using System.IO;
using System;

using MoonSharp.Interpreter;

[MoonSharpUserData]
class LuaSystemLibrary
{
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

    public LuaScript(string luaFilePath)
    {
        string fileData = File.ReadAllText(luaFilePath);

        script = new Script();
        script.Globals["System"] = typeof(LuaSystemLibrary);

        script.DoString(fileData);
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
}
