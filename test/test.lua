function Init(mode)
    local files = System.GetAllFilesWithExt(System.GetCurrentPath("source/"), "*.cpp");

    local project = {
        Name = "Program",
        Type = ProjectType.Executable,
        Files = files,
    };
    
    Project.AddProject(project);
end

function GetKeyName(table, value)
    for k, v in pairs(table) do
        if v == value then 
            return k; 
        end
    end

    return nil;
end