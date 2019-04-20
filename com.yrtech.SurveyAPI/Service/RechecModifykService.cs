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
    public class RecheckModifService
    {
        Survey db = new Survey();
        #region 复审修改查询
        /// <summary>
        /// 查询需要进行复审修改的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<RecheckDto> GetNeedRecheckkModifyInfo(string projectId, string shopId,string subjectId,string agreeCheck)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                       ,new SqlParameter("@ShopId", shopId)
                                                       ,new SqlParameter("@SubjectId", subjectId)
                                                       ,new SqlParameter("@AgreeCheck",agreeCheck)};
            Type t = typeof(RecheckDto);
            string sql = @"SELECT  A.*, B.ShopCode,B.ShopName,C.SubjectCode,C.[CheckPoint],C.OrderNO
                                    , D.AccountName AS RecheckUserName,ISNULL(E.AccountName, '') AS AgreeUserName
                                    , ISNULL(F.AccountName, '') AS LastConfirmUserName
                          FROM Recheck A INNER JOIN Shop B ON A.ShopId = B.ShopId
                                         INNER JOIN[Subject] C ON A.ProjectId = C.ProjectId AND A.SubjectId = C.SubjectId
                                         INNER JOIN UserInfo D ON A.RecheckUserId = D.Id
                                         LEFT JOIN UserInfo E ON A.AgreeUserId = E.Id
                                         LEFT JOIN UserInfo F ON A.LastConfirmUserId = F.Id
                        WHERE A.ProjectId = @ProjectId AND A.PassRecheck=0";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }
            // 如果为null 就是查询未进行修改的数据
            if (agreeCheck == null||agreeCheck=="")
            {
                sql += "AND (A.AgreeCheck IS NULL OR AgreeCheck ='')";
            }
            else if (agreeCheck == "1" || agreeCheck=="0")
            {
                sql += "AND A.AgreeCheck = @AgreeCheck";
            }

            return db.Database.SqlQuery(t, sql, para).Cast<RecheckDto>().ToList();
        }
        public void SaveRecheckModifyInfo(string recheckId,bool? agreeCheck,string agreeReason,int? agreeUserId)
        {
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == Convert.ToInt32(recheckId))).FirstOrDefault();
            if (findOne!=null)
            {
                findOne.AgreeCheck = agreeCheck;
                findOne.AgreeDateTime = DateTime.Now;
                findOne.AgreeReason = agreeReason;
                findOne.AgreeUserId = agreeUserId;
            }
            db.SaveChanges();
        }
        #endregion
    }
}