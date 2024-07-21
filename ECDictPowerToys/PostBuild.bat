taskkill /im PowerToys.exe /f
taskkill /im PowerToys.PowerLauncher.exe /f
ping 127.0.0.1 -n 2 > nul
xcopy bin\x64\Debug\net8.0-windows\* "%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\ECDict\" /y /e
explorer "%PROGRAMFILES%\PowerToys\PowerToys.exe"
exit 0