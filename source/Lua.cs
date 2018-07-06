using System.IO;
using System;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

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

class LuaScriptLoader : ScriptLoaderBase
{
    public override object LoadFile(string file, Table globalContext)
    {
        return string.Format("System.WriteLine(\"Need to load file {0}\")", file);
    }

    public override bool ScriptFileExists(string name)
    {
        Console.WriteLine(name);
        return true;
    }
}

class LuaScript
{
    private Script script;

    public LuaScript()
    {
        script = new Script();
        LuaScriptLoader loader = new LuaScriptLoader();
        loader.ModulePaths = new string[] { "?" };
        script.Options.ScriptLoader = loader;
        script.Globals["System"] = typeof(LuaSystemLibrary);
        script.Globals["Project"] = typeof(LuaInterface.ProjectInterface);
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

    public void AddEnumType(string name, Type enumType)
    {
        if(!enumType.IsEnum)
            throw new Exception("The type needs to be a enum");
        
        Table table = new Table(script);

        foreach(var value in enumType.GetEnumValues())
        {
            table.Set(value.ToString(), DynValue.NewNumber((int)value));
        }

        script.Globals.Set(name, DynValue.NewTable(table));
    }
}
