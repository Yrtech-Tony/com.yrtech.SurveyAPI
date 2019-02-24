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
    public class AreaService
    {
        Survey db = new Survey();
        #region 厂商各权限登陆时使用
        /// <summary>
        /// 获取下级区域信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<AreaDto> GetChildAreaByParentId(string parentId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ParentId", parentId) };
            string sql = "";
            sql = @"SELECT * FROM Area WHERE ParentId = @ParentId";
            return db.Database.SqlQuery<AreaDto>(sql, para).ToList();
        }
        public List<ShopDto> GetShopByAreaId(string areaId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AreaId", areaId) };
            string sql = "";
            sql = @"SELECT * FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId WHERE AreaId = @AreaId";
            return db.Database.SqlQuery<ShopDto>(sql, para).ToList();
        }
        #endregion
    }
}