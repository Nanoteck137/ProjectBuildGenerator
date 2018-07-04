@echo off

set Dir=%cd%
set BuildDir=%Dir%\build

goto :init

:init
if not exist "%BuildDir%" (
    mkdir %BuildDir%
)

if "%1"=="build" (
    goto :build
)

if "%1"=="clean" (
    goto :clean
)

goto :build

:build

pushd build
cl -Zi -EHsc %Dir%\main.cpp -Feprogram Kernel32.lib	
popd

goto :exit

:clean

pushd build
del program.*
popd

goto :exit

:exit

echo Exiting build.bat