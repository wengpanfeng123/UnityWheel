set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables

dotnet %LUBAN_DLL% ^
    -t all ^
    -d json ^
	-c cs-simple-json ^
    --conf %CONF_ROOT%\luban.conf ^
	-x outputCodeDir=..\..\Client\Assets\Scripts\Hotfix\DataTable ^
	-x outputDataDir=..\..\Client\Assets\AssetsPackage\DataTable
pause