call %~dp0Cfg.bat

set MODULE_DIR=%BANNERLORD_DIR%\Modules\OsbarMods.Assassinate

if not exist %MODULE_DIR% mkdir %MODULE_DIR%
if not exist %MODULE_DIR%\bin mkdir %MODULE_DIR%\bin

rem copy files & folders

rem copy the submodule
copy %~dp0SubModule.xml %MODULE_DIR%\SubModule.xml

rem copy the GUI folder
xcopy %~dp0GUI %MODULE_DIR%\GUI /E /y /I

rem copy the ModuleData folder
xcopy %~dp0ModuleData %MODULE_DIR%\ModuleData /E /y /I

echo "Bannerlord mod files copied"