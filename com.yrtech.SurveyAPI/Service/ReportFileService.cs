using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.SurveyAPI.Service
{
    public class ReportFileService
    {
        Survey db = new Survey();
        /// <summary>
        /// 文件上传查询
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLSearch(string projectId, string keyword)
        {
            if (projectId == null) projectId = "";
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@Keyword", keyword) };
            Type t = typeof(ReportFileUploadDto);
            string sql = @"
                        SELECT ProjectId,ShopId,ShopCode,ShopName,ShopShortName
		                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
		                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video
                        FROM(
		                        SELECT A.ProjectId,A.ShopId,C.ShopCode,C.ShopName,C.ShopShortName,
				                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
				                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
		                        FROM ReportFile A INNER JOIN Project B ON A.ProjectId = B.ProjectId
				                                 INNER JOIN Shop C ON A.ShopId = C.ShopId
				                            ) X
                        WHERE 1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND X.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (X.ShopCode LIKE '%'+@Keyword+'%' OR X.ShopName LIKE '%'+@Keyword+'%)'";
            }
            sql += " GROUP BY X.ProjectId,X.ShopId,X.ShopCode,X.ShopName,X.ShopShortName";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileUploadDto>().ToList();
        }
        public List<ReportFileUploadDto> ReportFileCountYear()
        {
            SqlParameter[] para = new SqlParameter[] {  };
            Type t = typeof(ReportFileUploadDto);
            string sql = @"
                        SELECT ProjectId,ProjectCode,ProjectName
                                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
                                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video
                        FROM(
                                SELECT A.ProjectId,B.ProjectCode,B.ProjectName,
                                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
                                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
                                FROM ReportFile A INNER JOIN Project B ON A.ProjectId = B.ProjectId AND B.[Year] = YEAR(GETDATE())
                                                 INNER JOIN Shop C ON A.ShopId = C.ShopId
                                            ) X
                        WHERE 1=1
                        GROUP BY X.ProjectId,X.ProjectCode,X.ProjectName";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileUploadDto>().ToList();
        }
        public List<ReportFileUploadDto> ReportFileListUploadALLByPageSearch(string projectId, string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
           
            return ReportFileListUploadALLSearch(projectId, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询特定经销商的文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<ReportFile> ReportFileSearch(string projectId, string shopId)
        {
            if (projectId == null) projectId = "";
            if (shopId == null) shopId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@ShopId", shopId) };
            Type t = typeof(ReportFile);
            string sql = @"
                        SELECT * FROM ReportFile 
                        WHERE 1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFile>().ToList();
        }
        public ReportFile ReportFileSave(ReportFile reportFile)
        {
            if (reportFile.SeqNO == 0)
            {
                ReportFile findOneMax = db.ReportFile.Where(x => (x.ProjectId == reportFile.ProjectId&&x.ShopId==reportFile.ShopId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    reportFile.SeqNO = 1;
                }
                else
                {
                    reportFile.SeqNO = findOneMax.SeqNO + 1;
                }
                reportFile.InDateTime = DateTime.Now;
                db.ReportFile.Add(reportFile);

            }
            else
            {
                ReportFile findOne = db.ReportFile.Where(x => (x.ProjectId == reportFile.ProjectId&&x.ShopId==reportFile.ShopId && x.SeqNO == reportFile.SeqNO)).FirstOrDefault();
                if (findOne == null)
                {
                    reportFile.InDateTime = DateTime.Now;
                    db.ReportFile.Add(reportFile);
                }
                else
                {
                    findOne.ReportFileName = reportFile.ReportFileName;
                    findOne.ReportFileType = reportFile.ReportFileType;
                    findOne.Url_OSS = reportFile.Url_OSS;
                }
            }
            db.SaveChanges();
            return reportFile;

        }
        public void ReportFileDelete(string projectId,string shopId,string seqNO)
        {
            if (seqNO == null||seqNO=="0") seqNO = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@ShopId", shopId), new SqlParameter("@SeqNO", seqNO)};
            string sql = @"DELETE ReportFile WHERE ProjectId = @ProjectId   
                        ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(seqNO))
            {
                sql += " AND SeqNO = @SeqNO ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }

        /// <summary>
        /// 报告下载查询
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
        public List<ReportFileDto> ReportFileDownloadAllSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
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
            Type t = typeof(ReportFileDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,A.ReportFileType,A.ReportFileName,A.Url_OSS,A.InDateTime
                    FROM ReportFile A INNER JOIN Shop B ON A.ShopId = B.ShopId " ;
            if (!string.IsNullOrEmpty(shopIdStr))
            {
                string[] shopIdList = shopIdStr.Split(';');
                sql += " WHERE A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%') AND A.ShopId IN('";
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
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId -- smallArea
                    WHERE D.AreaId = @SmallArea AND A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                    WHERE A.ProjectId = @ProjectId AND E.AreaId = @MiddleArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                        INNER JOIN Area F ON E.ParentId = F.AreaId --bigArea
                    WHERE A.ProjectId = @ProjectId AND F.AreaId = @BigArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                        INNER JOIN Area F ON E.ParentId = F.AreaId --bigArea
                        INNER JOIN Area G ON F.ParentId = G.AreaId --WideArea
                    WHERE A.ProjectId = @ProjectId AND G.AreaId = @WideArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId --smallArea
                        INNER JOIN Area E ON D.ParentId = E.AreaId -- middleArea
                        INNER JOIN Area F ON E.ParentId = F.AreaId --bigArea
                        INNER JOIN Area G ON F.ParentId = G.AreaId --WideArea
                        INNER JOIN Area H ON G.ParentId = H.AreaId --businessType
                    WHERE A.ProjectId = @ProjectId AND H.AreaId = @BusinessType AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else
            {
                sql += " WHERE A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileDto>().ToList();
        }
        public List<ReportFileDto> ReportFileDownloadAllByPageSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ReportFileDownloadAllSearch(projectId,bussinessType,wideArea,bigArea,middleArea,smallArea,shopIdStr,keyword).Skip(startIndex).Take(pageCount).ToList();
        }
    }
}