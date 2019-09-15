using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;
using com.yrtech.SurveyAPI.DTO.Account;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class AccountController : ApiController
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();

        /// <summary>
        /// 调研公司执行人员登陆使用
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/LoginForMobile")]
        public APIResult LoginForMobile(string accountId, string password)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<AccountDto> accountlist = accountService.Login(accountId, password);
                if (accountlist != null && accountlist.Count != 0)
                {
                    AccountDto account = accountlist[0];
                    string tenantId = account.TenantId.ToString();
                    #region 登录成功后查询品牌信息
                    resultList.Add(accountlist);
                    // 租户信息 Tenant
                    List<Tenant> tenantList = masterService.GetTenant(tenantId,"");
                    resultList.Add(tenantList);
                    // 品牌信息 Brand
                    List<Brand> brandList = new List<Brand>();
                    foreach (AccountDto ac in accountlist)
                    {
                        brandList.AddRange(masterService.GetBrand(tenantId, ac.Id.ToString(), ac.RoleType, ac.BrandId.ToString()));
                    }
                    resultList.Add(brandList);
                    // 期号信息 Project
                    List<Project> projectList = new List<Project>();
                    foreach (Brand brand in brandList)
                    {
                        projectList.AddRange(masterService.GetProject(tenantId, brand.BrandId.ToString(), "", ""));
                    }
                    resultList.Add(projectList);
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
                    #endregion
                }
                else
                {
                    return new APIResult() { Status = true, Body = "用户不存在或者密码不正确" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 调研公司网页端登陆
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/Login")]
        public APIResult Login(string accountId, string password)
        {
            try
            {
                List<AccountDto> accountlist = accountService.Login(accountId, password);
                if (accountlist != null && accountlist.Count != 0)
                {
                    accountlist = accountService.GetLoginInfo(accountId);
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(accountlist) };
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
        [HttpGet]
        [Route("Account/GetUserInfo")]
        public APIResult GetUserInfo(string tenantId, string userId)
        {
            try
            {
                List<UserInfo> userinfoList = accountService.GetUserInfo(tenantId, userId,"","");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userinfoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Account/SaveUserInfo")]
        public APIResult SaveUserInfo([FromBody]UserInfo obj)
        {
            try
            {
                accountService.SaveUserInfo(obj);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message };
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
                List<UserInfo> userList = accountService.GetUserInfo("",obj.UserId,"","");
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
        /// <summary>
        /// 厂商各权限登陆使用
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/LoginForBrand")]
        public APIResult LoginForBrand(string accountId, string password, string tenantId, string brandId)
        {
            try
            {
                //CommonHelper.log(tenantId + " "+brandId);
                //List<object> resultList = new List<object>();
                List<AccountDto> accountlist = accountService.LoginForBrand(accountId, password, tenantId, brandId);
                if (accountlist != null && accountlist.Count != 0)
                {

                    AccountDto account = accountlist[0];
                    string userId = account.Id.ToString();
                    //CommonHelper.log("b" +userId);
                    string roleType = account.RoleType;
                    account.ShopList = accountService.GetShopListByRole(brandId, userId, roleType);
                    //CommonHelper.log("a");
                    account.GroupList = accountService.GetGroupByRole(brandId, userId, roleType);
                    //CommonHelper.log("2");
                    account.SmallAreaList = accountService.GetSmallAreaByRole(brandId, userId, roleType);
                    //CommonHelper.log("3");
                    account.MiddleAreaList = accountService.GetMiddleAreaByRole(brandId, userId, roleType);
                    //CommonHelper.log("4");
                    account.BigAreaList = accountService.GetBigAreaByRole(brandId, userId, roleType);
                    //CommonHelper.log("5");
                    account.WideAreaList = accountService.GetWideAreaByRole(brandId, userId, roleType);
                    //CommonHelper.log("6");
                    account.BusinessAreaList = accountService.GetBussnessListByRole(brandId, userId, roleType);
                    //CommonHelper.log("7");
                    //resultList.Add(accountlist);
                    //CommonHelper.log("8");
                    //CommonHelper.log(CommonHelper.Encode(resultList).ToString());
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(accountlist) };
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
                    List<Tenant> tenantList_Name = masterService.GetTenant("", userInfoList[0].TenantName);
                    List<UserInfo> userInfo_TelNO = accountService.GetUserInfo("","", userInfoList[0].TelNO,""); // 注册时初始化登陆账号为手机号
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
                        tenant.TenantCode = userInfoList[0].TenantName; // Code没有什么特别的意义，所以用名字代替
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
                        accountService.SaveUserInfo(userInfo);
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
    }
}
