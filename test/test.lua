local win32_config = {
    Compiler = "cl",
    CompilerOnlySwitch = "/c",
    Packer = "lib"
};

--[[local linux_config = {
    Compiler = "clang++",
    CompilerOnlySwitch = "-c",
    Packer = "ar"
};--]]

function CreateProgramProject()
    local files = System.GetAllFilesWithExt(System.GetCurrentPath("source/"), "*.cpp");

    local project = {
        Name = "program",
        Type = ProjectType.Executable,
        Files = files,
    };
    
    Project.AddProject(project);    
end

function CreateTestProject()
    local files = System.GetAllFilesWithExt(System.GetCurrentPath("test/"), "*.cpp");

    local project = {
        Name = "test",
        Type = ProjectType.Executable,
        Files = files,
    };
    
    Project.AddProject(project);
end

function Init(mode)
    Config.AddConfig(Mode.Windows, win32_config);
    -- Config.AddConfig(Mode.Linux, linux_config);

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