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
    public class MasterService
    {
        Entities db = new Entities();
        /// <summary>
        /// 获取复审类型
        /// </summary>
        /// <returns></returns>
        public List<SubjectRecheckType> GetSubjectRecheckType()
        {
            Type t = typeof(SubjectRecheckType);
            string sql = "";

            sql = @"SELECT [RecheckTypeId]
                  ,[RecheckTypeName]
                  ,[InUserId]
                  ,[InDateTime]
                  ,[ModifyUserId]
                  ,[ModifyDateTime]
              FROM [SubjectRecheckType] ";
            return db.Database.SqlQuery(t, sql, null).Cast<SubjectRecheckType>().ToList();
        }
        /// <summary>
        /// 获取体系类型
        /// </summary>
        /// <returns></returns>
        public List<SubjectType> GetSubjectType()
        {
            Type t = typeof(SubjectType);
            string sql = "";

            sql = @"SELECT [SubjectTypeId]
                          ,[SubjectTypeName]
                          ,[InUserId]
                          ,[InDateTime]
                   FROM [SubjectType] ";
            return db.Database.SqlQuery(t, sql, null).Cast<SubjectType>().ToList();
            
        }
        /// <summary>
        /// 获取体系试卷类型
        /// </summary>
        /// <returns></returns>
        public List<SubjectTypeExam> GetSubjectTypeExam()
        {
            Type t = typeof(SubjectTypeExam);
            string sql = "";

            sql = @"SELECT [SubjectTypeExamId]
                          ,[SubjectTypeExamName]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
                      FROM [SubjectTypeExam] ";
            return db.Database.SqlQuery(t, sql, null).Cast<SubjectTypeExam>().ToList();
            
        }
        /// <summary>
        /// 查询租户信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<APIResult> GetTenant(string tenantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId) };
            Type t = typeof(Tenant);
            string sql = "";

            sql = @"SELECT TenantId,TenantName,TenantCode,Email,TelNo,InUserid,InDateTime,ModifyUserId,ModifyDateTime
	                    FROM Tenant WHERE 1=1 ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            List<Tenant> list = db.Database.SqlQuery(t, sql, para).Cast<Tenant>().ToList();
            return new APIResult() { Status = true, Body = CommonHelper.EncodeDto<Tenant>(list) };
        }
        /// <summary>
        /// 查询品牌信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<APIResult> GetBrand(string tenantId, string userId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                       new SqlParameter("@UserId", userId)};
            Type t = typeof(Brand);
            string sql = "";

            sql = @"SELECT A.BrandId,A.TenantId,A.BrandName,A.BrandCode,A.Remark,A.InUserId,A.InDateTime,A.ModifyUserId,A.ModifyDateTime
                    FROM Brand A INNER JOIN UserInfoBrand B ON A.BrandId = B.BrandId
                    WHERE 1=1 AND TenantId = @TenantId ";
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND UserId = @UserId";
            }
            List<Brand> list = db.Database.SqlQuery(t, sql, para).Cast<Brand>().ToList();
            return new APIResult() { Status = true, Body = CommonHelper.EncodeDto<Brand>(list) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<APIResult> GetProject(string tenantId, string brandId, string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProjectId", projectId)};
            Type t = typeof(Project);
            string sql = "";
            sql = @"SELECT [ProjectId]
                          ,[TenantId]
                          ,[BrandId]
                          ,[ProjectCode]
                          ,[ProjectName]
                          ,[Year]
                          ,[Quarter]
                          ,[OrderNO]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
                    FROM [Project]
                    WHERE TenantId = @TenantId AND BrandId = @Brand
                    ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            List<Project> list = db.Database.SqlQuery(t, sql, para).Cast<Project>().ToList();
            return new APIResult() { Status = true, Body = CommonHelper.EncodeDto<Project>(list) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<Shop> GetShop(string tenantId, string brandId, string shopId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(Shop);
            string sql = "";
            sql = @"SELECT [ShopId]
                          ,[TenantId]
                          ,[BrandId]
                          ,[ShopCode]
                          ,[ShopName]
                          ,[ShopShortName]
                          ,[Province]
                          ,[City]
                          ,[UseChk]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
                      FROM [Shop]
                    WHERE TenantId = @TenantId
                    AND BrandId = @BrandId 
                    ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Shop>().ToList();
        }
    }
}