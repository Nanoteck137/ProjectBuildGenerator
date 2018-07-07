local project = {
    Name = "Wowowo",
    Type = ProjectType.Executable,
}

function Init(mode)
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