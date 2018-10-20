using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Models;
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
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(ProjectShop);
            string sql = "";
            sql = @"SELECT Id,A.ProjectId,A.ShopId,A.InUserId,A.InDateTime
                    FROM ProjectShop A 
                    WHERE ProjectId = @ProjectId 
                    ";
            List<ProjectShop> list = db.Database.SqlQuery(t, sql, para).Cast<ProjectShop>().ToList();
            return list;
        }

        public List<ShopVM> GetShopByProjectId(string projectId, string tenantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                new SqlParameter("@TenantId", tenantId) };
            string sql = "SELECT A.ShopId,ShopCode,ShopName,Province,City FROM ProjectShop A" +
                        " INNER JOIN Shop B ON A.ShopId = B.ShopId AND TenantId=@TenantId" +
                        " WHERE A.ProjectId =@ProjectId";
            List<ShopVM> list = db.Database.SqlQuery<ShopVM>(sql, para).ToList();
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

        public List<ShopSubjectTypeExamVM> GetExamType(string projectId, string shopId)
        {
            SqlParameter[] para = new SqlParameter[] { 
                new SqlParameter("@ProjectId", projectId),
                new SqlParameter("@ShopId", shopId) };
            string sql = "";
            sql = "SELECT A.ShopSubjectTypeExamId,B.SubjectTypeExamName,A.ShopId,A.ProjectId FROM ShopSubjectTypeExam A " +
                " INNER JOIN SubjectTypeExam B ON ShopSubjectTypeExamId = B.SubjectTypeExamId" +
                " WHERE A.ProjectId =@ProjectId AND A.ShopId =@ShopId ";
            List<ShopSubjectTypeExamVM> list = db.Database.SqlQuery<ShopSubjectTypeExamVM>(sql, para).ToList();
            return list;
        }
    }
}