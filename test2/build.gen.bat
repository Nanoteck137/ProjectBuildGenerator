@echo off

if not exist "C:\Programming\ProjectBuildGenerator\test2\build" (
	mkdir C:\Programming\ProjectBuildGenerator\test2\build
)

make -f C:\Programming\ProjectBuildGenerator\test2\Makefile.win32.gen program.exe

