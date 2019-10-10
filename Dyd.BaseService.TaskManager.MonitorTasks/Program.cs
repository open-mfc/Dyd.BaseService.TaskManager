using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyd.BaseService.TaskManager.MonitorTasks
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            TaskManageErrorSendTask task = new TaskManageErrorSendTask();
            task.SystemRuntimeInfo = new XXF.BaseService.TaskManager.SystemRuntime.TaskSystemRuntimeInfo() { TaskConnectString = "Server=10.10.10.35;Database=dyd_bs_task;UID=TxoooNewDataBaseDesignUser;Password=Tx)))NewSJCoolSJPassWORderAdmin;" };

            task.TestRun();
        }
    }
}
