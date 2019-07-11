set path=%CD%
%windir%\system32\sc.exe create Never.TestWebApi binpath= %path%\Never.TestWebApi.exe
pause