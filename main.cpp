#include <stdio.h>
#include <string>
#include <vector>

//TODO: Make a unix version of all the windows functions
#include <Windows.h>

struct Command
{
    std::string command; //NOTE: i.e cl, del
    std::vector<std::string> paramaters; //NOTE i.e -Zi *
};

struct Section
{
    std::string name;
    std::vector<Command> commands;
};

struct Variable
{
    std::string name;
    std::string value;
};

struct BuildInfo
{
    std::string name;
    std::vector<Variable> globalVariables; //NOTE: Like set dir=%cd%
    std::vector<Section> sections;
};

void GenerateVariable(Variable* variable, std::string* str)
{
    str->append(variable->name);
    str->append("=");
    str->append(variable->value);
    str->append("\n");
}

void GenerateCommand(Command* command, std::string* str)
{
    str->append(command->command);
    for (auto& param : command->paramaters)
    {
        str->append(" ");
        str->append(param);
    }
}

void GenerateSection(Section* section, std::string* str)
{
    str->append(section->name);
    str->append(":\n");

    for (auto& command : section->commands)
    {
        str->append("\t");
        GenerateCommand(&command, str);
    }

    str->append("\n");
}

std::string GenerateCode(BuildInfo* buildInfo)
{
    std::string result;

    for (auto& var : buildInfo->globalVariables)
    {
        GenerateVariable(&var, &result);
    }

    result.append("\n");

    for (auto& section : buildInfo->sections)
    {
        GenerateSection(&section, &result);
    }

    return result;
}

#define BUFSIZE 4096

std::vector<std::string> GetAllFilesInDir(const std::string& dirPath)
{
    std::vector<std::string> result = {};

    std::string path = dirPath;
    path.append("*");

    WIN32_FIND_DATA file;
    HANDLE findHandle = FindFirstFileA(path.c_str(), &file);
    if (findHandle != INVALID_HANDLE_VALUE)
    {
        do
        {
            char buffer[BUFSIZE] = {};
            GetFullPathNameA(file.cFileName, BUFSIZE, buffer, NULL);
            std::string filePath(buffer);

            result.push_back(filePath);
        } while (FindNextFileA(findHandle, &file));
    }

    result.erase(result.begin(), result.begin() + 2);
    return result;
}

int main(int argc, char** argv)
{
    std::vector<Command> commands = {
        { "cl", { "-Zi", "%dir%\\main.cpp" } },
    };

    Section section  = {};
    section.name     = "program";
    section.commands = commands;

    Variable ccVar = { "CC", "cl" };

    Variable dirVar = { "DIR", "{CURDIR}" };

    BuildInfo buildInfo = {};
    buildInfo.name      = "make.win32";
    buildInfo.sections.push_back(section);
    buildInfo.globalVariables.push_back(ccVar);
    buildInfo.globalVariables.push_back(dirVar);

    std::vector<std::string> files = GetAllFilesInDir("build/");
    for(auto& file : files)
    {
        printf("File: %s\n", file.c_str());
    }

    std::string str = GenerateCode(&buildInfo);
    printf("%s\n", str.c_str());

    getchar();
    return 0;
}