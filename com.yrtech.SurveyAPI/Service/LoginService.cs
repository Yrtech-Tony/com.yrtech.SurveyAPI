using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using Infragistics.Documents.Excel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using System.Web.Hosting;
using System.Collections;
using System.Net.Http;

namespace com.yrtech.SurveyAPI.Service
{
    public class LoginService
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();
        //获取小程序的Token
        public string GetWXToken()
        {
            try
            {
                string token = "";
                WxToken wt = new WxToken();
                wt = GetAppIdAndSecret("轻智巡");
                // 从数据库获取Token
                List<AppInfo> appInfoList = accountService.GetAppInfo(wt.AppId);
                if (appInfoList == null || appInfoList.Count == 0)
                {
                    token = AppInfoSave();
                }
                else
                {
                    TimeSpan ts = DateTime.Now - Convert.ToDateTime(appInfoList[0].ModifyDateTime);
                    double second = ts.TotalSeconds;
                    // 如未超时，直接使用数据库token，已超时重新获取
                    if (second < 7000)
                    {
                        token = appInfoList[0].Token;
                    }
                    else
                    {
                        token = AppInfoSave();
                    }
                }
                return token;
                //return new APIResult() { Status = true, Body = CommonHelper.Encode(token) };
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
                //return new APIResult() { Status = false, Body = "绑定失败！ " + ex.Message };
            }
        }
        // Token 保存
        public string AppInfoSave()
        {
            string token = "";
            WxToken wt = new WxToken();
            wt = GetAppIdAndSecret("轻智巡");
            HttpClient client = new HttpClient();
            Uri uri = new Uri("https://api.weixin.qq.com/");
            client.BaseAddress = uri;
            //添加请求的头文件
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string getUserApi = string.Format("cgi-bin/token?grant_type={0}&appid={1}&secret={2}", "client_credential", wt.AppId, wt.AppSecret);
            HttpResponseMessage message = client.GetAsync(getUserApi).Result;
            string json = message.Content.ReadAsStringAsync().Result;
            WxToken wxToken = CommonHelper.DecodeString<WxToken>(json);
            if (wxToken != null)
            {
                AppInfo appInfo = new AppInfo();
                appInfo.AppId = wt.AppId;
                appInfo.Token = wxToken.access_token;
                accountService.AppInfoSave(appInfo);
                token = wxToken.access_token;
            }
            return token;
        }
        // 获取密钥
        public WxToken GetAppIdAndSecret(string groupName)
        {
            // 获取wx appId和secret
            WxToken wt = new WxToken();
            List<HiddenColumn> appInfoList_Id = masterService.GetHiddenCode(groupName, "AppId");
            if (appInfoList_Id != null && appInfoList_Id.Count > 0)
            {
                wt.AppId = appInfoList_Id[0].HiddenName;
            }
            List<HiddenColumn> appInfoList_secret = masterService.GetHiddenCode(groupName, "AppSecret");
            if (appInfoList_secret != null && appInfoList_secret.Count > 0)
            {
                wt.AppSecret = appInfoList_secret[0].HiddenName;
            }
            return wt;
        }
    }
}