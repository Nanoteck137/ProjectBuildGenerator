using System.Collections.Generic;
using System.IO;
using System;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class LuaProjectInterface
{
    private Program program;

    public LuaProjectInterface(Program program)
    {
        this.program = program;
    }

    private string[] GetStringArrayFromTable(Table filesTable)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < filesTable.Length; i++)
        {
            //TODO: Maybe give out a warning if one of the elements is not a string??
            string value = filesTable.Get(i + 1).String;
            if (value != null)
                result.Add(value);
        }

        return result.ToArray();
    }

    public void AddProject(Table projectData)
    {
        Project result = new Project();

        DynValue name = projectData.Get("Name");
        DynValue projectType = projectData.Get("Type");
        DynValue files = projectData.Get("Files");
        DynValue projectDependencies = projectData.Get("ProjectDependencies");

        result.Name = name.String;
        if(result.Name == null)
            Helper.ErrorExit("Lua 'AddProject Function'", "Name needs to be a string in the project table");

        if (projectType.Type == DataType.Number)
            result.Type = (ProjectType)projectType.Number;
        else
            result.Type = ProjectType.Executable;

        if(files.Table == null || files.Table.Length == 0)
            Helper.ErrorExit("Lua 'AddProject Function'", "Files need to be a table of strings in the project table");
        result.Files = GetStringArrayFromTable(files.Table);

        if(projectDependencies.Table == null)
            result.ProjectDependencies = null;
        else
            result.ProjectDependencies = GetStringArrayFromTable(projectDependencies.Table);

        this.program.AddProject(result);
    }

    public string GetWorkspaceDir()
    {
        return this.program.WorkspaceDir;
    }

    public string[] GetWorkspaceFiles(string workspacePath, string ext)
    {
        return Helper.GetAllFilesWithExt(Path.Combine(GetWorkspaceDir(), workspacePath), "*.cpp");
    }

}