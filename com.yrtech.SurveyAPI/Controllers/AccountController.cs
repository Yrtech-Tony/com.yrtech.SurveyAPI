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
                    List<Tenant> tenantList = masterService.GetTenant(tenantId);
                    resultList.Add(tenantList);
                    // 品牌信息 Brand
                    List<Brand> brandList = new List<Brand>();
                    foreach (AccountDto ac in accountlist)
                    {
                        brandList.AddRange(masterService.GetBrand(tenantId, ac.UserId.ToString(), ac.BrandId.ToString()));
                    }
                    resultList.Add(brandList);
                    // 期号信息 Project
                    List<Project> projectList = new List<Project>();
                    foreach (Brand brand in brandList)
                    {
                        projectList.AddRange(masterService.GetProject(tenantId, brand.BrandId.ToString(), "",""));
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
                List<UserInfo> userList = accountService.GetUserInfo(obj.UserId);
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

                return new APIResult() { Status = true, Body = "原密码不正确" };
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
        public APIResult LoginForBrand(string accountId, string password)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<AccountDto> accountlist = accountService.Login(accountId, password);
                if (accountlist != null && accountlist.Count != 0)
                {
                    AccountDto account = accountlist[0];
                    string tenantId = account.TenantId.ToString();
                    string brandId = account.BrandId.ToString();
                    string userId = account.UserId.ToString();
                    string roleType = account.RoleType;
                    account.ShopList = accountService.GetShopListByRole(brandId, userId, roleType);
                    account.GroupList = accountService.GetGroupByRole(brandId, userId, roleType);
                    account.SmallAreaList = accountService.GetSmallAreaByRole(brandId, userId, roleType);
                    account.MiddleAreaList = accountService.GetMiddleAreaByRole(brandId, userId, roleType);
                    account.BigAreaList = accountService.GetBigAreaByRole(brandId, userId, roleType);
                    account.WideAreaList = accountService.GetWideAreaByRole(brandId, userId, roleType);
                    account.BusinessAreaList = accountService.GetBussnessListByRole(brandId, userId, roleType);
                    resultList.Add(accountlist);
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
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
    }
}
