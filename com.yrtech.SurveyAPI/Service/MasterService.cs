﻿using com.yrtech.SurveyAPI.Common;
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
                    WHERE TenantId = @TenantId 
                    ";
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
                    WHERE TenantId = @TenantId
                    ";
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
        public List<Subject> GetSubject(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(Subject);
            string sql = "";
            sql = @"SELECT SubjectId,SubjectCode,ProjectId,SubjectTypeExamId,SubjectRecheckTypeId,OrderNO,Implementation,[CheckPoint]," +
                  "[Desc],AdditionalDesc,InspectionDesc,Remark,InUserId,InDateTime,ModifyUserId,ModifyDateTime  FROM Subject WHERE ProjectId = @ProjectId";
            List<Subject> list = db.Database.SqlQuery(t, sql, para).Cast<Subject>().ToList();
            return list;
        }

        /// <summary>
        /// 获取标准照片信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectFile> GetSubjectFile(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectFile);
            string sql = "";
            sql = @"SELECT sf.FileId,sf.SubjectId,sf.SeqNO,sf.FileName,sf.FileType,sf.InUserId,sf.InDateTime,sf.ModifyUserId,sf.ModifyDateTime" +
                  " FROM SubjectFile sf,Subject s WHERE sf.SubjectId=s.SubjectId and ProjectId = @ProjectId";
            List<SubjectFile> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectFile>().ToList();
            return list;
        }

        /// <summary>
        /// 获取检查标准信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectInspectionStandard> GetSubjectInspectionStandard(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectInspectionStandard);
            string sql = "";
            sql = @"SELECT sis.InspectionStandardId,sis.InspectionStandardName,sis.SubjectId,sis.SeqNO,sis.InUserId,sis.InDateTime,sis.ModifyUserId,sis.ModifyDateTime" +
                  " FROM SubjectInspectionStandard sis,Subject s WHERE sis.SubjectId=s.SubjectId and ProjectId = @ProjectId";
            List<SubjectInspectionStandard> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectInspectionStandard>().ToList();
            return list;
        }

        /// <summary>
        /// 获取检查标准信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectLossResult> GetSubjectLossResult(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectLossResult);
            string sql = "";
            sql = @"SELECT slr.LossResultId,slr.SubjectId,slr.SeqNO,slr.LossResultName,slr.InUserId,slr.InDateTime,slr.ModifyUserId,slr.ModifyDateTime" +
                  " FROM SubjectLossResult slr,Subject s WHERE slr.SubjectId=s.SubjectId and s.ProjectId = @ProjectId";
            List<SubjectLossResult> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectLossResult>().ToList();
            return list;
        }
        
        /// <summary>
        /// 获取体系类型打分范围信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectTypeScoreRegion> GetSubjectTypeScoreRegion(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectTypeScoreRegion);
            string sql = "";
            sql = @"SELECT str.Id,str.SubjectId,str.SubjectTypeId,str.LowestScore,str.FullScore,str.InUserId,str.InDateTime,str.ModifyUserId,str.ModifyDateTime" +
                  " FROM SubjectTypeScoreRegion str,Subject s  WHERE str.SubjectId=s.SubjectId and ProjectId = @ProjectId";
            List<SubjectTypeScoreRegion> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectTypeScoreRegion>().ToList();
            return list;
        }

        /// <summary>
        /// 保存答题信息列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void InserAnswerList(List<Answer> lst)
        {
            if (lst == null) return;
            foreach(Answer answer in lst){
                Answer findOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.SubjectId == answer.SubjectId)).FirstOrDefault();
                if (findOne == null)
                {
                    db.Answer.Add(answer);
                }
                else
                {
                    db.Entry<Answer>(answer).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void InserAnswerShopInfoList(List<AnswerShopInfo> lst)
        {
            if (lst == null) return;
            foreach (AnswerShopInfo answerShopInfo in lst)
            {
                AnswerShopInfo findOne = db.AnswerShopInfo.Where(x => (x.ProjectId == answerShopInfo.ProjectId && x.ShopId == answerShopInfo.ShopId)).FirstOrDefault();
                if (findOne == null)
                {
                    db.AnswerShopInfo.Add(answerShopInfo);
                }
                else
                {
                    db.Entry<AnswerShopInfo>(findOne).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 保存顾问信息列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void InserAnswerShopConsultantList(List<AnswerShopConsultant> lst)
        {
            if (lst == null) return;
            foreach (AnswerShopConsultant item in lst)
            {
                AnswerShopConsultant findOne = db.AnswerShopConsultant.Where(x => (x.ProjectId == item.ProjectId && x.ShopId == item.ShopId)).FirstOrDefault();
                if (findOne == null)
                {
                    db.AnswerShopConsultant.Add(item);
                }
                else
                {
                    db.Entry<AnswerShopConsultant>(findOne).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
    }
}