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

namespace com.yrtech.SurveyAPI.Controllers
{
    public class AccountControl : ApiController
    {
        AccountService accountService = new AccountService();

        [HttpGet]
        [ActionName("Login")]
        public  Task<APIResult> Login(string accountId,string password)
        {
            return accountService.Login(accountId, password);
        }

    }
}
