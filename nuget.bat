@echo off
:start
echo =================================================
echo ============ 0:build
echo ============ 1:pack
echo ============ 2:delete
echo ==================================================

set /P var="input num:"
if %var%==0 goto build
if %var%==1 goto pack
if %var%==2 goto delete


:build
set path=%CD%
set exe=%path%\tools\nuget.exe
set outPath=%path%\tools\
set x86="c:\Program Files\dotnet\dotnet.exe"
set x64="c:\Program Files (x86)\dotnet\dotnet.exe"
set file=""
if exist %x86% ( set file=%x86% )
if exist %x64% ( set file=%x64%)
del %outPath%\*.nupkg

for /R %path% %%f in (*.sln) do (
echo build file %%f
%file% build %%f -c release
)

goto start

:pack
set path=%CD%
set exe=%path%\tools\nuget.exe
set outPath=%path%\tools\
set x86="c:\Program Files\dotnet\dotnet.exe"
set x64="c:\Program Files (x86)\dotnet\dotnet.exe"
set file=""
if exist %x86% ( set file=%x86% )
if exist %x64% ( set file=%x64%)
del %outPath%\*.nupkg

for /R %outPath% %%f in (*.nuspec) do (
%exe% pack %%f  -OutputDirectory %outPath%
)

goto start


:delete
set path=%CD%
set outPath=%path%\tools\
del %outPath%\*.nupkg
echo ....
:ipdhcp

pause