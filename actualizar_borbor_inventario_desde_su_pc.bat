@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Borbor Inventario - 3 commits funcionales

echo ============================================================
echo   BORBOR INVENTARIO - 3 COMMITS FUNCIONALES REALES
echo ============================================================
echo.

where git >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Git no esta instalado o no esta agregado al PATH.
    pause
    exit /b 1
)

if not exist ".git" (
    echo [ERROR] Este BAT debe estar en la raiz de:
    echo trabajo-en-equipo-bike_store
    pause
    exit /b 1
)

if not exist "borbor_patches\01.patch" (
    echo [ERROR] No se encontro la carpeta borbor_patches.
    pause
    exit /b 1
)

for /f "delims=" %%A in ('git config user.name') do set "GIT_NAME=%%A"
for /f "delims=" %%A in ('git config user.email') do set "GIT_EMAIL=%%A"

if not defined GIT_NAME (
    echo [ERROR] Configure primero su nombre de Git.
    pause
    exit /b 1
)
if not defined GIT_EMAIL (
    echo [ERROR] Configure primero su correo asociado a GitHub.
    pause
    exit /b 1
)

echo Autor configurado:
echo   %GIT_NAME%
echo   %GIT_EMAIL%
echo.
echo Los commits quedaran registrados con esta identidad.
choice /C SN /M "Continuar"
if errorlevel 2 exit /b 0

echo.
echo [1/7] Verificando cambios locales...
git diff --quiet
if errorlevel 1 goto :dirty
git diff --cached --quiet
if errorlevel 1 goto :dirty

echo.
echo [2/7] Actualizando informacion desde GitHub...
git fetch origin
if errorlevel 1 goto :error

echo.
echo [3/7] Abriendo rama borbor_inventario...
git show-ref --verify --quiet refs/heads/borbor_inventario
if errorlevel 1 (
    git checkout -b borbor_inventario origin/borbor_inventario
) else (
    git checkout borbor_inventario
)
if errorlevel 1 goto :error

echo.
echo [4/7] Actualizando rama remota...
git pull --ff-only origin borbor_inventario
if errorlevel 1 goto :error

echo.
echo [5/7] Incorporando el main vigente...
git merge --ff-only origin/main
if errorlevel 1 (
    echo.
    echo [AVISO] No fue posible actualizar mediante fast-forward.
    echo No se reescribira el historial automaticamente.
    pause
    exit /b 1
)

call :applycommit 01 "feat(inventario): valida filtros y umbral de stock bajo"
if errorlevel 1 exit /b 1

call :applycommit 02 "feat(inventario): agrega limite configurable para stock bajo"
if errorlevel 1 exit /b 1

call :applycommit 03 "feat(inventario): agrega resumen de existencias y alertas de stock"
if errorlevel 1 exit /b 1

echo.
echo ============================================================
echo   3 COMMITS FUNCIONALES CREADOS CORRECTAMENTE
echo ============================================================
echo.
git log -3 --format="%%h - %%an - %%ae - %%s"
echo.

choice /C SN /M "Subir ahora los 3 commits a GitHub"
if errorlevel 2 goto :done

echo.
echo [7/7] Publicando borbor_inventario...
git push -u origin borbor_inventario
if errorlevel 1 goto :error

echo.
echo ============================================================
echo   RAMA PUBLICADA CORRECTAMENTE
echo ============================================================
echo.
echo En GitHub cree un Pull Request:
echo   base: main
echo   compare: borbor_inventario
echo.
goto :done

:applycommit
echo.
echo Aplicando %~1.patch...
git apply --check "borbor_patches\%~1.patch"
if errorlevel 1 (
    echo [ERROR] El parche %~1 no coincide con el codigo actual.
    echo El proceso se detuvo para evitar sobrescribir codigo.
    pause
    exit /b 1
)
git apply "borbor_patches\%~1.patch"
if errorlevel 1 exit /b 1
git add -A
git commit -m "%~2"
if errorlevel 1 exit /b 1
exit /b 0

:dirty
echo.
echo [ERROR] Hay cambios locales pendientes.
echo Haga commit, guarde o descarte esos cambios antes de continuar.
pause
exit /b 1

:error
echo.
echo [ERROR] El proceso no pudo continuar.
echo Revise el mensaje anterior.
echo No se utilizo force push.
pause
exit /b 1

:done
echo.
echo Proceso finalizado.
pause
exit /b 0
