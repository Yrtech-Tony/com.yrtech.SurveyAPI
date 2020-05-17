using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using com.yrtech.SurveyAPI.DTO.Account;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class AccountController : ApiController
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();

        #region 登陆
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Account/Login")]
        public APIResult Login(string accountId, string password, string tenantCode)
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
                    account.TenantList = tenantList;
                    account.BrandList = accountService.GetBrandByRole(tenantId, account.Id.ToString(), account.RoleType.ToString());
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

                return new APIResult() { Status = true, Body = CommonHelper.Encode(account) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        #endregion
       
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
                List<UserInfo> userList = masterService.GetUserInfo("","", obj.UserId, "", "","","","");
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
                    List<Tenant> tenantList_Name = masterService.GetTenant("", userInfoList[0].TenantCode,"");
                    List<UserInfo> userInfo_TelNO = masterService.GetUserInfo("", "","", userInfoList[0].TelNO, "","","",""); // 注册时初始化登陆账号为手机号
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
    }
}
