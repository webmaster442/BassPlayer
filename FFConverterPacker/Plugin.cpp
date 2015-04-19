#include <Windows.h>
#include <Shlwapi.h>
#include "packer.h"

HINSTANCE hinst;
HMODULE FLibHandle = 0;
EXTERN_C IMAGE_DOS_HEADER __ImageBase;

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

//-------------------------------------------------------------------------------------------------
HANDLE __stdcall OpenArchive(tOpenArchiveData *ArchiveData)
{
	ArchiveData->OpenResult = E_NO_FILES;
	return 0;
}

int __stdcall ReadHeader(HANDLE hArcData, tHeaderData *HeaderData)
{
	return 0;
}

int __stdcall ProcessFile(HANDLE hArcData, int Operation, char *DestPath, char *DestName)
{
	return E_NO_FILES;
}

int __stdcall CloseArchive(HANDLE hArcData)
{
	return 0;
}

void __stdcall SetChangeVolProc(HANDLE hArcData, tChangeVolProc pChangeVolProc1)
{

}

void __stdcall SetProcessDataProc(HANDLE hArcData, tProcessDataProc pProcessDataProc)
{
}

//-------------------------------------------------------------------------------------------------

int __stdcall PackFiles(char *PackedFile, char *SubPath, char *SrcPath, char *AddList, int Flags)
{
	wchar_t dllpath[MAX_PATH];
	GetModuleFileName((HINSTANCE)&__ImageBase, dllpath, _MAX_PATH);
	PathRemoveFileSpec(dllpath);



	return 0;
}