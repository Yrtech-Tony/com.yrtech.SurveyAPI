using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace com.yrtech.SurveyAPI.Service
{
    public class RecheckService
    {
        Survey db = new Survey();
       
        #region 复审状态
        public List<RecheckStatusDto> GetShopRecheckStauts(string projectId, string shopId, string statusCode)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@StatusCode", statusCode)};
            Type t = typeof(RecheckStatusDto);
            string sql = "";
            sql = @"SELECT [RecheckStatusId]
                          ,A.[ProjectId]
                          ,B.ProjectCode,B.ProjectName
                          ,A.[ShopId],C.ShopCode,C.ShopName
                          ,A.[StatusCode]
                          ,A.[InUserId],E.AccountName
                          ,A.[InDateTime]
                      FROM [ReCheckStatus] A INNER JOIN Project B ON A.ProjectId = B.ProjectId
						                    INNER JOIN Shop C ON A.ShopId = C.ShopId
						                    INNER JOIN UserInfo E ON A.InUserId = E.Id
                                            INNER JOIN (SELECT ProjectId,ShopId,Max(StatusCode) AS StatusCode 
                                                        FROM [ReCheckStatus]
									                    GROUP BY ProjectId,ShopId) F ON F.ProjectId = A.ProjectId 
																                        AND F.ShopId = A.ShopId 
																                        AND F.StatusCode = A.StatusCode
                    WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += "AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(statusCode))
            {
                sql += "AND A.StatusCode = @StatusCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDto>().ToList();
        }
        //public List<RecheckStatusDtlDto> GetShopRecheckStautsDtl(string projectId, string shopId)
        //{
        //    SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
        //                                               new SqlParameter("@ShopId", shopId)};
        //    Type t = typeof(RecheckStatusDtlDto);
        //    string sql = "";
        //    sql = @"SELECT A.ProjectId,B.ProjectCode,B.ProjectName
	       //                 ,A.RecheckTypeId,RecheckTypeName
	       //                 ,InUserId AS RecheckUserId,E.AccountName AS RecheckUserName
        //            FROM RecheckStatusDtl A INNER JOIN Project B ON A.ProjectId = B.ProjectId
								//			INNER JOIN Shop C ON A.ShopId = C.ShopId
								//			INNER JOIN SubjectRecheckType D ON A.RecheckTypeId = D.RecheckTypeId
								//											AND D.ProjectId = B.ProjectId
								//			INNER JOIN UserInfo E ON A.InUserId = E.Id
        //            WHERE A.ProjectId = @ProjectId";
        //    if (!string.IsNullOrEmpty(shopId))
        //    {
        //        sql += "AND A.ShopId = @ShopId";
        //    }
        //    return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDtlDto>().ToList();
        //}
        public List<RecheckStatusDtlDto> GetShopRecheckStautsDtl(string projectId, string shopId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(RecheckStatusDtlDto);
            string sql = "";
            sql = @"SELECT A.*, B.RecheckTypeName
                            ,CASE WHEN C.InUserId IS NOT NULL THEN '复审完毕'
                            ELSE '' END AS StatusName
                            ,C.InUserId AS RecheckUserId,D.AccountName AS RecheckUserName,C.InDateTime AS RecheckDateTime
                    FROM Shop A INNER JOIN SubjectRecheckType B ON 1=1 
			                    LEFT JOIN RecheckStatusDtl C ON A.ShopId = C.ShopId 
										                    AND C.ProjectId = @ProjectId 
										                    AND B.RecheckTypeId = C.RecheckTypeId
			                    LEFT JOIN UserInfo D ON C.InUserId = D.Id
                    WHERE 1 = 1";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += "AND A.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckStatusDtlDto>().ToList();
        }
        public void SaveRecheckStatus(ReCheckStatus status)
        {
            ReCheckStatus findOne = db.ReCheckStatus.Where(x => (x.ProjectId == status.ProjectId && x.ShopId == status.ShopId && x.StatusCode == status.StatusCode)).FirstOrDefault();
            if (findOne == null)
            {
                status.InDateTime = DateTime.Now;
                db.ReCheckStatus.Add(status);
            }
            db.SaveChanges();
        }
        public void SaveRecheckStatusDtl(RecheckStatusDtl statusDtl)
        {
            RecheckStatusDtl findOne = db.RecheckStatusDtl.Where(x => (x.ProjectId == statusDtl.ProjectId&&x.ShopId==statusDtl.ShopId&&x.RecheckTypeId==statusDtl.RecheckTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                statusDtl.InDateTime = DateTime.Now;
                db.RecheckStatusDtl.Add(statusDtl);
            }
            db.SaveChanges();
        }
        #endregion
    }
}