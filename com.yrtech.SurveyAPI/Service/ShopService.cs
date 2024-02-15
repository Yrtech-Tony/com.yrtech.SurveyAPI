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
    public class ShopService
    {
        Survey db = new Survey();
        /// <summary>
        /// 获取经销商卷别信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ProjectShopExamTypeDto> GetProjectShopExamType(string brandId,string projectId, string shopId)
        {
            projectId = projectId == null ? "" : projectId;
            brandId = brandId == null ? "" : brandId;
            shopId = shopId == null ? "" : shopId;
            if (string.IsNullOrEmpty(projectId))
            {
                return new List<ProjectShopExamTypeDto>();
            }
            if (string.IsNullOrEmpty(brandId))
            {
                MasterService masterService = new MasterService();
                List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                if (projectList != null && projectList.Count > 0)
                {
                    brandId = projectList[0].BrandId.ToString();
                }
            }
            SqlParameter[] para = new SqlParameter[] {
                                            new SqlParameter("@BrandId", brandId),
                                            new SqlParameter("@ProjectId", projectId),
                                            new SqlParameter("@ShopId", shopId) };
            Type t = typeof(ProjectShopExamTypeDto);
            string sql = @"SELECT A.ShopId,ShopCode, ShopName,ShopshortName,B.ProjectId,ISNULL(Address,'') AS Address,
                                    CASE WHEN B.ProjectId IS NULL THEN ''
                                         ELSE(SELECT TOP 1 ProjectCode FROM Project WHERE ProjectId = @ProjectId)
                                    END AS ProjectCode,B.ExamTypeId,
                                    B.InDateTime,B.ModifyDateTime
                            FROM Shop A  LEFT JOIN  ProjectShopExamType B ON A.ShopId = B.ShopId AND B.ProjectId = @ProjectId
                            WHERE A.BrandId = @BrandId ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId =@ShopId";
            }
            sql += " ORDER BY A.ShopId";
            return db.Database.SqlQuery(t, sql, para).Cast<ProjectShopExamTypeDto>().ToList();
        }
        /// <summary>
        /// 保存经销商卷别类型
        /// </summary>
        /// <param name="projectShopExamType"></param>
        public void SaveProjectShopExamType(ProjectShopExamType projectShopExamType)
        {
            ProjectShopExamType findOne = db.ProjectShopExamType.Where(x => (x.ProjectId == projectShopExamType.ProjectId&&x.ShopId== projectShopExamType.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                projectShopExamType.InDateTime = DateTime.Now;
                projectShopExamType.ModifyDateTime = DateTime.Now;
                db.ProjectShopExamType.Add(projectShopExamType);
            }
            else
            {
                findOne.ExamTypeId = projectShopExamType.ExamTypeId;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = projectShopExamType.ModifyUserId;
            }
            db.SaveChanges();
        }
    }
}