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
        public List<ReportFileDto> ReportFileListUploadALLSearch(string projectId, string shopId)
        {
            if (projectId == null) projectId = "";
            if (shopId == null) shopId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@ShopId", shopId) };
            Type t = typeof(ReportFileDto);
            string sql = @"
                        SELECT ProjectId,ShopId,ShopCode,ShopName,ShopShortName
		                        ,SUM(ReportFileCount_File) AS ReportFileCount_File
		                        ,SUM(ReportFileCount_Video) AS ReportFileCount_Video
                        FROM(
		                        SELECT A.ProjectId,A.ShopId,C.ShopCode,C.ShopName,C.ShopShortName,
				                        CASE WHEN A.ReportFileType = '01' THEN 1 ELSE 0 END AS ReportFileCount_File,
				                        CASE WHEN A.ReportFileType = '02' THEN 1 ELSE 0 END AS ReportFileCount_Video
		                        FROM ReportFile A INNER JOIN ProjectShop B ON A.ProjectId = B.ProjectId AND A.ShopId = B.ShopId
				                                 INNER JOIN Shop C ON B.ShopId = C.ShopId
				                            ) X
                        WHERE 1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            sql += " GROUP BY X.ProjectId,X.ShopId,X.ShopCode,X.ShopName,X.ShopShortName";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportFileDto>().ToList();
        }
        public List<ReportFileDto> ReportFileListUploadALLByPageSearch(string projectId, string shopId, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;
           
            return ReportFileListUploadALLSearch(projectId,shopId).Skip(startIndex).Take(pageCount).ToList();
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
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            sql += " GROUP BY X.ProjectId,X.ShopId,X.ShopCode,X.ShopName,X.ShopShortName";
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
            string sql = @"DELETE ReportFile WHERE ProjectId = @ProjectId AND AND ShopId = @ShopId 
                        ";
            if (!string.IsNullOrEmpty(seqNO))
            {
                sql += " AND SeqNO = @SeqNO ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
    }
}