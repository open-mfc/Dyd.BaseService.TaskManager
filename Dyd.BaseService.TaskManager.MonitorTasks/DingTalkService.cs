using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static DingTalk.Api.Request.OapiMessageCorpconversationAsyncsendV2Request;
using static DingTalk.Api.Request.OapiWorkrecordAddRequest;

namespace Dyd.BaseService.TaskManager.MonitorTasks
{
    public class DingTalkService
    {
        const string m_host = "https://oapi.dingtalk.com";
        //const string m_agentId = "259375402";
        const long m_agentId = 259375402;
        private readonly string appKey;
        private readonly string appsecret;
        static string access_token;
        static DateTime get_token_time;
        public DingTalkService(string appKey, string appsecret)
        {
            this.appKey = appKey;
            this.appsecret = appsecret;

            if (string.IsNullOrWhiteSpace(access_token) || get_token_time == null || get_token_time.AddHours(1) < DateTime.Now)
            {
                GetToken();
            }
        }
        public void GetToken()
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient($"{m_host}/gettoken");
            OapiGettokenRequest request = new OapiGettokenRequest();
            request.Appkey = appKey;
            request.Appsecret = appsecret;
            request.SetHttpMethod("GET");
            OapiGettokenResponse response = client.Execute(request);

            if (response.Errcode == 0)
            {
                access_token = response.AccessToken;
                //LogService.AppendDebugLog(typeof(DingTalkService), $"获得最新token:{access_token}");
            }
            else
            {
                access_token = string.Empty;
                //LogService.AppendDebugLog(typeof(DingTalkService), response.ToString());
            }
            get_token_time = DateTime.Now;
        }
        public string GetUserInfo(string code)
        {
            try
            {
                var _url = $"{m_host}/user/getuserinfo";
                var client = new DefaultDingTalkClient(_url);
                OapiUserGetuserinfoRequest request = new OapiUserGetuserinfoRequest();
                request.Code = code;
                request.SetHttpMethod("GET");
                OapiUserGetuserinfoResponse response = client.Execute(request, access_token);
                if (response.Errcode != 0)
                {
                    // this.AppendDebugLog($"请求地址[{_url}]，参数code=[{code}],返回{response.ToJson()}");
                }
                return response.Userid;
            }
            catch (Exception ex)
            {
                //this.AppendErrorLog("请求服务器异常", ex);
            }
            return null;
        }

        #region SDK发送工作通知消息
        public JObject SendText(string userid, string content)
        {
            var msgContent = new OapiMessageCorpconversationAsyncsendV2Request.TextDomain
            {
                Content = content
            };
            var msg = new OapiMessageCorpconversationAsyncsendV2Request.MsgDomain
            {
                Msgtype = "text",
                Text = msgContent
            };
            return SendWorkMsg(userid, msg);
        }
        public JObject SendLink(string userid, string messageUrl, string picUrl, string title, string text)
        {
            var msgContent = new OapiMessageCorpconversationAsyncsendV2Request.LinkDomain()
            {
                MessageUrl = messageUrl,
                PicUrl = picUrl,
                Title = title,
                Text = text
            };
            var msg = new OapiMessageCorpconversationAsyncsendV2Request.MsgDomain
            {
                Msgtype = "link",
                Link = msgContent
            };
            return SendWorkMsg(userid, msg);
        }
        /// <summary>
        /// 发送工作消息公共函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userid">多用户逗号分隔</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public JObject SendWorkMsg(string userid, MsgDomain msg)
        {
            try
            {
                var url = $"{m_host}/topapi/message/corpconversation/asyncsend_v2";
                var client = new DefaultDingTalkClient(url);
                var request = new OapiMessageCorpconversationAsyncsendV2Request();
                request.AgentId = m_agentId;
                request.UseridList = userid;
                request.Msg_ = msg;
                var response = client.Execute(request, access_token);
                var _json = JObject.Parse(response.Body);
                //if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
                //{
                //    //LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
                //}
                return _json;
            }
            catch (Exception ex)
            {
                //this.AppendErrorLog(ex.Message, ex);
            }
            return null;
        }
        public JObject SendWorkMsgStatus(long taskId)
        {
            var url = $"{m_host}/topapi/message/corpconversation/getsendresult";
            var client = new DefaultDingTalkClient(url);
            var request = new OapiMessageCorpconversationGetsendprogressRequest();
            request.AgentId = m_agentId;
            request.TaskId = taskId;
            var response = client.Execute(request, access_token);
            var _json = JObject.Parse(response.Body);
            if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
            {
                // LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
            }
            return _json;
        }
        #endregion

        #region SDK待办事项
        public JObject SendWorkRecord(string userid, long time, string title, string msgUrl, string Ltitle, string Lcontent)
        {
            var url = $"{m_host}/topapi/workrecord/add";
            var client = new DefaultDingTalkClient(url);
            var request = new OapiWorkrecordAddRequest();
            request.Userid = userid;
            request.CreateTime = time;
            request.Title = title;
            request.Url = msgUrl;
            var _list = new List<FormItemVoDomain>() { new FormItemVoDomain { Title = Ltitle, Content = Lcontent } };
            request.FormItemList_ = _list;
            var response = client.Execute(request, access_token);
            var _json = JObject.Parse(response.Body);
            if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
            {
                //LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
            }
            return _json;
        }
        public JObject GetWorkRecord(string userid, int status, int offset, int limit)
        {
            var url = $"{m_host}/topapi/workrecord/getbyuserid";
            var client = new DefaultDingTalkClient(url);
            var request = new OapiWorkrecordGetbyuseridRequest();
            request.Userid = userid;
            request.Status = status;
            request.Offset = offset;
            request.Limit = limit;
            var response = client.Execute(request, access_token);
            var _json = JObject.Parse(response.Body);
            if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
            {
                //LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
            }
            return _json;
        }
        public JObject UpdateWorkRecord(string userid, string record_id)
        {
            var url = $"{m_host}/topapi/workrecord/update";
            var client = new DefaultDingTalkClient(url);
            var request = new OapiWorkrecordUpdateRequest();
            request.Userid = userid;
            request.RecordId = record_id;
            var response = client.Execute(request, access_token);
            var _json = JObject.Parse(response.Body);
            if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
            {
                //LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
            }
            return _json;
        }
        #endregion

        #region 部门操作
        /// <summary>
        /// 获得部门id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JObject DepartmentIds(string id)
        {
            var url = $"{m_host}/department/list_ids";
            var client = new DefaultDingTalkClient(url);
            var request = new OapiDepartmentListIdsRequest();
            request.Id = id;
            request.SetHttpMethod("GET");
            var response = client.Execute(request, access_token);
            var _json = JObject.Parse(response.Body);
            if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
            {
                //LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
            }
            return _json;
        }
        /// <summary>
        /// 获得部门用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JObject Simplelist(long id)
        {
            var url = $"{m_host}/user/simplelist";
            var client = new DefaultDingTalkClient(url);
            var request = new OapiUserSimplelistRequest();
            request.DepartmentId = id;
            request.Offset = 0;
            request.Size = 100;
            request.SetHttpMethod("GET");
            var response = client.Execute(request, access_token);
            var _json = JObject.Parse(response.Body);
            if (Convert.ToInt32(_json["errcode"].ToString()) != 0)
            {
                //LogService.AppendDebugLog(typeof(DingTalkService), $"请求地址[{url}]返回{_json.ToString()}");
            }
            return _json;
        }
        #endregion

    }
}
