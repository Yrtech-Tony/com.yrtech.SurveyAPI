using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace com.yrtech.SurveyAPI.Service
{
    public class ReportFileService
    {
        Survey db = new Survey();
        #region 报告设置
        public List<ReportSetDto> GetReportSet(string projectId)
        {
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(ReportSetDto);
            string sql = "";
            sql = @"SELECT 
                        A.ProjectId
                        ,A.ProjectCode
                        ,A.ProjectName
                        ,B.InDateTime
                        ,B.ModifyDateTime
                    FROM Project A LEFT JOIN ReportSet B ON A.ProjectId = B.ProjectId
                    WHERE A.ProjectId = @ProjectId ";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportSetDto>().ToList();
        }
        public void SaveReportSet(ReportSet reportSet)
        {
            ReportSet findOne = db.ReportSet.Where(x => (x.ProjectId == reportSet.ProjectId)).FirstOrDefault();
            if (findOne == null)
            {
                reportSet.InDateTime = DateTime.Now;
                reportSet.ModifyDateTime = DateTime.Now;
                db.ReportSet.Add(reportSet);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = reportSet.ModifyUserId;
            }
            db.SaveChanges();
        }
        #endregion
        #region 首页
        /// <summary>
        /// 首页报告统计查询
        /// </summary>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileCountYear(string tenantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId), };
            Type t = typeof(ReportFileUploadDto);
            string sql = @"
                        SELECT ProjectId,ProjectCode,ProjectName
                                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
                                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video
                        FROM(
                                SELECT A.ProjectId,B.ProjectCode,B.ProjectName,
                                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
                                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
                                FROM ReportFile A INNER JOIN Project B ON A.ProjectId = B.ProjectId AND B.[Year] = YEAR(GETDATE()) AND B.TenantId = @TenantId
                                                 INNER JOIN Shop C ON A.ShopId = C.ShopId 
                                            ) X
                        WHERE 1=1
                        GROUP BY X.ProjectId,X.ProjectCode,X.ProjectName";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileUploadDto>().ToList();
        }
        #endregion
        #region 报告上传
        /// <summary>
        /// 文件上传查询-经销商报告
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLSearch(string brandId, string projectId, string bussinessTypeId, string keyword)
        {
            if (brandId == null) brandId = "";
            if (projectId == null) projectId = "";
            if (bussinessTypeId == null) bussinessTypeId = "";
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@Keyword", keyword)
                                                        , new SqlParameter("@BrandId", brandId)
                                                        , new SqlParameter("@BussinessTypeId", bussinessTypeId)};
            Type t = typeof(ReportFileUploadDto);
            string sql = @"
                        SELECT ProjectId,BussinessTypeId,ShopId,ShopCode,ShopName,ShopShortName
		                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
		                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video INTO #T
                            FROM(
		                        SELECT A.ProjectId,BussinessTypeId,A.ShopId,C.ShopCode,C.ShopName,C.ShopShortName,
				                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
				                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
		                        FROM ReportFile A INNER JOIN Project B ON A.ProjectId = B.ProjectId
				                                 INNER JOIN Shop C ON A.ShopId = C.ShopId
				                            ) X
                        WHERE 1=1
                        GROUP BY X.ProjectId,BussinessTypeId,X.ShopId,X.ShopCode,X.ShopName,X.ShopShortName";
            sql += @" SELECT ShopId,ShopCode,ShopName,ShopShortName
                       ,ISNULL((SELECT ReportFileCount_File FROM #T WHERE ShopId = A.ShopId AND ProjectId = @ProjectId AND BussinessTypeId = @BussinessTypeId),0) AS ReportFileCount_File,
                        ISNULL((SELECT ReportFileCount_Video FROM #T WHERE ShopId = A.ShopId AND ProjectId = @ProjectId AND BussinessTypeId = @BussinessTypeId),0) ReportFileCount_Video
                      FROM Shop A WHERE 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (A.ShopCode LIKE '%'+@Keyword+'%' OR A.ShopName LIKE '%'+@Keyword+'%')";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileUploadDto>().ToList();
        }
        /// <summary>
        /// 文件上传查询-区域报告
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLSearch_Area(string brandId, string projectId, string keyword)
        {
            if (brandId == null) brandId = "";
            if (projectId == null) projectId = "";
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@Keyword", keyword)
                                                        , new SqlParameter("@BrandId", brandId)};
            Type t = typeof(ReportFileUploadDto);
            string sql = @"
                         SELECT ProjectId,AreaId,AreaCode,AreaName
		                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
		                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video INTO #T
                            FROM(
		                        SELECT A.ProjectId,A.AreaId,C.AreaCode,C.AreaName,
				                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
				                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
		                        FROM ReportFileArea A INNER JOIN Project B ON A.ProjectId = B.ProjectId
				                                 INNER JOIN Area C ON A.AreaId = C.AreaId
				                            ) X
                        WHERE 1=1
                        GROUP BY X.ProjectId,X.AreaId,X.AreaCode,X.AreaName";
            sql += @" SELECT AreaId,AreaCode,AreaName
                       ,ISNULL((SELECT ReportFileCount_File FROM #T WHERE AreaId = A.AreaId AND ProjectId = @ProjectId),0) AS ReportFileCount_File,
                        ISNULL((SELECT ReportFileCount_Video FROM #T WHERE AreaId = A.AreaId AND ProjectId = @ProjectId ),0) ReportFileCount_Video
                      FROM Area A WHERE 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (A.AreaCode LIKE '%'+@Keyword+'%' OR A.AreaName LIKE '%'+@Keyword+'%')";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileUploadDto>().ToList();
        }
        /// <summary>
        /// 按页码查询
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLByPageSearch(string brandId, string projectId, string bussinessTypeId, string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
            return ReportFileListUploadALLSearch(brandId, projectId, bussinessTypeId, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        public List<ReportFileUploadDto> ReportFileListUploadALLByPageSearch_Area(string brandId, string projectId, string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
            return ReportFileListUploadALLSearch_Area(brandId, projectId, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询特定经销商的文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<ReportFile> ReportFileSearch(string projectId, string bussinessTypeId, string shopId, string reportFileType)
        {
            if (projectId == null) projectId = "";
            if (shopId == null || shopId == "0") shopId = "";
            if (reportFileType == null) reportFileType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@ShopId", shopId)
                                                        , new SqlParameter("@BussinessTypeId", bussinessTypeId)
                                                     , new SqlParameter("@ReportFileType", reportFileType)};
            Type t = typeof(ReportFile);
            string sql = @"
                        SELECT * FROM ReportFile 
                        WHERE 1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(bussinessTypeId))
            {
                sql += " AND BussinessTypeId = @BussinessTypeId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(reportFileType))
            {
                sql += " AND ReportFileType = @ReportFileType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFile>().ToList();
        }
        /// <summary>
        /// 查询特定区域的文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="areaId"></param>
        /// <param name="reportFileType"></param>
        /// <returns></returns>
        public List<ReportFileArea> ReportFileSearch_Area(string projectId, string areaId, string reportFileType)
        {
            if (projectId == null) projectId = "";
            if (areaId == null || areaId == "0") areaId = "";
            if (reportFileType == null) reportFileType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@AreaId", areaId)
                                                     , new SqlParameter("@ReportFileType", reportFileType)};
            Type t = typeof(ReportFileArea);
            string sql = @"
                        SELECT * FROM ReportFileArea 
                        WHERE 1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND AreaId = @AreaId";
            }
            if (!string.IsNullOrEmpty(reportFileType))
            {
                sql += " AND ReportFileType = @ReportFileType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileArea>().ToList();
        }
        /// <summary>
        /// 报告文件保存
        /// </summary>
        /// <param name="reportFile"></param>
        /// <returns></returns>
        public ReportFile ReportFileSave(ReportFile reportFile)
        {
            if (reportFile.SeqNO == 0)
            {
                ReportFile findOneMax = db.ReportFile.Where(x => (x.ProjectId == reportFile.ProjectId && x.ShopId == reportFile.ShopId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
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
                ReportFile findOne = db.ReportFile.Where(x => (x.ProjectId == reportFile.ProjectId && x.ShopId == reportFile.ShopId && x.SeqNO == reportFile.SeqNO)).FirstOrDefault();
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
        /// <summary>
        /// 报告文件保存_Area
        /// </summary>
        /// <param name="reportFile"></param>
        /// <returns></returns>
        public ReportFileArea ReportFileSave_Area(ReportFileArea reportFile)
        {
            if (reportFile.SeqNO == 0)
            {
                ReportFileArea findOneMax = db.ReportFileArea.Where(x => (x.ProjectId == reportFile.ProjectId && x.AreaId == reportFile.AreaId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    reportFile.SeqNO = 1;
                }
                else
                {
                    reportFile.SeqNO = findOneMax.SeqNO + 1;
                }
                reportFile.InDateTime = DateTime.Now;
                db.ReportFileArea.Add(reportFile);

            }
            else
            {
                ReportFileArea findOne = db.ReportFileArea.Where(x => (x.ProjectId == reportFile.ProjectId && x.AreaId == reportFile.AreaId && x.SeqNO == reportFile.SeqNO)).FirstOrDefault();
                if (findOne == null)
                {
                    reportFile.InDateTime = DateTime.Now;
                    db.ReportFileArea.Add(reportFile);
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
        /// <summary>
        /// 报告文件删除
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="seqNO"></param>
        public void ReportFileDelete(string projectId, string shopId, string seqNO)
        {
            if (seqNO == null || seqNO == "0") seqNO = "";
            if (shopId == null || shopId == "0") shopId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@ShopId", shopId), new SqlParameter("@SeqNO", seqNO) };
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
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="areaId"></param>
        /// <param name="seqNO"></param>
        public void ReportFileDelete_Area(string projectId, string areaId, string seqNO)
        {
            if (seqNO == null || seqNO == "0") seqNO = "";
            if (areaId == null || areaId == "0") areaId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@AreaId", areaId),
                                                        new SqlParameter("@SeqNO", seqNO) };
            string sql = @"DELETE ReportFileArea WHERE ProjectId = @ProjectId   
                        ";
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND AreaId = @AreaId";
            }
            if (!string.IsNullOrEmpty(seqNO))
            {
                sql += " AND SeqNO = @SeqNO ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion
        #region 报告下载
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
        public List<ReportFileDto> ReportFileDownloadAllSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType)
        {
            if (bussinessType == null) bussinessType = "";
            if (wideArea == null) wideArea = "";
            if (bigArea == null) bigArea = "";
            if (middleArea == null) middleArea = "";
            if (smallArea == null) smallArea = "";
            if (shopIdStr == null) shopIdStr = "";
            if (keyword == null) keyword = "";
            if (reportFileType == null) reportFileType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@SmallArea", smallArea),
                                                        new SqlParameter("@MiddleArea", middleArea),
                                                        new SqlParameter("@BigArea", bigArea),
                                                        new SqlParameter("@WideArea", wideArea),
                                                        new SqlParameter("@BussinessType", bussinessType),
                                                    new SqlParameter("@KeyWord", keyword), new SqlParameter("@ReportFileType", reportFileType)};
            Type t = typeof(ReportFileDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,B.ShopShortName,A.ReportFileType,A.ReportFileName,A.Url_OSS,A.InDateTime
                    FROM ReportFile A INNER JOIN Shop B ON A.ShopId = B.ShopId AND A.BussinessTypeId = @BussinessType";
            if (!string.IsNullOrEmpty(shopIdStr))
            {
                string[] shopIdList = shopIdStr.Split(',');
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
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                    WHERE D.AreaId = @SmallArea AND A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                    WHERE A.ProjectId = @ProjectId AND E.AreaId = @MiddleArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                    WHERE A.ProjectId = @ProjectId AND F.AreaId = @BigArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                    WHERE A.ProjectId = @ProjectId AND G.AreaId = @WideArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                        INNER JOIN Area H ON G.ParentId = H.AreaId 
                    WHERE A.ProjectId = @ProjectId AND H.AreaId = @BussinessType AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else
            {
                sql += " WHERE 1=2"; // 业务类型也没有选择的情况下什么都不查询，未设置区域信息不能查询数据
                                     // sql += " WHERE A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            if (!string.IsNullOrEmpty(reportFileType))
            {
                sql += " AND A.ReportFileType = @ReportFileType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileDto>().ToList();
        }
        /// <summary>
        /// 报告下载查询-区域
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="bussinessType"></param>
        /// <param name="wideArea"></param>
        /// <param name="bigArea"></param>
        /// <param name="middleArea"></param>
        /// <param name="smallArea"></param>
        /// <param name="keyword"></param>
        /// <param name="reportFileType"></param>
        /// <returns></returns>
        public List<ReportFileAreaDto> ReportFileDownloadAllSearch_Area(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string keyword, string reportFileType)
        {
            if (bussinessType == null) bussinessType = "";
            if (wideArea == null) wideArea = "";
            if (bigArea == null) bigArea = "";
            if (middleArea == null) middleArea = "";
            if (smallArea == null) smallArea = "";
            if (keyword == null) keyword = "";
            if (reportFileType == null) reportFileType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@SmallArea", smallArea),
                                                        new SqlParameter("@MiddleArea", middleArea),
                                                        new SqlParameter("@BigArea", bigArea),
                                                        new SqlParameter("@WideArea", wideArea),
                                                        new SqlParameter("@BussinessType", bussinessType),
                                                    new SqlParameter("@KeyWord", keyword), new SqlParameter("@ReportFileType", reportFileType)};
            Type t = typeof(ReportFileAreaDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.AreaId,B.AreaCode,B.AreaName,A.ReportFileType,A.ReportFileName,A.Url_OSS,A.InDateTime,
                    (SELECT HiddenName FROM hiddenColumn WHERE HiddenCodeGroup = '区域类型' AND HiddenCode =  B.AreaType) AS AreaType
                    FROM ReportFileArea A INNER JOIN Area B ON A.AreaId = B.AreaId
                    WHERE A.ProjectId = @ProjectId";

            if (!string.IsNullOrEmpty(smallArea))
            {
                sql += @" AND  B.AreaId = @SmallArea";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @" AND  B.AreaId IN (SELECT AreaId FROM Area WHERE ParentId = @MiddleArea OR AreaId = @MiddleArea)";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @" AND (B.AreaId IN (SELECT AreaId FROM Area WHERE ParentId = @BigArea OR AreaId = @BigArea)
                               OR B.AreaId IN (SELECT D.AreaId 
                                               FROM Area D INNER JOIN Area E ON D.ParentId = E.AreaId
                                                           INNER JOIN Area F ON E.ParentId = F.AreaId AND F.AreaId = @BigArea)) ";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @" AND (B.AreaId IN (SELECT AreaId FROM Area WHERE ParentId = @WideArea OR AreaId = @WideArea) 
                               OR B.AreaId IN (SELECT D.AreaId 
                                               FROM Area D INNER JOIN Area E ON D.ParentId = E.AreaId
                                                           INNER JOIN Area F ON E.ParentId = F.AreaId AND F.AreaId = @WideArea)
                               OR B.AreaId IN (SELECT D.AreaId 
                                               FROM Area D INNER JOIN Area E ON D.ParentId = E.AreaId
                                                           INNER JOIN Area F ON E.ParentId = F.AreaId 
                                                           INNER JOIN Area G ON F.ParentId = G.AreaId AND G.AreaId = @WideArea))";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @" AND (B.AreaId IN (SELECT AreaId FROM Area WHERE ParentId = @BussinessType OR AreaId = @BussinessType) 
                               OR B.AreaId IN (SELECT D.AreaId 
                                               FROM Area D INNER JOIN Area E ON D.ParentId = E.AreaId
                                                           INNER JOIN Area F ON E.ParentId = F.AreaId AND F.AreaId = @BussinessType)
                               OR B.AreaId IN (SELECT D.AreaId 
                                               FROM Area D INNER JOIN Area E ON D.ParentId = E.AreaId
                                                           INNER JOIN Area F ON E.ParentId = F.AreaId 
                                                           INNER JOIN Area G ON F.ParentId = G.AreaId AND G.AreaId = @BussinessType)
                              OR B.AreaId IN (SELECT D.AreaId 
                                              FROM Area D INNER JOIN Area E ON D.ParentId = E.AreaId
                                                          INNER JOIN Area F ON E.ParentId = F.AreaId 
                                                          INNER JOIN Area G ON F.ParentId = G.AreaId 
                                                          INNER jOIN Area H ON G.ParentId = H.AreaId AND H.AreaId = @BussinessType))";
            }
            else
            {
                sql += " WHERE 1=2"; // 业务类型也没有选择的情况下什么都不查询，未设置区域信息不能查询数据
                                     // sql += " WHERE A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            if (!string.IsNullOrEmpty(reportFileType))
            {
                sql += " AND A.ReportFileType = @ReportFileType";
            }
            sql += " ORDER BY B.AreaId";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileAreaDto>().ToList();
        }
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="bussinessType"></param>
        /// <param name="wideArea"></param>
        /// <param name="bigArea"></param>
        /// <param name="middleArea"></param>
        /// <param name="smallArea"></param>
        /// <param name="shopIdStr"></param>
        /// <param name="keyword"></param>
        /// <param name="reportFileType"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileDto> ReportFileDownloadAllByPageSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ReportFileDownloadAllSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, reportFileType).Skip(startIndex).Take(pageCount).ToList();
        }
        public List<ReportFileAreaDto> ReportFileDownloadAllByPageSearch_Area(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string keyword, string reportFileType, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ReportFileDownloadAllSearch_Area(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, keyword, reportFileType).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 打包下载文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="bussinessType"></param>
        /// <param name="wideArea"></param>
        /// <param name="bigArea"></param>
        /// <param name="middleArea"></param>
        /// <param name="smallArea"></param>
        /// <param name="shopIdStr"></param>
        /// <param name="keyword"></param>
        /// <param name="reportFileType"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public string ReportFileDownLoad(string userId, string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType, int pageNum, int pageCount)
        {

            List<ReportFileDto> list = ReportFileDownloadAllByPageSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, reportFileType, pageNum, pageCount);
            if (list == null || list.Count == 0) return "";
            string fileStr = "";
            string defaultPath = HostingEnvironment.MapPath(@"~/");
            string basePath = defaultPath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // 从OSS把文件下载到服务器
            foreach (ReportFileDto reportFile in list)
            {
                fileStr += reportFile.ReportFileName + ";";
                if (File.Exists(folder + @"\" + reportFile.ReportFileName))
                {
                    File.Delete(folder + @"\" + reportFile.ReportFileName);
                }
                try
                {
                    OSSClientHelper.GetObject(reportFile.Url_OSS, folder + @"\" + reportFile.ReportFileName);
                }
                catch (Exception ex) { }
            }
            List<string> fileNameList = new List<string>();
            foreach (ReportFileDto reportFile in list)
            {
                fileNameList.Add(reportFile.ReportFileName);
            }
            // 打包文件
            if (!ZipInForFiles(fileNameList, downLoadfolder, basePath, downLoadPath, 9)) return "";
            // 保存下载记录
            if (!string.IsNullOrEmpty(fileStr))
            {
                ReportFileActionLog log = new ReportFileActionLog();
                log.Action = "批量下载-经销商";
                log.InUserId = Convert.ToInt32(userId);
                log.ProjectId = Convert.ToInt32(projectId);
                log.ReportFileName = fileStr;
                ReportFileActionLogSave(log);
            }
            return downLoadPath.Replace(defaultPath, "");
        }
        /// <summary>
        /// 打包下载文件-区域
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <param name="bussinessType"></param>
        /// <param name="wideArea"></param>
        /// <param name="bigArea"></param>
        /// <param name="middleArea"></param>
        /// <param name="smallArea"></param>
        /// <param name="keyword"></param>
        /// <param name="reportFileType"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public string ReportFileDownLoad_Area(string userId, string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string keyword, string reportFileType, int pageNum, int pageCount)
        {

            List<ReportFileAreaDto> list = ReportFileDownloadAllByPageSearch_Area(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, keyword, reportFileType, pageNum, pageCount);
            if (list == null || list.Count == 0) return "";
            string fileStr = "";
            string defaultPath = HostingEnvironment.MapPath(@"~/");
            string basePath = defaultPath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // 从OSS把文件下载到服务器
            foreach (ReportFileAreaDto reportFile in list)
            {
                fileStr += reportFile.ReportFileName + ";";
                if (File.Exists(folder + @"\" + reportFile.ReportFileName))
                {
                    File.Delete(folder + @"\" + reportFile.ReportFileName);
                }
                try
                {
                    OSSClientHelper.GetObject(reportFile.Url_OSS, folder + @"\" + reportFile.ReportFileName);
                }
                catch (Exception ex) { }
            }
            List<string> fileNameList = new List<string>();
            foreach (ReportFileAreaDto reportFile in list)
            {
                fileNameList.Add(reportFile.ReportFileName);
            }
            // 打包文件
            if (!ZipInForFiles(fileNameList, downLoadfolder, basePath, downLoadPath, 9)) return "";
            // 保存下载记录
            if (!string.IsNullOrEmpty(fileStr))
            {
                ReportFileActionLog log = new ReportFileActionLog();
                log.Action = "批量下载-区域";
                log.InUserId = Convert.ToInt32(userId);
                log.ProjectId = Convert.ToInt32(projectId);
                log.ReportFileName = fileStr;
                ReportFileActionLogSave(log);
            }
            return downLoadPath.Replace(defaultPath, "");
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="foler"></param>
        /// <param name="folderToZip"></param>
        /// <param name="zipedFile"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool ZipInForFiles(List<string> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (string fileName in fileNames)
                    {
                        string file = Path.Combine(folderToZip, foler, fileName);
                        string extension = string.Empty;
                        if (!System.IO.File.Exists(file))
                        {
                            comment += foler + "，文件：" + fileName + "不存在。\r\n";
                            continue;
                        }

                        using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, fileName)))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            entry = new ZipEntry(foler + "/" + fileName);
                            entry.DateTime = DateTime.Now;
                            entry.Size = fs.Length;
                            fs.Close();
                            crc.Reset();
                            crc.Update(buffer);
                            entry.Crc = crc.Value;
                            zipOutStream.PutNextEntry(entry);
                            zipOutStream.Write(buffer, 0, buffer.Length);
                        }
                    }

                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion

        #region 得分查询
        /// <summary>
        /// 
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
        public List<AnswerDto> ShopAnswerSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
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
                                                        new SqlParameter("@BussinessType", bussinessType),
                                                        new SqlParameter("@KeyWord", keyword)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,X.SubjectId,A.PhotoScore, A.Remark,A.InspectionStandardResult,A.FileResult,A.LossResult,A.InDateTime,A.ModifyDateTime
                    ,X.SubjectCode,X.[CheckPoint],X.OrderNO,X.[Desc],X.InspectionDesc,X.HiddenCode_SubjectType,B.ShopCode,B.ShopName
                    FROM Answer A INNER JOIN Shop B ON A.ShopId = B.ShopId AND A.ProjectId = @ProjectId 
                                  INNER JOIN [Subject] X ON A.ProjectId = X.ProjectId AND A.SubjectId = X.SubjectId AND A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopIdStr))
            {
                string[] shopIdList = shopIdStr.Split(',');
                sql += " WHERE (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%') AND A.ShopId IN('";
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
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                    WHERE D.AreaId = @SmallArea AND A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                    WHERE A.ProjectId = @ProjectId AND E.AreaId = @MiddleArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                    WHERE A.ProjectId = @ProjectId AND F.AreaId = @BigArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                    WHERE A.ProjectId = @ProjectId AND G.AreaId = @WideArea AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                        INNER JOIN Area H ON G.ParentId = H.AreaId 
                    WHERE A.ProjectId = @ProjectId AND H.AreaId = @BussinessType AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else
            {
                sql += " WHERE 1=2"; // 业务类型也没有选择的情况下什么都不查询，未设置区域信息不能查询数据
                                     // sql += " WHERE A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="bussinessType"></param>
        /// <param name="wideArea"></param>
        /// <param name="bigArea"></param>
        /// <param name="middleArea"></param>
        /// <param name="smallArea"></param>
        /// <param name="shopIdStr"></param>
        /// <param name="keyword"></param>
        /// <param name="reportFileType"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<AnswerDto> ShopAnswerSearchByPageSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ShopAnswerSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        #endregion
        #region 报告下载日志
        /// <summary>
        /// 报告文件操作记录
        /// </summary>
        /// <param name="action"></param>
        /// <param name="account"></param>
        /// <param name="project"></param>
        /// <param name="reportFileName"></param>
        /// <returns></returns>
        public List<ReportFileActionLogDto> ReportFileActionLogSearch(string userId, string action, string account, string project, string reportFileName, string startDate, string endDate)
        {
            if (action == null) action = "";
            if (account == null) account = "";
            if (project == null) project = "";
            if (reportFileName == null) reportFileName = "";
            if (userId == null) userId = "";
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = "2022-01-01 00:00:00";
            }
            else
            {
                startDate = startDate + " 00:00:00";
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            }
            else
            {
                endDate = endDate + " 23:59:59";
            }
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@Action", action),
                                                        new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@Account", account),
                                                        new SqlParameter("@Project", project),
                                                        new SqlParameter("@ReportFileName", reportFileName),
                                                        new SqlParameter("@StartDate", startDate),
                                                        new SqlParameter("@EndDate", endDate)};
            Type t = typeof(ReportFileActionLogDto);
            string sql = @" SELECT A.*,B.AccountId,B.AccountName,C.ProjectCode,C.ProjectName
                            FROM ReportFileActionLog A INNER JOIN UserInfo B ON A.InUserId = B.Id
                                                       INNER JOIN Project C ON A.ProjectId = C.ProjectId
                            WHERE 1=1 ";
            if (!string.IsNullOrEmpty(project))
            {
                sql += " AND A.ProjectId = @Project";
            }
            if (!string.IsNullOrEmpty(action))
            {
                sql += " AND Action = @Action";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND A.InUserId = @UserId";
            }
            if (!string.IsNullOrEmpty(account))
            {
                sql += " AND (B.AccountId LIKE '%'+@Account+'%' OR B.AccountName LIKE '%'+@Account+'%')";
            }
            if (!string.IsNullOrEmpty(reportFileName))
            {
                sql += " AND A.ReportFileName LIKE '%'+@ReportFileName+'%'";
            }
            sql += " AND A.InDateTime BETWEEN @StartDate AND @EndDate";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileActionLogDto>().ToList();

        }
        public void ReportFileActionLogSave(ReportFileActionLog reportFileActionLog)
        {
            reportFileActionLog.InDateTime = DateTime.Now;
            db.ReportFileActionLog.Add(reportFileActionLog);
            db.SaveChanges();

        }
        #endregion
        #region 报告平台-数据
        #region 执行数量统计
        // 各区域类型每个区域的数量
        public List<ReportShopCompleteCountDto> ReportShopCompleteCountSearch(string projectId, string areaId, string shopType)
        {
            if (areaId == null) areaId = "";
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@AreaId", areaId),
                                                        new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportShopCompleteCountDto);
            string sql = "";
            sql = @"SELECT * FROM ReportShopCompleteCount
                   WHERE ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += @" AND AreaId=@AreaId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += " AND ShopType = @ShopType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportShopCompleteCountDto>().ToList();
        }
        // 各区域类型每个区域Appeal数量
        public List<ReportShopCompleteCountDto> ReportShopCompleteCountSearch_Appeal(string projectId, string areaId, string shopType)
        {
            if (areaId == null) areaId = "";
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@AreaId", areaId),
                                                        new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportShopCompleteCountDto);
            string sql = "";
            sql = @"SELECT * FROM ReportShopCompleteCount_Appeal
                   WHERE ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += @" AND AreaId=@AreaId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += " AND ShopType = @ShopType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportShopCompleteCountDto>().ToList();
        }
        // 全国数量
        public List<ReportShopCompleteCountDto> ReportShopCompleteCountCountrySearch(string projectId, string shopType)
        {
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportShopCompleteCountDto);
            string sql = "";
            sql = @"SELECT ISNULL(SUM(Count_Complete),0) AS Count_Complete,ISNULL(SUM(Count_UnComplete),0) AS Count_UnComplete
                    FROM ReportShopCompleteCount A INNER JOIN Shop B ON A.AreaId = B.ShopId  
                   WHERE A.ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += " AND ShopType = @ShopType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportShopCompleteCountDto>().ToList();
        }
        // 全国Appeal数量
        public List<ReportShopCompleteCountDto> ReportShopCompleteCountCountrySearch_Appeal(string projectId, string shopType)
        {
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportShopCompleteCountDto);
            string sql = "";
            sql = @"SELECT ISNULL(SUM(Count_Complete),0) AS Count_Complete,ISNULL(SUM(Count_UnComplete),0) AS Count_UnComplete
                    FROM ReportShopCompleteCount_Appeal A INNER JOIN Shop B ON A.AreaId = B.ShopId  
                   WHERE A.ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += " AND ShopType = @ShopType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportShopCompleteCountDto>().ToList();
        }
        #endregion
        #region 一级指标统计
        // 经销商得分和一级指标得分
        public List<ReportChapterScoreDto> ReportShopChapterScoreSearch(string projectId, string shopId, string shopType)
        {
            if (shopId == null) shopId = "";
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopId", shopId)
                                                        ,new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.ChapterCode,B.ChapterName,C.ShopCode,C.ShopName
                 , (SELECT ISNULL(FullScore,0) FROM ChapterShopType WHERE ChapterId = B.ChapterId AND ShopType=@ShopType) FullScore
                  ,  (SELECT ISNULL(SumScore,0) FROM ReportShopChapterSumScore WHERE ProjectId = @ProjectId AND ShopId = A.ShopId AND ShopType = @ShopType) SumScore
                    FROM ReportShopChapterScore A INNER JOIN Chapter B ON A.ProjectId = B.ProjectId 
                                                                       AND A.ChapterId = B.ChapterId
                                                   INNER JOIN ChapterShopType D ON B.ChapterId = D.ChapterId AND D.ShopType = @ShopType
                                                   INNER JOIN Shop C ON A.ShopId = C.ShopId
        
                   WHERE A.ProjectId=@ProjectId ";

            if (!string.IsNullOrEmpty(shopId))
            {
                sql += @" AND A.ShopId=@ShopId";
            }
            sql += " ORDER BY B.ChapterId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 省份的一级指标得分和总分
        public List<ReportChapterScoreDto> ReportProvinceChapterScoreSearch(string projectId, string provinceId, string shopType)
        {
            if (provinceId == null) provinceId = "";
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ProvinceId", provinceId),
                                                         new SqlParameter("@ShopType", shopType),};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.ChapterCode,B.ChapterName,C.ProvinceCode,C.ProvinceName
			            ,(SELECT FullScore FROM ChapterShopType WHERE ChapterId = B.ChapterId AND ShopType=@ShopType) AS FullScore
			            ,(SELECT SumScore FROM ReportProvinceChapterSumScore WHERE ProjectId =@ProjectId AND ShopType=@ShopType AND ProvinceId = A.ProvinceId) AS SumScore
                    FROM ReportProvinceChapterScore A INNER JOIN Chapter B ON A.ProjectId = B.ProjectId 
                                                                            AND A.ChapterId = B.ChapterId  
                                                      INNER JOIN ChapterShopType D ON B.ChapterId = D.ChapterId 
                                                                            AND D.ShopType = @ShopType  
                                                     INNER JOIN Province C ON A.ProvinceId = C.ProvinceId
                    WHERE A.ProjectId=@ProjectId ";

            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += @" AND A.ProvinceId=@ProvinceId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += @" AND A.ShopType=@ShopType";
            }
            sql += " ORDER BY B.ChapterId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 区域一级指标得分和总分
        public List<ReportChapterScoreDto> ReportAreaChapterScoreSearch(string projectId, string areaId, string shopType)
        {
            if (areaId == null) areaId = "";
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@AreaId", areaId),
                                                         new SqlParameter("@ShopType", shopType),};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.ChapterCode,B.ChapterName,C.AreaCode,C.AreaName
            ,(SELECT FullScore FROM ChapterShopType WHERE ChapterId = B.ChapterId AND ShopType=@ShopType) AS FullScore
            ,(SELECT SumScore FROM ReportAreaChapterSumScore WHERE ProjectId =@ProjectId AND ShopType=@ShopType AND AreaId = A.AreaId) AS SumScore
                FROM ReportAreaChapterScore A INNER JOIN Chapter B ON A.ProjectId = B.ProjectId 
                                                                       AND A.ChapterId = B.ChapterId 
                                              INNER JOIN ChapterShopType D ON B.ChapterId = D.ChapterId 
                                                                        AND D.ShopType = @ShopType   
                                              INNER JOIN Area C ON A.AreaId = C.AreaId
                WHERE A.ProjectId=@ProjectId ";

            if (!string.IsNullOrEmpty(areaId))
            {
                sql += @" AND A.AreaId=@AreaId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += @" AND A.ShopType=@ShopType";
            }
            sql += " ORDER BY B.ChapterId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 全国一级指标得分和总分（按期）
        public List<ReportChapterScoreDto> ReportCountryChapterScoreSearch(string projectId, string shopType)
        {
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                         new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.ChapterCode,B.ChapterName
                ,(SELECT FullScore FROM ChapterShopType WHERE ChapterId = B.ChapterId AND ShopType=@ShopType) AS FullScore
                ,(SELECT SumScore FROM ReportCountryChapterSumScore WHERE ProjectId = @ProjectId AND ShopType=@ShopType) AS SumScore
                 FROM ReportCountryChapterScore A INNER JOIN Chapter B ON A.ProjectId = B.ProjectId 
                                                                       AND A.ChapterId = B.ChapterId
                                                INNER JOIN ChapterShopType D ON B.ChapterId = D.ChapterId 
                                                                        AND D.ShopType = @ShopType
                                                                       
                   WHERE A.ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += @" AND A.ShopType=@ShopType";
            }
            sql += " ORDER BY B.ChapterId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        #endregion
        #region 二级指标得分
        // 经销商二级指标得分
        public List<ReportSubjectScoreDto> ReportShopSubjectScoreSearch(string projectId, string shopId, string chapterId)
        {
            if (shopId == null) shopId = "";
            if (chapterId == null) chapterId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@ChapterId", chapterId)};
            Type t = typeof(ReportSubjectScoreDto);
            string sql = "";
            sql = @"SELECT A.*,C.SubjectId,C.SubjectCode,C.[CheckPoint],ISNULL(C.FullScore,0) AS FullScore
                    ,(SELECT ISNULL(Score,0) FROM ReportShopChapterScore WHERE ProjectId = @ProjectId AND ChapterId = B.ChapterId AND ShopId = A.ShopId) AS SumScore
                    FROM ReportShopSubjectScore A INNER JOIN ChapterSubject B ON A.SubjectId = B.SubjectId
                                                  INNER JOIN Subject C ON B.SubjectId = C.SubjectId
                   WHERE A.ProjectId=@ProjectId ";

            if (!string.IsNullOrEmpty(shopId))
            {
                sql += @" AND A.ShopId=@ShopId";
            }
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += @" AND B.ChapterId=@ChapterId";
            }
            sql += " ORDER BY B.ChapterId,B.SubjectId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportSubjectScoreDto>().ToList();
        }
        // 区域二级指标得分
        public List<ReportSubjectScoreDto> ReportAreaSubjectScoreSearch(string projectId, string areaId, string chapterId, string shopType)
        {
            if (areaId == null) areaId = "";
            if (shopType == null) shopType = "";
            if (chapterId == null) chapterId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@AreaId", areaId),
                                                          new SqlParameter("@ShopType", shopType),
                                                        new SqlParameter("@ChapterId", chapterId)};
            Type t = typeof(ReportSubjectScoreDto);
            string sql = "";
            sql = @"SELECT A.*,C.SubjectId,C.SubjectCode,C.[CheckPoint],ISNULL(C.FullScore,0) AS FullScore  
                ,(SELECT ISNULL(Score,0) FROM ReportAreaChapterScore WHERE ProjectId = @ProjectId AND ChapterId = B.ChapterId AND AreaId = A.AreaId AND ShopType = @ShopType) AS SumScore
                    FROM ReportAreaSubjectScore A INNER JOIN ChapterSubject B ON A.SubjectId = B.SubjectId
                                                  INNER JOIN Subject C ON B.SubjectId = C.SubjectId
                   WHERE A.ProjectId=@ProjectId ";

            if (!string.IsNullOrEmpty(areaId))
            {
                sql += @" AND A.AreaId=@AreaId";
            }
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += @" AND B.ChapterId=@ChapterId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += @" AND A.ShopType=@ShopType";
            }
            sql += " ORDER BY B.ChapterId,B.SubjectId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportSubjectScoreDto>().ToList();
        }
        // 省份二级指标得分
        public List<ReportSubjectScoreDto> ReportProvinceSubjectScoreSearch(string projectId, string provinceId, string chapterId, string shopType)
        {
            if (provinceId == null) provinceId = "";
            if (shopType == null) shopType = "";
            if (chapterId == null) chapterId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ProvinceId", provinceId),
                                                          new SqlParameter("@ShopType", shopType),
                                                        new SqlParameter("@ChapterId", chapterId)};
            Type t = typeof(ReportSubjectScoreDto);
            string sql = "";
            sql = @"SELECT A.*,C.SubjectId,C.SubjectCode,C.[CheckPoint],ISNULL(C.FullScore,0) AS FullScore  
                ,(SELECT ISNULL(Score,0) FROM ReportProvinceChapterScore WHERE ProjectId = @ProjectId AND ChapterId = B.ChapterId AND ProvinceId = A.ProvinceId AND ShopType = @ShopType) AS SumScore
                    FROM ReportProvinceSubjectScore A INNER JOIN ChapterSubject B ON A.SubjectId = B.SubjectId
                                                  INNER JOIN Subject C ON B.SubjectId = C.SubjectId
                   WHERE A.ProjectId=@ProjectId ";

            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += @" AND A.ProvinceId=@ProvinceId";
            }
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += @" AND B.ChapterId=@ChapterId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += @" AND A.ShopType=@ShopType";
            }
            sql += " ORDER BY B.ChapterId,B.SubjectId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportSubjectScoreDto>().ToList();
        }
        // 全国二级指标得分
        public List<ReportSubjectScoreDto> ReportCountrySubjectScoreSearch(string projectId, string chapterId, string shopType)
        {
            if (chapterId == null) chapterId = "";
            if (shopType == null) shopType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),new SqlParameter("@ShopType", shopType),
                                                        new SqlParameter("@ChapterId", chapterId)};
            Type t = typeof(ReportSubjectScoreDto);
            string sql = "";
            sql = @"SELECT A.* ,C.SubjectId,C.SubjectCode,C.[CheckPoint],ISNULL(C.FullScore,0) AS FullScore
            ,(SELECT ISNULL(Score,0) FROM ReportCountryChapterScore WHERE ProjectId = @ProjectId AND ChapterId = B.ChapterId  AND ShopType = @ShopType) AS SumScore
                    FROM ReportCountrySubjectScore A INNER JOIN ChapterSubject B ON A.SubjectId = B.SubjectId
                                                     INNER JOIN Subject C ON B.SubjectId = C.SubjectId
                   WHERE A.ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += @" AND B.ChapterId=@ChapterId";
            }
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += @" AND A.ShopType=@ShopType";
            }
            sql += " ORDER BY B.ChapterId,B.SubjectId ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportSubjectScoreDto>().ToList();
        }
        #endregion
        // 扣分细节项
        public List<AnswerDto> ReportShopLossResult(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopId, string keyword)
        {
            if (bussinessType == null) bussinessType = "";
            if (wideArea == null) wideArea = "";
            if (bigArea == null) bigArea = "";
            if (middleArea == null) middleArea = "";
            if (smallArea == null) smallArea = "";
            if (shopId == null) shopId = "";
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@SmallArea", smallArea),
                                                        new SqlParameter("@MiddleArea", middleArea),
                                                        new SqlParameter("@BigArea", bigArea),
                                                        new SqlParameter("@WideArea", wideArea),
                                                        new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@Keyword", keyword),
                                                        new SqlParameter("@BussinessType", bussinessType)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,X.ShopCode,X.ShopName,Y.SubjectId,Y.[CheckPoint],B.LossResult,B.LossResultAdd,
                    Z.ChapterId,O.ChapterCode,O.ChapterName
                    FROM ReportShopLossResult A INNER JOIN Answer B ON A.ProjectId = B.ProjectId AND A.ShopId = B.ShopId AND A.SubjectId = B.SubjectId
                                                                       AND A.ProjectId = @ProjectId
                                                INNER JOIN Shop X ON B.ShopId = X.ShopId
                                                INNER JOIN Subject Y ON B.SubjectId = Y.SubjectId  
                                                INNER JOIN ChapterSubject Z ON Y.SubjectId = Z.SubjectId 
                                                INNER JOIN Chapter O ON Z.ChapterId = O.ChapterId";

            if (!string.IsNullOrEmpty(shopId))
            {
                string[] shopIdList = shopId.Split(',');
                sql += " WHERE  (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%') AND A.ShopId IN('";
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
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                    WHERE D.AreaId = @SmallArea AND A.ProjectId = @ProjectId AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(middleArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                    WHERE A.ProjectId = @ProjectId AND E.AreaId = @MiddleArea AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bigArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                    WHERE A.ProjectId = @ProjectId AND F.AreaId = @BigArea AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(wideArea))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                    WHERE A.ProjectId = @ProjectId AND G.AreaId = @WideArea AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else if (!string.IsNullOrEmpty(bussinessType))
            {
                sql += @"
                        INNER JOIN AreaShop C ON B.ShopId = C.ShopId
                        INNER JOIN Area D ON C.AreaId = D.AreaId 
                        INNER JOIN Area E ON D.ParentId = E.AreaId 
                        INNER JOIN Area F ON E.ParentId = F.AreaId 
                        INNER JOIN Area G ON F.ParentId = G.AreaId 
                        INNER JOIN Area H ON G.ParentId = H.AreaId 
                    WHERE A.ProjectId = @ProjectId AND H.AreaId = @BussinessType AND (X.ShopCode LIKE '%'+@KeyWord+'%' OR X.ShopName LIKE '%'+@KeyWord+'%')";
            }
            else
            {
                sql += " WHERE 1=2"; // 业务类型也没有选择的情况下什么都不查询，未设置区域信息不能查询数据
                                     // sql += " WHERE A.ProjectId = @ProjectId AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        #region 数据分析
        //趋势表现（全国、区域) 当年所有期次的得分
        public List<ReportChapterScoreDto> ReportYearCountryAreaTrend(string year, string brandId, string areaId, string shopType)
        {
            if (shopType == null) shopType = "";
            if (year == null || year == "") year = DateTime.Now.Year.ToString();
            if (areaId == null) areaId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@Year", year),
                                                       new SqlParameter("@BrandId", brandId), new SqlParameter("@AreaId", areaId),
                                                       new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql += @"SELECT '全国' AS AreaCode,'全国' AS AreaName, A.ProjectName,A.ProjectId,A.ProjectCode,ISNULL(B.SumScore, 0) AS SumScore
                        FROM Project A INNER JOIN ReportCountryChapterSumScore B ON A.ProjectId = B.ProjectId AND ShopType = @ShopType
                        WHERE Year = @Year AND A.BrandId = @BrandId
                        UNION ALL
                        SELECT AreaCode, AreaName, A.ProjectName,A.ProjectId,A.ProjectCode,ISNULL(B.SumScore, 0) AS SumScore
                        FROM Project A INNER JOIN ReportAreaChapterSumScore B ON A.ProjectId = B.ProjectId AND ShopType = @ShopType
                        INNER JOIN Area C ON B.AreaId = C.AreaId
                        WHERE Year = @Year AND A.BrandId = @BrandId";
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND C.AreaId = @AreaId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        //趋势表现（省份) 当年所有期次的得分
        public List<ReportChapterScoreDto> ReportYearProvinceTrend(string year, string brandId, string provinceId, string shopType)
        {
            if (shopType == null) shopType = "";
            if (year == null || year == "") year = DateTime.Now.Year.ToString();
            if (provinceId == null) provinceId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@Year", year),
                                                       new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProvinceId", provinceId),
                                                       new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql += @"
                        SELECT ProvinceCode AS AreaCode, ProvinceName AS AreaName, A.ProjectName,A.ProjectId,A.ProjectCode,ISNULL(B.SumScore, 0) AS SumScore
                        FROM Project A INNER JOIN ReportProvinceChapterSumScore B ON A.ProjectId = B.ProjectId AND ShopType = @ShopType
                        INNER JOIN Province C ON B.ProvinceId = C.ProvinceId                        
                        WHERE Year = @Year AND A.BrandId = @BrandId";
            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += " AND C.ProvinceId = @ProvinceId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        //趋势表现（经销商) 当年所有期次的得分
        public List<ReportChapterScoreDto> ReportYearShopTrend(string year, string brandId, string shopId)
        {
            if (year == null || year == "") year = DateTime.Now.Year.ToString();
            if (shopId == null) shopId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@Year", year),
                                                       new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql += @"
                        SELECT ShopCode AS AreaCode, ShopName AS AreaName, A.ProjectName,A.ProjectId,A.ProjectCode,ISNULL(B.SumScore, 0) AS SumScore
                        FROM Project A INNER JOIN ReportShopChapterSumScore B ON A.ProjectId = B.ProjectId 
                        INNER JOIN Shop C ON B.ShopId = C.ShopId                          
                        WHERE Year = @Year AND A.BrandId = @BrandId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND C.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 全国得分查询
        public List<ReportChapterScoreDto> ReportCountrySumScore(string projectId, string preProjectId, string shopType)
        {
            if (shopType == null) shopType = "";
            if (preProjectId == null) preProjectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@PreProjectId", preProjectId),
                                                         new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,
                    (SELECT ISNULL(SumScore,0) FROM ReportCountryChapterSumScore WHERE ProjectId = @PreProjectId  AND ShopType = @ShopType) AS PreSumScore
                    FROM ReportCountryChapterSumScore A 
				    WHERE ProjectId = @ProjectId  ";
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += " AND A.ShopType = @ShopType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 区域得分查询
        public List<ReportChapterScoreDto> ReportAreaSumScore(string projectId, string preProjectId, string areaId, string shopType)
        {
            if (shopType == null) shopType = "";
            if (areaId == null) areaId = "";
            if (preProjectId == null) preProjectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@PreProjectId", preProjectId),
                                                        new SqlParameter("@AreaId", areaId),
                                                         new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.AreaCode,B.AreaName,B.AreaId,
                    (SELECT ISNULL(SumScore,0) FROM ReportAreaChapterSumScore WHERE ProjectId = @PreProjectId AND AreaId = A.AreaId AND ShopType = @ShopType) AS PreSumScore
                    FROM ReportAreaChapterSumScore A INNER JOIN Area B ON A.AreaId = B.AreaId
                    
				    WHERE ProjectId = @ProjectId  ";
            if (!string.IsNullOrEmpty(shopType))
            {
                sql += " AND A.ShopType = @ShopType";
            }
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND A.AreaId = @AreaId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 省份得分查询
        public List<ReportChapterScoreDto> ReportProvinceSumScore(string projectId, string preProjectId, string provinceId, string shopType)
        {
            if (shopType == null) shopType = "";
            if (provinceId == null) provinceId = "";
            if (preProjectId == null) preProjectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ProvinceId", provinceId),
                                                       new SqlParameter("@PreProjectId", preProjectId),
                                                       new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.ProvinceCode,B.ProvinceName,D.AreaCode,D.AreaName,D.AreaId,
                    (SELECT ISNULL(SumScore,0) FROM ReportProvinceChapterSumScore WHERE ProjectId = @PreProjectId AND ProvinceId = A.ProvinceId AND ShopType = @ShopType) AS PreSumScore
                    FROM ReportProvinceChapterSumScore A INNER JOIN Province B ON A.ProvinceId = B.ProvinceId 
															AND A.ShopType = @ShopType
									 INNER JOIN AreaProvince C ON B.ProvinceId = C.ProvinceId
									 INNER JOIN Area D ON C.AreaId = D.AreaId
                    WHERE ProjectId = @ProjectId   ";
            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += " AND C.ProvinceId = @ProvinceId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        // 经销商得分查询
        public List<ReportChapterScoreDto> ReportShopSumScore(string projectId, string preProjectId, string provinceId, string areaId, string shopId, string shopType)
        {

            if (shopType == null) shopType = "";
            if (areaId == null) areaId = "";
            if (shopId == null) shopId = "";
            if (provinceId == null) provinceId = "";
            if (preProjectId == null) preProjectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@PreProjectId", preProjectId),
                                                       new SqlParameter("@AreaId", areaId),
                                                       new SqlParameter("@ProvinceId", provinceId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@ShopType", shopType)};
            Type t = typeof(ReportChapterScoreDto);
            string sql = "";
            sql = @"SELECT A.*,B.ShopCode,B.ShopName,D.AreaCode,D.AreaName,D.AreaId
                    ,(SELECT ISNULL(SumScore ,0)
			            FROM ReportAreaChapterSumScore 
			            WHERE ProjectId =A.ProjectId 
			            AND AreaId = C.AreaId AND ShopType = @ShopType) AS AreaSumScore
                    ,(SELECT ISNULL(SumScore,0) 
                        FROM ReportCountryChapterSumScore 
                        WHERE ProjectId =A.ProjectId AND ShopType = @ShopType) AS CountrySumScore
                    ,(SELECT ISNULL(SumScore ,0)
			            FROM ReportAreaChapterSumScore 
			            WHERE ProjectId =@PreProjectId 
			            AND AreaId = C.AreaId AND ShopType = @ShopType) AS PreAreaSumScore
                    ,(SELECT ISNULL(SumScore,0) 
                        FROM ReportCountryChapterSumScore 
                        WHERE ProjectId =@PreProjectId AND ShopType = @ShopType) AS PreCountrySumScore 
                   , (SELECT ISNULL(SumScore,0) FROM ReportShopChapterSumScore WHERE ProjectId = @PreProjectId AND ShopId = A.ShopId ) AS PreSumScore     
                FROM ReportShopChapterSumScore A INNER JOIN Shop B ON A.ShopId = B.ShopId 
							     INNER JOIN AreaShop C ON B.ShopId = C.ShopId
							     INNER JOIN Area D ON C.AreaId = D.AreaId
                    WHERE ProjectId = @ProjectId   ";
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND D.AreaId = @AreaId";
            }
            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += " AND B.ProvinceId = @ProvinceId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportChapterScoreDto>().ToList();
        }
        #endregion
        #region 生成报告数据
        public void ReportDataCreate(string brandId, string projectId)
        {

            if (brandId == null) brandId = "";
            if (projectId == null) projectId = "";

            int intBrandId = Convert.ToInt32(brandId);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                        , new SqlParameter("@BrandId", brandId)};
            string sql = "";
            if (intBrandId == 32)
            {
                sql = @" EXEC sp_Lotus_Report  @BrandId = @BrandId,@ProjectId = @ProjectId";
            }
            else if (intBrandId == 18)
            {
                sql = @" EXEC sp_ARCFOX_Report  @BrandId = @BrandId,@ProjectId = @ProjectId
                        ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion
        #endregion
        #region 岗位满足率
        public List<ReportJobRateDto> ReportJobRateSearcht(string projectId, string smallArea)
        {
            if (smallArea == null) smallArea = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@SmallArea", smallArea)};
            Type t = typeof(ReportJobRateDto);
            string sql = "";
            sql = @"SELECT A.AreaId,B.AreaCode,B.AreaName,A.JobName,A.JobFullCount,A.JobActualCount,
                    CASE WHEN A.JobFullCount IS NOT NULL AND A.JobActualCount IS NOT NULL AND (A.JobFullCount=A.JobActualCount OR A.JobFullCount<A.JobActualCount)
                         THEN '是'
                          ELSe '否'
                    END AS MeetChk
                    ,A.InDateTime
                    FROM ReportJobRate A INNER JOIN Area B ON A.AreaId = B.AreaId
                    WHERE ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(smallArea))
            {
                sql += " AND A.AreaId = @SmallArea";
            }

            return db.Database.SqlQuery(t, sql, para).Cast<ReportJobRateDto>().ToList();
        }
        public List<ReportJobRateDto> ReportBaseJobRateSearcht(string projectId, string smallArea)
        {
            if (smallArea == null) smallArea = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@SmallArea", smallArea)};
            Type t = typeof(ReportJobRateDto);
            string sql = "";
            sql = @"SELECT X.ProjectId,X.AreaId,X.AreaCode,X.AreaName,CAST(X.JobFullCount AS INT) AS JobFullCount
                    ,CAST(X.JobActualCount AS INT) AS JobActualCount,
                        CASE WHEN X.JobFullCount=0 OR X.JobFullCount IS NULL THEN 0
                            ELSE CAST(100*(X.JobActualCount/X.JobFullCount) AS DECIMAL(19,2))
                        END AS MeetRate
                    FROM (
                            SELECT A.ProjectId,A.AreaId,B.AreaCode,B.AreaName,CAST(Count(A.JobName) AS DECIMAL(19,2)) AS JobFullCount,
                                    CAST(SUM(CASE WHEN A.JobFullCount IS NOT NULL AND A.JobActualCount IS NOT NULL AND (A.JobFullCount=A.JobActualCount OR A.JobFullCount<A.JobActualCount)
                                         THEN 1
                                         ELSE 0
                                    END)AS DECIMAL(19,2)) AS JobActualCount
                             FROM ReportJobRate A INNER JOIN Area B ON A.AreaId = B.AreaId
                             WHERE ProjectId = 187
                             GROUP BY A.ProjectId,A.AreaId,B.AreaCode,B.AreaName) X";
            if (!string.IsNullOrEmpty(smallArea))
            {
                sql += " WHERE X.AreaId = @SmallArea";
            }

            return db.Database.SqlQuery(t, sql, para).Cast<ReportJobRateDto>().ToList();
        }
        public void SaveReportJobRate(ReportJobRate reportJobRate)
        {
            reportJobRate.InDateTime = DateTime.Now;
            db.ReportJobRate.Add(reportJobRate);
            db.SaveChanges();
        }
        public void ReportJobRateDelete(string projectId, string areaId)
        {
            if (areaId == null || areaId == "0") areaId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@AreaId", areaId) };
            string sql = @"DELETE ReportJobRate WHERE ProjectId = @ProjectId   
                        ";
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND AreaId = @AreaId";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion
    }
}