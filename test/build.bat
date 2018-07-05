@echo off

set dir=%cd%

pushd %dir%\build

make srcDir=%dir%\source -f %dir%/Makefile.win32 program.exe

popd