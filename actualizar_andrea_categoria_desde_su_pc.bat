@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Andrea Categorias - 5 commits funcionales

echo ============================================================
echo   ANDREA CATEGORIAS - 5 COMMITS FUNCIONALES REALES
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

if not exist "andrea_patches\01.patch" (
    echo [ERROR] No se encontro la carpeta andrea_patches.
    pause
    exit /b 1
)

for /f "delims=" %%A in ('git config user.name') do set "GIT_NAME=%%A"
for /f "delims=" %%A in ('git config user.email') do set "GIT_EMAIL=%%A"

if not defined GIT_NAME (
    echo [ERROR] Configure primero el nombre Git de Andrea.
    echo Ejemplo: git config --global user.name "ajvs9124-maker"
    pause
    exit /b 1
)

if not defined GIT_EMAIL (
    echo [ERROR] Configure primero el correo asociado a GitHub de Andrea.
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
echo [1/9] Verificando cambios locales...
git diff --quiet
if errorlevel 1 goto :dirty
git diff --cached --quiet
if errorlevel 1 goto :dirty

echo.
echo [2/9] Actualizando informacion desde GitHub...
git fetch origin
if errorlevel 1 goto :error

echo.
echo [3/9] Abriendo rama andrea_categoria...
git show-ref --verify --quiet refs/heads/andrea_categoria
if errorlevel 1 (
    git checkout -b andrea_categoria origin/andrea_categoria
) else (
    git checkout andrea_categoria
)
if errorlevel 1 goto :error

echo.
echo [4/9] Actualizando la rama remota...
git pull --ff-only origin andrea_categoria
if errorlevel 1 goto :error

echo.
echo [5/9] Incorporando el main vigente...
git merge --ff-only origin/main
if errorlevel 1 (
    echo.
    echo [AVISO] La rama no se puede actualizar por fast-forward.
    echo No se reescribira el historial automaticamente.
    pause
    exit /b 1
)

echo.
echo Aplicando cambios funcionales...

call :applycommit 01 "feat(categorias): fortalece validaciones de nombre y estado"
if errorlevel 1 exit /b 1

call :applycommit 02 "feat(categorias): normaliza nombres y busqueda en el servicio"
if errorlevel 1 exit /b 1

call :applycommit 03 "feat(categorias): mejora contratos HTTP del API"
if errorlevel 1 exit /b 1

call :applycommit 04 "fix(categorias): controla errores y normaliza datos del formulario"
if errorlevel 1 exit /b 1

call :applycommit 05 "feat(categorias): mejora listado formulario y acciones de interfaz"
if errorlevel 1 exit /b 1

echo.
echo ============================================================
echo   5 COMMITS FUNCIONALES CREADOS CORRECTAMENTE
echo ============================================================
echo.
git log -5 --format="%%h - %%an - %%ae - %%s"
echo.

choice /C SN /M "Subir ahora los 5 commits a GitHub"
if errorlevel 2 goto :done

echo.
echo [9/9] Publicando andrea_categoria...
git push -u origin andrea_categoria
if errorlevel 1 goto :error

echo.
echo ============================================================
echo   RAMA PUBLICADA CORRECTAMENTE
echo ============================================================
echo.
echo En GitHub cree un nuevo Pull Request:
echo   base: main
echo   compare: andrea_categoria
echo.
goto :done

:applycommit
echo.
echo Aplicando %~1.patch...
git apply --check "andrea_patches\%~1.patch"
if errorlevel 1 (
    echo [ERROR] El parche %~1 no coincide con el codigo actual.
    echo El proceso se detuvo para evitar sobrescribir codigo.
    pause
    exit /b 1
)
git apply "andrea_patches\%~1.patch"
if errorlevel 1 exit /b 1
git add -A
git commit -m "%~2"
if errorlevel 1 exit /b 1
exit /b 0

:dirty
echo.
echo [ERROR] Hay cambios locales pendientes.
echo Haga commit, guarde o descarte esos cambios antes de ejecutar el BAT.
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
