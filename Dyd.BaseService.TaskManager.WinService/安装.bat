%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe C:\Users\马发才\Desktop\TXOOO\TaskSite\任务调度\Dyd.BaseService.TaskManager.WinService.exe
Net Start Dyd.BaseService.TaskManager.WinService
sc config Dyd.BaseService.TaskManager.WinService start= auto