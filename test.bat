@echo off

call :test "Hello Test" Test

goto :exit

:test
echo First: %~1
echo Second: %~2

exit /b 0

:exit
echo Exiting the batch script
