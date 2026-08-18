@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Nancy Clientes - 5 commits funcionales

echo ============================================================
echo   NANCY CLIENTES - ACTUALIZAR RAMA Y CREAR 5 COMMITS REALES
echo ============================================================
echo.

where git >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Git no esta instalado o no esta agregado al PATH.
    pause
    exit /b 1
)

if not exist ".git" (
    echo [ERROR] Este BAT debe estar dentro de la carpeta raiz:
    echo trabajo-en-equipo-bike_store
    echo.
    pause
    exit /b 1
)

if not exist "nancy_patches\01.patch" (
    echo [ERROR] No se encontro la carpeta nancy_patches.
    echo Debe estar junto a este BAT.
    pause
    exit /b 1
)

echo Verificando repositorio remoto...
for /f "delims=" %%A in ('git remote get-url origin 2^>nul') do set "REMOTE_URL=%%A"
if not defined REMOTE_URL (
    echo [ERROR] No existe el remoto origin.
    pause
    exit /b 1
)

echo Remoto:
echo   %REMOTE_URL%
echo.

for /f "delims=" %%A in ('git config user.name') do set "GIT_NAME=%%A"
for /f "delims=" %%A in ('git config user.email') do set "GIT_EMAIL=%%A"

if not defined GIT_NAME (
    echo [ERROR] Falta configurar el nombre de Git.
    echo Ejecuta:
    echo git config --global user.name "Nombre de Nancy"
    pause
    exit /b 1
)

if not defined GIT_EMAIL (
    echo [ERROR] Falta configurar el correo de Git.
    echo Debe ser un correo asociado a la cuenta GitHub de Nancy.
    pause
    exit /b 1
)

echo Autor que quedara registrado:
echo   Nombre: %GIT_NAME%
echo   Correo: %GIT_EMAIL%
echo.
echo IMPORTANTE:
echo Estos commits deben ser ejecutados por la persona que realmente
echo realiza o revisa estos cambios en su propia computadora.
echo.
choice /C SN /M "Continuar"
if errorlevel 2 exit /b 0

echo.
echo [1/9] Comprobando que no existan cambios locales sin guardar...
git diff --quiet
if errorlevel 1 (
    echo [ERROR] Hay cambios locales sin commit.
    echo Guarda, confirma o descarta esos cambios antes de continuar.
    pause
    exit /b 1
)
git diff --cached --quiet
if errorlevel 1 (
    echo [ERROR] Hay archivos preparados en staging.
    echo Haz commit o quitalos del staging antes de continuar.
    pause
    exit /b 1
)

echo.
echo [2/9] Descargando informacion actualizada de GitHub...
git fetch origin
if errorlevel 1 goto :error

echo.
echo [3/9] Cambiando a nancy_clientes...
git show-ref --verify --quiet refs/heads/nancy_clientes
if errorlevel 1 (
    git checkout -b nancy_clientes origin/nancy_clientes
) else (
    git checkout nancy_clientes
)
if errorlevel 1 goto :error

echo.
echo [4/9] Sincronizando la rama remota de Nancy...
git pull --ff-only origin nancy_clientes
if errorlevel 1 goto :error

echo.
echo [5/9] Incorporando el main actual a nancy_clientes...
git merge --ff-only origin/main
if errorlevel 1 (
    echo.
    echo [AVISO] No fue posible actualizar mediante fast-forward.
    echo Esto puede ocurrir si existen cambios nuevos independientes en la rama.
    echo No se modificara el historial automaticamente.
    pause
    exit /b 1
)

echo.
echo La rama ya esta actualizada con main.
echo Aplicando los 5 cambios funcionales...
echo.

echo Commit 1/5: validaciones de clientes
git apply --check "nancy_patches\01.patch"
if errorlevel 1 goto :patcherror
git apply "nancy_patches\01.patch"
if errorlevel 1 goto :patcherror
git add -A
git commit -m "feat(clientes): fortalece validaciones de datos del cliente"
if errorlevel 1 goto :error

echo.
echo Commit 2/5: normalizacion de datos y filtros
git apply --check "nancy_patches\02.patch"
if errorlevel 1 goto :patcherror
git apply "nancy_patches\02.patch"
if errorlevel 1 goto :patcherror
git add -A
git commit -m "feat(clientes): normaliza filtros nombres y correo"
if errorlevel 1 goto :error

echo.
echo Commit 3/5: mejoras del API de clientes
git apply --check "nancy_patches\03.patch"
if errorlevel 1 goto :patcherror
git apply "nancy_patches\03.patch"
if errorlevel 1 goto :patcherror
git add -A
git commit -m "feat(clientes): mejora contratos HTTP del API de clientes"
if errorlevel 1 goto :error

echo.
echo Commit 4/5: manejo de errores
git apply --check "nancy_patches\04.patch"
if errorlevel 1 goto :patcherror
git apply "nancy_patches\04.patch"
if errorlevel 1 goto :patcherror
git add -A
git commit -m "fix(clientes): controla errores de consulta edicion y eliminacion"
if errorlevel 1 goto :error

echo.
echo Commit 5/5: interfaz y formulario
git apply --check "nancy_patches\05.patch"
if errorlevel 1 goto :patcherror
git apply "nancy_patches\05.patch"
if errorlevel 1 goto :patcherror
git add -A
git commit -m "feat(clientes): mejora busqueda formulario y acciones de la interfaz"
if errorlevel 1 goto :error

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
echo [9/9] Publicando nancy_clientes...
git push -u origin nancy_clientes
if errorlevel 1 goto :error

echo.
echo ============================================================
echo   RAMA PUBLICADA CORRECTAMENTE
echo ============================================================
echo.
echo Ahora en GitHub:
echo   1. Abre la rama nancy_clientes.
echo   2. Revisa los 5 commits nuevos.
echo   3. Crea un nuevo Pull Request hacia main.
echo.
goto :done

:patcherror
echo.
echo ============================================================
echo   NO SE PUDO APLICAR UNO DE LOS CAMBIOS
echo ============================================================
echo.
echo El codigo actual es diferente al esperado por el parche.
echo El proceso se detuvo para no sobrescribir archivos incorrectamente.
echo.
echo Los commits realizados antes del error permanecen en la rama.
echo No hagas push hasta revisar el problema.
echo.
pause
exit /b 1

:error
echo.
echo ============================================================
echo   ERROR
echo ============================================================
echo Revisa el mensaje mostrado arriba.
echo No se utilizo force push ni se reescribio el historial remoto.
echo.
pause
exit /b 1

:done
echo.
echo Proceso finalizado.
pause
exit /b 0
