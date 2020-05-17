using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace com.yrtech.SurveyAPI.Service
{
    public class AccountService
    {
        Survey db = new Survey();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public List<AccountDto> Login(string accountId, string password,string tenantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AccountId", accountId),
                                                       new SqlParameter("@Password",password),new SqlParameter("@TenantId",tenantId)};
            Type t = typeof(AccountDto);
            string sql = @"SELECT A.Id,A.TenantId,AccountId,AccountName,ISNULL(UseChk,0) AS UseChk,A.TelNO,A.Email,A.HeadPicUrl,A.RoleType,A.UserType
                            FROM UserInfo A
                            WHERE AccountId = @AccountId AND[Password] = @Password AND TenantId = @TenantId
                            AND UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<AccountDto>().ToList();
        }
        
        /// <summary>
        ///更新密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UpdatePassword(string userId, string password)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@Password", password)};
            Type t = typeof(UserInfo);
            string sql = @"UPDATE [UserInfo] SET Password=@Password Where Id = @UserId";
            return db.Database.ExecuteSqlCommand(sql, para) > 0;
        }
        #region 根据权限查询基本信息
        /// <summary>
        /// 根据权限和账号查询对应的经销商信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<ShopDto> GetShopListByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(ShopDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                sql = @"SELECT A.*,C.AreaId
                        FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
			                        INNER JOIN Area C ON B.AreaId = C.AreaId -- smallArea
                        WHERE A.BrandId = @BrandId ";
            }
            else if (roleType == "B_Bussiness")
            {
                sql = @"SELECT A.*,C.AreaId
                        FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
			                        INNER JOIN Area C ON B.AreaId = C.AreaId 
			                        INNER JOIN Area D ON C.ParentId = D.AreaId
			                        INNER JOIN Area E ON D.ParentId = E.AreaId 
			                        INNER JOIN Area F ON E.ParentId = F.AreaId 
			                        INNER JOIN Area G ON F.ParentId = G.AreaId 
			                        INNER JOIN UserInfoObject H ON G.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            else if (roleType == "B_WideArea")
            {
                sql = @"SELECT A.*,C.AreaId
                        FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
			                        INNER JOIN Area C ON B.AreaId = C.AreaId 
			                        INNER JOIN Area D ON C.ParentId = D.AreaId 
			                        INNER JOIN Area E ON D.ParentId = E.AreaId 
			                        INNER JOIN Area F ON E.ParentId = F.AreaId 
			                        INNER JOIN UserInfoObject H ON F.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            else if (roleType == "B_BigArea")
            {
                sql = @"SELECT A.*,C.AreaId 
                        FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
			                        INNER JOIN Area C ON B.AreaId = C.AreaId 
			                        INNER JOIN Area D ON C.ParentId = D.AreaId 
			                        INNER JOIN Area E ON D.ParentId = E.AreaId 
			                        INNER JOIN UserInfoObject H ON E.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            else if (roleType == "B_MiddleArea")
            {
                sql = @"SELECT A.*,C.AreaId 
                        FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
			                        INNER JOIN Area C ON B.AreaId = C.AreaId 
			                        INNER JOIN Area D ON C.ParentId = D.AreaId 
			                        INNER JOIN UserInfoObject H ON D.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            else if (roleType == "B_SmallArea")
            {
                sql = @"SELECT A.*,C.AreaId 
                        FROM Shop A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
			                        INNER JOIN Area C ON B.AreaId = C.AreaId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId";
            }
            else if (roleType == "B_Group")
            {
                sql = @"SELECT A.* 
                        FROM Shop A INNER JOIN [Group] B ON A.GroupId = B.GroupId
			                        INNER JOIN UserInfoObject H ON B.GroupId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            else if (roleType == "B_Shop")
            {

                sql = @"SELECT A.*,C.AreaId
                        FROM Shop A 
                                    INNER JOIN AreaShop B ON A.ShopId = B.ShopId
                                    INNER JOIN Area C ON B.AreaId = C.AreaId                                   
			                        INNER JOIN UserInfoObject H ON A.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<ShopDto>().ToList();
        }
        /// <summary>
        /// 根据权限和账号查询对应的业务类型
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<AreaDto> GetBussinessListByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(AreaDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN UserInfoObject H ON A.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND A.AreaType = 'Bussiness' ";
            }
            else if (roleType == "B_Bussiness")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN UserInfoObject H ON A.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'Bussiness '";
            }
            else if (roleType == "B_WideArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN Area B ON B.ParentId = A.AreaId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'Bussiness' ";
            }
            else if (roleType == "B_BigArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'Bussiness'";
            }
            else if (roleType == "B_MiddleArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
                                    INNER JOIN Area D ON D.ParentId = C.AreaId 
			                        INNER JOIN UserInfoObject H ON D.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'Bussiness' ";
            }
            else if (roleType == "B_SmallArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
                                    INNER JOIN Area D ON D.ParentId = C.AreaId 
                                    INNER JOIN Area E ON E.ParentId = D.AreaId 
			                        INNER JOIN UserInfoObject H ON E.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'Bussiness' ";
            }
            else if (roleType == "B_Shop")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN Area B ON B.ParentId = A.AreaId
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
                                    INNER JOIN Area D ON D.ParentId = C.AreaId 
                                    INNER JOIN Area E ON E.ParentId = D.AreaId 
                                    INNER JOIN AreaShop F ON E.AreaId = F.AreaId
                                    INNER JOIN Shop G ON F.ShopId = G.ShopId
			                        INNER JOIN UserInfoObject H ON G.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'Bussiness' ";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();
        }
        /// <summary>
        /// 根据权限和账号查询对应的广域区域
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></retur``ns>
        public List<AreaDto> GetWideAreaByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(AreaDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId  AND A.AreaType = 'WideArea' ";
            }
            else if (roleType == "B_Bussiness")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'WideArea' ";
            }
            else if (roleType == "B_WideArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN UserInfoObject H ON A.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'WideArea' ";
            }
            else if (roleType == "B_BigArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'WideArea' ";
            }
            else if (roleType == "B_MiddleArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'WideArea' ";
            }
            else if (roleType == "B_SmallArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
                                    INNER JOIN Area D ON D.ParentId = C.AreaId 
			                        INNER JOIN UserInfoObject H ON D.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'WideArea' ";
            }
            else if (roleType == "B_Shop")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
                                    INNER JOIN Area D ON D.ParentId = C.AreaId 
                                    INNER JOIN AreaShop E ON D.AreaId = E.AreaId
                                    INNER JOIN Shop F ON E.ShopId = F.ShopId
			                        INNER JOIN UserInfoObject H ON F.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'WideArea' ";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();
        }
        /// <summary>
        /// 根据权限和账号查询对应的大区区域
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<AreaDto> GetBigAreaByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(AreaDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId  AND A.AreaType = 'BigArea'";
            }
            else if (roleType == "B_Bussiness")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'BigArea'";
            }
            else if (roleType == "B_WideArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'BigArea'";
            }
            else if (roleType == "B_BigArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN UserInfoObject H ON A.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'BigArea'";
            }
            else if (roleType == "B_MiddleArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'BigArea'";
            }
            else if (roleType == "B_SmallArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'BigArea'";
            }
            else if (roleType == "B_Shop")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId 
                                    INNER JOIN Area C ON C.ParentId = B.AreaId 
                                    INNER JOIN AreaShop D ON C.AreaId = D.AreaId
                                    INNER JOIN Shop E ON D.ShopId = E.ShopId
			                        INNER JOIN UserInfoObject H ON E.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'BigArea'";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();
        }
        /// <summary>
        /// 根据权限和账号查询对应的中区区域
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<AreaDto> GetMiddleAreaByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(AreaDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                 sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
                                    INNER JOIN Area D ON D.AreaId = C.ParentId 
			                        INNER JOIN UserInfoObject H ON D.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId  AND A.AreaType = 'MiddleArea'";
            }
            else if (roleType == "B_Bussiness")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
                                    INNER JOIN Area D ON D.AreaId = C.ParentId 
			                        INNER JOIN UserInfoObject H ON D.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'MiddleArea'";
            }
            else if (roleType == "B_WideArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'MiddleArea'";
            }
            else if (roleType == "B_BigArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'MiddleArea'";
            }
            else if (roleType == "B_MiddleArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN UserInfoObject H ON A.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'MiddleArea'";
            }
            else if (roleType == "B_SmallArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'MiddleArea'";
            }
            else if (roleType == "B_Shop")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.ParentId = A.AreaId
                                    INNER JOIN AreaShop C ON B.AreaId = C.AreaId
                                    INNER JOIN Shop D ON C.ShopId = D.ShopId
			                        INNER JOIN UserInfoObject H ON D.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'MiddleArea'";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();
        }
        /// <summary>
        /// 根据权限和账号查询对应的小区区域
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<AreaDto> GetSmallAreaByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(AreaDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
                                    INNER JOIN Area D ON D.AreaId = C.ParentId 
                                    INNER JOIN Area E ON E.AreaId = D.ParentId 
			                        INNER JOIN UserInfoObject H ON E.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId  AND A.AreaType = 'SmallArea'";
            }
            else if (roleType == "B_Bussiness")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
                                    INNER JOIN Area D ON D.AreaId = C.ParentId 
                                    INNER JOIN Area E ON E.AreaId = D.ParentId 
			                        INNER JOIN UserInfoObject H ON E.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'SmallArea'";
            }
            else if (roleType == "B_WideArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
                                    INNER JOIN Area D ON D.AreaId = C.ParentId 
			                        INNER JOIN UserInfoObject H ON D.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'SmallArea'";
            }
            else if (roleType == "B_BigArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
                                    INNER JOIN Area C ON C.AreaId = B.ParentId 
			                        INNER JOIN UserInfoObject H ON C.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'SmallArea'";
            }
            else if (roleType == "B_MiddleArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN Area B ON B.AreaId = A.ParentId 
			                        INNER JOIN UserInfoObject H ON B.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'SmallArea'";
            }
            else if (roleType == "B_SmallArea")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
			                        INNER JOIN UserInfoObject H ON A.AreaId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'SmallArea'";
            }
            else if (roleType == "B_Shop")
            {
                sql = @"SELECT A.* 
                        FROM Area A 
                                    INNER JOIN AreaShop B ON A.AreaId = B.AreaId
                                    INNER JOIN Shop C ON B.ShopId = C.ShopId
			                        INNER JOIN UserInfoObject H ON C.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId AND A.AreaType = 'SmallArea'";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();
        }
        /// <summary>
        /// 根据权限查询集团信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="userId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<GroupDto> GetGroupByRole(string brandId, string userId, string roleType)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId)
                                                        ,new SqlParameter("@UserId", userId) };
            Type t = typeof(GroupDto);
            string sql = "";
            if (roleType == "B_Brand")
            {
                sql = @"SELECT A.* 
                        FROM [Group] A 
                        WHERE A.BrandId = @BrandId ";
            }
            else if (roleType == "B_Group")
            {
                sql = @"SELECT A.* 
                        FROM [Group] A 
			                        INNER JOIN UserInfoObject H ON A.GroupId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId";
            }
            else if (roleType == "B_Shop")
            {
                sql = @"SELECT A.* 
                        FROM [Group] A 
                                    INNER JOIN Shop B ON A.GroupId = B.GroupId
			                        INNER JOIN UserInfoObject H ON B.ShopId = H.ObjectId
                        WHERE A.BrandId = @BrandId AND H.UserId = @UserId ";
            }
            sql += " AND A.UseChk = 1";
            return db.Database.SqlQuery(t, sql, para).Cast<GroupDto>().ToList();
        }
        public List<Brand> GetBrandByRole(string tenantId, string userId, string roleType)
        {
            tenantId = tenantId == null ? "" : tenantId;
            userId = userId == null ? "" : userId;
            roleType = roleType == null ? "" : roleType;
           // brandId = brandId == null ? "" : brandId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                       new SqlParameter("@UserId", userId)};
            Type t = typeof(Brand);
            string sql = "";

            if (roleType == "S_Sysadmin")
            {
                sql = @"SELECT A.* FROM Brand A";
            }
            else if (roleType == "S_BrandSysadmin" ||
                roleType == "S_Execute" ||
                roleType == "S_Recheck")
            {
                sql = @"SELECT A.* 
                        FROM Brand A INNER JOIN UserInfoBrand B ON A.BrandId = B.BrandId 
                        WHERE A.TenantId = @TenantId AND B.UserId = @UserId";
            }
            else
            {
                sql = @"SELECT A.* FROM Brand A INNER JOIN UserInfo B ON A.BrandId = B.BrandId
                      WHERE B.Id = @UserId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Brand>().ToList();

        }
        #endregion
    }
}