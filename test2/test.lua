function CreateProgramProject()
    local files = Project.GetWorkspaceFiles("source/", "*.cpp");

    local project = {
        Name = "program",
        Type = ProjectType.Executable,
        Files = files,
        ProjectDependencies = { "test" },
    };
    
    Project.AddProject(project);    
end

function CreateTestProject()
    local files = Project.GetWorkspaceFiles("test/", "*.cpp");

    local project = {
        Name = "test",
        Type = ProjectType.StaticLibrary,
        Files = files,
    };
    
    Project.AddProject(project);
end

function Init()
    CreateProgramProject();
    CreateTestProject();
end

function GetKeyName(table, value)
    for k, v in pairs(table) do
        if v == value then 
            return k; 
        end
    end

    return nil;
end