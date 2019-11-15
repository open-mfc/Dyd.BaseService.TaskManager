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
            var _json = new DingTalkService("dinglfmorrawr77sau47", "KPiUN-KQyXODKlZN6r9Xfg7XZUnRH1wszJ-i80ouf8xWwizMqurwuikBTG27mQb9").SendText("01523069397077", "测试001");

            Console.ReadKey();
            //TaskManageErrorSendTask task = new TaskManageErrorSendTask();
            //task.SystemRuntimeInfo = new XXF.BaseService.TaskManager.SystemRuntime.TaskSystemRuntimeInfo() { TaskConnectString = "Server=10.10.10.35;Database=dyd_bs_task;UID=TxoooNewDataBaseDesignUser;Password=Tx)))NewSJCoolSJPassWORderAdmin;" };

            //task.TestRun();
        }
    }
}
