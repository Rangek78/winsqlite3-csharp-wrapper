@echo off
setlocal enabledelayedexpansion

:: Configuration paths
set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set SRC_DIR=lib
set "REF_DIR="
set OUT_DIR=dependencies
set OUT=%OUT_DIR%\MySqlite.dll

:: Ensure target directory exists
if not exist %OUT_DIR% mkdir "%OUT_DIR%"

:: 1. Loop through DLLs, build semicolon string AND copy them to target
set "REFS="
for %%f in ("REF_DIR\*.dll") do (
    copy "%%f" "OUT_DIR\" >nul
    if "!REFS!"=="" (
        set "REFS=%OUT_DIR%\%%~nxf"
    ) else (
        set "REFS=!REFS!;%OUT_DIR%\%%~nxf"
    )
)

:: 2. Recursively loop through all .cs files in subdirectories and build a space-separated list
set "SOURCES="
for \R %SRC_DIR% %%f in (*.cs") do (
    set "SOURCES=!SOURCES! "%%f""
)

:: 3. Execute compilation with references (if any) and recursive sources
if defined REFS (
    echo Compiling with references and recursive source files...
    %CSC% /out: "%OUT%" /target:library /reference:"!REFS!" %SOURCES%
) else (
    echo Compiling with recursive source files only...
    %CSC% /out: "%OUT%" /target:library %SOURCES%
)

if %errorlevel% equ 0 (
    echo Compilation successful!
) else (
    echo Compilation failed.
    pause
)

endlocal