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

                    #region 登录成功后下载基础数据到Mobile本地
                    // 用户信息
                    resultList.Add(accountService.GetUserInfo(account.UserId.ToString()));
                    // 品牌用户信息
                    resultList.Add(accountService.GetUserInfoBrand(account.UserId.ToString()));
                    // 体系类型
                    resultList.Add(masterService.GetSubjectType());
                    // 试卷类型
                    resultList.Add(masterService.GetSubjectTypeExam());
                    // 租户信息
                    resultList.Add(masterService.GetTenant(tenantId));
                    // 品牌信息
                    List<Brand> brandList = new List<Brand>();
                    foreach (AccountDto ac in accountlist)
                    {
                        brandList.AddRange(masterService.GetBrand(tenantId, ac.UserId.ToString(), ac.BrandId.ToString()));
                    }
                    resultList.Add(brandList);
                    // 期号信息
                    List<Project> projectList = new List<Project>();
                    foreach (AccountDto ac in accountlist)
                    {
                        projectList.AddRange(masterService.GetProject(tenantId, ac.BrandId.ToString(), ""));
                    }
                    resultList.Add(projectList);
                    // 经销商信息
                    List<Shop> shopList = new List<Shop>();
                    foreach (AccountDto ac in accountlist)
                    {
                        shopList.AddRange(masterService.GetShop(tenantId, ac.BrandId.ToString(), ""));
                    }
                    resultList.Add(shopList);
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
                    #endregion
                }
                else
                {
                    return new APIResult() { Status = true, Body = "" };
                }
            }
            catch (Exception ex)
            {

                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

    }
}
