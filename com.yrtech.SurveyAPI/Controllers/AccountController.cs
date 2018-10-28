using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO.Account;
using Purchase.DAL;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class AccountController : ApiController
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();

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
                    // 用户信息 UserInfo
                    // resultList.Add(accountService.GetUserInfo(account.UserId.ToString()));
                    // 品牌用户信息 UserInfoBrand
                    //resultList.Add(accountService.GetUserInfoBrand(account.UserId.ToString()));
                    // 体系类型 SubjectType
                    // resultList.Add(masterService.GetSubjectType());
                    // 试卷类型 SubjectTypeExam
                    //resultList.Add(masterService.GetSubjectTypeExam());
                    // 登录信息
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
                        projectList.AddRange(masterService.GetProject(tenantId, brand.BrandId.ToString(), ""));
                    }
                    resultList.Add(projectList);
                    // 经销商信息 Shop
                    //List<Shop> shopList = new List<Shop>();
                    //foreach (Brand brand in brandList)
                    //{
                    //    shopList.AddRange(masterService.GetShop(tenantId, brand.BrandId.ToString(),""));
                    //}
                    //resultList.Add(shopList);
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
