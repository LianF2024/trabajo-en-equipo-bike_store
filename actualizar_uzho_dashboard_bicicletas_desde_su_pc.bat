@echo off
setlocal EnableExtensions
cd /d "%~dp0"
title Uzho Dashboard y Bicicletas - 5 commits funcionales

echo ============================================================
echo   UZHO DASHBOARD Y BICICLETAS - 5 COMMITS FUNCIONALES
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

if not exist "uzho_patches\01.patch" (
  echo [ERROR] No se encontro la carpeta uzho_patches.
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

git show-ref --verify --quiet refs/heads/uzho_Dashboard_y_Bicicletas
if errorlevel 1 (
  git checkout -b uzho_Dashboard_y_Bicicletas origin/uzho_Dashboard_y_Bicicletas
) else (
  git checkout uzho_Dashboard_y_Bicicletas
)
if errorlevel 1 goto :error

git pull --ff-only origin uzho_Dashboard_y_Bicicletas
if errorlevel 1 goto :error

git merge --ff-only origin/main
if errorlevel 1 (
  echo.
  echo [ERROR] No fue posible actualizar la rama con main por fast-forward.
  echo No se reescribira el historial.
  pause
  exit /b 1
)

call :apply 01 "feat(bicicletas): valida y normaliza marca y modelo"
if errorlevel 1 exit /b 1

call :apply 02 "feat(bicicletas): mejora contratos HTTP del API"
if errorlevel 1 exit /b 1

call :apply 03 "fix(bicicletas): mejora filtros y manejo de errores"
if errorlevel 1 exit /b 1

call :apply 04 "feat(dashboard): agrega metricas mensuales y de inventario"
if errorlevel 1 exit /b 1

call :apply 05 "feat(dashboard): mejora indicadores y alertas visuales"
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
echo Publicando uzho_Dashboard_y_Bicicletas...
git push -u origin uzho_Dashboard_y_Bicicletas
if errorlevel 1 goto :error

echo.
echo ============================================================
echo   RAMA PUBLICADA CORRECTAMENTE
echo ============================================================
echo.
echo Cree un Pull Request:
echo   base: main
echo   compare: uzho_Dashboard_y_Bicicletas
goto :done

:apply
echo.
echo Aplicando %~1.patch...
git apply --check "uzho_patches\%~1.patch"
if errorlevel 1 (
  echo [ERROR] El parche %~1 no coincide con el codigo actual.
  echo El proceso se detuvo para evitar sobrescribir archivos incorrectos.
  pause
  exit /b 1
)
git apply "uzho_patches\%~1.patch"
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
