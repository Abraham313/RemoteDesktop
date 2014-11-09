// RemoteDesktopServer.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "ServiceInstaller.h"
#include "ServerService.h"
#include "Server.h"

// Internal name of the service
#define SERVICE_NAME             L"CppWindowsService"
// Displayed name of the service
#define SERVICE_DISPLAY_NAME     L"CppWindowsService Sample Service"
// Service start options.
#define SERVICE_START_TYPE       SERVICE_DEMAND_START
// List of service dependencies - "dep1\0dep2\0\0"

#define DEFAULTPORT 443

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPTSTR    lpCmdLine,
	_In_ int       nCmdShow)
{
	int argc;

	LPWSTR* argv = CommandLineToArgvW(GetCommandLine(), &argc);

	if ((argc > 1) && ((*argv[1] == L'-' || (*argv[1] == L'/'))))
	{
		if (_wcsicmp(L"install", argv[1] + 1) == 0)
		{
			// Install the service when the command is "-install" or "/install".
			InstallService(
				SERVICE_NAME,               // Name of service
				SERVICE_DISPLAY_NAME,       // Name to display
				SERVICE_DEMAND_START, 
				L"",
				NULL,
				NULL
				);
		}
		else if (_wcsicmp(L"remove", argv[1] + 1) == 0)
		{
			// Uninstall the service when the command is 
			// "-remove" or "/remove".
			UninstallService(SERVICE_NAME);
		}
		else if (_wcsicmp(L"service_mon", argv[1] + 1) == 0)
		{
			ServerService service(SERVICE_NAME);
			ServerService::Run(service);
		}
	}
	else
	{
		auto _Server = std::make_shared<RemoteDesktop::Server>();
		_Server->Listen(DEFAULTPORT);
	}
	return 0;
}
