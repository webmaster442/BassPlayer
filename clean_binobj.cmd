@echo off
rem Bin & Obj folder cleanup
rem Written by Webmaster442

:cleanup
rem .net cleanup
for /D %%d in (*) do if exist "%%d\bin" echo Y | rmdir /s "%%d\bin"
for /D %%d in (*) do if exist "%%d\bin" echo I | rmdir /s "%%d\bin"
for /D %%d in (*) do if exist "%%d\obj" echo Y | rmdir /s "%%d\obj"
for /D %%d in (*) do if exist "%%d\obj" echo I | rmdir /s "%%d\obj"
rem visual c++ cleanup
for /D %%d in (*) do if exist "%%d\x64" echo I | rmdir /s "%%d\x64"
for /D %%d in (*) do if exist "%%d\x64" echo Y | rmdir /s "%%d\x64"
