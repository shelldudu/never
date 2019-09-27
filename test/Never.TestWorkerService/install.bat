set path=%CD%
%windir%\system32\sc.exe create Never.TestWorkerService binpath= %path%\Never.TestWorkerService.exe
pause