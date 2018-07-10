using System.IO;
using System;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

[MoonSharpUserData]
public class LuaSystemLibrary
{
    private LuaScript script;

    public LuaSystemLibrary(LuaScript script) 
    { 
        this.script = script;
    }

    public void Print(string str)
    {
        Console.Write(str);
    }

    public void PrintLine(string str)
    {
        Console.WriteLine(str);
    }

    public string[] GetAllFilesWithExt(string dirPath, string ext)
    {
        return Helper.GetAllFilesWithExt(dirPath, ext);
    }

}

public class LuaScriptLoader : ScriptLoaderBase
{
    public override object LoadFile(string file, Table globalContext)
    {
        return string.Format("print(\"Need to load file {0}\")", file);
    }

    public override bool ScriptFileExists(string name)
    {
        Console.WriteLine(name);
        return true;
    }
}

public class LuaScript
{
    private Script script;

    public LuaScript()
    {
        script = new Script();
        
        LuaScriptLoader loader = new LuaScriptLoader();
        loader.ModulePaths = new string[] { "?" };
        script.Options.ScriptLoader = loader;

        LuaSystemLibrary systemLib = new LuaSystemLibrary(this);
        script.Globals["System"] = systemLib;
    }

    public void AddInterface(string name, object instance)
    {
        script.Globals[name] = instance;
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
            throw e;
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