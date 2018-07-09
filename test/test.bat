@echo off

pushd "C:\Programming\ProjectBuildGenerator\test\build"
	make -f C:\Programming\ProjectBuildGenerator\test
popd

if not exist "C:\Programming\ProjectBuildGenerator\test\dirTest" (
	mkdir C:\Programming\ProjectBuildGenerator\test\dirTest
)

