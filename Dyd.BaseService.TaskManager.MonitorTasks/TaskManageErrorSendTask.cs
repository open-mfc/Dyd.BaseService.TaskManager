using Dyd.BaseService.TaskManager.Core;
using Dyd.BaseService.TaskManager.Domain.Dal;
using Dyd.BaseService.TaskManager.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XXF.ProjectTool;

namespace Dyd.BaseService.TaskManager.MonitorTasks
{
    /// <summary>
    /// 任务调度平台之系统错误信息发送-监控任务
    /// </summary>
    public class TaskManageErrorSendTask : XXF.BaseService.TaskManager.BaseDllTask
    {

        public override void Run()
        {
            var tempinfo = this.OpenOperator.GetDataBaseTempData<TaskManageErrorSendTaskTempInfo>();
            if (tempinfo == null)
            {
                tempinfo = new TaskManageErrorSendTaskTempInfo();
                tempinfo.LastLogID = 0;
            }
            //扫描
            List<tb_senderror_model> senderrormodels = new List<tb_senderror_model>();
            List<tb_user_model> usermodels = new List<tb_user_model>();
            SqlHelper.ExcuteSql(this.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                tb_error_dal errordal = new tb_error_dal();
                senderrormodels = errordal.GetErrors(c, tempinfo.LastLogID);
                tb_user_dal userdal = new tb_user_dal();
                usermodels = userdal.GetAllUsers(c);
            });
            foreach (var u in usermodels)
            {
                if (string.IsNullOrEmpty(u.useremail))
                    continue;
                if (u.userrole == (int)EnumUserRole.Admin)
                    SendEmail(senderrormodels, u.useremail);
                else
                    SendEmail(senderrormodels.Where(c => c.taskcreateuserid == u.id).ToList(), u.useremail);
            }
            if (senderrormodels.Count > 0)
            {
                int maxid = senderrormodels.Max(c => c.error_model.id);
                tempinfo.LastLogID = maxid;
                this.OpenOperator.Log(string.Format("本次处理错误日志{0}条", senderrormodels.Count));
            }
            this.OpenOperator.SaveDataBaseTempData(tempinfo);
        }

        private void SendEmail(List<tb_senderror_model> errors, string email)
        {
            StringBuilder sb = new StringBuilder();
            if (errors != null)
                foreach (var e in errors)
                {
                    sb.AppendLine($"【错误id】{e.error_model.id}【任务id】{e.error_model.taskid}【创建时间】{e.error_model.errorcreatetime}【集群id】{e.error_model.nodeid}<br/>");
                }
            string content = sb.ToString();
            if (string.IsNullOrEmpty(content))
                return;
            content += "\r\n详情请查看错误信息表!";
            try
            {
                var _json = new DingTalkService(this.AppConfig["appKey"], this.AppConfig["appsecret"]).SendText(email, content);
                OpenOperator.Log(_json.ToString());
            }
            catch (Exception exp)
            {
                OpenOperator.Error("发送错误钉钉消息失败", exp);
            }

            //EmailHelper emailhelper = new EmailHelper();
            //emailhelper.mailFrom = this.AppConfig["sendmailname"];
            //emailhelper.mailPwd = this.AppConfig["password"];
            //emailhelper.mailSubject = "任务调度平台之错误日报" + DateTime.Now.ToString("yyyyMMddHHmmss")+"【系统邮件】";
            //emailhelper.mailBody = content;
            //emailhelper.isbodyHtml = true;    //是否是HTML
            //emailhelper.host = this.AppConfig["sendmailhost"];//如果是QQ邮箱则：smtp:qq.com,依次类推
            //emailhelper.mailToArray = new string[] { email };//接收者邮件集合
            //emailhelper.mailCcArray = new string[] { };//抄送者邮件集合
            //try
            //{
            //    emailhelper.Send();
            //}
            //catch (Exception exp)
            //{
            //    OpenOperator.Error("发送错误邮件错误", exp);
            //}
        }

        public override void TestRun()
        {
            this.AppConfig = new XXF.BaseService.TaskManager.SystemRuntime.TaskAppConfigInfo();
            this.AppConfig.Add("sendmailhost", "smtp.qq.com");
            this.AppConfig.Add("sendmailname", "2060632377@qq.com");
            this.AppConfig.Add("password", "xx");

            base.TestRun();
        }
    }
    /// <summary>
    /// 任务调度平台之系统错误发送的临时数据信息
    /// 用于临时数据保存至数据库
    /// </summary>
    public class TaskManageErrorSendTaskTempInfo
    {
        public int LastLogID { get; set; }
    }
}
