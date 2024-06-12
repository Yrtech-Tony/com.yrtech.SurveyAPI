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
    public class ImproveService
    {
        Survey db = new Survey();
        public Improve ImproveSave(Improve improve)
        {
            Improve findOne = db.Improve.Where(x => (x.ProjectId == improve.ProjectId
                                                     && x.ShopId == improve.ShopId
                                                     && x.SubjectId == improve.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                improve.InDateTime = DateTime.Now;
                db.Improve.Add(improve);
                db.SaveChanges();
            }
            else
            {
                findOne.ImproveContent = improve.ImproveContent;
                findOne.ImproveCycle = improve.ImproveCycle;
                findOne.ImproveStatus = improve.ImproveStatus;
                findOne.ModifyUserId = improve.ModifyUserId;
                findOne.ModifyDateTime = DateTime.Now;
                improve = findOne;
                db.SaveChanges();
            }
            return improve;
        }
        /// <summary>
        /// 按经销商删除改善
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="userId"></param>
        public void DeleteImproveByShopId(string projectId, string shopId, string userId)
        {
            if (shopId == null) shopId = "";
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                ,new SqlParameter("@ShopId", shopId),new SqlParameter("@UserId", userId) };

            sql += @" DELETE Improve WHERE ProjectId = @ProjectId        ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 按期号查询改善信息-后台
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ImproveDto> GetImprove(string projectId, string keyword, string improveStatus, string userId)
        {
            if (keyword == null) keyword = "";
            if (userId == null) userId = "";
            if (improveStatus == null) improveStatus = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@KeyWord", keyword) };
            Type t = typeof(ImproveDto);
            string sql = "";
            sql = @"SELECT [ImproveId]
                                  ,A.[ProjectId]
                                  ,X.[ProjectCode]
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
                                  ,Case 
		                          WHEN [ImproveStatus]  = 1 THEN '已完结'
		                          ELSE '未完结'
	                               END AS ImproveStatusStr
                                  ,[ImproveStatus]
                                  ,[ImproveContent]
                              FROM [Improve] A WITH(NOLOCK) INNER JOIN Shop B ON A.ShopId = B.ShopId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')
                                                INNER JOIN [Subject] C ON A.SubjectId = C.SubjectId AND A.ProjectId = C.ProjectId
                                                LEFT JOIN Answer D ON A.ProjectId = D.ProjectId AND A.ShopId = D.ShopId AND A.SubjectId =D.SubjectId
                                               INNER JOIN Project X ON A.ProjectId = X.ProjectId AND A.ProjectId = @ProjectId  ";
            sql += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(improveStatus))
            {
                sql += " AND A.ImproveStatus = @ImproveStatus";
            }
            sql += "  ORDER BY A.ShopId,B.ShopCode,A.SubjectId,C.SubjectCode ";
            return db.Database.SqlQuery(t, sql, para).Cast<ImproveDto>().ToList();
        }
        /// <summary>
        /// 查询改善信息-厂商
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="bussinessType"></param>
        /// <param name="wideArea"></param>
        /// <param name="bigArea"></param>
        /// <param name="middleArea"></param>
        /// <param name="smallArea"></param>
        /// <param name="shopIdStr"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ImproveDto> GetShopImprove(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
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
            Type t = typeof(ImproveDto);
            string sql = "";
            sql = @"SELECT [ImproveId]
                                  ,A.[ProjectId]
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
                                  ,Case WHEN [ImproveStatus]  = 1 THEN '已完结'
		                          ELSE '未完结'
	                               END AS ImproveStatusStr
                                  ,[ImproveStatus]
                                  ,[ImproveContent]
                              FROM [Improve] A  INNER JOIN Shop X ON A.ShopId = X.ShopId AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')
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
            return db.Database.SqlQuery(t, sql, para).Cast<ImproveDto>().ToList();
        }
        /// <summary>
        /// 改善详细保存
        /// </summary>
        /// <param name="improveDetail"></param>
        /// <returns></returns>
        public ImproveDetail ImproveDetailSave(ImproveDetail improveDetail)
        {
            if (improveDetail.SeqNO == 0)
            {
                ImproveDetail findOneMax = db.ImproveDetail.Where(x => (x.ImproveId == improveDetail.ImproveId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    improveDetail.SeqNO = 1;
                }
                else
                {
                    improveDetail.SeqNO = findOneMax.SeqNO + 1;
                }
                improveDetail.InDateTime = DateTime.Now;
                db.ImproveDetail.Add(improveDetail);
                db.SaveChanges();
            }
            else
            {
                ImproveDetail findOne = db.ImproveDetail.Where(x => (x.ImproveId == improveDetail.ImproveId && x.SeqNO == improveDetail.SeqNO)).FirstOrDefault();
                findOne.ImproveDesc = improveDetail.ImproveDesc;
                findOne.StartDate = improveDetail.StartDate;
                findOne.EndDate = improveDetail.EndDate;
                findOne.ImproveFeedBackDateTime = DateTime.Now;
                findOne.ImproveFeedBackStatus = improveDetail.ImproveFeedBackStatus;
                findOne.ImproveFeedBackUserId = improveDetail.ImproveFeedBackUserId;
                findOne.CommitDateTime = improveDetail.CommitDateTime;
                findOne.CommitUserId = improveDetail.CommitUserId;
                improveDetail = findOne;
                db.SaveChanges();
            }
            return improveDetail;
        }
        /// <summary>
        /// 查询改善详细
        /// </summary>
        /// <param name="improveId"></param>
        /// <returns></returns>
        public List<ImproveDetail> GetShopImproveDetail(string improveId)
        {
            if (improveId == null) improveId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ImproveId", improveId) };
            Type t = typeof(ImproveDetail);
            string sql = "";
            sql = @"SELECT 
                        A.ImproveId
                        ,A.SeqNO
		                ,A.StartDate
                        ,A.EndDate
                        ,A.ImproveDesc
                        ,A.CommitDateTime
                        ,A.ImproveFeedBackStatus
                        ,A.ImproveFeedBackDesc
                        ,A.ImproveFeedBackDateTime
                    FROM ImproveDetail A INNER JOIN Improve B ON A.ImproveId = B.ImproveId
                    WHERE A.ImproveId = @ImproveId ";
            return db.Database.SqlQuery(t, sql, para).Cast<ImproveDetail>().ToList();
        }
        /// <summary>
        /// 改善详细附件保存
        /// </summary>
        /// <param name="improveFile"></param>
        public void ImproveFileSave(ImproveFile improveFile)
        {
            if (improveFile.FileSeqNO == 0)
            {
                ImproveFile findOneMax = db.ImproveFile.Where(x => (x.ImproveId == improveFile.ImproveId && x.SeqNO == improveFile.SeqNO)).OrderByDescending(x => x.FileSeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    improveFile.FileSeqNO = 1;
                }
                else
                {
                    improveFile.SeqNO = findOneMax.FileSeqNO + 1;
                }
                improveFile.InDateTime = DateTime.Now;
                db.ImproveFile.Add(improveFile);

            }
            else
            {

            }
            db.SaveChanges();

        }
        /// <summary>
        /// 改善详细附件删除
        /// </summary>
        /// <param name="improveFileId"></param>
        public void ImproveFileDelete(string improveFileId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ImproveFileId", improveFileId) };
            string sql = @"DELETE ImproveFile WHERE ImproveFileId = @ImproveFileId
                        ";
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 查询改善详细附件
        /// </summary>
        /// <param name="improveId"></param>
        /// <returns></returns>
        public List<ImproveFile> GetShopImproveDetailFile(string improveId, string seqNO)
        {
            if (improveId == null) improveId = "";
            if (seqNO == null) seqNO = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ImproveId", improveId)
                                                    , new SqlParameter("@SeqNO", seqNO) };
            Type t = typeof(ImproveFile);
            string sql = "";
            sql = @"SELECT 
                        A.ImproveId
                        ,A.SeqNO
		                ,A.StartDate
                        ,A.EndDate
                        ,A.ImproveDesc
                        ,A.CommitDateTime
                        ,A.ImproveFeedBackStatus
                        ,A.ImproveFeedBackDesc
                        ,A.ImproveFeedBackDateTime
                    FROM ImproveFile A INNER JOIN ImproveDetail B ON A.ImproveId = B.ImproveId AND A.SeqNO = B.SeqNO
                    WHERE A.ImproveId = @ImproveId ";
            if (!string.IsNullOrEmpty(seqNO))
            {
                sql += " AND A.SeqNO = @SeqNO";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ImproveFile>().ToList();
        }
    }
}