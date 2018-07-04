using com.yrtech.SurveyAPI.Common;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace com.yrtech.SurveyAPI.Service
{
    public class ShopService
    {
        Entities db = new Entities();
        
        /// <summary>
        /// 获取当前期下的经销商
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<APIResult> GetProjectShop(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)};
            Type t = typeof(ProjectShop);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,A.InUserId,A.InDateTime
                    FROM ProjectShop A 
                    WHERE ProjectId = @ProjectId 
                    ";
            List<ProjectShop> list = db.Database.SqlQuery(t, sql, para).Cast<ProjectShop>().ToList();
            return new APIResult() { Status = true, Body = CommonHelper.EncodeDto<ProjectShop>(list) };
        }
    }
}