using System.IO;
using System;

using CodeGen.Batch;

/*
    NOTE:
     1. Check the build folder
        Then create it
     2. Go in to the build folder
     3. Then call the make command with a specified build target
    TODO:
     - We need the make file path
     - Then specify the make targets we want
 */

class ProjectBatch
{
    private Project.Project projectToCall;
    private Generator batchGenerator;

    public ProjectBatch(Project.Project[] projects)
    {
        foreach(Project.Project project in projects)
        {
            if(project.Type == Project.Type.Executable)
                projectToCall = project;
        }
        this.batchGenerator = new Generator();
    }

    public string CreateBatchCode(string buildDir, string makeFilePath)
    {
        ICommand createDirCommand = BatchHelper.CreateCustomCommand("mkdir", buildDir);
        ICommand buildDirExistCommand = BatchHelper.CreateExistCommand(buildDir, ExistCondition.IfNot, createDirCommand);

        this.batchGenerator.AddCommand(buildDirExistCommand);

        ICommand makeCommand = BatchHelper.CreateCustomCommand("make", "-f", makeFilePath, projectToCall.Name + ".exe");
        this.batchGenerator.AddCommand(BatchHelper.CreatePushDirCommand(buildDir, makeCommand));
        
        return this.batchGenerator.GenCode();
    }
}