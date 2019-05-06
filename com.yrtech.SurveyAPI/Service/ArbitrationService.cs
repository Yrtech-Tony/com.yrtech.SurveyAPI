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
    public class ArbitrationService
    {
        Survey db = new Survey();
        #region 仲裁查询
        /// <summary>
        /// 查询需要进行仲裁的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<RecheckDto> GetNeedArbitrationInfo(string projectId, string shopId,string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                       ,new SqlParameter("@ShopId", shopId)
                                                       ,new SqlParameter("@SubjectId", subjectId)};
            Type t = typeof(RecheckDto);
            string sql = @"SELECT  A.*, B.ShopCode,B.ShopName,C.SubjectCode,C.[CheckPoint],C.OrderNO
                                    , D.AccountName AS RecheckUserName,ISNULL(E.AccountName, '') AS AgreeUserName
                                    , ISNULL(F.AccountName, '') AS LastConfirmUserName
                          FROM Recheck A INNER JOIN Shop B ON A.ShopId = B.ShopId
                                         INNER JOIN[Subject] C ON A.ProjectId = C.ProjectId AND A.SubjectId = C.SubjectId
                                         INNER JOIN UserInfo D ON A.RecheckUserId = D.Id
                                         LEFT JOIN UserInfo E ON A.AgreeUserId = E.Id
                                         LEFT JOIN UserInfo F ON A.LastConfirmUserId = F.Id
                        WHERE A.ProjectId = @ProjectId AND A.PassRecheck=0 AND A.AgreeCheck = 0";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }

            return db.Database.SqlQuery(t, sql, para).Cast<RecheckDto>().ToList();
        }
        public void SaveArbitrationInfo(string recheckId,string lastConfirm,string lastConfirmReason,int? lastConfirmUserId)
        {
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == Convert.ToInt32(recheckId))).FirstOrDefault();
            if (findOne!=null)
            {
                findOne.LastConfirmCheck = lastConfirm;
                findOne.LastConfirmDate = DateTime.Now;
                findOne.LastConfirmReason = lastConfirmReason;
                findOne.LastConfirmUserId = lastConfirmUserId;
            }
            db.SaveChanges();
        }
        #endregion
    }
}