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
        public List<RecheckStatusDto> GetShopRecheckStatusInfo(string projectId, string shopId, string statusCode)
        {
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)
                                                       ,new SqlParameter("@StatusCode", statusCode)};
            Type t = typeof(RecheckStatusDto);
            string sql = "";
            sql = @"SELECT A.*,B.HiddenName AS StatusName,C.ShopCode,C.ShopName,D.ProjectCode,D.ProjectName
                            FROM ReCheckStatus A INNER JOIN HiddenColumn B ON A.StatusCode = B.HiddenCode AND B.HiddenCodeGroup='调研进度'
                                                 INNER JOIN Shop C ON A.ShopId = C.ShopId
                                                 INNER JOIN Project D ON A.ProjectId = D.ProjectId
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
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<RecheckStatusDto> GetShopRecheckStatus(string projectId, string shopId, string shopCode)
        {
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            if (shopCode == null) shopCode = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),  new SqlParameter("@ShopCode", shopCode)};
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
                            (SELECT IndateTime FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S1') 
                            AS Status_S1_Date,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatusDtl WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId)
	                                THEN 'S2'
	                                ELSE ''
                            END AS Status_S2,
                            CASE WHEN (SELECT Count(*) FROM ReCheckStatusDtl WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId)>=
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
                            END AS Status_S7,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S8')
	                                THEN 'S8'
	                                ELSE ''
                            END AS Status_S8,
                            CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S9')
	                                THEN 'S9'
	                                ELSE ''
                            END AS Status_S9
                            ,CASE WHEN EXISTS(SELECT 1 FROM ReCheckStatus WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId AND StatusCode = 'S10')
	                                THEN 'S10'
	                                ELSE ''
                            END AS Status_S10
                            FROM ReCheckStatus A INNER JOIN Shop B ON A.ShopId = B.ShopId
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND B.ShopCode = @ShopCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDto>().ToList();
        }
        /// <summary>
        /// 查询经销商复审阶段，各类型的状态详细
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<RecheckStatusDtlDto> GetShopRecheckStautsDtl(string projectId, string shopId, string recheckTypeId)
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
        /// <summary>
        /// 查询经销商复审阶段，各类型的状态详细
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<RecheckStatusDtlDto> GetShopRecheckStautsDtlForAllType(string projectId, string shopId)
        {
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopId", shopId)};
            Type t = typeof(RecheckStatusDtlDto);
            string sql = "";
            sql = @"SELECT (SELECT TOP 1 ShopCode FROM Shop WHERE ShopId = @ShopId) AS ShopCode
                    ,(SELECT TOP 1 ShopName FROM Shop WHERE ShopId = @ShopId) AS ShopName
                    ,A.LabelId_RecheckType AS RecheckTypeId
		                    ,B.LabelCode AS RecheckTypeCode
		                    ,B.LabelName AS RecheckTypeName
		                    ,(SELECT InDateTime 
                                FROM RecheckStatusDtl 
                                WHERE ProjectId = @ProjectId 
                                AND ShopId = @ShopId 
                                AND RecheckTypeId = A.LabelId_RecheckType) AS InDateTime
                    FROM 
                    (SELECT DISTINCT LabelId_RecheckType 
                    FROM Subject WHERE ProjectId = @ProjectId) A INNER JOIN Label B ON A.LabelId_RecheckType = B.LabelId 
											                                         AND B.LabelType = 'RecheckType'";
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
        public void DeleteRecheckStatus(string recheckStatusId, string userId)
        {
            if (recheckStatusId == null) recheckStatusId = "";
            SqlParameter[] para = new SqlParameter[] { };
            string sql = "";
            sql += @" INSERT INTO RecheckStatusLog
                      SELECT RecheckStatusId,
                             0,
                             ProjectId,
                             ShopId,
                             StatusCode,
                             'D',
                             '',
                            " + userId + ",GETDATE() FROM RecheckStatus WHERE RecheckStatusId=" + recheckStatusId;
            sql += " DELETE RecheckStatus WHERE RecheckStatusId = " + recheckStatusId;
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public void DeleteRecheckStatusDtl(string recheckStatusDtlId,string userId)
        {
            if (recheckStatusDtlId == null) recheckStatusDtlId = "";
            SqlParameter[] para = new SqlParameter[] { };
            string sql = "";
            sql += @" INSERT INTO RecheckStatusLog
                      SELECT 0,
                             RecheckStatusDtlId,
                             ProjectId,
                             ShopId,
                             RecheckTypeId,
                             'D',
                             '',
                            " + userId + ",GETDATE() WHERE RecheckStatusId=" + recheckStatusDtlId;
             sql = "DELETE RecheckStatusDtl WHERE RecheckStatusDtlId = " + recheckStatusDtlId;
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion
        #region 一审
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
                            A.PhotoScore, A.Remark,A.LossResult,X.RecheckId,X.PassRecheck,B.LabelId_RecheckType AS RecheckTypeId,X.AgreeCheck
                            ,(SELECT TOP 1 LabelName FROM Label WHERE LabelId = B.LabelId_RecheckType) AS RecheckTypeName
                            ,CASE WHEN X.PassReCheck=1 THEN '是'
                                  WHEN X.PassRecheck=0 THEN '否'
                                    ELSE ''
                            END AS PassRecheckName,X.RecheckContent,X.RecheckError,X.RecheckScore,X.RecheckUserId,X.RecheckDateTime
                             ,CASE WHEN X.PassReCheck_Confirm=1 THEN '是'
                                   WHEN  X.PassReCheck_Confirm=0 THEN '否'
                                    ELSE ''
                            END AS PassReCheckName_Confirm
                        ,X.PassReCheck_Confirm,X.ReCheckContent_Confirm,X.ReCheckError_Confirm,X.ReCheckScore_Confirm,X.ReCheckUserId_Confirm,X.ReCheckDateTime_Confirm
                         ,CASE WHEN X.PassReCheck_Sec=1 THEN '是'
                               WHEN X.PassReCheck_Sec=0 THEN '否'
                                    ELSE ''
                            END AS PassReCheckName_Sec                        
                        ,X.PassReCheck_Sec,X.RecheckContent_Sec,X.ReCheckError_Sec,X.ReCheckScore_Sec,X.ReCheckUserId_Sec,X.ReCheckDateTime_Sec
                      FROM Answer A INNER JOIN [Subject] B ON A.ProjectId = B.ProjectId 
									                       AND  A.SubjectId = B.SubjectId
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
            sql += " ORDER BY A.ProjectId,C.ShopCode,A.SubjectId,B.OrderNO";
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
                                    A.SubjectCode,A.OrderNO,a.Remark AS [Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc
                            FROM  [Subject] A INNER JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                            AND A.SubjectId = B.SubjectId 
                                                            AND B.ShopId =  @ShopId
                         WHERE A.ProjectId  = @ProjectId 
                                AND  A.OrderNO =(SELECT ISNULL(MIN(OrderNO),0) 
                                                FROM [Subject] X INNER JOIN Answer Y ON X.ProjectId = Y.ProjectId
                                                                                    AND X.SubjectId = Y.SubjectId
                                                                                    AND Y.ShopId = @ShopId
                                                WHERE X.ProjectId = @ProjectId 
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
                                A.SubjectCode,A.OrderNO,a.Remark AS [Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc
                         FROM  [Subject] A INNER JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                        AND A.SubjectId = B.SubjectId 
                                                        AND B.ShopId =  @ShopId
                        WHERE A.ProjectId  = @ProjectId 
                            AND  A.OrderNO =(SELECT ISNULL(MAX(OrderNO),0) 
                                             FROM [Subject] X INNER JOIN Answer Y ON X.ProjectId = Y.ProjectId
                                                                                    AND X.SubjectId = Y.SubjectId
                                                                                    AND Y.ShopId = @ShopId
                                            WHERE X.ProjectId = @ProjectId 
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
        #region 一审确认保存
        public void SaveRecheckConfirmInfo(string recheckId, bool? recheckConfirmCheck, string recheckConfirmContent, string recheckConfirmError, decimal? recheckConfirmScore, int? recheckConfirmUserId)
        {
            int recheckIdinput = Convert.ToInt32(recheckId);
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == recheckIdinput)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.PassReCheck_Confirm = recheckConfirmCheck;
                findOne.ReCheckDateTime_Confirm = DateTime.Now;
                findOne.ReCheckContent_Confirm = recheckConfirmContent;
                findOne.ReCheckError_Confirm = recheckConfirmError;
                findOne.ReCheckScore_Confirm = recheckConfirmScore;
                findOne.ReCheckUserId_Confirm = recheckConfirmUserId;
            }
            db.SaveChanges();
        }
        #endregion

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
            sql += @"SELECT B.SubjectId,B.SubjectCode,A.AnswerId
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
                sql += "  AND B.LabelId_RecheckType = @SubjectRecheckTypeId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        #endregion
        #region 二审详细
        /// <summary>
        /// 二审信息保存
        /// </summary>
        /// <param name="recheckId"></param>
        /// <param name="recheckConfirmCheck"></param>
        /// <param name="recheckConfirmContent"></param>
        /// <param name="recheckConfirmError"></param>
        /// <param name="recheckConfirmScore"></param>
        /// <param name="recheckConfirmUserId"></param>
        public void SaveRecheckSecondInfo(string recheckId, bool? recheckConfirmCheck, string recheckConfirmContent, string recheckConfirmError, decimal? recheckConfirmScore, int? recheckConfirmUserId)
        {
            int recheckIdinput = Convert.ToInt32(recheckId);
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == recheckIdinput)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.PassReCheck_Sec = recheckConfirmCheck;
                findOne.ReCheckDateTime_Sec = DateTime.Now;
                findOne.ReCheckContent_Sec = recheckConfirmContent;
                findOne.ReCheckError_Sec = recheckConfirmError;
                findOne.ReCheckScore_Sec = recheckConfirmScore;
                findOne.ReCheckUserId_Sec = recheckConfirmUserId;
            }
            db.SaveChanges();
        }
        #endregion
    }
}