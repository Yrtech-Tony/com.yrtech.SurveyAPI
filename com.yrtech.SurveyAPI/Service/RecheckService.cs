using com.yrtech.SurveyAPI.Common;
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
        public List<ReCheckStatus> GetShopRecheckStatusInfo(string projectId, string shopId,string statusCode)
        {
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),new SqlParameter("@StatusCode", statusCode)};
            Type t = typeof(ReCheckStatus);
            string sql = "";
            sql = @"SELECT *
                            FROM ReCheckStatus A
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(statusCode))
            {
                sql += " AND A.StatusCode = @StatusCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReCheckStatus>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<RecheckStatusDto> GetShopRecheckStatus(string projectId, string shopId)
        {
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(RecheckStatusDto);
            string sql = "";
            sql = @"SELECT DISTINCT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S0')
	                                THEN 'S0'
	                                ELSE ''
                            END AS Status_S0,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S1')
	                                THEN 'S1'
	                                ELSE ''
                            END AS Status_S1,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatusDtl WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId)
	                                THEN 'S2'
	                                ELSE ''
                            END AS Status_S2,
                            CASE WHEN (SELECT Count(*) FROM ReCheckStatusDtl WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId)=
                            (SELECT COUNT(*) FROM (SELECT DISTINCT LabelId_RecheckType FROM [Subject] WHERE ProjectId = A.ProjectId)X)
	                                THEN 'S3'
	                                ELSE ''
                            END AS Status_S3,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S4')
	                                THEN 'S4'
	                                ELSE ''
                            END AS Status_S4,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S5')
	                                THEN 'S5'
	                                ELSE ''
                            END AS Status_S5,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S6')
	                                THEN 'S6'
	                                ELSE ''
                            END AS Status_S6,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S7')
	                                THEN 'S7'
	                                ELSE ''
                            END AS Status_S7
                            FROM ReCheckStatus A INNER JOIN Shop B ON A.ShopId = B.ShopId
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDto>().ToList();
        }
        /// <summary>
        /// 查询经销商复审阶段，各类型的状态详细
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<RecheckStatusDtlDto> GetShopRecheckStautsDtl(string projectId, string shopId,string recheckTypeId)
        {
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            if (recheckTypeId == null) recheckTypeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@RecheckTypeId", recheckTypeId),
                                                        new SqlParameter("@ShopId", shopId)};
            Type t = typeof(RecheckStatusDtlDto);
            string sql = "";
            sql = @"SELECT A.*,B.LabelCode AS RecheckTypeCode,B.LabelName AS RecheckTypeName
                    FROM RecheckStatusDtl A INNER JOIN Label B ON A.ReCheckTypeId = B.LabelId 
											AND B.LabelType = 'RecheckType'
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(recheckTypeId))
            {
                sql += " AND A.RecheckTypeId = @RecheckTypeId";
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
        #region 查询经销商复审清单
        public List<RecheckDto> GetShopRecheckScoreInfo(string projectId, string shopId, string subjectId, string recheckTypeId)
        {
            shopId = shopId == null ? "" : shopId;
            subjectId = subjectId == null ? "" : subjectId;
            recheckTypeId = recheckTypeId == null ? "" : recheckTypeId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectId", subjectId),
                                                       new SqlParameter("@RecheckTypeId", recheckTypeId)
                                                       };
            Type t = typeof(RecheckDto);
            string sql = "";
            sql = @"  SELECT A.ProjectId,D.ProjectCode,D.ProjectName,A.ShopId,A.SubjectId,C.ShopCode,C.ShopName,B.SubjectCode,B.[CheckPoint],B.OrderNO,B.[Desc],B.InspectionDesc,
                            A.PhotoScore, A.Remark,A.LossResult,X.RecheckId,X.PassRecheck,B.LabelId_RecheckType AS RecheckTypeId
                            ,(SELECT TOP 1 LabelName FROM Label WHERE LabelId = B.LabelId_RecheckType) AS RecheckTypeName
                            ,CASE WHEN X.PassReCheck=1 THEN '是'
                                    ELSE '否'
                            END AS PassRecheckName,X.RecheckContent,X.RecheckError,X.RecheckScore,X.RecheckUserId,X.RecheckDateTime
                      FROM Answer A INNER JOIN [Subject] B ON A.ProjectId = B.ProjectId 
									                       AND A.SubjectId = B.SubjectId
			                        INNER JOIN Shop C ON A.ShopId = C.ShopId
			                        INNER JOIN Project D ON A.ProjectId = D.ProjectId
			                        LEFT JOIN Recheck X ON A.ProjectId  = X.ProjectId 
									                    AND A.ShopId = X.ShopId 
									                    AND A.SubjectId = X.SubjectId
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId =@SubjectId ";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId =@ShopId ";
            }
            if (!string.IsNullOrEmpty(recheckTypeId))
            {
                sql += " AND B.LabelId_RecheckType = @RecheckTypeId";
            }
            sql += " ORDER BY A.ProjectId,C.ShopCode,B.OrderNO,A.SubjectId";
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckDto>().ToList();
        }
        #endregion
        #region 查询体系信息
        /// <summary>
        /// 点击下一个按钮时调用,查询同一个recheckType，需要得分和体系的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectRecheckTypeId"></param>
        /// <param name="orderNO"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopNextRecheckSubject(string projectId, string shopId, string recheckTypeId, string orderNO)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@RecheckTypeId", recheckTypeId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t_subject = typeof(AnswerDto);
            string sql = @"SELECT B.AnswerId,A.ProjectId,CAST(@ShopId AS INT) AS ShopId,A.SubjectId,B.PhotoScore,B.InspectionStandardResult,
                                    B.FileResult,B.LossResult,B.Remark,B.Indatetime,B.ModifyDateTime,
                                    A.SubjectCode,A.OrderNO,a.[Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc
                            FROM  [Subject] A INNER JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                            AND A.SubjectId = B.SubjectId 
                                                            AND B.ShopId =  @ShopId
                         WHERE A.ProjectId  = @ProjectId 
                                AND  A.OrderNO =(SELECT ISNULL(MIN(OrderNO),0) 
                                                FROM [Subject] 
                                                WHERE ProjectId = @ProjectId 
                                                AND LabelId_RecheckType =  @RecheckTypeId 
                                                AND OrderNO > @OrderNO)";
            return db.Database.SqlQuery(t_subject, sql, para).Cast<AnswerDto>().ToList();
        }
        /// <summary>
        /// 点击上一个调用，查询同一个RecheckTypeId 上一个题目的打分信息和题目信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectRecheckTypeId"></param>
        /// <param name="orderNO"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopPreRecheckSubject(string projectId, string shopId, string recheckTypeId, string orderNO)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@RecheckTypeId", recheckTypeId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t_subject = typeof(AnswerDto);
            string sql = @"SELECT B.AnswerId,A.ProjectId,CAST(@ShopId AS INT) AS ShopId,A.SubjectId,B.PhotoScore,B.InspectionStandardResult,
                                B.FileResult,B.LossResult,B.Remark,B.Indatetime,B.ModifyDateTime,
                                A.SubjectCode,A.OrderNO,a.[Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc
                         FROM  [Subject] A INNER JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                        AND A.SubjectId = B.SubjectId 
                                                        AND B.ShopId =  @ShopId
                        WHERE A.ProjectId  = @ProjectId 
                            AND  A.OrderNO =(SELECT ISNULL(MAX(OrderNO),0) 
                                             FROM [Subject] 
                                            WHERE ProjectId = @ProjectId 
                                            AND LabelId_RecheckType =  @RecheckTypeId
                                            AND OrderNO < @OrderNO)	
		            ";
            return db.Database.SqlQuery(t_subject, sql, para).Cast<AnswerDto>().ToList();
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recheck"></param>
        public void SaveShopRecheckInfo(ReCheck recheck)
        {
            ReCheck findOne = db.ReCheck.Where(x => x.ProjectId == recheck.ProjectId && x.ShopId == recheck.ShopId && x.SubjectId == recheck.SubjectId).FirstOrDefault();
            if (findOne == null)
            {
                recheck.InDateTime = DateTime.Now;
                recheck.ReCheckDateTime = DateTime.Now;
                db.ReCheck.Add(recheck);
            }
            else
            {
                findOne.PassReCheck = recheck.PassReCheck;
                findOne.ReCheckContent = recheck.ReCheckContent;
                findOne.ReCheckDateTime = DateTime.Now;
                findOne.ReCheckError = recheck.ReCheckError;
                findOne.ReCheckScore = recheck.ReCheckScore;
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
    }
}