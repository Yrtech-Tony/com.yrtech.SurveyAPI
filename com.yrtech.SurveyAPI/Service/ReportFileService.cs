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
        /// <summary>
        /// 文件上传查询
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLSearch(string brandId, string projectId, string keyword)
        {
            if (brandId == null) brandId = "";
            if (projectId == null) projectId = "";
            if (keyword == null) keyword = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@Keyword", keyword), new SqlParameter("@BrandId", brandId) };
            Type t = typeof(ReportFileUploadDto);
            string sql = @"
                        SELECT ProjectId,ShopId,ShopCode,ShopName,ShopShortName
		                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
		                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video INTO #T
                            FROM(
		                        SELECT A.ProjectId,A.ShopId,C.ShopCode,C.ShopName,C.ShopShortName,
				                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
				                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
		                        FROM ReportFile A INNER JOIN Project B ON A.ProjectId = B.ProjectId
				                                 INNER JOIN Shop C ON A.ShopId = C.ShopId
				                            ) X
                        WHERE 1=1
                        GROUP BY X.ProjectId,X.ShopId,X.ShopCode,X.ShopName,X.ShopShortName";
            sql += @" SELECT ShopId,ShopCode,ShopName,ShopShortName
                       ,ISNULL((SELECT ReportFileCount_File FROM #T WHERE ShopId = A.ShopId AND ProjectId = @ProjectId),0) AS ReportFileCount_File,
                        ISNULL((SELECT ReportFileCount_Video FROM #T WHERE ShopId = A.ShopId AND ProjectId = @ProjectId),0) ReportFileCount_Video
                      FROM Shop A WHERE 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (A.ShopCode LIKE '%'+@Keyword+'%' OR A.ShopName LIKE '%'+@Keyword+'%)";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileUploadDto>().ToList();
        }
        public List<ReportFileUploadDto> ReportFileCountYear()
        {
            SqlParameter[] para = new SqlParameter[] { };
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
        public List<ReportFileUploadDto> ReportFileListUploadALLByPageSearch(string brandId, string projectId, string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ReportFileListUploadALLSearch(brandId, projectId, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        /// <summary>
        /// 查询特定经销商的文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<ReportFile> ReportFileSearch(string projectId, string shopId, string reportFileType)
        {
            if (projectId == null) projectId = "";
            if (shopId == null || shopId == "0") shopId = "";
            if (reportFileType == null) reportFileType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@ShopId", shopId)
                                                     , new SqlParameter("@ReportFileType", reportFileType)};
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
            if (!string.IsNullOrEmpty(reportFileType))
            {
                sql += " AND ReportFileType = @ReportFileType";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFile>().ToList();
        }
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
                                                        new SqlParameter("@BusinessType", bussinessType),
                                                    new SqlParameter("@KeyWord", keyword), new SqlParameter("@ReportFileType", reportFileType)};
            Type t = typeof(ReportFileDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,A.ReportFileType,A.ReportFileName,A.Url_OSS,A.InDateTime
                    FROM ReportFile A INNER JOIN Shop B ON A.ShopId = B.ShopId ";
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
                    WHERE A.ProjectId = @ProjectId AND H.AreaId = @BusinessType AND (B.ShopCode LIKE '%'+@KeyWord+'%' OR B.ShopName LIKE '%'+@KeyWord+'%')";
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
        public List<ReportFileDto> ReportFileDownloadAllByPageSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword,string reportFileType, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ReportFileDownloadAllSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, reportFileType).Skip(startIndex).Take(pageCount).ToList();
        }
        public string ReportFileDownLoad(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType, int pageNum, int pageCount)
        {

            List<ReportFileDto> list = ReportFileDownloadAllByPageSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, reportFileType, pageNum,pageCount);
            if (list == null || list.Count == 0) return "";
            string basePath = HostingEnvironment.MapPath(@"~/")+"DownLoadFile";//根目录
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
                if (File.Exists(folder + @"\" + reportFile.ReportFileName))
                {
                    File.Delete(folder + @"\" + reportFile.ReportFileName);
                }
                try {
                    OSSClientHelper.GetObject("yrsurvey",reportFile.Url_OSS, folder + @"\" + reportFile.ReportFileName);
                }
                catch(Exception ex){ }
            }
            // 打包文件
            if (!ZipInForFiles(list, downLoadfolder, basePath, downLoadPath, 9)) return "";
            return downLoadPath;
        }
        private static bool ZipInForFiles(List<ReportFileDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
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

                    foreach (ReportFileDto report in fileNames)
                    {
                        string file = Path.Combine(folderToZip, foler, report.ReportFileName);
                        string extension = string.Empty;
                        if (!System.IO.File.Exists(file))
                        {
                            comment += foler + "，文件：" + report.ReportFileName + "不存在。\r\n";
                            continue;
                        }

                        using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, report.ReportFileName)))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            entry = new ZipEntry(foler + "/" + report.ReportFileName);
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

    }
}