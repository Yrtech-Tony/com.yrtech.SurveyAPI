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
        #region 首页
        /// <summary>
        /// 首页报告统计查询
        /// </summary>
        /// <returns></returns>
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
        #endregion
        #region 报告上传
        /// <summary>
        /// 文件上传查询
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLSearch(string brandId, string projectId, string bussinessTypeId,string keyword)
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
        /// 按页码查询
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <param name="keyword"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ReportFileUploadDto> ReportFileListUploadALLByPageSearch(string brandId, string projectId, string bussinessTypeId,string keyword, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return ReportFileListUploadALLSearch(brandId, projectId, bussinessTypeId, keyword).Skip(startIndex).Take(pageCount).ToList();
        }
        

        /// <summary>
        /// 查询特定经销商的文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<ReportFile> ReportFileSearch(string projectId, string bussinessTypeId,string shopId, string reportFileType)
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
                                                        new SqlParameter("@BussinessTypeId", bussinessType),
                                                    new SqlParameter("@KeyWord", keyword), new SqlParameter("@ReportFileType", reportFileType)};
            Type t = typeof(ReportFileDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,A.ReportFileType,A.ReportFileName,A.Url_OSS,A.InDateTime
                    FROM ReportFile A INNER JOIN Shop B ON A.ShopId = B.ShopId AND A.BussinessTypeId = @BussinessTypeId";
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
        public string ReportFileDownLoad(string userId,string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType, int pageNum, int pageCount)
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
            // 打包文件
            if (!ZipInForFiles(list, downLoadfolder, basePath, downLoadPath, 9)) return "";
            // 保存下载记录
            if (!string.IsNullOrEmpty(fileStr))
            {
                ReportFileActionLog log = new ReportFileActionLog();
                log.Action = "批量下载";
                log.InUserId = Convert.ToInt32(userId);
                log.ProjectId = Convert.ToInt32(projectId);
                log.ReportFileName = fileStr;
                ReportFileActionLogSave(log);
            }
            return downLoadPath.Replace(defaultPath,"");
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
                                                        new SqlParameter("@BussinessTypeId", bussinessType),
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
        public List<ReportFileActionLogDto> ReportFileActionLogSearch(string userId,string action, string account, string project, string reportFileName,string startDate,string endDate)
        {
            if (action == null) action = "";
            if (account == null) account = "";
            if (project == null) project = "";
            if (reportFileName == null) reportFileName = "";
            if (userId == null) userId = "";
            endDate = endDate + " 23:59:59";
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
                            WHERE 1=1";
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
            if (!string.IsNullOrEmpty(project))
            {
                sql += " AND (C.ProjectCode LIKE '%'+@Project+'%' OR C.ProjectName LIKE '%'+@Project+'%')";
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
    }
}