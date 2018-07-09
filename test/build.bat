@echo off

set dir=%cd%

pushd %dir%\build

make -f %dir%/Makefile.win32 program.exe

popd