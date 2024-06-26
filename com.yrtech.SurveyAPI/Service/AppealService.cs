﻿
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
        #region 申诉
        public void CreateAppeal(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            string sql = "";
            sql += @"
                   INSERT INTO Appeal 
                    SELECT * FROM 
                    (SELECT A.ProjectId,A.ShopId,null AS AppealStatus,A.SubjectId,'' LossResultImport,'' AppealReason,null AppealUserId,null AppealDateTime,null FeedBackStatus
                    ,'' FeedBackReason,null FeedBackUserId,null FeedBackDateTime
                    FROM Answer A INNER JOIN Subject B ON A.ProjectId = B.ProjectId AND A.SubjectId = B.SubjectId
                    WHERE A.ProjectId = @ProjectId AND A.PhotoScore<B.FullScore
                    AND EXISTS(SELECT 1 FROM RecheckStatus WHERE ProjectId = @ProjectId AND StatusCode = 'S9' AND ShopId = A.ShopId)) A
                    WHERE 1=1 AND NOT EXISTS(SELECT 1 FROM Appeal WHERE Projectid= A.ProjectId AND ShopId=A.ShopId AND SubjectId = A.SubjectId )";
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
        public List<AppealSetDto> GetAppealShopSet(string projectId,string shopId,string shopCode)
        {
            if (projectId == null) projectId = "";
            if (shopId == null) shopId = "";
            if (shopCode == null) shopCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@ShopId", shopId)
                                                        , new SqlParameter("@ShopCode", shopCode)};
            Type t = typeof(AppealSetDto);
            string sql = "";
            sql = @"SELECT 
                        A.ProjectId
                        ,C.ProjectCode
                        ,C.ProjectName
                        ,D.ShopId
                        ,D.ShopCode
                        ,D.ShopName
		                ,B.AppealStartDate
                        ,B.AppealEndDate
                        ,B.InDateTime
                        ,B.ModifyDateTime
                    FROM ProjectShopExamType A INNER JOIN Project C ON A.ProjectId = C.ProjectId
                                               INNER JOIN Shop D ON A.ShopId = D.ShopId
                                               LEFT JOIN AppealShopSet B ON A.ProjectId = B.ProjectId AND A.ShopId = B.ShopId
                    WHERE A.ProjectId = @ProjectId ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND A.ShopCode = @ShopCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AppealSetDto>().ToList();
        }
        public void SaveAppealShopSet(AppealShopSet appealSet)
        {
            if (appealSet.AppealEndDate != null) // 判断是否包含时间
            {
                DateTime dt;
                dt = Convert.ToDateTime(appealSet.AppealEndDate);
                if (dt.Hour == 0 && dt.Minute==0&&dt.Second==0)
                {
                    appealSet.AppealEndDate = Convert.ToDateTime(Convert.ToDateTime(appealSet.AppealEndDate).ToString("yyyy-MM-dd") + " 23:59:59");
                }
            }
            AppealShopSet findOne = db.AppealShopSet.Where(x => (x.ProjectId == appealSet.ProjectId&&x.ShopId==appealSet.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                appealSet.InDateTime = DateTime.Now;
                appealSet.ModifyDateTime = DateTime.Now;
                db.AppealShopSet.Add(appealSet);
            }
            else
            {
                findOne.AppealEndDate = appealSet.AppealEndDate;
                findOne.AppealStartDate = appealSet.AppealStartDate;
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
                                  ,A.LossResultImport
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
                                  ,(SELECT TOP 1 AppealEndDate FROM AppealShopSet WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId) AS AppealEndDate
                                  ,(SELECT TOP 1 ShopType FROM ChapterShopType I INNER JOIN Chapter L ON I.ChapterId = L.ChapterId 
                                                                                 INNER JOIN ChapterSubject K ON L.ChapterId = K.ChapterId
                                    WHERE Y.SubjectId = K.SubjectId) AS ShopType
                              FROM [Appeal] A  INNER JOIN Shop X ON A.ShopId = X.ShopId AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')
                                                INNER JOIN [Subject] Y ON A.SubjectId = Y.SubjectId AND A.ProjectId = Y.ProjectId
                                                LEFT JOIN Answer Z ON A.ProjectId = Z.ProjectId AND A.ShopId = Z.ShopId AND A.SubjectId =Z.SubjectId
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
        public List<AppealDto> GetFeedBackInfoByPage(string projectId,string keyword,string userId, string appealStatus,string feedbackStatus,int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
            return GetFeedBackInfoByAll(projectId, keyword, userId, appealStatus, feedbackStatus).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询反馈信息_后台系统
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<AppealDto> GetFeedBackInfoByAll(string projectId, string keyword,string userId,string appealStatus,string feedbackStatus)
        {
            if (keyword == null) keyword = "";
            if (appealStatus == null) appealStatus = "";
            if (feedbackStatus == null) feedbackStatus = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@AppealStatus", appealStatus)
                                                    ,new SqlParameter("@FeedbackStatus", feedbackStatus)
                                                    ,new SqlParameter("@KeyWord", keyword) };
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
                                  ,A.LossResultImport
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
                                  ,CASE WHEN EXISTS(SELECT 1 FROM AppealFile WHERE AppealId = A.AppealId AND FileType = 'Shop') 
                                        THEN CAST(1 AS BIT)
                                    ELSE CAST(0 AS BIT) END AS AppealFileChk_Apply
                                   ,CASE WHEN EXISTS(SELECT 1 FROM AppealFile WHERE AppealId = A.AppealId AND FileType = 'FeedBack') 
                                        THEN CAST(1 AS BIT)
                                    ELSE CAST(0 AS BIT) END AS AppealFileChk_FeedBack
                              FROM [Appeal] A WITH(NOLOCK) INNER JOIN Shop B ON A.ShopId = B.ShopId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')
                                                INNER JOIN [Subject] C ON A.SubjectId = C.SubjectId AND A.ProjectId = C.ProjectId
                                                LEFT JOIN Answer D ON A.ProjectId = D.ProjectId AND A.ShopId = D.ShopId AND A.SubjectId =D.SubjectId
                                               INNER JOIN Project X ON A.ProjectId = X.ProjectId AND A.ProjectId = @ProjectId  ";
                           // WHERE 1=1 ";
            //Jeep 申诉反馈临时使用
            if (!string.IsNullOrEmpty(userId))
            {
                string  areaId = "";
                string parentAreaId = "";
                
                 if (userId == "4058") // 王志
                {
                    areaId = "(478)";// 北京
                }
                else if (userId == "2990") // 胡克娟
                {
                    areaId = "(476)"; // 华北
                }
                else if (userId == "4214") // 刘伟
                {
                    areaId = "(477,473)"; // 西北和沪浙
                }
                else if (userId == "4056")// 方颖
                {
                    areaId = "(474)";// 华东
                }
                //else if (userId == "2992") // 牛博文
                //{
                //    areaId = "(473)";//沪浙
                //}
                else if (userId == "4057") // 林道
                {
                    areaId = "(475)";//华南
                }
                if (!string.IsNullOrEmpty(areaId))
                {
                    sql += " INNER JOIN AreaShop Y ON B.ShopId = Y.ShopId AND Y.AreaId IN " + areaId;
                }
                if (userId == "3333")
                {
                    parentAreaId = "(398,472)";
                }
                else if (userId == "4216")
                {
                    parentAreaId = "(398,472)";
                }
                if (!string.IsNullOrEmpty(parentAreaId))
                {
                    sql += " INNER JOIN AreaShop Y ON B.ShopId = Y.ShopId  " ;
                    sql += " INNER JOIN Area Z ON Z.AreaId = Y.AreaId AND Z.ParentId IN " + parentAreaId;
                }
            }
            sql += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(appealStatus))
            {
                sql += " AND A.AppealStatus = @AppealStatus";
            }
            if (!string.IsNullOrEmpty(feedbackStatus))
            {
                if (feedbackStatus == "0")
                {
                    sql += " AND A.FeedBackStatus = @FeedbackStatus";
                }
                else
                {
                    sql += " AND (A.FeedBackStatus = @FeedbackStatus OR A.FeedBackStatus IS NULL)";
                }
            }
           
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
                                  ,A.AppealStatus AppealStatus
                                  ,X.[ProjectCode]
                                  ,X.[ProjectName]
                                  ,ISNULL(X.ProjectShortName,X.ProjectName) ProjectShortName
                                  ,A.[ShopId]
                                  ,B.[ShopCode]
                                  ,B.[ShopName]
                                  ,A.[SubjectId]
                                  ,C.[SubjectCode]
                                  ,C.[CheckPoint]
                                  ,C.Remark
                                  ,D.[PhotoScore] AS Score
                                  ,D.LossResult
                                  ,A.LossResultImport
                                  ,D.FileResult
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
                                  ,(SELECT TOP 1 AppealEndDate FROM AppealShopSet WHERE ProjectId = A.ProjectId AND ShopId = A.ShopId) AS AppealEndDate
                              FROM [Appeal] A  INNER JOIN Shop B ON A.ShopId = B.ShopId 
                                                INNER JOIN [Subject] C ON A.SubjectId = C.SubjectId AND A.ProjectId = C.ProjectId
                                                LEFT JOIN Answer D ON A.ProjectId = D.ProjectId AND A.ShopId = D.ShopId AND A.SubjectId =D.SubjectId
                                               INNER JOIN Project X ON A.ProjectId = X.ProjectId  
                              WHERE A.AppealId = @AppealId";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealDto>().ToList();
        }
        /// <summary>
        /// 插入申诉信息,导入时使用的方法
        /// </summary>
        /// <param name="brand"></param>
        public void SaveAppeal(Appeal appeal,int? modifyUserId)
        {
            Appeal findOne = db.Appeal.Where(x => (x.ProjectId==appeal.ProjectId&&x.ShopId==appeal.ShopId&&x.SubjectId==appeal.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                db.Appeal.Add(appeal);
                db.SaveChanges();
                SaveAppealLogInfo(appeal,modifyUserId,"I");
            }
            else
            {
                findOne.LossResultImport = appeal.LossResultImport;
                db.SaveChanges();
                SaveAppealLogInfo(findOne, modifyUserId,"U");
            }
            
        }
        /// <summary>
        /// 插入申诉信息日志
        /// </summary>
        /// <param name="appeal"></param>
        public void SaveAppealLogInfo(Appeal appeal,int? userId,string status)
        {
            AppealLog appealLog = new AppealLog();
            appealLog.AppealDateTime = appeal.AppealDateTime;
            appealLog.AppealId = appeal.AppealId;
            appealLog.AppealReason = appeal.AppealReason;
            appealLog.AppealStatus = appeal.AppealStatus;
            appealLog.AppealUserId = appeal.AppealUserId;
            appealLog.FeedBackDateTime = appeal.FeedBackDateTime;
            appealLog.FeedBackReason = appeal.FeedBackReason;
            appealLog.FeedBackStatus = appeal.FeedBackStatus;
            appealLog.FeedBackUserId = appeal.FeedBackUserId;
            appealLog.LossResultImport = appeal.LossResultImport;
            appealLog.ProjectId = appeal.ProjectId;
            appealLog.ShopId = appeal.ShopId;
            appealLog.SubjectId = appeal.SubjectId;
            appealLog.DataStatus = status;
            appealLog.InUserId = userId;
            appealLog.InDateTime = DateTime.Now;
            db.AppealLog.Add(appealLog);
            db.SaveChanges();
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
                db.SaveChanges();
                SaveAppealLogInfo(appeal,appeal.AppealUserId,"I");
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
                db.SaveChanges();
                SaveAppealLogInfo(findOne,appeal.AppealUserId, "U");
            }
            
            
            return appeal;
        }
        /// <summary>
        /// 申诉反馈byAppealId
        /// </summary>
        /// <param name="appeal"></param>
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
            SaveAppealLogInfo(findOne, appeal.FeedBackUserId,"U");
        }
        public void AppealFeedBackBySubjectId(Appeal appeal)
        {
            Appeal findOne = db.Appeal.Where(x => (x.ProjectId == appeal.ProjectId&&x.ShopId==appeal.ShopId&&x.SubjectId==appeal.SubjectId)).FirstOrDefault();
            if (findOne != null)
            {
                findOne.FeedBackStatus = appeal.FeedBackStatus;
                findOne.FeedBackReason = appeal.FeedBackReason;
                findOne.FeedBackUserId = appeal.FeedBackUserId;
                findOne.FeedBackDateTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 删除反馈和申诉的信息，重新申诉
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        public void AppealFeedBackDelete(string projectId, string shopId,string userId)
        {
            if (shopId == null) shopId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@ShopId", shopId)
                                                         , new SqlParameter("@UserId", userId)};
            string sql = @"UPDATE Appeal SET AppealStatus = null
                            ,AppealReason = null
                            ,AppealUserid=null
                            ,AppealDatetime=null
                            ,FeedbackStatus=null
                            ,FeedbackReason=null
                            ,FeedbackUserid=null
                            ,FeedbackDatetime=null 
                            WHERE ProjectId=@ProjectId AND ShopId= @ShopId
                        ";
            db.Database.ExecuteSqlCommand(sql, para);

            // 更新后，插入日志
            sql = @" INSERT INTO AppealLog 
                      SELECT [AppealId]
                              ,[ProjectId]
                              ,[ShopId]
                              ,[AppealStatus]
                              ,[SubjectId]
                              ,[LossResultImport]
                              ,[AppealReason]
                              ,[AppealUserId]
                              ,[AppealDateTime]
                              ,[FeedBackStatus]
                              ,[FeedBackReason]
                              ,[FeedBackUserId]
                              ,[FeedBackDateTime]
                              ,'U'
                              ,@UserId
                              ,GETDATE()
                       FROM Appeal WHERE ProjectId=@ProjectId AND ShopId= @ShopId";
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 申诉删除
        /// </summary>
        /// <param name="appealId"></param>
        public void AppealDelete(string appealId,string userId)
        {
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AppealID", appealId), new SqlParameter("@UserId", userId) };
            // 删除前，先插入日志
            string sql = "";
            sql = @" INSERT INTO AppealLog 
                      SELECT [AppealId]
                              ,[ProjectId]
                              ,[ShopId]
                              ,[AppealStatus]
                              ,[SubjectId]
                              ,[LossResultImport]
                              ,[AppealReason]
                              ,[AppealUserId]
                              ,[AppealDateTime]
                              ,[FeedBackStatus]
                              ,[FeedBackReason]
                              ,[FeedBackUserId]
                              ,[FeedBackDateTime]
                              ,'D'
                              ,@UserId
                              ,GETDATE()
                       FROM Appeal WHERE AppealId = @AppealId";
            db.Database.ExecuteSqlCommand(sql, para);
            sql = @"DELETE Appeal WHERE AppealId = @AppealID      ";
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public void AppealDeleteByShopId(string projectId,string shopId,string userId)
        {
            if (shopId == null) shopId = "";
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                ,new SqlParameter("@ShopId", shopId),new SqlParameter("@UserId", userId) };
            // 删除前先把数据插入到日志表
            sql = @" INSERT INTO AppealLog 
                      SELECT [AppealId]
                              ,[ProjectId]
                              ,[ShopId]
                              ,[AppealStatus]
                              ,[SubjectId]
                              ,[LossResultImport]
                              ,[AppealReason]
                              ,[AppealUserId]
                              ,[AppealDateTime]
                              ,[FeedBackStatus]
                              ,[FeedBackReason]
                              ,[FeedBackUserId]
                              ,[FeedBackDateTime]
                              ,'D'
                              ,@UserId
                              ,GETDATE()
                       FROM Appeal WHERE ProjectId = @ProjectId ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
           // db.Database.ExecuteSqlCommand(sql, para);
            // 删除数据
            sql += @" DELETE Appeal WHERE ProjectId = @ProjectId        ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
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
                                CASE WHEN A.FeedBackStatus IS NOT NULL  THEN 1
                                    ELSE 0
                                END AS FeedBackCount
                         FROM Appeal A INNER JOIN Shop B ON A.ShopId = B.ShopId
                        WHERE ProjectId = @ProjectId) X GROUP BY ShopId,ShopCode,ShopName ";
            return db.Database.SqlQuery(t, sql, para).Cast<AppealCountDto>().ToList();
        }
        #endregion
    }
}