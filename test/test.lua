local win32_config = {
    Compiler = "cl",
    CompilerOnlySwitch = "/c",
    Packer = "lib"
};

local linux_config = {
    Compiler = "clang++",
    CompilerOnlySwitch = "-c",
    Packer = "ar"
};

function Init(mode)
    Config.AddConfig(Mode.Windows, win32_config);
    -- Config.AddConfig(Mode.Linux, linux_config);

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