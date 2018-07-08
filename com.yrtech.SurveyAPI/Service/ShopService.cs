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
        public List<ProjectShop> GetProjectShop(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)};
            Type t = typeof(ProjectShop);
            string sql = "";
            sql = @"SELECT Id,A.ProjectId,A.ShopId,A.InUserId,A.InDateTime
                    FROM ProjectShop A 
                    WHERE ProjectId = @ProjectId 
                    ";
            List<ProjectShop> list = db.Database.SqlQuery(t, sql, para).Cast<ProjectShop>().ToList();
            return list;
        }

        /// <summary>
        /// 获取经销商试卷信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ShopSubjectTypeExam> GetShopSubjectTypeExam(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(ShopSubjectTypeExam);
            string sql = "";
            sql = @"SELECT Id,ProjectId,ShopId,ShopSubjectTypeExamId,InUserId,InDateTime,ModifyUserId,ModifyDateTime FROM ShopSubjectTypeExam WHERE ProjectId = @ProjectId";
            List<ShopSubjectTypeExam> list = db.Database.SqlQuery(t, sql, para).Cast<ShopSubjectTypeExam>().ToList();
            return list;
        }
    }
}