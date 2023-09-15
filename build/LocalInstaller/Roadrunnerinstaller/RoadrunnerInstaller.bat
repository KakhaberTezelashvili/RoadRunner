@ECHO OFF

>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"

if '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params = %*:"=""
    echo UAC.ShellExecute "cmd.exe", "/c %~s0 %params%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    del "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
    pushd "%CD%"
    CD /D "%~dp0"
    
:START
certutil -p qwer1234 -importPFX root .\ssl\localhost.pfx >NUL
docker info >NUL 2>NUL
IF ERRORLEVEL 1 (ECHO Docker desktop is not running. Start Docker desktop to continue && GOTO B) ELSE (ECHO Starting installation && GOTO C)

:B
timeout /t 2>NUL
docker info >NUl 2>NUL
IF ERRORLEVEL 1 (GOTO B)

:C
docker info 2>NUL | find /i "OSType: linux" >NUL
IF ERRORLEVEL 1 ("C:\Program Files\Docker\Docker\DockerCli.exe" -SwitchDaemon)
timeout /t 10>NUL
ECHO Installation ready
pause
ECHO.
docker login crdhstdocdevroadrunnertest.azurecr.io -u crDHSTDocDevRoadrunnerTest -p REiq8UDqEvYqFLruvgMOmklb1Q=5WO0i >NUL 2>NUL
docker-compose pull && ECHO.
ECHO Building application
ECHO.
docker-compose up -d
ECHO.
ECHO Waiting for database initialization . . .

:D
timeout /t 1 >nul
CURL -i "https://localhost:5001/production/api/v1/auth/login" -H "accept: text/plain" -H "Content-Type: application/json-patch+json" -d "{'userInitials':'da'}" 2>NUL | find /i "200 OK">NUL   
if ERRORLEVEL 1 (GOTO D)
ECHO Database initialization successful
ECHO.
ECHO Installation complete! Press any key to exit . . .
pause >NUL

:EXIT
