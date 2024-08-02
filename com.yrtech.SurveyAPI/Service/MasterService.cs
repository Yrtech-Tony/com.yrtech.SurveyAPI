﻿using CDO;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyAPI.DTO.Master;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.SurveyAPI.Service
{
    public class MasterService
    {
        Survey db = new Survey();
        public List<AppVersion> GetAppVersion()
        {
            SqlParameter[] para = new SqlParameter[] { };
            Type t = typeof(AppVersion);
            string sql = @"SELECT *
                            FROM AppVersion A ";
            return db.Database.SqlQuery(t, sql, para).Cast<AppVersion>().ToList();
        }
        public List<RoleType> GetRoleType(string type, string roleTypeCode, string roleTypeName)
        {

            if (type == null) type = "";
            if (roleTypeCode == null) roleTypeCode = "";
            if (roleTypeName == null) roleTypeName = "";
            Type t = typeof(RoleType);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@Type", type)
                                                    ,new SqlParameter("@RoleTypeCode", roleTypeCode)
                                                    , new SqlParameter("@RoleTypeName", roleTypeName) };
            string sql = "";
            sql = @"SELECT *
                   FROM [RoleType] WHERE 1=1";
            if (!string.IsNullOrEmpty(type))
            {
                sql += " AND Type = @Type";
            }
            if (!string.IsNullOrEmpty(roleTypeCode))
            {
                sql += " AND RoleTypeCode = @RoleTypeCode";
            }
            if (!string.IsNullOrEmpty(roleTypeName))
            {
                sql += " AND RoleTypeName = @RoleTypeName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RoleType>().ToList();
        }
        public List<HiddenColumn> GetHiddenCode(string hiddenCodeGroup, string hiddenCode)
        {

            if (hiddenCodeGroup == null) hiddenCodeGroup = "";
            if (hiddenCode == null) hiddenCode = "";
            Type t = typeof(HiddenColumn);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@HiddenCodeGroup", hiddenCodeGroup),
                                                        new SqlParameter("@HiddenCode", hiddenCode) };
            string sql = "";
            sql = @"SELECT *
                   FROM [HiddenColumn] WHERE 1=1";
            if (!string.IsNullOrEmpty(hiddenCodeGroup))
            {
                sql += " AND HiddenCodeGroup = @HiddenCodeGroup";
            }
            if (!string.IsNullOrEmpty(hiddenCode))
            {
                sql += " AND HiddenCode = @HiddenCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<HiddenColumn>().ToList();
        }
        #region 获取用户权限菜单
        public List<RoleProgramDto> GetTenantProgram(string tenantId, bool? isChild, string parentId)
        {

            if (tenantId == null) tenantId = "";

            Type t = typeof(RoleProgramDto);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId), new SqlParameter("@ParentId", parentId) };
            string sql = "";
            sql = @"SELECT A.*,'S_Sysadmin' AS RoleTypeCode,C.TenantId 
                    FROM Program A 
                    INNER JOIN TenantProgram C ON A.ProgramCode = C.ProgramCode  
                    WHERE C.TenantId=@TenantId ";
            if (isChild.HasValue)
            {
                if (isChild == false)
                {
                    sql += " AND A.UpperProgramId IS NULL";
                }
                else
                {
                    sql += " AND A.UpperProgramId IS NOT NULL";
                    if (!string.IsNullOrEmpty(parentId))
                    {
                        sql += " AND A.UpperProgramId = @ParentId";
                    }
                }
            }
            sql += " ORDER BY A.ShowOrder ";
            return db.Database.SqlQuery(t, sql, para).Cast<RoleProgramDto>().ToList();
        }
        public List<RoleProgramDto> GetRoleProgram(string tenantId, string roleTypeCode, bool? isChild, string parentId)
        {
            if (roleTypeCode == null) roleTypeCode = "";
            if (tenantId == null) tenantId = "";
            if (parentId == null) parentId = "";
            Type t = typeof(RoleProgramDto);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId)
                                                        , new SqlParameter("@RoleTypeCode", roleTypeCode)
                                                        , new SqlParameter("@ParentId", parentId)};
            string sql = "";
            sql += @"SELECT A.*,B.RoleTypeCode,C.TenantId 
                    FROM Program A 
                    INNER JOIN RoleProgram B ON A.ProgramCode = B.ProgramCode AND B.TenantId = @TenantId
                    INNER JOIN TenantProgram C ON A.ProgramCode = C.ProgramCode  AND B.TenantId = C.TenantId
                    WHERE C.TenantId=@TenantId AND B.RoleTypeCode = @RoleTypeCode";
            if (isChild.HasValue)
            {
                if (isChild == false)
                {
                    sql += " AND A.UpperProgramId IS NULL";
                }
                else
                {
                    sql += " AND A.UpperProgramId IS NOT NULL";
                    if (!string.IsNullOrEmpty(parentId))
                    {
                        sql += " AND A.UpperProgramId = @ParentId";
                    }
                }
            }
            sql += " ORDER BY A.ShowOrder ";
            return db.Database.SqlQuery(t, sql, para).Cast<RoleProgramDto>().ToList();
        }
        public List<RoleProgramDto> GetRoleProgram_Tree(string tenantId, string roleTypeCode)
        {
            List<RoleProgramDto> list_Parent = new List<RoleProgramDto>();
            if (roleTypeCode == "S_Sysadmin") // 租户管理员直接查询租户的权限信息
            {
                list_Parent = GetTenantProgram(tenantId, false, "");

                foreach (RoleProgramDto parent in list_Parent)
                {
                    parent.ChildMenu = GetTenantProgram(tenantId, true, parent.ProgramId.ToString());
                }
            }
            else
            {
                list_Parent = GetRoleProgram(tenantId, roleTypeCode, false, "");

                foreach (RoleProgramDto parent in list_Parent)
                {
                    parent.ChildMenu = GetRoleProgram(tenantId, roleTypeCode, true, parent.ProgramId.ToString());
                }
            }
            return list_Parent;
        }
        #endregion
        #region 租户信息
        /// <summary>
        /// 查询租户信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<Tenant> GetTenant(string tenantId, string tenantCode, string tenantName)
        {
            if (tenantId == null) tenantId = "";
            if (tenantCode == null) tenantCode = "";
            if (tenantName == null) tenantName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId)
                                                    ,new SqlParameter("@TenantCode", tenantCode)
                                                    ,new SqlParameter("@TenantName", tenantName) };
            Type t = typeof(Tenant);
            string sql = "";

            sql = @"SELECT *
	                    FROM Tenant WHERE 1=1 ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(tenantCode))
            {
                sql += " AND TenantCode = @TenantCode";
            }
            if (!string.IsNullOrEmpty(tenantName))
            {
                sql += " AND TenantName = @TenantName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Tenant>().ToList();

        }
        /// <summary>
        /// 保存租户
        /// </summary>
        /// <param name="tenant"></param>
        public void SaveTenant(Tenant tenant)
        {
            Tenant findOne = db.Tenant.Where(x => (x.TenantId == tenant.TenantId)).FirstOrDefault();
            if (findOne == null)
            {
                tenant.InDateTime = DateTime.Now;
                tenant.ModifyDateTime = DateTime.Now;
                //tenant.MemberType = "Common";
            }
            db.Tenant.Add(tenant);
            db.SaveChanges();
        }
        #endregion
        #region 省份信息
        /// <summary>
        /// 查询租户信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<Province> GetProvince(string provinceId, string provinceName)
        {
            if (provinceId == null) provinceId = "";
            if (provinceName == null) provinceName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProvinceId", provinceId)
                                                    ,new SqlParameter("@ProvinceName", provinceName) };
            Type t = typeof(Province);
            string sql = "";

            sql = @"SELECT *
	                    FROM Province WHERE 1=1 ";
            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += " AND ProvinceId = @ProvinceId";
            }
            if (!string.IsNullOrEmpty(provinceName))
            {
                sql += " AND ProvinceName = @ProvinceName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Province>().ToList();

        }
        #endregion
        #region 品牌信息
        /// <summary>
        /// 查询品牌信息,
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Brand> GetBrand(string tenantId, string brandId, string brandCode)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            brandCode = brandCode == null ? "" : brandCode;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@BrandCode", brandCode)};
            Type t = typeof(Brand);
            string sql = "";

            sql = @"SELECT A.*
                    FROM Brand A WHERE 1=1";

            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND A.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(brandCode))
            {
                sql += " AND A.BrandCode = @BrandCode";
            }
            sql += " ORDER BY BrandId DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<Brand>().ToList();

        }
        /// <summary>
        /// 保存品牌
        /// </summary>
        /// <param name="brand"></param>
        public void SaveBrand(Brand brand)
        {

            Brand findOne = db.Brand.Where(x => (x.BrandId == brand.BrandId)).FirstOrDefault();
            if (findOne == null)
            {
                brand.InDateTime = DateTime.Now;
                brand.ModifyDateTime = DateTime.Now;
                db.Brand.Add(brand);
            }
            else
            {
                findOne.BrandCode = brand.BrandCode;
                findOne.BrandName = brand.BrandName;
                findOne.Remark = brand.Remark;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = brand.ModifyUserId;
            }
            db.SaveChanges();
        }
        #endregion
        #region 用户信息管理
        /// <summary>
        /// 获取账号的基本信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfo(string tenantId, string brandId, string userId, string accountId, string accountName, string roleTypeCode, string telNO, string email, bool? useChk, string openId)
        {
            if (tenantId == null) tenantId = "";
            if (brandId == null) brandId = "";
            if (userId == null) userId = "";
            if (accountId == null) accountId = "";
            if (accountName == null) accountName = "";
            if (roleTypeCode == null) roleTypeCode = "";
            if (telNO == null) telNO = "";
            if (email == null) email = "";
            if (openId == null) openId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@AccountId", accountId),
                                                        new SqlParameter("@AccountName", accountName),
                                                        new SqlParameter("@RoleType", roleTypeCode),
                                                        new SqlParameter("@TelNO", telNO),
                                                        new SqlParameter("@Email", email),
                                                        new SqlParameter("@OpenId", openId)
                                                        };
            Type t = typeof(UserInfo);
            string sql = @"SELECT A.*
                            FROM [UserInfo] A 
                            WHERE 1=1";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND A.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND A.Id = @UserId";
            }
            if (!string.IsNullOrEmpty(accountId))
            {
                sql += " AND A.AccountId = @AccountId";
            }
            if (!string.IsNullOrEmpty(accountName))
            {
                sql += " AND A.AccountName LIKE '%'+@AccountName+'%'";
            }
            if (!string.IsNullOrEmpty(roleTypeCode))
            {
                sql += " AND A.RoleType = @RoleType";
            }
            if (!string.IsNullOrEmpty(telNO))
            {
                sql += " AND TelNO = @TelNO";
            }
            if (!string.IsNullOrEmpty(email))
            {
                sql += " AND Email = @Email";
            }
            if (useChk.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND UseChk = @UseChk";
            }
            if (!string.IsNullOrEmpty(openId))
            {
                sql += " AND OpenId = @OpenId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfo>().ToList();
        }
        /// <summary>
        /// 保存账号信息
        /// </summary>
        /// <param name="userinfo"></param>
        public void SaveUserInfo(UserInfo userinfo)
        {
            UserInfo findOne = db.UserInfo.Where(x => (x.Id == userinfo.Id)).FirstOrDefault();
            if (findOne == null)
            {
                userinfo.InDateTime = DateTime.Now;
                userinfo.ModifyDateTime = DateTime.Now;
                db.UserInfo.Add(userinfo);
            }
            else
            {
                findOne.AccountId = userinfo.AccountId;
                findOne.Password = userinfo.Password;
                findOne.AccountName = userinfo.AccountName;
                findOne.Email = userinfo.Email;
                findOne.TelNO = userinfo.TelNO;
                findOne.HeadPicUrl = userinfo.HeadPicUrl;
                findOne.UseChk = userinfo.UseChk;
                findOne.BrandId = userinfo.BrandId;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = userinfo.ModifyUserId;
                findOne.OpenId = userinfo.OpenId;
            }
            db.SaveChanges();
        }
        public void DeleteUserInfo(int userId)
        {
            UserInfo findone = db.UserInfo.Where(x => x.Id == userId).FirstOrDefault();
            db.UserInfo.Remove(findone);
            db.SaveChanges();
        }
        public List<UserInfoBrandDto> GetUserInfoBrand(string tenantId, string userId, string brandId)
        {
            if (tenantId == null) tenantId = "";
            if (brandId == null) brandId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@UserId", userId)
                                                        };
            Type t = typeof(UserInfoBrandDto);
            string sql = @"SELECT A.TenantId,B.UserId,C.BrandCode,C.BrandName,C.BrandId,B.Id
                            FROM [UserInfo] A INNER JOIN UserInfoBrand B ON A.Id = B.UserId
                                              INNER JOIN Brand C ON B.BrandId = C.BrandId
                            WHERE 1=1";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND A.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND B.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND A.Id = @UserId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfoBrandDto>().ToList();
        }
        public void SaveUserInfoBrand(UserInfoBrand userInfoBrand)
        {
            UserInfoBrand findOne = db.UserInfoBrand.Where(x => (x.UserId == userInfoBrand.UserId && x.BrandId == userInfoBrand.BrandId)).FirstOrDefault();
            if (findOne == null)
            {
                userInfoBrand.InDateTime = DateTime.Now;
                userInfoBrand.ModifyDateTime = DateTime.Now;
                db.UserInfoBrand.Add(userInfoBrand);
            }
            db.SaveChanges();
        }
        public void DeleteUserInfoBrand(int userInfoBrandId)
        {
            UserInfoBrand findone = db.UserInfoBrand.Where(x => x.Id == userInfoBrandId).FirstOrDefault();
            db.UserInfoBrand.Remove(findone);
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@FileId", fileId) };
            //Type t = typeof(int);
            //string sql = @"DELETE [AppealFile] WHERE FileId = @FileId";
            //db.Database.SqlQuery(t, sql, para).Cast<int>().ToList();
        }
        public List<UserInfoObjectDto> GetUserInfoObject(string tenantId, string userId, string objectId, string roleTypeCode)
        {
            if (tenantId == null) tenantId = "";
            if (objectId == null) objectId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@ObjectId", objectId),
                                                        new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@RoleTypeCode", roleTypeCode)
                                                        };
            Type t = typeof(UserInfoObjectDto);

            string sql = "";
            if (roleTypeCode == "B_Bussiness")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='Bussiness'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_WideArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='WideArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_BigArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='BigArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_MiddleArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='MiddleArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_SmallArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='SmallArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_Group")
            {
                sql = @"SELECT B.Id,B.UserId,C.GroupCode AS ObjectCode,C.GroupName AS ObjectName,C.GroupId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN [Group] C ON B.ObjectId = C.GroupId
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_Shop")
            {
                sql = @"SELECT B.Id,B.UserId,C.ShopCode AS ObjectCode,C.ShopName AS ObjectName,C.ShopId AS ObjectId,ISNULL(A.TelNO,'') TelNO,C.ProvinceId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                            INNER JOIN Shop C ON B.ObjectId = C.ShopId
                          WHERE 1=1";
            }
            else if (roleTypeCode == "S_Execute")// 设置执行人员有权限的经销商
            {
                sql = @"SELECT B.Id,B.UserId,C.ShopCode AS ObjectCode,C.ShopName AS ObjectName,C.ShopId AS ObjectId,ISNULL(A.TelNO,'') TelNO
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                            INNER JOIN Shop C ON B.ObjectId = C.ShopId
                          WHERE 1=1 ";
            }
            else
            {

            }
            sql += " AND A.RoleType  = @RoleTypeCode";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND A.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(objectId))
            {
                sql += " AND B.ObjectId = @ObjectId";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND A.Id = @UserId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfoObjectDto>().ToList();
        }
        public void SaveUserInfoObject(UserInfoObject userInfoObject)
        {
            UserInfoObject findOne = db.UserInfoObject.Where(x => (x.UserId == userInfoObject.UserId && x.ObjectId == userInfoObject.ObjectId)).FirstOrDefault();
            if (findOne == null)
            {
                userInfoObject.InDateTime = DateTime.Now;
                db.UserInfoObject.Add(userInfoObject);
            }
            db.SaveChanges();
        }
        public void DeleteUserInfoObject(int userObjectId)
        {
            UserInfoObject findone = db.UserInfoObject.Where(x => x.Id == userObjectId).FirstOrDefault();
            db.UserInfoObject.Remove(findone);
            db.SaveChanges();
        }
        #endregion
        #region 区域管理
        public List<AreaDto> GetArea(string areaId, string brandId, string areaCode, string areaName, string areaType, string parentId, bool? useChk)
        {
            areaId = areaId == null ? "" : areaId;
            areaCode = areaCode == null ? "" : areaCode;
            brandId = brandId == null ? "" : brandId;
            areaName = areaName == null ? "" : areaName;
            areaType = areaType == null ? "" : areaType;
            parentId = parentId == null ? "" : parentId;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@AreaCode", areaCode),
                                                        new SqlParameter("@AreaName", areaName),
                                                        new SqlParameter("@AreaType", areaType),
                                                        new SqlParameter("@ParentId", parentId),
                                                        new SqlParameter("@AreaId", areaId)};
            Type t = typeof(AreaDto);
            string sql = "";

            sql = @"SELECT A.*,
                        (SELECT TOP 1 AreaCode FROM Area X WHERE X.AreaId = A.ParentId) AS ParentCode,
                        (SELECT TOP 1 AreaName FROM Area Y WHERE Y.AreaId = A.ParentId) AS ParentName,
                        (SELECT TOP 1 HiddenName FROM HiddenColumn Z WHERE Z.HiddenCode = A.AreaType AND Z.HiddenCodeGroup = '区域类型') AS AreaTypeName
                    FROM Area A WHERE 1=1";

            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND A.AreaId = @AreaId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(areaCode))
            {
                sql += " AND A.AreaCode = @AreaCode";
            }
            if (!string.IsNullOrEmpty(areaType))
            {
                sql += " AND A.AreaType = @AreaType";
            }
            if (!string.IsNullOrEmpty(areaName))
            {
                sql += " AND A.AreaName LIKE '%'+ @AreaName+'%'";
            }
            if (!string.IsNullOrEmpty(parentId))
            {
                sql += " AND A.ParentId = @ParentId";
            }
            if (useChk.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND A.UseChk = @UseChk";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();

        }
        public void SaveArea(Area area)
        {
            Area findOne = db.Area.Where(x => (x.AreaId == area.AreaId)).FirstOrDefault();
            if (findOne == null)
            {
                area.InDateTime = DateTime.Now;
                area.ModifyDateTime = DateTime.Now;
                db.Area.Add(area);
            }
            else
            {
                findOne.AreaCode = area.AreaCode;
                findOne.AreaName = area.AreaName;
                findOne.AreaType = area.AreaType;
                findOne.ParentId = area.ParentId;
                findOne.UseChk = area.UseChk;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = area.ModifyUserId;
            }
            db.SaveChanges();
        }
        #endregion
        #region 集团管理
        public List<Group> GetGroup(string brandId, string groupId, string groupCode, string groupName, bool? useChk)
        {
            groupCode = groupCode == null ? "" : groupCode;
            brandId = brandId == null ? "" : brandId;
            groupName = groupName == null ? "" : groupName;
            groupId = groupId == null ? "" : groupId;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@GroupId", groupId),
                                                        new SqlParameter("@GroupCode", groupCode),
                                                        new SqlParameter("@GroupName", groupName)};
            Type t = typeof(Group);
            string sql = "";

            sql = @"SELECT A.*
                    FROM [Group] A WHERE 1=1";

            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                sql += " AND A.GroupId = @GroupId";
            }
            if (!string.IsNullOrEmpty(groupCode))
            {
                sql += " AND A.GroupCode = @GroupCode";
            }
            if (!string.IsNullOrEmpty(groupName))
            {
                sql += " AND A.GroupName LIKE '%'+ @GroupName+'%'";
            }
            if (useChk.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND A.UseChk = @UseChk";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Group>().ToList();

        }
        public void SaveGroup(Group group)
        {
            Group findOne = db.Group.Where(x => (x.GroupId == group.GroupId)).FirstOrDefault();
            if (findOne == null)
            {
                group.InDateTime = DateTime.Now;
                group.ModifyDateTime = DateTime.Now;
                db.Group.Add(group);
            }
            else
            {
                findOne.GroupCode = group.GroupCode;
                findOne.GroupName = group.GroupName;
                findOne.UseChk = group.UseChk;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = group.ModifyUserId;
            }
            db.SaveChanges();
        }
        #endregion
        #region 期号
        /// <summary>
        /// 获取期号
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ProjectDto> GetProject(string tenantId, string brandId, string projectId, string projectCode, string year, string orderNO
                                            , string projectType, DateTime? startDate, DateTime? endDate, string key)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            projectId = projectId == null ? "" : projectId;
            year = year == null ? "" : year;
            projectCode = projectCode == null ? "" : projectCode;
            orderNO = orderNO == null ? "" : orderNO;
            projectType = projectType == null ? "" : projectType;
            key = key == null ? "" : key;
            if (startDate == null)
            {
                startDate = new DateTime(2000, 1, 1);
            }
            else
            {
                startDate = Convert.ToDateTime(Convert.ToDateTime(startDate).Date.ToString("yyyy-MM-dd") + " 00:00:00");
            }
            if (endDate == null)
            {
                endDate = new DateTime(9999, 12, 31);
            }
            else
            {
                endDate = Convert.ToDateTime(Convert.ToDateTime(endDate).Date.ToString("yyyy-MM-dd") + " 23:59:59");
            }
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                       new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ProjectCode", projectCode),
                                                       new SqlParameter("@Year", year),
                                                       new SqlParameter("@OrderNO", orderNO),
                                                       new SqlParameter("@ProjectType", projectType),
                                                       new SqlParameter("@StartDate", startDate),
                                                       new SqlParameter("@EndDate", endDate),
                                                        new SqlParameter("@Key", key)};
            Type t = typeof(ProjectDto);
            string sql = "";
            sql = @"SELECT A.[ProjectId]
                          ,A.[TenantId]
                          ,A.[BrandId]
                          ,[ProjectCode]
                          ,[ProjectName]
                          ,CASE WHEN [ProjectShortName] IS NULL OR [ProjectShortName] ='' THEN ProjectName 
                            ELSE [ProjectShortName]
                           END AS ProjectShortName
                          ,[Year]
                          ,[Quarter]
                          ,[OrderNO]
                          ,[ReportDeployDate]
                          ,[RechckShopShow]
                          ,[SelfTestChk]
                          ,LossCopyToSupplyChk
                          ,[ScoreShowType]
                          ,[PhotoUploadMode]
                          ,[PhotoSize]
                          ,[AppealShow]
                          ,StartDate
                          ,EndDate
                          ,ProjectGroup
                          ,A.[InUserId]
                          ,A.[InDateTime]
                          ,A.[ModifyUserId]
                          ,A.[ModifyDateTime]
                          ,CASE WHEN GETDATE()<ReportDeployDate OR ReportDeployDate IS NULL THEN CAST(0 AS BIT) 
                             ELSE CAST(1 AS BIT) END AS ReportDeployChk,ISNULL(ProjectType,'明检') AS ProjectType
                          --,B.ShopId
                          --,C.ShopCode
                    FROM [Project] A  
                    ";
            if (!string.IsNullOrEmpty(key))
            {
                sql += @" INNER JOIN ProjectShopExamType B ON A.ProjectId = B.ProjectId
                          INNER JOIN Shop C ON B.ShopId = C.ShopId";
            }
            sql += " WHERE 1=1 AND ISNULL(StartDate,'2020-01-01') BETWEEN @StartDate AND @EndDate";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND A.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
                // 有品牌的情况下才按照年份进行查询，如果品牌信息为空不查询
                if (!string.IsNullOrEmpty(year))
                {
                    sql += " AND A.Year = @Year";
                }
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(projectCode))
            {
                sql += " AND A.ProjectCode = @ProjectCode";
            }
            if (!string.IsNullOrEmpty(orderNO))
            {
                sql += " AND OrderNO = @OrderNO";
            }
            if (!string.IsNullOrEmpty(projectType))
            {
                sql += " AND ProjectType = @ProjectType";
            }
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (C.ShopCode LIKE '%'+@Key+'%' OR C.ShopShortName LIKE '%'+@Key+'%' OR C.ShopName LIKE '%'+@Key+'%')";
            }
            sql += " ORDER BY A.Year,A.OrderNO DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<ProjectDto>().ToList();
        }
        /// <summary>
        /// 获取期号
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ProjectDto> GetPreProjectByProjectId(string brandId, string projectId, string year)
        {

            brandId = brandId == null ? "" : brandId;
            projectId = projectId == null ? "" : projectId;
            year = year == null ? "" : year;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@Year", year),
                                                    };
            Type t = typeof(ProjectDto);
            string sql = "";
            sql = @"SELECT * 
                    FROM Project 
                    WHERE OrderNO = (
                                    SELECT MAX(OrderNo) 
                                    FROM Project A  WHERE OrderNO < (SELECT OrderNO FROM Project WHERE ProjectId = @ProjectId)
                                    AND YEAR = @Year AND BrandId = @BrandId) 
                   AND Year =  @Year AND BrandId=@BrandId 
                    ";
            return db.Database.SqlQuery(t, sql, para).Cast<ProjectDto>().ToList();
        }
        /// <summary>
        /// 保存期号信息
        /// </summary>
        /// <param name="project"></param>
        public Project SaveProject(Project project)
        {
            //List<UserInfo> userList = accountService.GetUserInfo("",project.ModifyUserId.ToString(),"","");
            //if (userList == null || userList.Count == 0)
            //{
            //    throw new Exception("没有找到对应的用户");
            //}
            //string brandId = project.BrandId.ToString();
            //string accountId = userList[0].AccountId;

            //if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }

            //webService.SaveProject('I', project.ProjectCode, project.Year, project.Quarter, Convert.ToInt32(project.OrderNO));

            Project findOne = db.Project.Where(x => (x.ProjectId == project.ProjectId)).FirstOrDefault();
            if (findOne == null)
            {
                if (project.OrderNO == null || project.OrderNO == 0)
                {
                    Project findOneMax = db.Project.Where(x => x.BrandId == project.BrandId).OrderByDescending(x => x.OrderNO).FirstOrDefault();
                    project.OrderNO = findOneMax.OrderNO == null ? 0 : findOneMax.OrderNO + 1;
                }
                project.InDateTime = DateTime.Now;
                project.ModifyDateTime = DateTime.Now;
                db.Project.Add(project);
                db.SaveChanges();
                return project;
            }
            else
            {
                findOne.ProjectName = project.ProjectName;
                findOne.ProjectCode = project.ProjectCode;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = project.ModifyUserId;
                findOne.RechckShopShow = project.RechckShopShow;
                findOne.PhotoUploadMode = project.PhotoUploadMode;
                findOne.PhotoSize = project.PhotoSize;
                findOne.ScoreShowType = project.ScoreShowType;
                findOne.SelfTestChk = project.SelfTestChk;
                findOne.LossCopyToSupplyChk = project.LossCopyToSupplyChk;
                findOne.ReportDeployDate = project.ReportDeployDate;// 报告发布时间
                findOne.OrderNO = project.OrderNO;
                findOne.Quarter = project.Quarter;
                findOne.Year = project.Year;
                findOne.ProjectType = project.ProjectType;
                findOne.ProjectShortName = project.ProjectShortName;
                findOne.AppealShow = project.AppealShow;
                // 经销商自检任务开始和结束时间
                findOne.StartDate = project.StartDate;
                findOne.EndDate = project.EndDate;
                findOne.ProjectGroup = project.ProjectGroup;
                project = findOne;
                db.SaveChanges();

            }
            return project;
        }
        #endregion
        #region 经销商
        /// <summary>
        /// 获取经销商
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<ShopDto> GetShop(string tenantId, string brandId, string shopId, string shopCode, string key, bool? useChk)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            shopId = shopId == null ? "" : shopId;
            shopCode = shopCode == null ? "" : shopCode;
            key = key == null ? "" : key;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ShopId", shopId),
                                                    new SqlParameter("@ShopCode", shopCode)};
            Type t = typeof(ShopDto);
            string sql = "";
            sql = @"SELECT [ShopId]
                          ,[TenantId]
                          ,[BrandId]
                          ,[ShopCode]
                          ,[ShopName]
                          ,[ShopShortName]
                          ,(SELECT TOP 1 ProvinceName FROM Province WHERE ProvinceId = A.ProvinceId) Province
                          ,ProvinceId
                          ,[City]
                          ,[Address]
                          ,[GroupId]
                          ,(SELECT TOP 1  GroupName FROM  [GROUP] X WHERE X.GroupId = A.GroupId) AS GroupName
                          ,(SELECT TOP 1  GroupCode FROM  [GROUP] Y WHERE Y.GroupId = A.GroupId) AS GroupCode
                          ,(SELECT TOP 1 AreaName FROM Area X INNER JOIN AreaShop Y ON X.AreaId = Y.AreaId AND Y.ShopId = A.ShopId) AS AreaName
                          ,[UseChk]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
                      FROM [Shop] A
                    WHERE  1=1 
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                if (brandId == "33") // 针对广丰特殊处理
                {
                    sql += " AND BrandId = @BrandId AND A.ShopCode NOT IN('gtmc','dealer1','dealer2','dealer3','dealer4','dealer5')";
                }
                else {
                    sql += " AND BrandId = @BrandId";
                }
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND ShopCode = @ShopCode";
            }
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (ShopCode LIKE '%" + key + "%' OR ShopName LIKE '%" + key + "%' OR ShopShortName LIKE '%" + key + "%')";
            }
            if (useChk.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND UseChk = @UseChk";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ShopDto>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shop"></param>
        public void SaveShop(Shop shop)
        {
            Shop findOne = db.Shop.Where(x => (x.ShopId == shop.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                shop.InDateTime = DateTime.Now;
                shop.ModifyDateTime = DateTime.Now;
                db.Shop.Add(shop);
            }
            else
            {
                findOne.ShopCode = shop.ShopCode;
                findOne.ShopName = shop.ShopName;
                findOne.GroupId = shop.GroupId;
                findOne.City = shop.City;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = shop.ModifyUserId;
                findOne.Province = shop.Province;
                findOne.ShopShortName = shop.ShopShortName;
                findOne.Address = shop.Address;
                findOne.UseChk = shop.UseChk;
                findOne.ProvinceId = shop.ProvinceId;
            }
            db.SaveChanges();
        }
        public void ImportShop(List<Shop> shopList)
        {
            string sql = "";
            foreach (Shop shop in shopList)
            {
                sql += " INSERT INTO Shop VALUES('" + shop.TenantId.ToString() + "','";
                sql += shop.BrandId.ToString() + "','";
                sql += shop.ShopCode + "','";
                sql += shop.ShopName + "','";
                sql += shop.ShopShortName + "','";
                sql += shop.Province + "','";
                sql += shop.ProvinceId + "','";
                sql += shop.City + "','";
                sql += shop.Address + "','";
                sql += shop.GroupId + "','";
                sql += shop.InUserId + "','";
                sql += DateTime.Now.ToString() + "','";
                sql += shop.ModifyUserId + "','";
                sql += DateTime.Now.ToString() + ",')";
            }
        }
        public void DeleteShop(int shopId)
        {
            Shop findone = db.Shop.Where(x => x.ShopId == shopId).FirstOrDefault();
            db.Shop.Remove(findone);
            db.SaveChanges();
        }
        // 只针对lotus项目使用，其他项目不使用
        public List<AreaDto> GetSaleAreaIdByShopId(string shopId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ShopId", shopId) };
            Type t = typeof(AreaDto);
            string sql = "";
            sql = @"SELECT B.AreaId,C.AreaCode,C.AreaName
                      FROM [Shop] A INNER JOIN AreaShop B ON A.ShopId = B.ShopId
                                    INNER JOIN Area C ON B.AreaId = C.AreaId
                                    INNER JOIN Area D ON C.ParentId = D.AreaId
                                    INNER JOIN Area E ON D.ParentId = E.AreaId
                                    INNER JOIN Area F ON E.ParentId = F.AreaId
                                    INNER JOIN Area G ON F.ParentId = G.AreaId AND G.AreaCode = '销售' AND A.BrandId=32 
                    WHERE  A.ShopId = @ShopId
                    ";
            return db.Database.SqlQuery(t, sql, para).Cast<AreaDto>().ToList();
        }

        #endregion
        #region 经销商区域设置
        public List<ShopDto> GetAreaShop(string tenantId, string brandId, string shopId, string areaId)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            shopId = shopId == null ? "" : shopId;
            areaId = areaId == null ? "" : areaId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ShopId", shopId),
                                                    new SqlParameter("@AreaId", areaId)};
            Type t = typeof(ShopDto);
            string sql = "";
            sql = @"SELECT A.*,C.AreaCode,C.AreaName,C.AreaId,B.AreaShopId
                      FROM [Shop] A INNER JOIN AreaShop B ON A.ShopId = B.ShopId 
                                    INNER JOIN Area C ON B.AreaId = C.AreaId
                    WHERE  1=1 
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND A.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND C.AreaId = @AreaId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ShopDto>().ToList();
        }
        public void SaveAreaShop(AreaShop areaShop)
        {
            AreaShop findOne = db.AreaShop.Where(x => (x.ShopId == areaShop.ShopId && x.AreaId == areaShop.AreaId)).FirstOrDefault();
            if (findOne == null)
            {
                areaShop.InDateTime = DateTime.Now;
                areaShop.ModifyDateTime = DateTime.Now;
                db.AreaShop.Add(areaShop);
            }
            db.SaveChanges();
        }
        public void DeleteAreaShop(int areaShopId)
        {
            AreaShop findone = db.AreaShop.Where(x => x.AreaShopId == areaShopId).FirstOrDefault();
            db.AreaShop.Remove(findone);
            db.SaveChanges();
        }
        #endregion
        #region 省份区域设置
        public List<AreaProvinceDto> GetAreaProvince(string brandId, string provinceId, string areaId)
        {
            brandId = brandId == null ? "" : brandId;
            provinceId = provinceId == null ? "" : provinceId;
            areaId = areaId == null ? "" : areaId;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProvinceId", provinceId),
                                                    new SqlParameter("@AreaId", areaId)};
            Type t = typeof(AreaProvinceDto);
            string sql = "";
            sql = @"SELECT A.ProvinceCode,A.ProvinceName,A.ProvinceId,C.AreaCode,C.AreaName,C.AreaId,B.AreaProvinceId
                      FROM [Province] A INNER JOIN AreaProvince B ON A.ProvinceId = B.ProvinceId 
                                    INNER JOIN Area C ON B.AreaId = C.AreaId
                    WHERE  C.BrandId = @BrandId
                    ";
            if (!string.IsNullOrEmpty(provinceId))
            {
                sql += " AND A.ProvinceId = @ProvinceId";
            }
            if (!string.IsNullOrEmpty(areaId))
            {
                sql += " AND C.AreaId = @AreaId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AreaProvinceDto>().ToList();
        }
        public void SaveAreaProvince(AreaProvince areaProvince)
        {
            AreaProvince findOne = db.AreaProvince.Where(x => (x.ProvinceId == areaProvince.ProvinceId && x.AreaId == areaProvince.AreaId)).FirstOrDefault();
            if (findOne == null)
            {
                areaProvince.InDateTime = DateTime.Now;
                areaProvince.ModifyDateTime = DateTime.Now;
                db.AreaProvince.Add(areaProvince);
            }
            db.SaveChanges();
        }
        public void DeleteAreaProvince(int areaProvinceId)
        {
            AreaProvince findone = db.AreaProvince.Where(x => x.AreaProvinceId == areaProvinceId).FirstOrDefault();
            db.AreaProvince.Remove(findone);
            db.SaveChanges();
        }
        #endregion
        #region 体系管理

        /// <summary>
        /// 获取体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectDto> GetSubject(string projectId, string subjectId, string subjectCode, string orderNO)
        {
            projectId = projectId == null ? "" : projectId;
            subjectId = subjectId == null ? "" : subjectId;
            subjectCode = subjectCode == null ? "" : subjectCode;
            orderNO = orderNO == null ? "" : orderNO;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectId", subjectId),
                                                       new SqlParameter("@SubjectCode", subjectCode),
                                                       new SqlParameter("@OrderNO", orderNO)};
            Type t = typeof(SubjectDto);
            string sql = "";
            sql = @"SELECT A.*,B.ProjectCode,B.ProjectName,C.LabelCode As ExamTypeCode,C.LabelName AS ExamTypeName,[Desc]
                        ,D.LabelCode As RecheckTypeCode,D.LabelName AS RecheckTypeName,E.HiddenName AS HiddenCode_SubjectTypeName
                        ,A.ImproveAdvice,A.LabelId_SubjectPattern,F.LabelCode AS SubjectPatternCode,F.LabelName AS SubjectPatternName
                    FROM [Subject] A INNER JOIN Project B ON A.ProjectId = B.ProjectId 
                                    LEFT JOIN Label C ON   ISNULL(A.LabelId,0)  =  C.LabelId
                                    LEFT JOIN Label D ON   A.LabelId_RecheckType  =  D.LabelId
                                    LEFT JOIN HiddenColumn E ON A.HiddenCode_SubjectType = E.HiddenCode AND E.HiddenCodeGroup = '体系类型'
                                    LEFT JOIN Label F ON A.LabelId_SubjectPattern = F.LabelId
                    WHERE 1=1 AND A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }
            if (!string.IsNullOrEmpty(subjectCode))
            {
                sql += " AND A.SubjectCode =@SubjectCode ";
            }
            if (!string.IsNullOrEmpty(orderNO))
            {
                sql += " AND A.OrderNO =@OrderNO ";
            }
            sql += " ORDER BY A.OrderNO ";
            List<SubjectDto> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectDto>().ToList();
            return list;
        }
        /// <summary>
        /// 保存体系信息
        /// </summary>
        /// <param name="subject"></param>
        public Subject SaveSubject(Subject subject)
        {
            Subject findOne = db.Subject.Where(x => (x.SubjectId == subject.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                subject.InDateTime = DateTime.Now;
                subject.ModifyDateTime = DateTime.Now;
                db.Subject.Add(subject);
                db.SaveChanges();
            }
            else
            {
                findOne.CheckPoint = subject.CheckPoint;
                findOne.Desc = subject.Desc;
                findOne.FullScore = subject.FullScore;
                findOne.Implementation = subject.Implementation;
                findOne.InspectionDesc = subject.InspectionDesc;
                findOne.LowScore = subject.LowScore;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subject.ModifyUserId;
                findOne.OrderNO = subject.OrderNO;
                findOne.Remark = subject.Remark;
                findOne.LabelId = subject.LabelId;
                findOne.LabelId_RecheckType = subject.LabelId_RecheckType;
                findOne.HiddenCode_SubjectType = subject.HiddenCode_SubjectType;
                findOne.LabelId_SubjectPattern = subject.LabelId_SubjectPattern;
                findOne.SubjectCode = subject.SubjectCode;
                findOne.MustScore = subject.MustScore;
                findOne.ImproveAdvice = subject.ImproveAdvice;
                subject = findOne;
                db.SaveChanges();
            }
            return subject;
        }
        /// <summary>
        /// 删除体系
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="seqNo"></param>
        public void DeleteSubject(long subjectId)
        {
            Subject findone = db.Subject.Where(x => x.SubjectId == subjectId).FirstOrDefault();
            db.Subject.Remove(findone);
            db.SaveChanges();
        }
        /// <summary>
        /// 获取标准照片信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectFile> GetSubjectFile(string projectId, string subjectId)
        {
            subjectId = subjectId == null ? "" : subjectId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(SubjectFile);
            string sql = "";
            sql = @"SELECT A.*
                   FROM SubjectFile A INNER JOIN [Subject] B ON A.SubjectId = B.SubjectId
                  WHERE B.ProjectId = @ProjectId ";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectFile>().ToList();

        }
        /// <summary>
        /// 保存标准照片
        /// </summary>
        /// <param name="subjectFile"></param>
        public void SaveSubjectFile(SubjectFile subjectFile)
        {
            // CommonHelper.log("ServiceSaveSubjectFile:" + subjectFile.FileDemo);
            if (subjectFile.SeqNO == 0)
            {
                SubjectFile findOneMax = db.SubjectFile.Where(x => (x.SubjectId == subjectFile.SubjectId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    subjectFile.SeqNO = 1;
                }
                else
                {
                    subjectFile.SeqNO = findOneMax.SeqNO + 1;
                }
                subjectFile.InDateTime = DateTime.Now;
                subjectFile.ModifyDateTime = DateTime.Now;
                db.SubjectFile.Add(subjectFile);
            }
            else
            {
                SubjectFile findOne = db.SubjectFile.Where(x => (x.SubjectId == subjectFile.SubjectId && x.SeqNO == subjectFile.SeqNO)).FirstOrDefault();
                if (findOne == null)
                {
                    subjectFile.InDateTime = DateTime.Now;
                    subjectFile.ModifyDateTime = DateTime.Now;
                    db.SubjectFile.Add(subjectFile);
                }
                else
                {
                    findOne.FileName = subjectFile.FileName;
                    findOne.FileDemo = subjectFile.FileDemo;
                    findOne.FileDemoDesc = subjectFile.FileDemoDesc;
                    findOne.FileRemark = subjectFile.FileRemark;
                    findOne.ModifyDateTime = DateTime.Now;
                    findOne.ModifyUserId = subjectFile.ModifyUserId;
                }
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 删除标准照片
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="seqNo"></param>
        public void DeleteSubjectFile(long subjectId, int seqNo)
        {
            if (seqNo == 0)
            {
                string sql = "DELETE SubjectFile WHERE SubjectId= '" + subjectId.ToString() + "'";
                SqlParameter[] para = new SqlParameter[] { };
                db.Database.ExecuteSqlCommand(sql, para);
            }
            else
            {
                SubjectFile findone = db.SubjectFile.Where(x => x.SubjectId == subjectId && x.SeqNO == seqNo).FirstOrDefault();
                db.SubjectFile.Remove(findone);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 获取检查标准信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectInspectionStandard> GetSubjectInspectionStandard(string projectId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(SubjectInspectionStandard);
            string sql = "";
            sql = @"SELECT A.* 
                   FROM SubjectInspectionStandard A INNER JOIN Subject B ON A.SubjectId = B.SubjectId
                   WHERE B.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectInspectionStandard>().ToList();
        }
        /// <summary>
        /// 保存检查标准
        /// </summary>
        /// <param name="SubjectInspectionStandard"></param>
        public void SaveSubjectInspectionStandard(SubjectInspectionStandard subjectInspectionStandard)
        {
            if (subjectInspectionStandard.SeqNO == 0)
            {
                SubjectInspectionStandard findOneMax = db.SubjectInspectionStandard.Where(x => (x.SubjectId == subjectInspectionStandard.SubjectId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    subjectInspectionStandard.SeqNO = 1;
                }
                else
                {
                    subjectInspectionStandard.SeqNO = findOneMax.SeqNO + 1;
                }
                subjectInspectionStandard.InDateTime = DateTime.Now;
                subjectInspectionStandard.ModifyDateTime = DateTime.Now;
                db.SubjectInspectionStandard.Add(subjectInspectionStandard);

            }
            else
            {
                SubjectInspectionStandard findOne = db.SubjectInspectionStandard.Where(x => (x.SubjectId == subjectInspectionStandard.SubjectId && x.SeqNO == subjectInspectionStandard.SeqNO)).FirstOrDefault();
                if (findOne == null)
                {
                    subjectInspectionStandard.InDateTime = DateTime.Now;
                    db.SubjectInspectionStandard.Add(subjectInspectionStandard);
                }
                else
                {
                    findOne.InspectionStandardName = subjectInspectionStandard.InspectionStandardName;
                    findOne.ModifyDateTime = DateTime.Now;
                    findOne.ModifyUserId = subjectInspectionStandard.ModifyUserId;
                }
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 删除检查标准
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="seqNo"></param>
        public void DeleteSubjectInspectionStandard(long subjectId, int seqNo)
        {
            if (seqNo == 0)
            {
                string sql = "DELETE SubjectInspectionStandard WHERE SubjectId= '" + subjectId.ToString() + "'";
                SqlParameter[] para = new SqlParameter[] { };
                db.Database.ExecuteSqlCommand(sql, para);
            }
            else
            {
                SubjectInspectionStandard findone = db.SubjectInspectionStandard.Where(x => x.SubjectId == subjectId && x.SeqNO == seqNo).FirstOrDefault();
                db.SubjectInspectionStandard.Remove(findone);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 获取失分说明
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectLossResult> GetSubjectLossResult(string projectId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(SubjectLossResult);
            string sql = "";
            sql = @"SELECT A.*
                  FROM SubjectLossResult A INNER JOIN Subject B ON A.SubjectId = B.SubjectId
                  WHERE B.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = @SubjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectLossResult>().ToList();
        }
        public string GetSubjectLossCodeByAnswerLossName(string projectId, string subjectId, string lossDesc)
        {
            string lossResultStrCode = "";
            if (!string.IsNullOrEmpty(lossDesc))
            {
                string[] lossStr = lossDesc.Split(';');
                foreach (string loss in lossStr)
                {
                    if (!string.IsNullOrEmpty(loss))
                    {
                        List<SubjectLossResult> subjectLossList = GetSubjectLossResult(projectId.ToString(), subjectId.ToString()).Where(x => x.LossResultName == loss).ToList();
                        if (subjectLossList != null && subjectLossList.Count > 0)
                        {
                            lossResultStrCode += subjectLossList[0].LossResultCode + ";";
                        }
                    }
                }
            }
            // 去掉最后一个分号
            if (!string.IsNullOrEmpty(lossResultStrCode))
            {
                lossResultStrCode = lossResultStrCode.Substring(0, lossResultStrCode.Length - 1);
            }
            return lossResultStrCode;
        }
        /// <summary>
        /// 保存失分说明
        /// </summary>
        /// <param name="subjectLossResult"></param>
        public void SaveSubjectLossResult(SubjectLossResult subjectLossResult)
        {
            if (subjectLossResult.SeqNO == 0)
            {
                SubjectLossResult findOneMax = db.SubjectLossResult.Where(x => (x.SubjectId == subjectLossResult.SubjectId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    subjectLossResult.SeqNO = 1;
                }
                else
                {
                    subjectLossResult.SeqNO = findOneMax.SeqNO + 1;
                }
                subjectLossResult.InDateTime = DateTime.Now;
                subjectLossResult.ModifyDateTime = DateTime.Now;
                db.SubjectLossResult.Add(subjectLossResult);

            }
            else
            {
                SubjectLossResult findOne = db.SubjectLossResult.Where(x => (x.SubjectId == subjectLossResult.SubjectId && x.SeqNO == subjectLossResult.SeqNO)).FirstOrDefault();
                if (findOne == null)
                {
                    subjectLossResult.InDateTime = DateTime.Now;
                    db.SubjectLossResult.Add(subjectLossResult);
                }
                else
                {
                    findOne.LossResultName = subjectLossResult.LossResultName;
                    findOne.ModifyDateTime = DateTime.Now;
                    findOne.ModifyUserId = subjectLossResult.ModifyUserId;
                }
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 删除失分说明
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="seqNo"></param>
        public void DeleteSubjectLossResult(long subjectId, int seqNo)
        {
            if (seqNo == 0)
            {
                string sql = "DELETE SubjectLossResult WHERE SubjectId= '" + subjectId.ToString() + "'";
                SqlParameter[] para = new SqlParameter[] { };
                db.Database.ExecuteSqlCommand(sql, para);
            }
            else
            {
                SubjectLossResult findone = db.SubjectLossResult.Where(x => x.SubjectId == subjectId && x.SeqNO == seqNo).FirstOrDefault();
                db.SubjectLossResult.Remove(findone);
                db.SaveChanges();
            }
        }
        #endregion
        #region 章节和报告类型管理
        public List<ReportType> GetReportType(string projectId, string reportTypeId, string reportTypeCode)
        {
            if (reportTypeId == null) reportTypeId = "";
            if (reportTypeCode == null) reportTypeCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        ,new SqlParameter("@ReportTypeId", reportTypeId)
                                                        ,new SqlParameter("@ReportTypeCode", reportTypeCode)};
            Type t = typeof(ReportType);
            string sql = "";
            sql = @"SELECT A.*
                    FROM ReportType A
                   WHERE A.ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(reportTypeId))
            {
                sql += " AND A.ReportTypeId = @ReportTypeId";
            }
            if (!string.IsNullOrEmpty(reportTypeCode))
            {
                sql += " AND A.ReportTypeCode = @ReportTypeCode";
            }
            sql += " ORDER BY ReportTypeId";
            return db.Database.SqlQuery(t, sql, para).Cast<ReportType>().ToList();
        }
        public void SaveReportType(ReportType reportType)
        {
            ReportType findOne = db.ReportType.Where(x => (x.ReportTypeId == reportType.ReportTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                reportType.InDateTime = DateTime.Now;
                db.ReportType.Add(reportType);
            }
            else
            {
                findOne.ReportTypeCode = reportType.ReportTypeCode;
                findOne.ReportTypeName = reportType.ReportTypeName;
                findOne.FullScore = reportType.FullScore;
            }
            db.SaveChanges();
        }
        public List<ReportTypeShopDto> GetReportTypeShop(string projectId, string reportTypeId, string shopId)
        {
            if (reportTypeId == null) reportTypeId = "";
            if (shopId == null) shopId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        ,new SqlParameter("@ReportTypeId", reportTypeId)
                                                        ,new SqlParameter("@ShopId", shopId)};
            Type t = typeof(ReportTypeShopDto);
            string sql = "";
            sql = @"SELECT B.*,A.ReportTypeCode,A.ReportTypeName,C.ShopCode
                    FROM ReportType A INNER JOIN ReportTypeShop B ON A.ReportTypeId = B.ReportTypeId
                                    INNER JOIN Shop C ON B.ShopId = C.ShopId
                   WHERE 1=1 AND A.ProjectId = @ProjectId ";
            if (!string.IsNullOrEmpty(reportTypeId))
            {
                sql += " AND A.ReportTypeId = @ReportTypeId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND B.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ReportTypeShopDto>().ToList();
        }
        public void SaveReportTypeShop(ReportTypeShop reportTypeShop)
        {
            ReportTypeShop findOne = db.ReportTypeShop.Where(x => (x.ReportTypeId == reportTypeShop.ReportTypeId && x.ShopId == reportTypeShop.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                reportTypeShop.InDateTime = DateTime.Now;
                db.ReportTypeShop.Add(reportTypeShop);
            }

            db.SaveChanges();
        }
        public void DeleteReportTypeShop(int reportTypeShopId)
        {
            ReportTypeShop findone = db.ReportTypeShop.Where(x => x.ReportShopId == reportTypeShopId).FirstOrDefault();
            db.ReportTypeShop.Remove(findone);
            db.SaveChanges();
        }
        public List<ChapterReportTypeDto> GetChapterReportType(string projectId, string reportTypeId, string chapterId)
        {
            if (reportTypeId == null) reportTypeId = "";
            if (chapterId == null) chapterId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        ,new SqlParameter("@ReportTypeId", reportTypeId)
                                                        ,new SqlParameter("@ChapterId", chapterId)};
            Type t = typeof(ChapterReportTypeDto);
            string sql = "";
            sql = @"SELECT B.ChapterId,CAST(B.ShopType AS INT) AS ReportTypeId,
                        B.FullScore,A.ReportTypeCode,A.ReportTypeName,C.ChapterCode,C.ChapterName
                    FROM ReportType A INNER JOIN ChapterShopType B ON A.ReportTypeId = B.ShopType
                                      INNER JOIN Chapter C ON B.ChapterId = C.ChapterId
                   WHERE 1=1 AND A.ProjectId = @ProjectId ";
            if (!string.IsNullOrEmpty(reportTypeId))
            {
                sql += " AND A.ReportTypeId = @ReportTypeId";
            }
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += " AND B.ChapterId = @ChapterId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ChapterReportTypeDto>().ToList();
        }
        public void SaveChapterReportType(ChapterShopType chapterShopType)
        {
            ChapterShopType findOne = db.ChapterShopType.Where(x => (x.ShopType == chapterShopType.ShopType && x.ChapterId == chapterShopType.ChapterId)).FirstOrDefault();
            if (findOne == null)
            {
                chapterShopType.InDateTime = DateTime.Now;
                db.ChapterShopType.Add(chapterShopType);
            }
            db.SaveChanges();
        }
        public void DeleteChapterReportType(string reportTypeId, int chapterId)
        {
            ChapterShopType findone = db.ChapterShopType.Where(x => x.ShopType == reportTypeId && x.ChapterId == chapterId).FirstOrDefault();
            db.ChapterShopType.Remove(findone);
            db.SaveChanges();
        }
        public List<ChapterDto> GetChapter(string projectId, string reportTypeId, string chapterId, string chapterCode)
        {
            if (reportTypeId == null) reportTypeId = "";
            if (chapterId == null) chapterId = "";
            if (chapterCode == null) chapterCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        ,new SqlParameter("@ReportTypeId", reportTypeId)
                                                        ,new SqlParameter("@ChapterId", chapterId)
                                                        ,new SqlParameter("@ChapterCode", chapterCode)};
            Type t = typeof(ChapterDto);
            string sql = "";
            sql = @"SELECT A.*,B.ProjectCode,B.ProjectName
                    FROM Chapter A INNER JOIN Project B ON A.ProjectId = B.ProjectId
                   WHERE A.ProjectId=@ProjectId ";
            if (!string.IsNullOrEmpty(reportTypeId))
            {
                sql += " AND A.ReportTypeId = @ReportTypeId";
            }
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += " AND A.ChapterId = @ChapterId";
            }
            if (!string.IsNullOrEmpty(chapterCode))
            {
                sql += " AND A.ChapterCode = @ChapterCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ChapterDto>().ToList();
        }
        public Chapter SaveChapter(Chapter chapter)
        {
            Chapter findOne = db.Chapter.Where(x => (x.ChapterId == chapter.ChapterId)).FirstOrDefault();
            if (findOne == null)
            {
                chapter.InDateTime = DateTime.Now;
                db.Chapter.Add(chapter);
                db.SaveChanges();
            }
            else
            {
                findOne.ChapterCode = chapter.ChapterCode;
                findOne.ChapterName = chapter.ChapterName;
                findOne.FullScore = chapter.FullScore;
                findOne.ReportTypeId = chapter.ReportTypeId;
                chapter = findOne;
                db.SaveChanges();
            }
            return chapter;
        }
        public List<ChapterSubjectDto> GetChapterSubject(string projectId, string chapterId, string subjectId)
        {
            if (chapterId == null) chapterId = "";
            if (subjectId == null) subjectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        ,new SqlParameter("@ChapterId", chapterId)
                                                        ,new SqlParameter("@SubjectId", subjectId)};
            Type t = typeof(ChapterSubjectDto);
            string sql = "";
            sql = @"SELECT B.*,A.ChapterCode,A.ChapterName,C.SubjectCode
                    FROM Chapter A INNER JOIN ChapterSubject B ON A.ChapterId = B.ChapterId
                                    INNER JOIN Subject C ON B.SubjectId = C.SubjectId
                   WHERE 1=1";
            if (!string.IsNullOrEmpty(chapterId))
            {
                sql += " AND A.ChapterId = @ChapterId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND B.SubjectId = @SubjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ChapterSubjectDto>().ToList();
        }
        public void SaveChapterSubject(ChapterSubject chapterSubject)
        {
            ChapterSubject findOne = db.ChapterSubject.Where(x => (x.ChapterId == chapterSubject.ChapterId && x.SubjectId == chapterSubject.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                chapterSubject.InDateTime = DateTime.Now;
                db.ChapterSubject.Add(chapterSubject);
            }

            db.SaveChanges();
        }
        public void DeleteChapterSubject(int chapterSubjectId)
        {
            ChapterSubject findone = db.ChapterSubject.Where(x => x.Id == chapterSubjectId).FirstOrDefault();
            db.ChapterSubject.Remove(findone);
            db.SaveChanges();
        }
        #endregion
        #region 标签管理
        public List<LabelDto> GetLabel(string brandId, string labelId, string labelType, bool? useChk, string labelCode)
        {
            if (labelCode == null) labelCode = "";
            if (labelType == null) labelType = "";
            if (labelId == null) labelId = "";
            if (brandId == null) brandId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@LabelType", labelType),
                                                        new SqlParameter("@BrandId", brandId),
                                                         new SqlParameter("@LabelCode", labelCode),
                                                        new SqlParameter("@LabelId", labelId)};
            Type t = typeof(LabelDto);
            string sql = "";
            // 如果是试卷类型，查询出来通用卷
            if (!string.IsNullOrEmpty(labelType) && labelType == "ExamType")
            {
                sql = @"SELECT * FROM 
                            (SELECT * FROM [Label] WHERE LabelId=0
                                 UNION ALL
                            SELECT * 
                             FROM [Label] 
                            WHERE  BrandId = @BrandId AND 1=1) A WHERE  1=1
                            ";
            }
            else
            {
                sql = @"
                    SELECT * 
                      FROM [Label] 
                    WHERE  BrandId = @BrandId AND 1=1
                    ";
            }
            if (useChk.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND UseChk = @UseChk";
            }
            if (!string.IsNullOrEmpty(labelCode))
            {
                sql += " AND LabelCode = @LabelCode";
            }
            if (!string.IsNullOrEmpty(labelType))
            {
                sql += " AND LabelType = @LabelType";
            }
            if (!string.IsNullOrEmpty(labelId))
            {
                sql += " AND LabelId = @LabelId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<LabelDto>().ToList();
        }
        public void SaveLabel(Label label)
        {
            Label findOne = db.Label.Where(x => (x.LabelId == label.LabelId && x.BrandId == label.BrandId)).FirstOrDefault();
            if (findOne == null)
            {
                label.InDateTime = DateTime.Now;
                label.ModifyDateTime = DateTime.Now;
                db.Label.Add(label);
            }
            else
            {
                findOne.LabelCode = label.LabelCode;
                findOne.LabelName = label.LabelName;
                findOne.Remark = label.Remark;
                findOne.UseChk = label.UseChk;
                findOne.ExtenColumn = label.ExtenColumn;
                findOne.ModifyUserId = label.ModifyUserId;
                findOne.ModifyDateTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 根据不同业务传不同的labelTye
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="labelId"></param>
        /// <param name="objectId"></param>
        /// <param name="labelType"></param>
        /// <returns></returns>
        //public List<LabelObjectDto> GetLabelObject(string Id, string labelId, string objectId,string labelType)
        //{
        //    if (Id == null) Id = "";
        //    if (labelId == null) labelId = "";
        //    if (objectId == null) objectId = "";
        //    if (labelType == null) labelType = "";
        //    SqlParameter[] para = new SqlParameter[] { new SqlParameter("@Id", Id),
        //                                               new SqlParameter("@LabelId", labelId),
        //                                               new SqlParameter("@ObjectId", objectId),
        //                                               new SqlParameter("@LabelType", labelType)};
        //    Type t = typeof(LabelObjectDto);
        //    string sql = "";
        //    sql = @"SELECT * 
        //              FROM [Label] A
        //            WHERE  BrandId = @BrandId AND 1=1
        //            ";
        //    if (useChk != null)
        //    {
        //        sql += " AND UseChk = @UseChk";
        //    }
        //    if (!string.IsNullOrEmpty(labelCode))
        //    {
        //        sql += " AND LabelCode = @LabelCode";
        //    }
        //    if (!string.IsNullOrEmpty(labelType))
        //    {
        //        sql += " AND LabelType = @LabelType";
        //    }
        //    return db.Database.SqlQuery(t, sql, para).Cast<Label>().ToList();
        //}

        #endregion
        #region 照片下载命名
        public List<FileType> GetFileType()
        {
            SqlParameter[] para = new SqlParameter[] { };
            Type t = typeof(FileType);
            string sql = @"SELECT A.*
                            FROM FileType A ";
            return db.Database.SqlQuery(t, sql, para).Cast<FileType>().ToList();
        }
        public List<FileNameOption> GetFileNameOption(string fileTypeCode)
        {
            if (fileTypeCode == null) fileTypeCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@FileTypeCode", fileTypeCode) };
            Type t = typeof(FileNameOption);
            string sql = @"SELECT A.*
                            FROM FileNameOption A INNER JOIN FileTypeFileNameOption B ON B.FileNameOptionCode = A.OptionCode
                            WHERE 1=1   ";
            if (!string.IsNullOrEmpty(fileTypeCode))
            {
                sql += " AND B.FileTypeCode = @FileTypeCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<FileNameOption>().ToList();
        }
        public void SaveFileRename(FileRename fileRename)
        {
            // 只能新增不能修改
            if (fileRename.SeqNO == 0)
            {
                FileRename findOneMax = db.FileRename.Where(x => (x.ProjectId == fileRename.ProjectId && x.FileTypeCode == fileRename.FileTypeCode && x.PhotoType == fileRename.PhotoType)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    fileRename.SeqNO = 1;
                }
                else
                {
                    fileRename.SeqNO = findOneMax.SeqNO + 1;
                }
                fileRename.InDateTime = DateTime.Now;
                fileRename.ModifyDateTime = DateTime.Now;
                db.FileRename.Add(fileRename);
            }
            db.SaveChanges();
        }
        public void DeleteFileRename(FileRename fileRename)
        {
            string sql = "DELETE FileRename WHERE FileNameId= '" + fileRename.FileNameId.ToString() + "'";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public List<FileRenameDto> GetFileRename(string projectId, string fileTypeCode, string photoType)
        {
            if (projectId == null) projectId = "";
            if (fileTypeCode == null) fileTypeCode = "";
            if (photoType == null) photoType = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@FileTypeCode", fileTypeCode)
                                                    ,new SqlParameter("@PhotoType", photoType) };
            Type t = typeof(FileRenameDto);
            string sql = @"SELECT A.*,D.ProjectCode,D.ProjectName,B.FileTypeName,C.OptionName
                         FROM FileRename A INNER JOIN FileType B ON A.FileTypeCode = B.FileTypeCode
				                        INNER JOIN FileNameOption C ON A.OptionCode = C.OptionCode
				                    INNER JOIN Project D ON A.ProjectId = D.ProjectId
                        WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(fileTypeCode))
            {
                sql += " AND A.FileTypeCode = @FileTypeCode";
            }
            if (!string.IsNullOrEmpty(photoType))
            {
                sql += " AND A.PhotoType = @PhotoType";
            }
            sql += " ORDER BY A.SeqNO";
            return db.Database.SqlQuery(t, sql, para).Cast<FileRenameDto>().ToList();
        }
        #endregion
        #region 期号基本信息设置
        public List<ProjectBaseSetting> GetProjectBaseSetting(string projectId)
        {

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(ProjectBaseSetting);
            string sql = "";
            sql = @"SELECT * 
                      FROM [ProjectBaseSetting] A
                    WHERE  ProjectId = @ProjectId 
                    ";

            return db.Database.SqlQuery(t, sql, para).Cast<ProjectBaseSetting>().ToList();
        }
        #endregion
        #region 收费
        //public void SaveTenantMemberTypeCharge(TenantMemberTypeCharge tenantMemberTypeCharge)
        //{
        //    TenantMemberTypeCharge findOne = db.TenantMemberTypeCharge.Where(x => (x.Id == subjectTypeScoreRegion.Id)).FirstOrDefault();
        //    if (findOne == null)
        //    {
        //        subjectTypeScoreRegion.InDateTime = DateTime.Now;
        //        subjectTypeScoreRegion.ModifyDateTime = DateTime.Now;
        //        db.SubjectTypeScoreRegion.Add(subjectTypeScoreRegion);
        //    }
        //    else
        //    {
        //        findOne.FullScore = subjectTypeScoreRegion.FullScore;
        //        findOne.ModifyDateTime = DateTime.Now;
        //        findOne.ModifyUserId = subjectTypeScoreRegion.ModifyUserId;
        //        findOne.SubjectId = subjectTypeScoreRegion.SubjectId;
        //        findOne.LowestScore = subjectTypeScoreRegion.LowestScore;
        //        findOne.SubjectId = subjectTypeScoreRegion.SubjectId;
        //        findOne.SubjectTypeId = subjectTypeScoreRegion.SubjectTypeId;
        //    }
        //    db.SaveChanges();
        //}
        #endregion
        #region 通知公告
        // 公告查询
        public List<NoticeDto> GetNotice(string brandId, string noticeId, string noticeCode, string content, DateTime? startDate, DateTime? endDate, string userId)
        {
            brandId = brandId == null ? "" : brandId;
            noticeId = noticeId == null ? "" : noticeId;
            content = content == null ? "" : content;
            noticeCode = noticeCode == null ? "" : noticeCode;
            userId = userId == null ? "" : userId;
            startDate = startDate == null ? new DateTime(2024, 1, 1) : startDate;
            endDate = endDate == null ? new DateTime(9999, 12, 31) : endDate;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@NoticeId", noticeId),
                                                        new SqlParameter("@Content", content),
                                                        new SqlParameter("@NoticeCode", noticeCode),
                                                        new SqlParameter("@StartDate", startDate),
                                                        new SqlParameter("@EndDate", endDate),
                                                        new SqlParameter("@UserId", userId)};
            Type t = typeof(NoticeDto);
            string sql = "";

            sql = @"SELECT A.*,B.BrandCode,B.BrandName
                    FROM [Notice] A INNER JOIN Brand B ON A.BrandId = B.BrandId
                    WHERE 1=1 AND A.PublishDate BETWEEN @StartDate AND @EndDate ";

            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(noticeId))
            {
                sql += " AND A.NoticeId = @NoticeId";
            }
            if (!string.IsNullOrEmpty(content))
            {
                sql += " AND A.NoticeContent LIKE '%'+ @Content+'%'";
            }
            if (!string.IsNullOrEmpty(noticeCode))
            {
                sql += " AND A.NoticeCode LIKE '%'+ @NoticeCode+'%'";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                sql += @" AND (  EXISTS(SELECT 1 
                                         FROM NoticeUserId 
                                         WHERE NoticeId = A.NoticeId 
                                                AND UserId = @UserId
                                          )
                               OR EXISTS(SELECT 1 
                                         FROM NoticeRoleType X INNER JOIN RoleType Y ON X.RoleTypeId = Y.RoleTypeId
                                                             INNER JOIN UserInfo Z ON Y.RoleTypeCode = Z.RoleType
                                         WHERE NoticeId = A.NoticeId  
                                                AND Z.Id = @UserId
                                          )
                                )";
            }

            sql += " ORDER BY PublishDate DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<NoticeDto>().ToList();
        }
        // 公告附件查询
        public List<NoticeFile> GetNoticeFile(string noticeId)
        {
            if (noticeId == null) noticeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId) };
            Type t = typeof(NoticeFile);
            string sql = @"SELECT A.*
                         FROM NoticeFile A
	                    WHERE NoticeId = @NoticeId";
            return db.Database.SqlQuery(t, sql, para).Cast<NoticeFile>().ToList();
        }
        // 公告有权限查看人员-User
        public List<NoticeUserDto> GetNoticeUserId(string noticeId)
        {
            if (noticeId == null) noticeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId) };
            Type t = typeof(NoticeUserDto);
            string sql = @"SELECT A.*,B.AccountId,B.AccountName
                         FROM NoticeUserId A INNER JOIN UserInfo B ON A.UserId = B.Id
	                    WHERE NoticeId = @NoticeId";
            return db.Database.SqlQuery(t, sql, para).Cast<NoticeUserDto>().ToList();
        }
        // 公告有权限查看人员-RoleType
        public List<NoticeRoleTypeDto> GetNoticeRoleType(string noticeId)
        {
            if (noticeId == null) noticeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId) };
            Type t = typeof(NoticeRoleTypeDto);
            string sql = @"SELECT A.*,B.RoleTypeCode,B.RoleTypeName
                         FROM NoticeRoleType A INNER JOIN RoleType B ON A.RoleTypeId = B.RoleTypeId
	                    WHERE NoticeId = @NoticeId";
            return db.Database.SqlQuery(t, sql, para).Cast<NoticeRoleTypeDto>().ToList();
        }
        //公告有权限查看人员-RoleType-userId
        public List<NoticeUserDto> GetNoticeRoleTypeUser(string noticeId, string tenantId, string brandId)
        {
            if (noticeId == null) noticeId = "";
            if (brandId == null) brandId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId),
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@TenantId", tenantId)};
            Type t = typeof(NoticeUserDto);
            string sql = @"SELECT A.*,B.RoleTypeCode,B.RoleTypeName,C.AccountId,C.AccountName
                         FROM NoticeRoleType A INNER JOIN RoleType B ON A.RoleTypeId = B.RoleTypeId
                                               INNER JOIN UserInfo C ON B.RoleTypeCode = C.RoleType
	                    WHERE A.NoticeId = @NoticeId AND C.TenantId = @TenantId";
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND C.BrandId = @BrandId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<NoticeUserDto>().ToList();
        }
        // 公告阅读统计
        public List<NoticeUserDto> GetNoticeView(string noticeId, string userId)
        {
            if (noticeId == null) noticeId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId),
                                                        new SqlParameter("@UserId", userId) };
            Type t = typeof(NoticeUserDto);
            string sql = @"SELECT A.*,B.AccountId,B.AccountName
                         FROM NoticeView A INNER JOIN UserInfo B ON A.UserId = B.Id
	                    WHERE NoticeId = @NoticeId";
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND A.UserId = @UserId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<NoticeUserDto>().ToList();
        }
        // 公告保存
        public Notice SaveNotice(Notice notice)
        {
            Notice findOne = db.Notice.Where(x => (x.NoticeId == notice.NoticeId)).FirstOrDefault();
            if (findOne == null)
            {
                notice.InDateTime = DateTime.Now;
                notice.ModifyDateTime = DateTime.Now;
                db.Notice.Add(notice);
                db.SaveChanges();
            }
            else
            {
                findOne.BrandId = notice.BrandId;
                findOne.NoticeCode = notice.NoticeCode;
                findOne.NoticeContent = notice.NoticeContent;
                findOne.PublishDate = notice.PublishDate;
                findOne.ModifyUserId = notice.ModifyUserId;
                findOne.ModifyDateTime = DateTime.Now;
                notice = findOne;
                db.SaveChanges();
            }
            return notice;
        }
        // 公告附件保存
        public void SaveNoticeFile(NoticeFile noticeFile)
        {
            if (noticeFile.SeqNO == 0)
            {
                NoticeFile findOneMax = db.NoticeFile.Where(x => (x.NoticeId == noticeFile.NoticeId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    noticeFile.SeqNO = 1;
                }
                else
                {
                    noticeFile.SeqNO = findOneMax.SeqNO + 1;
                }
                noticeFile.InDateTime = DateTime.Now;
                db.NoticeFile.Add(noticeFile);

            }
            else
            {

            }
            db.SaveChanges();
        }
        // 公告附件删除
        public void NoticeFileDelete(string noticeId, string seqNO)
        {
            seqNO = seqNO == null ? "" : seqNO;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId),
                                                        new SqlParameter("@SeqNO", seqNO)};
            string sql = @"DELETE NoticeFile WHERE NoticeId = @NoticeId
                        ";
            if (!string.IsNullOrEmpty(seqNO))
            {
                sql += " AND SeqNO = @SeqNO";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        // 公告用户保存
        public void SaveNoticeUserId(NoticeUserId noticeUserId)
        {
            NoticeUserId findOne = db.NoticeUserId.Where(x => (x.NoticeId == noticeUserId.NoticeId && x.UserId == noticeUserId.UserId)).FirstOrDefault();
            if (findOne == null)
            {
                noticeUserId.InDateTime = DateTime.Now;
                db.NoticeUserId.Add(noticeUserId);
            }
            db.SaveChanges();
        }
        // 公告用户删除
        public void NoticeUserIdDelete(string noticeId, string userId)
        {
            userId = userId == null ? "" : userId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId),
                                                        new SqlParameter("@UserId", userId)};
            string sql = @"DELETE NoticeUserId WHERE NoticeId = @NoticeId
                        ";
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND UserId = @UserId";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        // 公告权限保存
        public void SaveNoticeRoleType(NoticeRoleType noticeRoleType)
        {
            NoticeRoleType findOne = db.NoticeRoleType.Where(x => (x.NoticeId == noticeRoleType.NoticeId && x.RoleTypeId == noticeRoleType.RoleTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                noticeRoleType.InDateTime = DateTime.Now;
                db.NoticeRoleType.Add(noticeRoleType);
            }
            db.SaveChanges();
        }
        // 公告权限删除
        public void NoticeRoleTypeDelete(string noticeId, string roleTypeId)
        {
            roleTypeId = roleTypeId == null ? "" : roleTypeId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@NoticeId", noticeId),
                                                        new SqlParameter("@RoleTypeId", roleTypeId)};
            string sql = @"DELETE NoticeRoleType WHERE NoticeId = @NoticeId
                        ";
            if (!string.IsNullOrEmpty(roleTypeId))
            {
                sql += " AND RoleTypeId = @RoleTypeId";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        // 公告查看保存
        public void SaveNoticeView(NoticeView noticeView)
        {
            NoticeView findOne = db.NoticeView.Where(x => (x.NoticeId == noticeView.NoticeId && x.UserId == noticeView.UserId)).FirstOrDefault();
            if (findOne == null)
            {
                noticeView.InDateTime = DateTime.Now;
                db.NoticeView.Add(noticeView);
            }
            db.SaveChanges();
        }

        #endregion
        #region 上报GTMC 记录
        //查询上报记录
        public List<AnswerShopInfoUploadLog> GetAnswerShopInfoUploadLog(string projectId, string shopId)
        {
            projectId = projectId == null ? "" : projectId;
            shopId = shopId == null ? "" : shopId;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopId", shopId)};
            Type t = typeof(AnswerShopInfoUploadLog);
            string sql = "";

            sql = @"SELECT A.*
                    FROM AnswerShopInfoUploadLog A WHERE 1=1";

            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerShopInfoUploadLog>().ToList();

        }
        // 保存上报记录
        public void SaveAnswerShopInfoUploadLog(AnswerShopInfoUploadLog answerShopInfoUploadLog)
        {
            AnswerShopInfoUploadLog findOne = db.AnswerShopInfoUploadLog.Where(x => (x.ProjectId == answerShopInfoUploadLog.ProjectId && x.ShopId == answerShopInfoUploadLog.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                answerShopInfoUploadLog.InDateTime = DateTime.Now;
                db.AnswerShopInfoUploadLog.Add(answerShopInfoUploadLog);
            }
            else
            {
            }
            db.SaveChanges();
        }
        // 保存调用日志
        public void SaveExtraCallLog(ExtraCallLog extraCallLog)
        {
            extraCallLog.InDateTime = DateTime.Now;
            db.ExtraCallLog.Add(extraCallLog);
            db.SaveChanges();
        }
        #endregion
        #region 短信发送记录
        // 短信发送查询
        public List<SMSInfo> GetSMSInfo(string projectId, string shopId, string smsBussinessType, string telNo)
        {
            projectId = projectId == null ? "" : projectId;
            shopId = shopId == null ? "" : shopId;
            smsBussinessType = smsBussinessType == null ? "" : smsBussinessType;
            telNo = telNo == null ? "" : telNo;
            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@SmsBussinessType", smsBussinessType),
                                                        new SqlParameter("@TelNo", telNo)};
            Type t = typeof(SMSInfo);
            string sql = "";

            sql = @"SELECT A.*
                    FROM SMSInfo A WHERE 1=1";

            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(smsBussinessType))
            {
                sql += " AND A.SmsBussinessType = @SmsBussinessType";
            }
            if (!string.IsNullOrEmpty(telNo))
            {
                sql += " AND A.TelNO = @TelNO";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<SMSInfo>().ToList();

        }
        // 保存短信发送
        public void SaveSMSInfo(SMSInfo smsInfo)
        {
            SMSInfo findOne = db.SMSInfo.Where(x => (x.ProjectId == smsInfo.ProjectId
                                                    && x.ShopId == smsInfo.ShopId
                                                    && x.TelNO == smsInfo.TelNO
                                                    && x.SMSBussinessType == smsInfo.SMSBussinessType)).FirstOrDefault();
            if (findOne == null)
            {
                smsInfo.InDateTime = DateTime.Now;
                smsInfo.ModifyDateTime = DateTime.Now;
                db.SMSInfo.Add(smsInfo);
            }
            else
            {
                findOne.BizId = smsInfo.BizId;
                findOne.ErrCode = smsInfo.ErrCode;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = smsInfo.ModifyUserId;
                findOne.RequestId = smsInfo.RequestId;
                findOne.SendStatus = smsInfo.SendStatus;
                findOne.SMSSendDate = smsInfo.SMSSendDate;
            }
            db.SaveChanges();
        }
        #endregion
        #region 自检导入任务
        public void TaskCreate(List<ProjectDto> projectList, List<FileResultDto> subjectFileList_all)
        {
            string sql = "";

            foreach (ProjectDto project in projectList)
            {
                string indexProject = projectList.IndexOf(project).ToString();
                string startDate = "";
                string endDate = "";
                if (project.StartDate != null)
                {
                    startDate = Convert.ToDateTime(project.StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (project.EndDate != null)
                {
                    endDate = Convert.ToDateTime(project.EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                }
                sql += " DECLARE @ProjectId"+ indexProject+" INT; " + "\r\n";
                sql += " DECLARE @OrderNO_Project" + indexProject + " INT; " + "\r\n";
                sql += " DECLARE @ChapterId" + indexProject +" INT; " + "\r\n";
                sql += " DECLARE @ShopId" + indexProject + " INT; " + "\r\n";
                // Project
                sql += " IF EXISTS(SELECT 1 FROM Project WHERE ProjectCode = '" + project.ProjectCode + "')" + "\r\n";
                sql += " BEGIN " + "\r\n";
                sql += " SELECT @ProjectId" + indexProject + " = ProjectId FROM Project WHERE ProjectCode ='" + project.ProjectCode + "'" + "\r\n";
                sql += " UPDATE Project SET ProjectCode='" + project.ProjectCode + "'," + "\r\n";
                sql += " ProjectName='" + project.ProjectName + "'," + "\r\n";
                sql += " ProjectShortName='" + project.ProjectShortName + "'," + "\r\n";
                //sql += " SET Year='" + project.Year + "'," + "\r\n";
                sql += " ProjectType='" + project.ProjectType + "'," + "\r\n";
                sql += " StartDate='" + startDate + "'," + "\r\n";
                sql += " EndDate='" + endDate + "'," + "\r\n";
                sql += " ProjectGroup='" + project.ProjectGroup + "'," + "\r\n";
                sql += " ModifyUserId='" + project.ModifyUserId + "'," + "\r\n";
                sql += " ModifyDateTime=GETDATE()" + "\r\n";
                sql += " WHERE ProjectId = @ProjectId" + indexProject + "\r\n";
                sql += " END" + "\r\n";
                sql += " ELSE " + "\r\n";
                sql += " BEGIN " + "\r\n";
                sql += " SELECT @OrderNO_Project"+ indexProject +"= Max(OrderNO)+1 FROM Project WHERE BrandId = " + project.BrandId.ToString() + "\r\n";
                sql += @" INSERT INTO Project(TenantId,BrandId,ProjectCode,ProjectName,ProjectShortName
                          ,OrderNO,ProjectType,StartDate,EndDate,ProjectGroup,InUserId,InDateTime,ModifyUserId,ModifyDateTime)
                         VALUES('";
                sql += project.TenantId.ToString() + "','";
                sql += project.BrandId.ToString() + "','";
                sql += project.ProjectCode + "','";
                sql += project.ProjectName + "','";
                sql += project.ProjectShortName + "',";
                sql += "@OrderNO_Project"+indexProject + ",'";
                sql += project.ProjectType + "','";
                sql += startDate + "','";
                sql += endDate + "','";
                sql += project.ProjectGroup + "','";
                sql += project.InUserId + "',";
                sql += "GETDATE(),'";
                sql += project.ModifyUserId + "',";
                sql += "GETDATE()";
                sql += ")" + "\r\n";
                sql += "SELECT @ProjectId"+indexProject+" = SCOPE_IDENTITY()" + "\r\n";
                sql += " END" + "\r\n";
                //Chapter
                sql += " IF EXISTS(SELECT 1 FROM Chapter WHERE ChapterCode = '" + project.ProjectCode + "')" + "\r\n";
                sql += " BEGIN " + "\r\n";
                sql += " SELECT @ChapterId"+indexProject+" = ChapterId FROM Chapter WHERE ChapterCode = '" + project.ProjectCode + "'" + "\r\n";
                sql += " UPDATE Chapter SET ChapterName = '" + project.ProjectCode + "'," + "\r\n";
                sql += " ProjectId=@ProjectId"+indexProject + "," + "\r\n";
                sql += " StartDate = '" + startDate + "'," + "\r\n";
                sql += " EndDate = '" + endDate + "'" + "\r\n";
                sql += " WHERE ChapterId = @ChapterId"+indexProject + "\r\n";
                sql += " END" + "\r\n";
                sql += " ELSE " + "\r\n";
                sql += " BEGIN" + "\r\n";
                sql += " INSERT INTO Chapter(ChapterCode,ChapterName,ProjectId,StartDate,EndDate,InUserId,InDateTime) VALUES('";
                sql += project.ProjectCode + "','";
                sql += project.ProjectCode + "',";
                sql += "@ProjectId"+indexProject + ",'";
                sql += startDate + "','";
                sql += endDate + "','";
                sql += project.InUserId + "',";
                sql += "GETDATE()";
                sql += ")" + "\r\n";
                sql += "SELECT @ChapterId"+indexProject+" = SCOPE_IDENTITY()" + "\r\n";
                sql += " END" + "\r\n";

                // ProjectShopExamType
                sql += "SELECT @ShopId"+indexProject+" = ShopId FROM Shop WHERE ShopCode= '";
                sql += project.ShopCode + "'";
                sql += "AND BrandId=";
                sql += project.BrandId + "\r\n";

                sql += " IF NOT EXISTS(SELECT 1 FROM ProjectShopExamType WHERE ProjectId = @ProjectId"+indexProject+" AND ShopId = @ShopId"+indexProject+")" + "\r\n";
                sql += " BEGIN " + "\r\n";
                sql += " INSERT INTO ProjectShopExamType VALUES(@ProjectId"+indexProject+",@ShopId"+indexProject+",null,";
                sql += project.InUserId + ",GETDATE(),";
                sql += project.InUserId + ",GETDATE())" + "\r\n";
                sql += "END" + "\r\n";
                List<FileResultDto> subjectFileList = subjectFileList_all.Where(x => x.Date == project.Date).ToList();
                foreach (FileResultDto subjectFile in subjectFileList)
                {
                    string index = "p"+projectList.IndexOf(project).ToString() + "s"+subjectFileList.IndexOf(subjectFile).ToString();
                    sql += " DECLARE @SubjectId"+ index+" INT; " + "\r\n";
                    sql += " DECLARE @LabelId"+index+" INT;" + "\r\n";
                    sql += " DECLARE @LabelId_Recheck"+index+" INT;" + "\r\n";
                    // Subject
                    sql += "SELECT @LabelId"+index+" = LabelId FROM Label WHERE LabelType='ExamType' AND BrandId=" + project.BrandId;
                    sql += " AND LabelCode = '" + subjectFile.ExamTypeCode + "'" + "\r\n";
                    sql += "SELECT @LabelId_Recheck"+index+"=LabelId FROM Label WHERE LabelType='RecheckType' AND BrandId=" + project.BrandId;
                    sql += " AND LabelCode='" + subjectFile.RecheckTypeCode + "'" + "\r\n";
                    sql += " IF EXISTS(SELECT 1 FROM Subject WHERE ProjectId = @ProjectId"+indexProject+" AND SubjectCode='";
                    sql += subjectFile.SubjectCode + "')" + "\r\n";
                    sql += "BEGIN" + "\r\n";
                    sql += "SELECT @SubjectId"+index+" = SubjectId FROM Subject WHERE ProjectId = @ProjectId"+indexProject+" AND SubjectCode='";
                    sql += subjectFile.SubjectCode + "'" + "\r\n";
                    sql += "UPDATE Subject SET SubjectCode = '" + subjectFile.SubjectCode + "'," + "\r\n";
                    sql += "ProjectId = @ProjectId"+indexProject+"," + "\r\n";
                    sql += "OrderNO = '" + subjectFile.OrderNO + "'," + "\r\n";
                    sql += "[CheckPoint] = '" + subjectFile.CheckPoint + "'," + "\r\n";
                    sql += "Implementation='" + subjectFile.Implementation + "'," + "\r\n";
                    sql += "InspectionDesc='" + subjectFile.InspectionDesc + "'," + "\r\n";
                    sql += "LabelId=@LabelId"+index+"," + "\r\n";
                    sql += "LabelId_RecheckType=@LabelId_Recheck"+index+"," + "\r\n";
                    //sql += "HiddenCode_SubjectType='" + subjectFile.HiddenCode_SubjectType + "'," + "\r\n";
                    sql += "Remark='" + subjectFile.Remark + "'," + "\r\n";
                    sql += "ImproveAdvice='" + subjectFile.ImproveAdvice + "'," + "\r\n";
                    sql += "ModifyUserId='" + project.InUserId + "'," + "\r\n";
                    sql += "ModifyDateTime=GETDATE()" + "\r\n";
                    sql += " WHERE SubjectId = @SubjectId"+index + "\r\n";
                    sql += "END" + "\r\n";
                    sql += "ELSE" + "\r\n";
                    sql += "BEGIN" + "\r\n";
                    sql += @"INSERT INTO Subject(SubjectCode,ProjectId,OrderNO,[CheckPoint],Implementation,InspectionDesc,LabelId,LabelId_RecheckType,
                           HiddenCode_SubjectType,Remark,ImproveAdvice,InUserId,InDateTime,ModifyUserId,ModifyDateTime) VALUES('";
                    sql += subjectFile.SubjectCode + "',@ProjectId"+indexProject+",'";
                    sql += subjectFile.OrderNO + "','";
                    sql += subjectFile.CheckPoint + "','";
                    sql += subjectFile.Implementation + "','";
                    sql += subjectFile.InspectionDesc + "',@LabelId"+index+",@LabelId_Recheck"+index+",'";
                    sql += "Photo" + "','";
                    sql += subjectFile.Remark + "','";
                    sql += subjectFile.ImproveAdvice + "','";
                    sql += project.InUserId + "',";
                    sql += "GETDATE(),'";
                    sql += project.InUserId + "',";
                    sql += "GETDATE())" + "\r\n";
                    sql += "SELECT @SubjectId"+index+" = SCOPE_IDENTITY()" + "\r\n";
                    sql += "END" + "\r\n";
                    // SubjectFile
                    sql += " IF EXISTS(SELECT 1 FROM SubjectFile WHERE SubjectId = @SubjectId"+index+" AND SeqNO=1)"+ "\r\n";
                    sql += " BEGIN" + "\r\n";
                    sql += " UPDATE SubjectFile SET [FileName] = '" + subjectFile.FileName + "'," + "\r\n"; ;
                    sql += " FileDemo = '" + subjectFile.FileDemo + "'," + "\r\n";
                    sql += " FileDemoDesc= '" + subjectFile.FileDemoDesc + "'," + "\r\n";
                    sql += " FileRemark='" + subjectFile.FileRemark + "'," + "\r\n";
                    sql += " ModifyUserId=" + project.InUserId + "," + "\r\n";
                    sql += " ModifyDateTime=GETDATE()" + "\r\n";
                    sql += " WHERE SubjectId = @SubjectId"+index+" AND SeqNO=1" + "\r\n";
                    sql += " END" + "\r\n";
                    sql += " ELSE" + "\r\n";
                    sql += " BEGIN" + "\r\n";
                    sql += " INSERT INTO SubjectFile VALUES(@SubjectId"+index+",1,'" + subjectFile.FileName + "','";
                    sql += project.InUserId + "',";
                    sql += "GETDATE(),";
                    sql += project.InUserId + ",";
                    sql += "GETDATE(),'";
                    sql += subjectFile.FileDemo + "','";
                    sql += subjectFile.FileDemoDesc + "','";
                    sql += subjectFile.FileRemark + "')" + "\r\n";
                    sql += " END " + "\r\n";

                    // chapterSubject
                    sql += " IF NOT EXISTS(SELECT 1 FROM ChapterSubject WHERE ChapterId = @ChapterId"+indexProject+" AND SubjectId = @SubjectId"+index+")" + "\r\n";
                    sql += " BEGIN " + "\r\n";
                    sql += " INSERT INTO ChapterSubject VALUES(@ChapterId"+indexProject+",@SubjectId"+index+",";
                    sql += project.InUserId + ",GETDATE())" + "\r\n";
                    sql += " END" + "\r\n";

                }
            }
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion

    }
}