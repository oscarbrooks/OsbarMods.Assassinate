call %~dp0Cfg.bat

set MODULE_DIR=%BANNERLORD_DIR%\Modules\OsbarMods.Assassinate

if not exist %MODULE_DIR% mkdir %MODULE_DIR%
if not exist %MODULE_DIR%\bin mkdir %MODULE_DIR%\bin

copy %~dp0SubModule.xml %MODULE_DIR%\SubModule.xml
xcopy %~dp0GUI %MODULE_DIR%\GUI /E /y /I

echo "Bannerlord mod files copied"