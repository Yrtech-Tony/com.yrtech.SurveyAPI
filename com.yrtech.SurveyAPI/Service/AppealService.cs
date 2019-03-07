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
        Survey db = new Survey();
        /// <summary>
        /// 生成经销商得分的信息用来申诉
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void CreateAppealInfoByProject(int projectId)
        {

            // 生成申诉的基本信息
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(Appeal);
            string sql = @"SELECT A.ProjectId,B.ProjectCode,B.ProjectName
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
            List<Appeal> appealList = db.Database.SqlQuery(t, sql, para).Cast<Appeal>().ToList();
            foreach (Appeal appeal in appealList)
            {
                db.Appeal.Add(appeal);
            }
            // 判断是导入的还是系统打分
            Project findOne = db.Project.Where(x => x.ProjectId == projectId).FirstOrDefault();
            if (findOne != null)
            {
                if (findOne.DataScore == "01")// 01:系统打分 02：导入
                {
                    //系统打分的时候，计算总分和失分说明，进行更新
                }
                // 更新申诉开始时间。
                findOne.AppealStartDate = DateTime.Now;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 查询经销商申诉列表_按页码
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AppealDto> GetShopAppealInfoByPage(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string keyword, string shopIdStr, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount + 1;
            return GetShopAppealInfoByAll(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, keyword, shopIdStr).Skip(startIndex + 1).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询所有的申诉列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopIdStr"></param>
        /// <returns></returns>
        public List<AppealDto> GetShopAppealInfoByAll(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
        {
            if (bussinessType == null) bussinessType = "";
            if (wideArea == null) wideArea = "";
            if (bigArea == null) bigArea = "";
            if (middleArea == null) middleArea = "";
            if (smallArea == null) smallArea = "";
            if (shopIdStr == null) shopIdStr = "";
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@SmallArea", smallArea),
                                                        new SqlParameter("@MiddleArea", middleArea),
                                                        new SqlParameter("@BigArea", bigArea),
                                                        new SqlParameter("@WideArea", wideArea),
                                                        new SqlParameter("@BusinessType", bussinessType),
                                                    new SqlParameter("@KeyWord", keyword)};
            Type t = typeof(AppealDto);
            string sql = "";
            sql = @"SELECT [AppealId]
                                  ,[ProjectId]
                                  ,[ProjectCode]
                                  ,[ProjectName]
                                  ,A.[ShopId]
                                  ,A.[ShopCode]
                                  ,A.[ShopName]
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
                              FROM [Appeal] A  ";
            if (!string.IsNullOrEmpty(shopIdStr))
            {
                string[] shopIdList = shopIdStr.Split(';');
                sql += " WHERE ProjectId = @ProjectId AND (A.ShopCode LIKE '%'+@KeyWord+'%' OR A.ShopName LIKE '%'+@KeyWord+'%') AND ShopId IN(";
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
            else if (!string.IsNullOrEmpty(smallArea))
            {
                sql += @" INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId -- smallArea
                    WHERE D.AreaId = @SmallArea AND ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                    WHERE ProjectId = @ProjectId AND E.AreaId = @MiddleArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                        INNER JOIN Area F ON E.ParentId = F.AreaId --bigArea
                    WHERE ProjectId = @ProjectId AND F.AreaId = @BigArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                        INNER JOIN Area F ON E.ParentId = F.AreaId --bigArea
                        INNER JOIN Area G ON F.ParentId = G.AreaId --WideArea
                    WHERE ProjectId = @ProjectId AND G.AreaId = @WideArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                        INNER JOIN Area F ON E.ParentId = F.AreaId --bigArea
                        INNER JOIN Area G ON F.ParentId = G.AreaId --WideArea
                        INNER JOIN Area H ON G.ParentId = H.AreaId --businessType
                    WHERE ProjectId = @ProjectId AND H.AreaId = @BusinessType AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
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
        public List<AppealDto> GetShopSubjectAppeal(string appealId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId) };
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
                              WHERE AppealId = @AppealId";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealDto>().ToList();
        }
        /// <summary>
        /// 申诉
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="appealReason"></param>
        /// <param name="appealUserId"></param>
        public void AppealApply(int appealId, string appealReason, int? appealUserId)
        {
            Appeal findOne = db.Appeal.Where(x => (x.AppealId == appealId)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.AppealReason = appealReason;
                findOne.AppealUserId = appealUserId;
                findOne.AppealDateTime = DateTime.Now;
            }
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
            //                                            new SqlParameter("@AppealReason", appealReason),
            //                                            new SqlParameter("@AppealUserId", appealUserId)};
            //Type t = typeof(int);
            //string sql = @"UPDATE Appeal SET AppealReason = @AppealReason,AppealUserId = @AppealUserId,AppealDateTime = GETDATE() 
            //               WHERE AppealId = @AppealId";
            //db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        /// <summary>
        /// 申诉反馈
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="appealReason"></param>
        /// <param name="appealUserId"></param>
        public void AppealFeedBack(int appealId, bool? feedbackStatus, string feedbackReason, int? feedbackUserId)
        {
            Appeal findOne = db.Appeal.Where(x => (x.AppealId == appealId)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.FeedBackStatus = feedbackStatus;
                findOne.FeedBackReason = feedbackReason;
                findOne.FeedBackUserId = feedbackUserId;
                findOne.FeedBackDateTime = DateTime.Now;
            }
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
            //                                            new SqlParameter("@FeedBackStatus", feedbackStatus),
            //                                            new SqlParameter("@FeedBackReason", feedbackReason),
            //                                            new SqlParameter("@FeedBackUserId", feedbackUserId)};
            //Type t = typeof(int);
            //string sql = @"UPDATE Appeal SET FeedBackStatus = @FeedBackStatus,FeedBackReason = @FeedBackReason,
            //                    FeedBackUserId = @FeedBackUserId,FeedBackDateTime = GETDATE() 
            //               WHERE AppealId = @AppealId";
            //db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        /// <summary>
        /// 经销商接受情况
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="shopAcceptStatus"></param>
        /// <param name="shopAcceptReason"></param>
        /// <param name="shopAcceptUserId"></param>
        public void AppealShopAccept(int appealId, bool? shopAcceptStatus, string shopAcceptReason, int? shopAcceptUserId)
        {
            Appeal findOne = db.Appeal.Where(x => (x.AppealId == appealId)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.ShopAcceptStatus = shopAcceptStatus;
                findOne.ShopAcceptReason = shopAcceptReason;
                findOne.ShopAcceptUserId = shopAcceptUserId;
                findOne.ShopAcceptDateTime = DateTime.Now;
            }
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
            //                                            new SqlParameter("@ShopAcceptStatus", shopAcceptStatus),
            //                                            new SqlParameter("@ShopAcceptReason", shopAcceptReason),
            //                                            new SqlParameter("@ShopAcceptUserId", shopAcceptUserId)};
            //Type t = typeof(int);
            //string sql = @"UPDATE Appeal SET ShopAcceptStatus = @ShopAcceptStatus,ShopAcceptReason = @ShopAcceptReason,
            //                    ShopAcceptUserId = @ShopAcceptUserId,ShopAcceptDateTime = GETDATE() 
            //               WHERE AppealId = @AppealId";
            //db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        /// <summary>
        /// 查询申诉附件信息
        /// </summary>
        /// <param name="appealId"></param>
        /// <returns></returns>
        public List<AppealFileDto> AppealFileSearch(string appealId, string fileType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId), new SqlParameter("@FileType", fileType) };
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
            if (!string.IsNullOrEmpty(fileType))
            {
                sql += " AND FileType = @FileType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AppealFileDto>().ToList();
        }
        public void AppealFileSave(int appealId, string fileType, string fileName, string serverFileName, int userId)
        {
            int seqNo = 1;
            SqlParameter[] paraMax = new SqlParameter[] { new SqlParameter("@AppealId", appealId) };
            string sql_Max = "SELECT TOP 1 * FROM AppealFile WHERE AppealId = @AppealId";
            Type t_max = typeof(AppealFile);
            AppealFile appealFile = db.Database.SqlQuery(t_max, sql_Max, paraMax).Cast<AppealFile>().ToList().FirstOrDefault();
            if (appealFile != null && appealFile.SeqNO > 0)
            {
              seqNo = appealFile.SeqNO + 1;
            }

            AppealFile findOne = new AppealFile();
            findOne.AppealId = appealId;
            findOne.InDateTime = DateTime.Now;
            findOne.InUserId = userId;
            findOne.SeqNO = seqNo;
            findOne.ServerFileName = serverFileName;
            findOne.FileName = fileName;
            findOne.FileType = fileType;
            db.AppealFile.Add(findOne);
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId),
            //                                                new SqlParameter("@SeqNO", seqNo),
            //                                                new SqlParameter("@FileType", fileType),
            //                                                new SqlParameter("@FileName", fileName),
            //                                                new SqlParameter("@ServerFileName", serverFileName),
            //                                                new SqlParameter("@InUserId]", userId)};
            //Type t = typeof(int);
            //string sql = @"INSERT INTO [AppealFile]
            //                   ([AppealId],SeqNO,[FileType],[FileName],[ServerFileName],[InUserId],[InDateTime])
            //                    VALUES
            //                   (@AppealId,@SeqNO, @FileType, @FileName, @ServerFileName, @UserId, GETDATE())";
            //db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();

        }
        public void AppealFileDelete(int fileId)
        {
            AppealFile findone = db.AppealFile.Where(x => x.FileId == fileId).FirstOrDefault();
            db.AppealFile.Remove(findone);
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@FileId", fileId) };
            //Type t = typeof(int);
            //string sql = @"DELETE [AppealFile] WHERE FileId = @FileId";
            //db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
    }
}