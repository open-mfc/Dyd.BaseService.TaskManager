%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe D:\Desktop\Debug\Dyd.BaseService.TaskManager.WinService.exe
Net Start Dyd.BaseService.TaskManager.WinService
sc config Dyd.BaseService.TaskManager.WinService start= auto