﻿using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace com.yrtech.SurveyAPI.Service
{
    public class RecheckService
    {
        Survey db = new Survey();

        #region 复审状态
        /// <summary>
        /// 查询当前经销商最新的状态
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<RecheckStatusDto> GetShopRecheckStatus(string projectId, string shopId, string statusCode)
        {
            if (shopId == null) shopId = "";
            if (statusCode == null) statusCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@StatusCode", statusCode)};
            Type t = typeof(RecheckStatusDto);
            string sql = "";
            sql = @"SELECT [RecheckStatusId]
                          ,A.[ProjectId]
                          ,B.ProjectCode,B.ProjectName
                          ,A.[ShopId],C.ShopCode,C.ShopName
                          ,A.[StatusCode]
                          ,A.[InUserId],E.AccountName
                          ,A.[InDateTime]
                      FROM [ReCheckStatus] A INNER JOIN Project B ON A.ProjectId = B.ProjectId
						                    INNER JOIN Shop C ON A.ShopId = C.ShopId
						                    INNER JOIN UserInfo E ON A.InUserId = E.Id
                                            INNER JOIN (SELECT ProjectId,ShopId,Max(StatusCode) AS StatusCode 
                                                        FROM [ReCheckStatus]
									                    GROUP BY ProjectId,ShopId) F ON F.ProjectId = A.ProjectId 
																                        AND F.ShopId = A.ShopId 
																                        AND F.StatusCode = A.StatusCode
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(statusCode))
            {
                sql += " AND A.StatusCode = @StatusCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDto>().ToList();
        }
        public List<RecheckStatusDto> GetShopRecheckStatusLog(string projectId, string shopId,string statusCode)
        {
            if (shopId == null) shopId = "";
            if (statusCode == null) statusCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@StatusCode", statusCode)};
            Type t = typeof(RecheckStatusDto);
            string sql = "";
            sql = @"SELECT [RecheckStatusId]
                          ,A.[ProjectId]
                          ,B.ProjectCode,B.ProjectName
                          ,A.[ShopId],C.ShopCode,C.ShopName
                          ,A.[StatusCode]
                          ,A.[InUserId],E.AccountName
                          ,A.[InDateTime]
                      FROM [ReCheckStatus] A INNER JOIN Project B ON A.ProjectId = B.ProjectId
						                    INNER JOIN Shop C ON A.ShopId = C.ShopId
						                    INNER JOIN UserInfo E ON A.InUserId = E.Id
                                            WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(statusCode))
            {
                sql += " AND A.StatusCode = @StatusCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDto>().ToList();
        }
        /// <summary>
        /// 查询经销商复审阶段，各类型的状态详细
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<RecheckStatusDtlDto> GetShopRecheckStautsDtl(string projectId, string shopId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(RecheckStatusDtlDto);
            string sql = "";
            sql = @"SELECT A.*, B.RecheckTypeName
                            ,CASE WHEN C.InUserId IS NOT NULL THEN '复审完毕'
                            ELSE '' END AS StatusName
                            ,C.InUserId AS RecheckUserId,D.AccountName AS RecheckUserName,C.InDateTime AS RecheckDateTime
                    FROM Shop A INNER JOIN SubjectRecheckType B ON 1=1 
			                    LEFT JOIN RecheckStatusDtl C ON A.ShopId = C.ShopId 
										                    AND C.ProjectId = @ProjectId 
										                    AND B.RecheckTypeId = C.RecheckTypeId
			                    LEFT JOIN UserInfo D ON C.InUserId = D.Id
                    WHERE 1 = 1";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += "AND A.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDtlDto>().ToList();
        }
        public void SaveRecheckStatus(ReCheckStatus status)
        {
            ReCheckStatus findOne = db.ReCheckStatus.Where(x => (x.ProjectId == status.ProjectId && x.ShopId == status.ShopId && x.StatusCode == status.StatusCode)).FirstOrDefault();
            if (findOne == null)
            {
                status.InDateTime = DateTime.Now;
                db.ReCheckStatus.Add(status);
            }
            db.SaveChanges();
        }
        public void SaveRecheckStatusDtl(RecheckStatusDtl statusDtl)
        {
            RecheckStatusDtl findOne = db.RecheckStatusDtl.Where(x => (x.ProjectId == statusDtl.ProjectId && x.ShopId == statusDtl.ShopId && x.RecheckTypeId == statusDtl.RecheckTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                statusDtl.InDateTime = DateTime.Now;
                db.RecheckStatusDtl.Add(statusDtl);
            }
            db.SaveChanges();
        }
        #endregion
        #region 复审详细
        #region 查询体系信息
        /// <summary>
        /// 查询需要进行复审的体系
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectRecheckTypeId"></param>
        /// <returns></returns>
        public List<Subject> GetShopNeedRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId)
        {
            #region 获取当前经销商最后一次复审的体系序号
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectRecheckTypeId", subjectRecheckTypeId)};
            Type t = typeof(int);
            string sql = "";
            int lastRecheckSubjectOrderNO = 0;// 最后一次的序号
            int recheckSubjectId = 0;// 需要复审的体系
            sql = @"SELECT ISNULL(MAX(B.OrderNO),0) AS OrderNO 
                    FROM Answer A JOIN [Subject] B ON A.ProjectId = B.ProjectId
                                                   AND A.SubjectId = B.SubjectId
                                INNER JOIN Recheck C ON A.ProjectId = C.ProjectId
                                                      AND A.ShopId = C.ShopId
                                                      AND A.SubjectId = C.SubjectId
                WHERE B.SubjectRecheckTypeId = @SubjectRecheckTypeId
                      AND A.ProjectId = @ProjectId
                      AND A.ShopId = @ShopId";
            lastRecheckSubjectOrderNO = db.Database.SqlQuery(t, sql, para).Cast<int>().FirstOrDefault();
            #endregion
            #region 查询需要打分体系Id
            SqlParameter[] para1 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@LastRecheckSubjectOrderNO", lastRecheckSubjectOrderNO),
                                                       new SqlParameter("@SubjectRecheckTypeId", subjectRecheckTypeId)};

            sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MIN(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO > @LastRecheckSubjectOrderNO	
		            AND A.SubjectRecheckTypeId = @SubjectRecheckTypeId";
            recheckSubjectId = db.Database.SqlQuery(t, sql, para1).Cast<int>().FirstOrDefault();
            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@RecheckSubjectId", recheckSubjectId),
                                                        new SqlParameter("@OrderNO", lastRecheckSubjectOrderNO)  };
            if (recheckSubjectId == 0) // 当已经复审到最后一题时recheckSubjectId==0,这是查询当前序号的体系即为最后一题
            {
                sql = @"SELECT * FROM Subject WHERE ProjectId = @ProjectId AND OrderNO =@OrderNO";
            }
            else
            {
                sql = @"SELECT * FROM Subject WHERE ProjectId = @ProjectId AND SubjectId = @RecheckSubjectId";
            }
            Type t_subject = typeof(Subject);
            return db.Database.SqlQuery(t_subject, sql, para2).Cast<Subject>().ToList();
            #endregion
        }
        /// <summary>
        /// 点击下一个按钮时调用
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectRecheckTypeId"></param>
        /// <param name="orderNO"></param>
        /// <returns></returns>
        public List<Subject> GetShopNextRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId, string orderNO)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectRecheckTypeId", subjectRecheckTypeId),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t_subject = typeof(Subject);
            string sql = @"SELECT * FROM Subject  WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MIN(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO > @OrderNO	
		            AND SubjectRecheckTypeId = @SubjectRecheckTypeId)";
            return db.Database.SqlQuery(t_subject, sql, para).Cast<Subject>().ToList();
        }
        /// <summary>
        /// 点击上一个调用
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectRecheckTypeId"></param>
        /// <param name="orderNO"></param>
        /// <returns></returns>
        public List<Subject> GetShopPreRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId, string orderNO)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectRecheckTypeId", subjectRecheckTypeId),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t_subject = typeof(Subject);
            string sql = @"SELECT * FROM Subject  WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MAX(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO < @OrderNO	
		            ";
            if (!string.IsNullOrEmpty(subjectRecheckTypeId))
            {
                sql += " AND SubjectRecheckTypeId = @SubjectRecheckTypeId) ";
            }
            else
            {
                sql += ")";
            }
            return db.Database.SqlQuery(t_subject, sql, para).Cast<Subject>().ToList();
        }
        #endregion
        #region 体系复审详细
        /// <summary>
        /// 查询打分信息和复审内容
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<RecheckDto> GetShopRecheckInfo(string projectId, string shopId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@SubjectId", subjectId)};
            string sql = "";
            Type t = typeof(RecheckDto);
            sql += @"SELECT A.*,B.AccountName AS RecheckUserName 
                    FROM Recheck A INNER JOIN UserInfo B ON A.RecheckUserId = B.UserId
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId ";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += "  AND A.SubjectId = @SubjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckDto>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recheck"></param>
        public void SaveShopRecheckInfo(ReCheck recheck)
        {
            ReCheck findOne = db.ReCheck.Where(x => x.ProjectId == recheck.ProjectId && x.ShopId == recheck.ShopId && x.SubjectId == recheck.SubjectId).FirstOrDefault();
            if (findOne == null)
            {
                recheck.RecheckDateTime = DateTime.Now;
                db.ReCheck.Add(recheck);
            }
            else
            {
                findOne.PassReCheck = recheck.PassReCheck;
                findOne.ReCheckContent = recheck.ReCheckContent;
                findOne.RecheckDateTime = DateTime.Now;
                findOne.RecheckError = recheck.RecheckError;
                findOne.ReCheckUserId = recheck.ReCheckUserId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 查询还未进行复审的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectRecheckTypeId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetNotRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@SubjectRecheckTypeId", subjectRecheckTypeId)};
            string sql = "";
            Type t = typeof(AnswerDto);
            sql += @"SELECT A.SubjectId,B.SubjectCode,A.AnswerId
                    FROM Answer A INNER JOIN Subject B ON A.ProjectId = B.ProjectId AND A.SubjectId = B.SubjectId
                    WHERE A.ProjectId = @ProjectId 
                    AND NOT EXISTS(SELECT 1 FROM Recheck WHERE ProjectId = A.ProjectId 
                                                        AND ShopId = A.ShopId 
                                                        AND SubjectId = A.SubjectId )";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId ";
            }
            if (!string.IsNullOrEmpty(subjectRecheckTypeId))
            {
                sql += "  AND B.SubjectRecheckTypeId = @SubjectRecheckTypeId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        #endregion 
        #endregion
    }
}