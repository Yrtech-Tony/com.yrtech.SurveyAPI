using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
            return db.Database.SqlQuery(t, sql, new SqlParameter[] { }).Cast<SubjectType>().ToList();

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
            return db.Database.SqlQuery(t, sql, new SqlParameter[] { }).Cast<SubjectTypeExam>().ToList();

        }
        /// <summary>
        /// 查询租户信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<Tenant> GetTenant(string tenantId)
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
            return db.Database.SqlQuery(t, sql, para).Cast<Tenant>().ToList();

        }
        /// <summary>
        /// 查询品牌信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Brand> GetBrand(string tenantId, string userId, string brandId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                       new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@BrandId", brandId)};
            Type t = typeof(Brand);
            string sql = "";

            sql = @"SELECT A.BrandId,A.TenantId,A.BrandName,A.BrandCode,A.Remark,A.InUserId,A.InDateTime,A.ModifyUserId,A.ModifyDateTime
                    FROM Brand A INNER JOIN UserInfoBrand B ON A.BrandId = B.BrandId
                    WHERE 1=1 AND A.TenantId = @TenantId ";
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND B.UserId = @UserId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Brand>().ToList();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<Project> GetProject(string tenantId, string brandId, string projectId)
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
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Project>().ToList();

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
                    WHERE  1=1 
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Shop>().ToList();
        }

        /// <summary>
        /// 获取体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectDto> GetSubject(string projectId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)};
            Type t = typeof(SubjectDto);
            string sql = "";
            sql = @"SELECT A.SubjectId
                          ,A.[SubjectCode]
                          ,A.[ProjectId]
                          ,A.[SubjectTypeExamId]
                          ,B.SubjectTypeExamName
                          ,A.[SubjectRecheckTypeId]
                          ,A.[SubjectLinkId]
                          ,C.SubjectLinkName
                          ,A.[OrderNO]
                          ,A.[Implementation]
                          ,A.[CheckPoint]
                          ,A.[Desc]
                          ,A.[AdditionalDesc]
                          ,A.[InspectionDesc]
                    FROM Subject A INNER JOIN SubjectTypeExam B ON A.SubjectTypeExamId = B.SubjectTypeExamId
				                   INNER JOIN SubjectLink C ON A.SubjectLinkId= C.SubjectLinkId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = "+subjectId;
            }
            List<SubjectDto> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectDto>().ToList();
            return list;
        }
        
        /// <summary>
        /// 获取标准照片信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectFile> GetSubjectFile(string projectId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(SubjectFile);
            string sql = "";
            sql = @"SELECT sf.FileId,sf.SubjectId,sf.SeqNO,sf.FileName,sf.FileType,sf.InUserId,sf.InDateTime,sf.ModifyUserId,sf.ModifyDateTime
                   FROM SubjectFile sf,Subject s 
                   WHERE sf.SubjectId=s.SubjectId AND ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            List<SubjectFile> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectFile>().ToList();
            return list;
        }

        /// <summary>
        /// 获取检查标准信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectInspectionStandard> GetSubjectInspectionStandard(string projectId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(SubjectInspectionStandard);
            string sql = "";
            sql = @"SELECT sis.InspectionStandardId,sis.InspectionStandardName,sis.SubjectId,sis.SeqNO,sis.InUserId,sis.InDateTime,sis.ModifyUserId,sis.ModifyDateTime" +
                  " FROM SubjectInspectionStandard sis,Subject s WHERE sis.SubjectId=s.SubjectId and ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            List<SubjectInspectionStandard> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectInspectionStandard>().ToList();
            return list;
        }

        /// <summary>
        /// 获取失分说明
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectLossResult> GetSubjectLossResult(string projectId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(SubjectLossResult);
            string sql = "";
            sql = @"SELECT slr.LossResultId,slr.SubjectId,slr.SeqNO,slr.LossResultName,slr.InUserId,slr.InDateTime,slr.ModifyUserId,slr.ModifyDateTime" +
                  " FROM SubjectLossResult slr,Subject s WHERE slr.SubjectId=s.SubjectId and s.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            List<SubjectLossResult> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectLossResult>().ToList();
            return list;
        }

        /// <summary>
        /// 获取体系类型打分范围信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectTypeScoreRegion> GetSubjectTypeScoreRegion(string projectId, string subjectId,string subjectTypeId)
        {
            //CommonHelper.log(projectId + " " + subjectId+" " + subjectTypeId);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@SubjectId", subjectId)
                                                        , new SqlParameter("@SubjectTypeId", subjectTypeId) };
            Type t = typeof(SubjectTypeScoreRegion);
            string sql = "";
            sql = @"SELECT str.Id,str.SubjectId,str.SubjectTypeId,str.LowestScore,str.FullScore,str.InUserId,str.InDateTime,str.ModifyUserId,str.ModifyDateTime" +
                  " FROM SubjectTypeScoreRegion str,Subject s  WHERE str.SubjectId=s.SubjectId and ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            if (!string.IsNullOrEmpty(subjectTypeId))
            {
                sql += " AND str.SubjectTypeId = @SubjectTypeId";
            }
            List<SubjectTypeScoreRegion> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectTypeScoreRegion>().ToList();
            return list;
        }
        public List<SubjectLink> GetSubjectLink(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectLink);
            string sql = @"SELECT * FROM [SubjectLink] WHERE ProjectId = @ProjectId";
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectLink>().ToList();
        }
    }
}