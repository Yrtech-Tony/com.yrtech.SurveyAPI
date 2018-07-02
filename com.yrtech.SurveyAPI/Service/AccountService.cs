using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO.Account;
using Survey.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace com.yrtech.SurveyAPI.Service
{
    public class AccountService
    {
        Entities db = new Entities();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<APIResult> Login(string accountId, string password)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AccountId", accountId),
                                                       new SqlParameter("@Password",password)};
            Type t = typeof(AccountDto);
            string sql = @"SELECT A.TenantId,C.BrandId,B.TenantCode,B.TenantName,D.BrandName,AccountId,AccountName,[Password],ISNULL(UseChk,0) AS UseChk 
                            FROM UserInfo A INNER JOIN Tenant B ON A.TenantId = B.TenantId
                                            INNER JOIN UserInfoBrand C ON A.Id = C.UserId
                                            INNER JOIN Brand D ON C.BrandId = D.BrandId AND B.TenantId = D.TenantId
                            WHERE AccountId = @AccountId AND[Password] = @Password
                            AND UseChk = 1";
            List<AccountDto> list = db.Database.SqlQuery(t, sql, para).Cast<AccountDto>().ToList();
            return new APIResult() { Status = true, Body = CommonHelper.EncodeDto<AccountDto>(list) };
        }
    }
}