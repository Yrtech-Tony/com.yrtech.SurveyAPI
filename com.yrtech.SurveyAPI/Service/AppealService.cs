using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace com.yrtech.SurveyAPI.Service
{
    public class AppealService
    {
        Entities db = new Entities();
        /// <summary>
        /// 生成经销商得分的信息用来申诉
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void CreateAppealInfoByProject(string projectId)
        {

            // 生成申诉的基本信息
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t_Project = typeof(Project);
            Type t = typeof(int);
            string sql_project = "SELECT * FROM Project WHERE ProjectId = @ProjectId";
            string sql = "";
            List<Project> project = db.Database.SqlQuery(t_Project, sql_project, para).Cast<Project>().ToList();

            sql = @"INSERT INTO Appeal
                        SELECT A.ProjectId,B.ProjectCode,B.ProjectName
		                        ,A.ShopId,D.ShopCode,D.ShopName
		                        ,A.SubjectId,C.SubjectCode,C.[CheckPoint]
		                        ,A.ImportScore AS Score,A.ImportLossResult AS LossResult
		                        ,'' AS AppealReason,null AS AppealUserId,null AS AppealDateTime
		                        ,'' AS FeedBackStatus ,'' AS FeedBackStatus,null AS FeedBackUserId,null AS FeedBackDateTime
		                        ,'' AS ShopAcceptStatus,'' AS ShopAcceptReason,null AS ShopAcceptUserId,null AS SHopAcceptDateTime
                         FROM Answer A INNER JOIN Project B ON A.ProjectId = B.ProjectId
			                           INNER JOIN [Subject] C ON A.SubjectId  = C.SubjectId AND A.ProjectId = C.ProjectId
			                           INNER JOIN Shop D ON A.ShopId = D.ShopId
                            WHERE A.ProjectId = @ProjectId ";
            db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
            if (project[0].DataScore == "01")// 01:系统打分 02：导入
            {
                //系统打分的时候，计算总分和失分说明，进行更新
            }
        }
        /// <summary>
        /// 查询经销商申诉列表_按页码
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AppealDto> GetShopAppealInfoByPage(string projectId, string shopIdStr, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount + 1;
            return GetShopAppealInfoByAll(projectId, shopIdStr).GetRange(startIndex, pageNum);
        }
        /// <summary>
        /// 查询所有的申诉列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopIdStr"></param>
        /// <returns></returns>
        public List<AppealDto> GetShopAppealInfoByAll(string projectId, string shopIdStr)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(AppealDto);
            string sql = @"SELECT [AppealId]
                                  ,[ProjectId]
                                  ,[ProjectCode]
                                  ,[ProjectName]
                                  ,[ShopId]
                                  ,[ShopCode]
                                  ,[ShopName]
                                  ,[SubjectId]
                                  ,[SubjectCode]
                                  ,[CheckPoint]
                                  ,[Score]
                                  ,[LossResult]
                                  ,[AppealReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = AppealUserId),'') AS AppealUserName
                                  ,AppealUserId
                                  ,[AppealDateTime]
                                  ,Case 
		                          WHEN [FeedBackStatus]  = 1 THEN '同意'
		                          WHEN [FeedBackStatus] = 0 THEN '不同意'
		                          ELSE ''
	                               END AS FeedBackStatusStr
                                  ,[FeedBackStatus]
                                  ,[FeedBackReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [FeedBackUserId]),'') AS FeedBackUserName
                                  ,[FeedBackUserId]
                                  ,[FeedBackDateTime]
                                  ,Case 
		                              WHEN ShopAcceptStatus  = 1 THEN '接受'
		                              WHEN ShopAcceptStatus = 0 THEN '不接受'
		                              ELSE ''
	                            END AS ShopAcceptStatusStr
                                  ,[ShopAcceptStatus]
                                  ,[ShopAcceptReason]
                                   ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [ShopAcceptUserId]),'') AS [ShopAcceptUserName]
                                  ,[ShopAcceptUserId]
                                  ,[ShopAcceptDateTime]
                              FROM [Appeal]
                              WHERE ProjectId = @ProjectId  ";
            if (string.IsNullOrEmpty(shopIdStr))
            {
                string[] shopIdList = shopIdStr.Split(';');
                sql += " AND ShopId IN(";
                for (int i = 0; i < shopIdList.Count(); i++)
                {
                    if (i == shopIdList.Count() - 1)
                    {
                        sql += shopIdList[i];
                    }
                    else
                    {
                        sql += shopIdList[i] + ",";
                    }
                }
                sql += ")";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AppealDto>().ToList();
        }
        /// <summary>
        /// 查询申诉详细
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<AppealDto> GetShopSubjectAppeal(string projectId, string shopId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@ShopId", shopId)
                                                    ,new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(AppealDto);
            string sql = @"SELECT [AppealId]
                                  ,[ProjectId]
                                  ,[ProjectCode]
                                  ,[ProjectName]
                                  ,[ShopId]
                                  ,[ShopCode]
                                  ,[ShopName]
                                  ,[SubjectId]
                                  ,[SubjectCode]
                                  ,[CheckPoint]
                                  ,[Score]
                                  ,[LossResult]
                                  ,[AppealReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = AppealUserId),'') AS AppealUserName
                                  ,AppealUserId
                                  ,[AppealDateTime]
                                  ,Case 
		                          WHEN [FeedBackStatus]  = 1 THEN '同意'
		                          WHEN [FeedBackStatus] = 0 THEN '不同意'
		                          ELSE ''
	                               END AS FeedBackStatusStr
                                  ,[FeedBackStatus]
                                  ,[FeedBackReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [FeedBackUserId]),'') AS FeedBackUserName
                                  ,[FeedBackUserId]
                                  ,[FeedBackDateTime]
                                  ,Case 
		                              WHEN ShopAcceptStatus  = 1 THEN '接受'
		                              WHEN ShopAcceptStatus = 0 THEN '不接受'
		                              ELSE ''
	                            END AS ShopAcceptStatusStr
                                  ,[ShopAcceptStatus]
                                  ,[ShopAcceptReason]
                                   ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [ShopAcceptUserId]),'') AS [ShopAcceptUserName]
                                  ,[ShopAcceptUserId]
                                  ,[ShopAcceptDateTime]
                              FROM [Appeal]
                              WHERE ProjectId = @ProjectId  AND ShopId = @ShopId AND SubjectId = @SubjectId";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealDto>().ToList();
        }
        /// <summary>
        /// 申诉
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="appealReason"></param>
        /// <param name="appealUserId"></param>
        public void AppealApply(string appealId, string appealReason, int appealUserId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
                                                        new SqlParameter("@AppealReason", appealReason),
                                                        new SqlParameter("@AppealUserId", appealUserId)};
            Type t = typeof(int);
            string sql = @"UPDATE Appeal SET AppealReason = @AppealReason,AppealUserId = @AppealUserId,AppealDateTime = GETDATE() 
                           WHERE AppealId = @AppealId";
            db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        /// <summary>
        /// 申诉反馈
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="appealReason"></param>
        /// <param name="appealUserId"></param>
        public void AppealFeedBack(string appealId, string feedbackStatus, string feedbackReason, int feedbackUserId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
                                                        new SqlParameter("@FeedBackStatus", feedbackStatus),
                                                        new SqlParameter("@FeedBackReason", feedbackReason),
                                                        new SqlParameter("@FeedBackUserId", feedbackUserId)};
            Type t = typeof(int);
            string sql = @"UPDATE Appeal SET FeedBackStatus = @FeedBackStatus,FeedBackReason = @FeedBackReason,
				                            FeedBackUserId = @FeedBackUserId,FeedBackDateTime = GETDATE() 
                           WHERE AppealId = @AppealId";
            db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        /// <summary>
        /// 经销商接受情况
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="shopAcceptStatus"></param>
        /// <param name="shopAcceptReason"></param>
        /// <param name="shopAcceptUserId"></param>
        public void AppealShopAccept(string appealId, string shopAcceptStatus, string shopAcceptReason, int shopAcceptUserId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
                                                        new SqlParameter("@ShopAcceptStatus", shopAcceptStatus),
                                                        new SqlParameter("@ShopAcceptReason", shopAcceptReason),
                                                        new SqlParameter("@ShopAcceptUserId", shopAcceptUserId)};
            Type t = typeof(int);
            string sql = @"UPDATE Appeal SET ShopAcceptStatus = @ShopAcceptStatus,ShopAcceptReason = @ShopAcceptReason,
				                            ShopAcceptUserId = @ShopAcceptUserId,ShopAcceptDateTime = GETDATE() 
                           WHERE AppealId = @AppealId";
            db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        /// <summary>
        /// 查询申诉附件信息
        /// </summary>
        /// <param name="appealId"></param>
        /// <returns></returns>
        public List<AppealFileDto> AppealFileSearch(string appealId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId) };
            Type t = typeof(AppealFileDto);
            string sql = @"SELECT FileId,AppealId,SeqNO
                                ,Case WHEN FileType = 'Recheck' THEN '审核人员'
                                      WHEN FileType = 'Shop' THEN '经销商'
                                END AS FileTypeName,
                                FileType,[FileName],ServerFileName
                                ,(SELECT AccountName FROM UserInfo WHERE Id = InUserId) AS InUserName
                                ,InUserId,InDateTime
                         FROM AppealFile
	                    WHERE AppealId = @AppealId";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealFileDto>().ToList();
        }
        public void AppealFileSave(string appealId, int seqNo, string fileType, string fileName, string serverFileName, int userId)
        {
            if (seqNo == 0)
            {
                SqlParameter[] paraMax = new SqlParameter[] { new SqlParameter("@AppealId", appealId) };
                string sql_Max = "SELECT TOP 1 * FROM AppealFile WHERE AppealId = @AppealId";
                Type t_max = typeof(AppealFile);
                AppealFile appealFile = db.Database.SqlQuery(t_max, sql_Max, paraMax).Cast<AppealFile>().ToList().FirstOrDefault();
                seqNo = appealFile.SeqNO + 1;
                SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
                                                            new SqlParameter("@SeqNO", seqNo),
                                                            new SqlParameter("@FileType", fileType),
                                                            new SqlParameter("@FileName", fileName),
                                                            new SqlParameter("@ServerFileName", serverFileName),
                                                            new SqlParameter("@InUserId]", userId)};
                Type t = typeof(int);
                string sql = @"INSERT INTO [AppealFile]
                               ([AppealId],SeqNO,[FileType],[FileName],[ServerFileName],[InUserId],[InDateTime])
                                VALUES
                               (@AppealId,@SeqNO, @FileType, @FileName, @ServerFileName, @UserId, GETDATE())";
                db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
            }

        }
        public void AppealFileDelete(string fileId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@FileId", fileId) };
            Type t = typeof(int);
            string sql = @"DELETE [AppealFile] WHERE FileId = @FileId";
            db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
    }
}