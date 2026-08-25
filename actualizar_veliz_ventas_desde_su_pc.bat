@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Veliz Ventas - 5 commits funcionales

echo ============================================================
echo   VELIZ VENTAS - 5 COMMITS FUNCIONALES REALES
echo ============================================================
echo.

where git >nul 2>&1
if errorlevel 1 (
  echo [ERROR] Git no esta instalado o no esta agregado al PATH.
  pause
  exit /b 1
)

if not exist ".git" (
  echo [ERROR] Este BAT debe estar en la raiz de trabajo-en-equipo-bike_store.
  pause
  exit /b 1
)

if not exist "veliz_patches\01.patch" (
  echo [ERROR] No se encontro la carpeta veliz_patches.
  pause
  exit /b 1
)

for /f "delims=" %%A in ('git config user.name') do set "GIT_NAME=%%A"
for /f "delims=" %%A in ('git config user.email') do set "GIT_EMAIL=%%A"

if not defined GIT_NAME (
  echo [ERROR] Configure primero git user.name.
  pause
  exit /b 1
)
if not defined GIT_EMAIL (
  echo [ERROR] Configure primero git user.email.
  pause
  exit /b 1
)

echo Autor configurado:
echo   %GIT_NAME%
echo   %GIT_EMAIL%
echo.
choice /C SN /M "Continuar"
if errorlevel 2 exit /b 0

git diff --quiet
if errorlevel 1 goto :dirty
git diff --cached --quiet
if errorlevel 1 goto :dirty

echo.
echo Actualizando informacion desde GitHub...
git fetch origin
if errorlevel 1 goto :error

git show-ref --verify --quiet refs/heads/veliz_ventas
if errorlevel 1 (
  git checkout -b veliz_ventas origin/veliz_ventas
) else (
  git checkout veliz_ventas
)
if errorlevel 1 goto :error

git pull --ff-only origin veliz_ventas
if errorlevel 1 goto :error

git merge --ff-only origin/main
if errorlevel 1 (
  echo.
  echo [ERROR] No fue posible actualizar veliz_ventas con main por fast-forward.
  echo No se modificara ni reescribira el historial.
  pause
  exit /b 1
)

call :apply 01 "feat(ventas): fortalece validaciones de cantidades"
if errorlevel 1 exit /b 1

call :apply 02 "feat(ventas): valida detalles y consolida productos repetidos"
if errorlevel 1 exit /b 1

call :apply 03 "feat(ventas): mejora contratos HTTP del API"
if errorlevel 1 exit /b 1

call :apply 04 "fix(ventas): valida fechas y controla errores de consulta"
if errorlevel 1 exit /b 1

call :apply 05 "feat(ventas): agrega resumen de ventas e impuestos"
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
echo Publicando veliz_ventas...
git push -u origin veliz_ventas
if errorlevel 1 goto :error

echo.
echo ============================================================
echo   RAMA PUBLICADA CORRECTAMENTE
echo ============================================================
echo.
echo Cree un Pull Request:
echo   base: main
echo   compare: veliz_ventas
goto :done

:apply
echo.
echo Aplicando %~1.patch...
git apply --check "veliz_patches\%~1.patch"
if errorlevel 1 (
  echo [ERROR] El parche %~1 no coincide con el codigo actual.
  echo El proceso se detuvo para evitar sobrescribir archivos incorrectos.
  pause
  exit /b 1
)
git apply "veliz_patches\%~1.patch"
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
echo No se utilizo force push.
pause
exit /b 1

:done
echo.
echo Proceso finalizado.
pause
exit /b 0
