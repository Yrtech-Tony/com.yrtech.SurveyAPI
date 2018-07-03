using Survey.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO.Account;

namespace com.yrtech.SurveyAPI.Controllers
{
    public class AccountControl : ApiController
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();

        [HttpGet]
        [ActionName("Login")]
        public  APIResult Login(string accountId,string password)
        {
            List<object> resultList = new List<object>();
            List<AccountDto> accountlist =  accountService.Login(accountId, password);
            if (accountlist != null && accountlist.Count != 0)
            {
                List<SubjectType> subjectTypeList = masterService.GetSubjectType();
                List<SubjectTypeExam> subjectTypeExamList = masterService.GetSubjectTypeExam();
                resultList.Add(subjectTypeList);
                resultList.Add(subjectTypeExamList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            else
            {
                return new APIResult() { Status = true, Body = "" };
            }
        }

    }
}
