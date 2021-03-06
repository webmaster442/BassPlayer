#include <Windows.h>
#include <cwchar>
#include <Shlwapi.h>
#include "listplug.h"

HINSTANCE hinst;
HMODULE FLibHandle = 0;
HWND timer = 0;
EXTERN_C IMAGE_DOS_HEADER __ImageBase;

#define EXTENSIONS "MULTIMEDIA & (EXT=\"MP3\" | EXT=\"MP4\" | EXT=\"M4A\" | EXT=\"M4B\" | EXT=\"AAC\" | EXT=\"FLAC\" | EXT=\"AC3\" | EXT=\"WV\" | EXT=\"WAV\" | EXT=\"WMA\" | EXT=\"OGG\" | EXT=\"M3U\" | EXT=\"PLS\" | EXT=\"BPL\" | EXT=\"WPL\")"


BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		hinst = (HINSTANCE)hModule;
		break;
	case DLL_PROCESS_DETACH:
		if (FLibHandle) FreeLibrary(FLibHandle);
		FLibHandle = NULL;
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	}
	return TRUE;
}

char* strlcpy(char* p, char*p2, int maxlen)
{
	if ((int)strlen(p2) >= maxlen)
	{
		strncpy(p, p2, maxlen);
		p[maxlen] = 0;
	}
	else strcpy(p, p2);
	return p;
}

void CALLBACK f(HWND hwnd, UINT uMsg, UINT timerId, DWORD dwTime)
{
	KillTimer(timer, 0);
	DestroyWindow(timer);
}

HWND __stdcall ListLoad(HWND ParentWin, char* FileToLoad, int ShowFlags)
{
	wchar_t file[MAX_PATH];
	wchar_t safefile[MAX_PATH];
	wchar_t dllpath[MAX_PATH];

	GetModuleFileName((HINSTANCE)&__ImageBase, dllpath, _MAX_PATH);

	PathRemoveFileSpec(dllpath);

	mbstowcs(file, FileToLoad, MAX_PATH);
	swprintf(safefile, L"\"%s\"", file);

	SHELLEXECUTEINFO ShExecInfo = { 0 };
	ShExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);
	ShExecInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
	ShExecInfo.hwnd = NULL;
	ShExecInfo.lpVerb = NULL;
	ShExecInfo.lpFile = L"BassPlayer.exe";
	ShExecInfo.lpParameters = safefile;
	ShExecInfo.lpDirectory = dllpath;
	ShExecInfo.nShow = SW_NORMAL;
	ShExecInfo.hInstApp = NULL;
	ShellExecuteEx(&ShExecInfo);
	Sleep(25);
	timer = ParentWin;
	SetTimer(timer, 0, 250, (TIMERPROC)&f);
	return NULL;
}

void __stdcall ListGetDetectString(char* DetectString, int maxlen)
{
	strlcpy(DetectString, EXTENSIONS, maxlen);
}

void __stdcall ListCloseWindow(HWND ListWin)
{
	DestroyWindow(ListWin);
	return;
}