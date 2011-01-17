@echo off
SET sn="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\sn.exe"
set gacutil="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\gacutil.exe"
set msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
REM set lib="\lib"
REM set configuration=Release
REM if exist commonfiles\ConfORM.snk goto build
REM echo Generating strong key...
REM %sn% -k commonfiles\ConfORM.snk
:build
%msbuild% default.msbuild /v:n /t:Build
:end
echo -------------------------------
pause