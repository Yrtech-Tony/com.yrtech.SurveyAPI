
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
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

        public void CreateAppeal(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            string sql = "";
            //sql = "DELETE Appeal WHERE ProjectId = @ProjectId ";
            sql += @"
                   INSERT INTO Appeal 
                    SELECT * FROM 
                    (SELECT A.ProjectId,A.ShopId,null AS AppealStatus,A.SubjectId,'' AppealReason,null AppealUserId,null AppealDateTime,null FeedBackStatus
                    ,'' FeedBackReason,null FeedBackUserId,null FeedBackDateTime
                    FROM Answer A INNER JOIN Subject B ON A.ProjectId = B.ProjectId AND A.SubjectId = B.SubjectId
                    WHERE A.ProjectId = @ProjectId AND A.PhotoScore<B.FullScore
                    AND EXISTS(SELECT 1 FROM RecheckStatus WHERE ProjectId = @ProjectId AND StatusCode = 'S9' AND ShopId = A.ShopId)) A
                    WHERE 1=1 AND NOT EXISTS(SELECT 1 FROM Appeal WHERE Projectid= A.ProjectId AND ShopId=A.ShopId AND
                    SubjectId = A.SubjectId )";
            sql += @" 
                    UPDATE AppealSet Set AppealCreateDateTime = GETDATE() WHERE ProjectId = @ProjectId";
            db.Database.ExecuteSqlCommand(sql,para);
        }
        /// <summary>
        /// 申诉设置查询
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<AppealSetDto> GetAppealSet(string projectId)
        {
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)};
            Type t = typeof(AppealSetDto);
            string sql = "";
            sql = @"SELECT 
                        A.ProjectId
                        ,A.ProjectCode
                        ,A.ProjectName
		                ,B.AppealStartDate
                        ,B.AppealEndDate
                        ,B.HiddenCode
                        ,B.AppealCreateDateTime
		                ,CASE WHEN HiddenCode IS NULL OR HiddenCode ='' THEN ''
			                ELSE (SELECT TOP 1 HiddenName FROM HiddenColumn WHERE HiddenCOdeGroup = '申诉模式' AND HiddenCode = B.HiddenCode )
		                END AS HiddenName
                        ,B.InDateTime
                        ,B.ModifyDateTime
                    FROM Project A LEFT JOIN AppealSet B ON A.ProjectId = B.ProjectId
                    WHERE A.ProjectId = @ProjectId ";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealSetDto>().ToList();
        }
        public void SaveAppealSet(AppealSet appealSet)
        {
            AppealSet findOne = db.AppealSet.Where(x => (x.ProjectId == appealSet.ProjectId)).FirstOrDefault();
            if (findOne == null)
            {
                appealSet.InDateTime = DateTime.Now;
                appealSet.ModifyDateTime = DateTime.Now;
                db.AppealSet.Add(appealSet);
            }
            else
            {
                findOne.AppealEndDate = appealSet.AppealEndDate;
                findOne.AppealStartDate = appealSet.AppealStartDate;
                findOne.AppealCreateDateTime = appealSet.AppealCreateDateTime;
                findOne.HiddenCode = appealSet.HiddenCode;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = appealSet.ModifyUserId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 查询经销商申诉列表_按页码
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AppealDto> GetShopAppealInfoByPage(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr,string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
            return GetShopAppealInfoByAll(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询所有的申诉列表_厂商
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
                                  ,A.[ProjectId]
                                    ,ISNULL(A.AppealStatus,1) AppealStatus
                                  ,U.[ProjectCode]
                                  ,U.[ProjectName]
                                  ,A.[ShopId]
                                  ,X.[ShopCode]
                                  ,X.[ShopName]
                                  ,A.[SubjectId]
                                  ,Y.[SubjectCode]
                                  ,Y.[CheckPoint]
                                  ,Y.Remark
                                  ,Z.[PhotoScore] AS Score
                                  ,Z.LossResult
                                  ,A.[AppealReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = AppealUserId),'') AS AppealUserName
                                  ,A.AppealUserId
                                  ,CONVERT(VARCHAR(19),[AppealDateTime],120) AS AppealDateTime
                                  ,Case 
		                          WHEN [FeedBackStatus]  = 1 THEN '同意'
		                          WHEN [FeedBackStatus] = 0 THEN '不同意'
		                          ELSE ''
	                               END AS FeedBackStatusStr
                                  ,[FeedBackStatus]
                                  ,[FeedBackReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [FeedBackUserId]),'') AS FeedBackUserName
                                  ,[FeedBackUserId]
                                  ,CONVERT(VARCHAR(19),[FeedBackDateTime],120) AS FeedBackDateTime
                              FROM [Appeal] A  INNER JOIN Shop X ON A.ShopId = X.ShopId AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')
                                                INNER JOIN [Subject] Y ON A.SubjectId = Y.SubjectId AND A.ProjectId = Y.ProjectId
                                                INNER JOIN Answer Z ON A.ProjectId = Z.ProjectId AND A.ShopId = Z.ShopId AND A.SubjectId =Z.SubjectId
                                               INNER JOIN Project U ON A.ProjectId = U.ProjectId AND A.ProjectId = @ProjectId  ";
            if (!string.IsNullOrEmpty(shopIdStr))
            {
                string[] shopIdList = shopIdStr.Split(',');
                sql += " WHERE A.ShopId IN('";
                for (int i = 0; i < shopIdList.Count(); i++)
                {
                    if (i == shopIdList.Count() - 1)
                    {
                        sql += shopIdList[i] + "'";
                    }
                    else
                    {
                        sql += shopIdList[i] + "','";
                    }
                }
                sql += ")";
            }
            else if (!string.IsNullOrEmpty(smallArea))
            {
                sql += @" 
                        INNER JOIN AreaShop C ON X.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                    WHERE D.AreaId = @SmallArea ";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                    WHERE E.AreaId = @MiddleArea ";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                    WHERE F.AreaId = @BigArea ";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                    WHERE  G.AreaId = @WideArea ";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @"INNER JOIN Shop B ON A.ShopId = B.ShopId
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                        INNER JOIN Area H ON G.ParentId = H.AreaId 
                    WHERE  H.AreaId = @BusinessType ";
            }
            else
            {
                sql += " WHERE 1=2"; // 业务类型也没有选择的情况下什么都不查询，
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AppealDto>().ToList();
        }
        /// <summary>
        /// 查询反馈信息_后台系统
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<AppealDto> GetFeedBackInfoByPage(string projectId,string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
            return GetFeedBackInfoByAll(projectId, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询反馈信息_后台系统
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<AppealDto> GetFeedBackInfoByAll(string projectId, string keyword)
        {
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@KeyWord", keyword)};
            Type t = typeof(AppealDto);
            string sql = "";
            sql = @"SELECT [AppealId]
                                  ,A.[ProjectId]
                                  ,X.[ProjectCode]
                                  ,ISNULL(A.AppealStatus,1) AppealStatus
                                  ,X.[ProjectName]
                                  ,A.[ShopId]
                                  ,B.[ShopCode]
                                  ,B.[ShopName]
                                  ,A.[SubjectId]
                                  ,C.[SubjectCode]
                                  ,C.[CheckPoint]
                                  ,D.[PhotoScore] AS Score
                                  ,C.Remark
                                  ,D.LossResult
                                  ,A.[AppealReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = AppealUserId),'') AS AppealUserName
                                  ,A.AppealUserId
                                  ,CONVERT(VARCHAR(19),[AppealDateTime],120) AS AppealDateTime
                                  ,Case 
		                          WHEN [FeedBackStatus]  = 1 THEN '同意'
		                          WHEN [FeedBackStatus] = 0 THEN '不同意'
		                          ELSE ''
	                               END AS FeedBackStatusStr
                                  ,[FeedBackStatus]
                                  ,[FeedBackReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [FeedBackUserId]),'') AS FeedBackUserName
                                  ,[FeedBackUserId]
                                  ,CONVERT(VARCHAR(19),[FeedBackDateTime],120) AS FeedBackDateTime
                              FROM [Appeal] A  INNER JOIN Shop B ON A.ShopId = B.ShopId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')
                                                INNER JOIN [Subject] C ON A.SubjectId = C.SubjectId AND A.ProjectId = C.ProjectId
                                                INNER JOIN Answer D ON A.ProjectId = D.ProjectId AND A.ShopId = D.ShopId AND A.SubjectId =D.SubjectId
                                               INNER JOIN Project X ON A.ProjectId = X.ProjectId AND A.ProjectId = @ProjectId  ";
            sql += "  ORDER BY A.ShopId,B.ShopCode,A.SubjectId,C.SubjectCode ";
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
                                  ,A.[ProjectId]
                                  ,ISNULL(A.AppealStatus,1) AppealStatus
                                  ,X.[ProjectCode]
                                  ,X.[ProjectName]
                                  ,A.[ShopId]
                                  ,B.[ShopCode]
                                  ,B.[ShopName]
                                  ,A.[SubjectId]
                                  ,C.[SubjectCode]
                                  ,C.[CheckPoint]
                                  ,C.Remark
                                  ,D.[PhotoScore] AS Score
                                  ,D.LossResult
                                  ,[AppealReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = AppealUserId),'') AS AppealUserName
                                  ,AppealUserId
                                  ,CONVERT(VARCHAR(19),[AppealDateTime],120) AS AppealDateTime
                                  ,Case 
		                          WHEN [FeedBackStatus]  = 1 THEN '同意'
		                          WHEN [FeedBackStatus] = 0 THEN '不同意'
		                          ELSE ''
	                               END AS FeedBackStatusStr
                                  ,[FeedBackStatus]
                                  ,[FeedBackReason]
                                  ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = [FeedBackUserId]),'') AS FeedBackUserName
                                  ,[FeedBackUserId]
                                  ,CONVERT(VARCHAR(19),[FeedBackDateTime],120) AS FeedBackDateTime
                              FROM [Appeal] A  INNER JOIN Shop B ON A.ShopId = B.ShopId 
                                                INNER JOIN [Subject] C ON A.SubjectId = C.SubjectId AND A.ProjectId = C.ProjectId
                                                INNER JOIN Answer D ON A.ProjectId = D.ProjectId AND A.ShopId = D.ShopId AND A.SubjectId =D.SubjectId
                                               INNER JOIN Project X ON A.ProjectId = X.ProjectId  
                              WHERE A.AppealId = @AppealId";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealDto>().ToList();
        }
        /// <summary>
        /// 申诉
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="appealReason"></param>
        /// <param name="appealUserId"></param>
        public Appeal AppealApply(Appeal appeal)
        {
            Appeal findOne = db.Appeal.Where(x => (x.AppealId == appeal.AppealId)).FirstOrDefault();
            if (findOne == null)
            {
                appeal.AppealDateTime = DateTime.Now;
                db.Appeal.Add(appeal);
            }
            else {
                findOne.AppealReason = appeal.AppealReason;
                findOne.AppealUserId = appeal.AppealUserId;
                findOne.ProjectId = appeal.ProjectId;
                findOne.ShopId = appeal.ShopId;
                findOne.AppealDateTime = DateTime.Now;
                findOne.AppealStatus = appeal.AppealStatus;
                findOne.SubjectId = appeal.SubjectId;
                appeal = findOne;
            }
            db.SaveChanges();
            return appeal;
        }
        /// <summary>
        /// 申诉反馈
        /// </summary>
        /// <param name="appealId"></param>
        /// <param name="appealReason"></param>
        /// <param name="appealUserId"></param>
        public void AppealFeedBack(Appeal appeal)
        {
            Appeal findOne = db.Appeal.Where(x => (x.AppealId == appeal.AppealId)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.FeedBackStatus = appeal.FeedBackStatus;
                findOne.FeedBackReason =appeal.FeedBackReason;
                findOne.FeedBackUserId =appeal.FeedBackUserId;
                findOne.FeedBackDateTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 申诉删除
        /// </summary>
        /// <param name="appealId"></param>
        public void AppealDelete(string appealId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealID", appealId) };
            string sql = @"DELETE Appeal WHERE AppealId = @AppealID
                        ";
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 查询申诉附件信息
        /// </summary>
        /// <param name="appealId"></param>
        /// <returns></returns>
        public List<AppealFileDto> AppealFileSearch(string appealId, string fileType)
        {
            if (fileType == null) fileType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealId", appealId), new SqlParameter("@FileType", fileType) };
            Type t = typeof(AppealFileDto);
            string sql = @"SELECT FileId,AppealId,SeqNO
                                ,Case WHEN FileType = 'FeedBack' THEN '申诉反馈'
                                      WHEN FileType = 'Shop' THEN '经销商申诉'
                                END AS FileTypeName,
                                FileType,[FileName],ServerFileName
                                ,ISNULL((SELECT AccountName FROM UserInfo WHERE Id = A.InUserId),'') AS InUserName
                                ,InUserId,InDateTime
                         FROM AppealFile A
	                    WHERE AppealId = @AppealId";
            if (!string.IsNullOrEmpty(fileType))
            {
                sql += " AND FileType = @FileType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AppealFileDto>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appealFile"></param>
        public void AppealFileSave(AppealFile appealFile)
        {
            if (appealFile.SeqNO == 0)
            {
                AppealFile findOneMax = db.AppealFile.Where(x => (x.AppealId == appealFile.AppealId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    appealFile.SeqNO = 1;
                }
                else
                {
                    appealFile.SeqNO = findOneMax.SeqNO + 1;
                }
                appealFile.InDateTime = DateTime.Now;
                db.AppealFile.Add(appealFile);

            }
            else
            {
                
            }
            db.SaveChanges();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        public void AppealFileDelete(string fileId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@FileId", fileId) };
            string sql = @"DELETE AppealFile WHERE FileId = @FileId
                        ";
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 查询申诉附件信息
        /// </summary>
        /// <param name="appealId"></param>
        /// <returns></returns>
        public List<AppealCountDto> AppealCountByShop(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(AppealCountDto);
            string sql = @"SELECT ShopId,ShopCode,ShopName,ISNULL(SUM(ApplyCount),0) AS ApplyCount,ISNULL(SUM(FeedBackCount),0) AS FeedBackCount
                           FROM 
                        (SELECT A.ShopId,B.ShopCode,B.ShopName,
                                 1 AS ApplyCount,
                                CASE WHEN A.FeedBackStatus IS NOT NULL AND A.FeedBackStatus<>'' THEN 1
                                    ELSE 0
                                END AS FeedBackCount
                         FROM Appeal A INNER JOIN Shop B ON A.ShopId = B.ShopId
                        WHERE ProjectId = @ProjectId) X GROUP BY ShopId,ShopCode,ShopName ";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealCountDto>().ToList();
        }



    }
}