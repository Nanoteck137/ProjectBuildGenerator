@echo off

if not exist "W:\CSharp\ProjectBuildGenerator\test2\build" (
	mkdir W:\CSharp\ProjectBuildGenerator\test2\build
)

pushd "W:\CSharp\ProjectBuildGenerator\test2\build"
	make -f W:\CSharp\ProjectBuildGenerator\test2\Makefile.gen.win32 program.exe
popd

