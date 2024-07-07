using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using com.yrtech.SurveyAPI.DTO.Account;
using System.Net.Http;
using System.Text;
//using System.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
using System.Web;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class AccountController : ApiController
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();
        #region 登陆和修改密码
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <param name="tenantCode"></param>
        /// <param name="platformType">A:管理平台 B：品牌使用平台</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/Login")]
        public APIResult Login(string accountId, string password, string tenantCode, string platformType)
        {
            try
            {
                //OSSClientHelper.AlibabaCloudSendSms();
                //var request = HttpContext.Current.Request;
                //var header = request.Headers[]
                // 获取租户信息
                string tenantId = "";
                List<Tenant> tenantList = masterService.GetTenant("", tenantCode, "");
                if (tenantList == null || tenantList.Count == 0)
                {
                    return new APIResult() { Status = false, Body = "租户代码不存在" };
                }
                tenantId = tenantList[0].TenantId.ToString();

                List<AccountDto> accountlist = accountService.Login(accountId, password, tenantId);
                if (accountlist != null && accountlist.Count != 0)
                {
                    AccountDto account = accountlist[0];
                    if (!platformTypeCheck(platformType, account.RoleType))
                    {
                        return new APIResult() { Status = false, Body = "该用户无此平台权限" };
                    }
                    account.TenantList = tenantList;
                    account.BrandList = accountService.GetBrandByRole(tenantId, account.Id.ToString(), account.RoleType.ToString());
                    account.OSSInfo = masterService.GetHiddenCode("OSS信息", "");
                    account.RoleProgramList = masterService.GetRoleProgram_Tree(tenantId, account.RoleType);
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(account) };
                  
                }
                else
                {
                    return new APIResult() { Status = false, Body = "用户不存在或者密码不正确" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #region 不使用


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="openId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Account/LoginByOpenId")]
        //public APIResult LoginByOpenId(string openId, string platformType)
        //{
        //    try
        //    {
        //        List<AccountDto> accountlistOpenId = accountService.LoginByOpenId(openId);
        //        if (accountlistOpenId == null || accountlistOpenId.Count == 0)
        //        {
        //            return new APIResult() { Status = false, Body = "未绑定账号" };
        //        }
        //        if (accountlistOpenId != null && accountlistOpenId.Count != 0)
        //        {
        //            List<AccountDto> accountlist = accountService.Login(accountlistOpenId[0].AccountId, accountlistOpenId[0].Password, accountlistOpenId[0].TenantId.ToString());
        //            if (accountlist != null && accountlist.Count != 0)
        //            {
        //                AccountDto account = accountlist[0];
        //                if (!platformTypeCheck(platformType, account.RoleType))
        //                {
        //                    return new APIResult() { Status = false, Body = "该用户无此平台权限" };
        //                }
        //                List<Tenant> tenantList = masterService.GetTenant(accountlistOpenId[0].TenantId.ToString(), "", "");
        //                account.TenantList = tenantList;
        //                account.BrandList = accountService.GetBrandByRole(accountlistOpenId[0].TenantId.ToString(), account.Id.ToString(), account.RoleType.ToString());
        //                account.OSSInfo = masterService.GetHiddenCode("OSS信息", "");
        //                account.RoleProgramList = masterService.GetRoleProgram_Tree(accountlistOpenId[0].TenantId.ToString(), account.RoleType);
        //                // 自检时只会有一个品牌
        //                if (account.RoleType == "B_Shop")
        //                {
        //                    account.ShopList = accountService.GetShopListByRole(accountlistOpenId[0].BrandId.ToString(), accountlistOpenId[0].Id.ToString(), accountlistOpenId[0].RoleType);
        //                }
        //                return new APIResult() { Status = true, Body = CommonHelper.Encode(account) };
        //            }
        //            else
        //            {
        //                return new APIResult() { Status = false, Body = "绑定账号信息有误" };
        //            }
        //        }
        //        else
        //        {
        //            return new APIResult() { Status = false, Body = "绑定账号信息有误" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        #endregion
        /// <summary>
        /// 厂商登陆返回的基本信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/LoginInfoForBrand")]
        public APIResult LoginInfoForBrand(string brandId, string userId, string roleType)
        {
            try
            {
                AccountDto account = new AccountDto();
                account.ShopList = accountService.GetShopListByRole(brandId, userId, roleType);
                account.GroupList = accountService.GetGroupByRole(brandId, userId, roleType);
                account.SmallAreaList = accountService.GetSmallAreaByRole(brandId, userId, roleType);
                account.MiddleAreaList = accountService.GetMiddleAreaByRole(brandId, userId, roleType);
                account.BigAreaList = accountService.GetBigAreaByRole(brandId, userId, roleType);
                account.WideAreaList = accountService.GetWideAreaByRole(brandId, userId, roleType);
                account.BussinessAreaList = accountService.GetBussinessListByRole(brandId, userId, roleType);
                if (roleType == "B_Shop" && (account.ShopList == null || account.ShopList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无经销商信息" };
                }
                if (roleType == "B_Group" && (account.GroupList == null || account.GroupList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无集团信息" };
                }
                if (roleType == "B_SmallArea" && (account.SmallAreaList == null || account.SmallAreaList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无小区信息" };
                }
                if (roleType == "B_MiddleArea" && (account.MiddleAreaList == null || account.MiddleAreaList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无中区信息" };
                }
                if (roleType == "B_BigArea" && (account.BigAreaList == null || account.BigAreaList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无大区信息" };
                }
                if (roleType == "B_WideArea" && (account.WideAreaList == null || account.WideAreaList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无广域区域信息" };
                }
                if (roleType == "B_Bussiness" && (account.BussinessAreaList == null || account.BussinessAreaList.Count == 0))
                {
                    return new APIResult() { Status = false, Body = "此用户无业务类型信息" };
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(account) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 刷新品牌信息时使用
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/LoginBrandInfo")]
        public APIResult LoginBrandInfo(string tenantId, string userId, string roleType)
        {
            try
            {
                List<Brand> brandList = accountService.GetBrandByRole(tenantId, userId, roleType);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(brandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/ChangePassword")]
        public APIResult ChangePassword([FromBody]ChangePasswordDto obj)
        {
            try
            {
                List<UserInfo> userList = masterService.GetUserInfo("", "", obj.UserId, "", "", "", "", "", null, "");
                if (userList != null && userList.Count > 0)
                {
                    if (userList[0].Password != obj.sOldPassword)
                    {//原密码不正确
                        return new APIResult() { Status = false, Body = "原密码不正确" };
                    }
                    accountService.UpdatePassword(obj.UserId, obj.sNewPassword);
                }
                else
                {
                    return new APIResult() { Status = false, Body = "用户不存在" };
                }

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = "密码修改失败！ " + ex.Message };
            }
        }
        #endregion
        #region 小程序
        // 小程序登陆
        [HttpGet]
        [Route("Account/LoginForWechat")]
        public APIResult LoginForWechat(string accountId, string password, string tenantCode, string platformType, string openId, string telNO)
        {
            try
            {
                // 获取租户信息
                string tenantId = "";
                List<Tenant> tenantList = masterService.GetTenant("", tenantCode, "");
                if (tenantList == null || tenantList.Count == 0)
                {
                    return new APIResult() { Status = false, Body = "租户代码不存在" };
                }
                tenantId = tenantList[0].TenantId.ToString();
                List<AccountDto> accountlist = accountService.Login(accountId, password, tenantId);
                if (accountlist != null && accountlist.Count != 0)
                {
                    AccountDto account = accountlist[0];
                    if (!platformTypeCheck(platformType, account.RoleType))
                    {
                        return new APIResult() { Status = false, Body = "该用户无此平台权限" };
                    }
                    // 保存小程序和UserId的关系
                    UserInfoOpenId userInfoOpenId = new UserInfoOpenId();
                    userInfoOpenId.UserId = account.Id;
                    userInfoOpenId.OpenId = openId;
                    userInfoOpenId.TelNO = telNO;
                    accountService.UserIdOpenIdSave(userInfoOpenId);
                    // 返回值信息的赋值
                    account.OpenId = openId;
                    account.TenantList = tenantList;
                    account.BrandList = accountService.GetBrandByRole(tenantId, account.Id.ToString(), account.RoleType.ToString());
                    List<HiddenColumn> ossInfoList = masterService.GetHiddenCode("OSS信息", "");
                    account.OSSInfo = ossInfoList;
                    string endPoint = "";
                    string bucket = "";
                    foreach (HiddenColumn hiddenColumn in ossInfoList)
                    {
                        if (hiddenColumn.HiddenCode == "EndPoint")
                        {
                            endPoint = hiddenColumn.HiddenName;
                        }
                        if (hiddenColumn.HiddenCode == "Bucket")
                        {
                            bucket = hiddenColumn.HiddenName;
                        }
                    }
                    account.OSSBaseUrl = endPoint.Insert(8, bucket + ".");
                    account.RoleProgramList = masterService.GetRoleProgram_Tree(tenantId, account.RoleType);
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(account) };
                }
                else
                {
                    return new APIResult() { Status = false, Body = "用户不存在或者密码不正确" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = "绑定失败！ " + ex.Message };
            }
        }
        // 获取OpenId
        [HttpGet]
        [Route("Account/GetWXOpenId")]
        public APIResult GetWXOpenId(string jsCode)
        {
            try
            {
                List<UserInfoOpenId> userInfoOpenIdList = new List<UserInfoOpenId>();
                WxToken wt = new WxToken();
                wt = GetAppIdAndSecret("轻智巡");
                HttpClient client = new HttpClient();
                Uri uri = new Uri("https://api.weixin.qq.com/");
                client.BaseAddress = uri;
                //添加请求的头文件
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                string getUserApi = string.Format("sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type={3}", wt.AppId, wt.AppSecret, jsCode, "authorization_code");
                HttpResponseMessage message = client.GetAsync(getUserApi).Result;
                string json = message.Content.ReadAsStringAsync().Result;
                WxToken wxToken = CommonHelper.DecodeString<WxToken>(json);
                if (wxToken != null)
                {
                    // 验证openId在DB是否有保存过，如保存过返回OpenId信息
                    userInfoOpenIdList = accountService.GetUserIdOpenId("", wxToken.openid);
                }
                // 如未保存过，把当前获取的OpenId返回
                if (userInfoOpenIdList.Count == 0)
                {
                    UserInfoOpenId userInfoOpenId = new UserInfoOpenId();
                    userInfoOpenId.OpenId = wxToken.openid;
                    userInfoOpenIdList.Add(userInfoOpenId);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoOpenIdList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message };
            }
        }
        // 获取电话
        [HttpGet]
        [Route("Account/GetWXTeNO")]
        public APIResult GetWXTeNO(string code)
        {
            try
            {
                string token = GetWXToken();
                HttpClient client = new HttpClient();
                string url = "https://api.weixin.qq.com/wxa/business/getuserphonenumber?access_token=" + token;
                //Uri uri = new Uri("https://api.weixin.qq.com/");
                // client.BaseAddress = uri;
                //添加请求的头文件
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //string getUserApi = string.Format("wxa/business/getuserphonenumber", wt.AppId, wt.AppSecret, jsCode, "authorization_code");
                var par = new { code = code };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(par), Encoding.UTF8, "application/json");
                HttpResponseMessage message = client.PostAsync(url, content).Result;
                string json = message.Content.ReadAsStringAsync().Result;
                WxToken wxToken = CommonHelper.DecodeString<WxToken>(json);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(wxToken) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message };
            }
        }
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
        #endregion
        #region 公用
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
        // 平台验证
        public bool platformTypeCheck(string platform, string roleTypeCode)
        {
            bool platformCheck = false;
            if (platform == "A" && roleTypeCode == "B_Shop") // 登录时，允许经销商账号使用APP
            {
                platformCheck = true;
            }
            List<RoleType> roleTypeList = masterService.GetRoleType(platform, "", "");// 通过平台类型获取对应的权限
            foreach (RoleType roleType in roleTypeList)
            {
                if (roleType.RoleTypeCode == roleTypeCode)
                {
                    platformCheck = true;
                    break;
                }
            }
            return platformCheck;
        }
        #endregion
        #region 租户注册
        /// <summary>
        /// 注册时需要填写的信息
        /// 必填:租户名称，手机号，账号名称，密码
        /// 非必填:邮箱
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/RegisterUserInfo")]
        public APIResult RegisterUserInfo(UploadData uploadData)
        {
            try
            {
                List<UserInfoDto> userInfoList = CommonHelper.DecodeString<List<UserInfoDto>>(uploadData.AnswerListJson);
                if (userInfoList != null && userInfoList.Count > 0)
                {
                    List<Tenant> tenantList_Name = masterService.GetTenant("", userInfoList[0].TenantCode, "");
                    List<UserInfo> userInfo_TelNO = masterService.GetUserInfo("", "", "", userInfoList[0].TelNO, "", "", "", "", null, ""); // 注册时初始化登陆账号为手机号
                    if (tenantList_Name != null && tenantList_Name.Count > 0 && tenantList_Name[0].TenantId != userInfoList[0].TenantId)
                    {
                        return new APIResult() { Status = false, Body = "该租户名称已存在,请更换其他租户名称" };
                    }
                    else if (userInfo_TelNO != null && userInfo_TelNO.Count > 0 && userInfo_TelNO[0].Id != userInfoList[0].Id)
                    {
                        return new APIResult() { Status = false, Body = "该手机号已经注册，请更换其他手机号" };
                    }
                    else
                    {
                        Tenant tenant = new Tenant();
                        tenant.TenantName = userInfoList[0].TenantName;
                        tenant.TenantCode = userInfoList[0].TenantCode;
                        masterService.SaveTenant(tenant);
                        UserInfo userInfo = new UserInfo();
                        userInfo.AccountId = userInfoList[0].TelNO; //注册时初始化登陆账号为手机号
                        userInfo.AccountName = userInfoList[0].AccountName;
                        userInfo.Email = userInfoList[0].Email;
                        userInfo.Password = userInfoList[0].Password;
                        userInfo.RoleType = "S_Sysadmin";
                        userInfo.UserType = "租户管理员";
                        userInfo.TenantId = tenantList_Name[0].TenantId;
                        userInfo.UseChk = true;
                        masterService.SaveUserInfo(userInfo);
                    }

                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message };
            }
        }
        #endregion
        #region GTMC接口
        public string CreateGTMCToken()
        {
            Guid guid = Guid.NewGuid();
            string tokenStr = guid.ToString("N");
            return tokenStr;
        }
        [HttpPost]
        [Route("Account/GetGTMCToken")]
        public string GetGTMCToken([FromBody]WxToken wxToken)
        {
            WxToken wx = new WxToken();
            try
            {
                string token = "";
                wx= GetAppIdAndSecret("GTMC");
                wx.token_type = "gtmc";
                wx.scope = "read";
                if (wxToken.client_id == wx.AppId && wxToken.client_secret == wx.AppSecret)
                {
                    // 从数据库获取Token
                    List<AppInfo> appInfoList = accountService.GetAppInfo(wxToken.client_id);
                    if (appInfoList == null || appInfoList.Count == 0)
                    {
                        token = CreateGTMCToken();
                        if (!string.IsNullOrEmpty(token))
                        {
                            AppInfo appInfo = new AppInfo();
                            appInfo.AppId = wxToken.client_id;
                            appInfo.Token = token;
                            accountService.AppInfoSave(appInfo);
                        }
                        wx.access_token = token;
                        wx.expires_in = 7200;
                        wx.errcode = "0";
                        wx.errmsg = "";
                        wx.AppId = null;
                        wx.AppSecret = null;
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - Convert.ToDateTime(appInfoList[0].ModifyDateTime);
                        double second = ts.TotalSeconds;
                        // 如未超时，直接使用数据库token，已超时重新获取
                        if (second < 7000)
                        {
                            token = appInfoList[0].Token;
                            wx.access_token = token;
                            wx.expires_in = Convert.ToInt32(7200-second);
                            wx.errcode = "0";
                            wx.errmsg = "";
                            wx.AppId = null;
                            wx.AppSecret = null;
                        }
                        else
                        {
                            token = CreateGTMCToken();
                            if (!string.IsNullOrEmpty(token))
                            {
                                AppInfo appInfo = new AppInfo();
                                appInfo.AppId = wxToken.client_id;
                                appInfo.Token = token;
                                accountService.AppInfoSave(appInfo);
                            }
                            wx.access_token = token;
                            wx.expires_in = 7200;
                            wx.errcode = "0";
                            wx.errmsg = "";
                            wx.AppId = null;
                            wx.AppSecret = null;
                        }
                    }
                }
                else
                {
                    wx.access_token = "";
                    wx.expires_in = 0;
                    wx.errcode = "400691";
                    wx.errmsg = "账号或密码不正确";
                    wx.AppId = null;
                    wx.AppSecret = null;
                }
                return  CommonHelper.Encode(wx) ;
            }
            catch (Exception)
            {
                wx.access_token = "";
                wx.expires_in = 0;
                wx.errcode = "-1";
                wx.errmsg = "系统问题请联系开发者";
                wx.AppId = null;
                wx.AppSecret = null;
                return  CommonHelper.Encode(wx);
            };
        }
        #endregion
    }
    [Serializable]
    public class WxToken
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string openid { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string grant_type { get; set;}
        public WxTelNO phone_info { get; set; }

    }
    public class WxTelNO
    {
        public string phoneNumber { get; set; }
        public string purePhoneNumber { get; set; }
        public string countryCode { get; set; }
    }
}
