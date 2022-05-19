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
        public List<RecheckDto> GetRecheckInfo(string projectId, string shopId, string subjectId, bool? passRecheck,bool? agreeCheck)
        {
            if (projectId == null) projectId = "";
            if (shopId == null) shopId = "";
            if (subjectId == null) subjectId = "";
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
                        WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }
            if (agreeCheck.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@AgreeCheck", agreeCheck) }).ToArray();
                sql += " AND A.AgreeCheck = @AgreeCheck";
            }
            if (passRecheck.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@PassRecheck", passRecheck) }).ToArray();
                sql += " AND A.PassRecheck = @PassRecheck";
            }

            return db.Database.SqlQuery(t, sql, para).Cast<RecheckDto>().ToList();
        }
        public void SaveRecheckModifyInfo(string recheckId, bool? agreeCheck, string agreeReason, int? agreeUserId)
        {
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == Convert.ToInt32(recheckId))).FirstOrDefault();
            if (findOne != null)
            {
                findOne.AgreeCheck = agreeCheck;
                findOne.AgreeDateTime = DateTime.Now;
                findOne.AgreeReason = agreeReason;
                findOne.AgreeUserId = agreeUserId;
            }
            db.SaveChanges();
        }
        public void SaveSupervisionSpotCheck(string recheckId, string supervisionSpotCheckContent, int? supervisionSpotCheckUserId)
        {
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == Convert.ToInt32(recheckId))).FirstOrDefault();
            if (findOne != null)
            {
                findOne.SupervisionSpotCheckContent = supervisionSpotCheckContent;
                findOne.SupervisionSpotCheckDateTime = DateTime.Now;
                findOne.SupervisionSpotCheckUserId = supervisionSpotCheckUserId;
            }
            db.SaveChanges();
        }
        public void SavePMSpotCheck(string recheckId, string pmSpotCheckContent, int? pmSpotCheckUserId)
        {
            ReCheck findOne = db.ReCheck.Where(x => (x.RecheckId == Convert.ToInt32(recheckId))).FirstOrDefault();
            if (findOne != null)
            {
                findOne.PMSpotCheckContent = pmSpotCheckContent;
                findOne.PMSpotCheckDateTime = DateTime.Now;
                findOne.PMSpotCheckUserId = pmSpotCheckUserId;
            }
            db.SaveChanges();
        }
        #endregion
    }
}