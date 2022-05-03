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
        public List<RoleType> GetRoleType(string type,string roleTypeCode,string roleTypeName)
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
        public List<UserInfo> GetUserInfo(string tenantId, string brandId, string userId, string accountId, string accountName, string roleTypeCode, string telNO, string email)
        {
            if (tenantId == null) tenantId = "";
            if (brandId == null) brandId = "";
            if (userId == null) userId = "";
            if (accountId == null) accountId = "";
            if (accountName == null) accountName = "";
            if (roleTypeCode == null) roleTypeCode = "";
            if (telNO == null) telNO = "";
            if (email == null) email = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@AccountId", accountId),
                                                        new SqlParameter("@AccountName", accountName),
                                                        new SqlParameter("@RoleType", roleTypeCode),
                                                        new SqlParameter("@TelNO", telNO),
                                                        new SqlParameter("@Email", email)
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
            }
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
        public List<UserInfoObjectDto> GetUserInfoObject(string tenantId, string userId, string objectId,string roleTypeCode)
        {
            if (tenantId == null) tenantId = "";
            if (objectId == null) objectId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@ObjectId", objectId),
                                                        new SqlParameter("@UserId", userId)
                                                        };
            Type t = typeof(UserInfoObjectDto);

            string sql = "";
            if (roleTypeCode == "B_Bussiness")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='Bussiness'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_WideArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='WideArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_BigArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='BigArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_MiddleArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='MiddleArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_SmallArea")
            {
                sql = @"SELECT B.Id,B.UserId,C.AreaCode AS ObjectCode,C.AreaName AS ObjectName,C.AreaId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Area C ON B.ObjectId = C.AreaId AND C.AreaType='SmallArea'
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_Group")
            {
                sql = @"SELECT B.Id,B.UserId,C.GroupCode AS ObjectCode,C.GroupName AS ObjectName,C.GroupId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN [Group] C ON B.ObjectId = C.GroupId
                          WHERE 1=1";
            }
            else if (roleTypeCode == "B_Shop")
            {
                sql = @"SELECT B.Id,B.UserId,C.ShopCode AS ObjectCode,C.ShopName AS ObjectName,C.ShopId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                              INNER JOIN Shop C ON B.ObjectId = C.ShopId
                          WHERE 1=1";
            }
            else if (roleTypeCode == "S_Execute")// 设置执行人员有权限的经销商
            {
                sql = @"SELECT B.Id,B.UserId,C.ShopCode AS ObjectCode,C.ShopName AS ObjectName,C.ShopId AS ObjectId
                          FROM [UserInfo] A INNER JOIN UserInfoObject B ON A.Id = B.UserId
                                            INNER JOIN Shop C ON B.ObjectId = C.ShopId
                          WHERE 1=1 ";
            }
            else {

            }
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
        public void DeleteUserInfoObject(int userInfoObjectId)
        {
            UserInfoObject findone = db.UserInfoObject.Where(x => x.Id == userInfoObjectId).FirstOrDefault();
            db.UserInfoObject.Remove(findone);
            db.SaveChanges();
            //SqlParameter[] para = new SqlParameter[] { new SqlParameter("@FileId", fileId) };
        }
        #endregion
        #region 区域管理
        public List<AreaDto> GetArea(string areaId,string brandId, string areaCode, string areaName, string areaType,string parentId,bool? useChk)
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
        public List<Group> GetGroup(string brandId, string groupId,string groupCode, string groupName)
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
        public List<ProjectDto> GetProject(string tenantId, string brandId, string projectId, string projectCode,string year,string orderNO)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            projectId = projectId == null ? "" : projectId;
            year = year == null ? "" : year;
            projectCode = projectCode == null ? "" : projectCode;
            orderNO = orderNO == null ? "" : orderNO;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ProjectCode", projectCode),
                                                        new SqlParameter("@Year", year),
                                                     new SqlParameter("@OrderNO", orderNO)};
            Type t = typeof(ProjectDto);
            string sql = "";
            sql = @"SELECT *,CASE WHEN GETDATE()<ReportDeployDate OR ReportDeployDate IS NULL THEN CAST(0 AS BIT) 
                             ELSE CAST(1 AS BIT) END AS ReportDeployChk
                    FROM [Project]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(projectCode))
            {
                sql += " AND ProjectCode = @ProjectCode";
            }
            if (!string.IsNullOrEmpty(year))
            {
                sql += " AND Year = @Year";
            }
            if (!string.IsNullOrEmpty(orderNO))
            {
                sql += " AND OrderNO = @OrderNO";
            }
            sql += " ORDER BY Year,OrderNO DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<ProjectDto>().ToList();

        }
        /// <summary>
        /// 保存期号信息
        /// </summary>
        /// <param name="project"></param>
        public void SaveProject(Project project)
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
                project.InDateTime = DateTime.Now;
                project.ModifyDateTime = DateTime.Now;
                db.Project.Add(project);
            }
            else
            {
                findOne.ProjectName = project.ProjectName;
                findOne.ProjectCode = project.ProjectCode;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = project.ModifyUserId;
                //findOne.DataScore = project.DataScore;
                //findOne.AppealStartDate = project.AppealStartDate;
                //findOne.AppealEndDate = project.AppealEndDate;
                //findOne.AppealMode = project.AppealMode;
                findOne.ReportDeployDate= project.ReportDeployDate;// 报告发布时间
                findOne.OrderNO = project.OrderNO;
                findOne.Quarter = project.Quarter;
                findOne.Year = project.Year;
            }
            db.SaveChanges();
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
        public List<ShopDto> GetShop(string tenantId, string brandId, string shopId, string shopCode, string key)
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
                          ,[Province]
                          ,[City]
                          ,[GroupId]
                           ,(SELECT TOP 1  GroupName FROM  [GROUP] X WHERE X.GroupId = A.GroupId) AS GroupName
                            ,(SELECT TOP 1  GroupCode FROM  [GROUP] Y WHERE Y.GroupId = A.GroupId) AS GroupCode
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
                sql += " AND BrandId = @BrandId";
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
                findOne.UseChk = shop.UseChk;
            }
            db.SaveChanges();
        }
        public void ImportShop(List<Shop> shopList)
        {
            string sql = "";
            foreach (Shop shop in shopList)
            {
                sql += " INSERT INTO Shop VALUES('"+shop.TenantId.ToString()+"','";
                sql += shop.BrandId.ToString() + "','";
                sql += shop.ShopCode + "','";
                sql += shop.ShopName + "','";
                sql += shop.ShopShortName + "','";
                sql += shop.Province + "','";
                sql += shop.City + "','";
                sql += shop.GroupId + "','";
                sql += shop.InUserId + "','";
                sql += DateTime.Now.ToString() + "','";
                sql += shop.ModifyUserId + "','";
                sql += DateTime.Now.ToString() + ",')";
            }
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
            AreaShop findOne = db.AreaShop.Where(x => (x.ShopId == areaShop.ShopId&&x.AreaId==areaShop.AreaId)).FirstOrDefault();
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
        #region 体系管理

        /// <summary>
        /// 获取体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectDto> GetSubject(string projectId, string subjectId,string subjectCode,string orderNO)
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
            sql = @"SELECT A.*,B.ProjectCode,B.ProjectName,C.LabelCode As ExamTypeCode,C.LabelName AS ExamTypeName
                        ,D.LabelCode As RecheckTypeCode,D.LabelName AS RecheckTypeName,E.HiddenName AS HiddenCode_SubjectTypeName
                    FROM [Subject] A INNER JOIN Project B ON A.ProjectId = B.ProjectId 
                                    LEFT JOIN Label C ON B.BrandId = C.BrandId AND A.LabelId  =  C.LabelId
                                    LEFT JOIN Label D ON B.BrandId = D.BrandId AND A.LabelId_RecheckType  =  D.LabelId
                                    LEFT JOIN HiddenColumn E ON A.HiddenCode_SubjectType = E.HiddenCode AND E.HiddenCodeGroup = '体系类型'
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
                sql += " AND A.OrderNO =@OrderNO " ;
            }
            List<SubjectDto> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectDto>().ToList();
            return list;
        }
        /// <summary>
        /// 保存体系信息
        /// </summary>
        /// <param name="subject"></param>
        public void SaveSubject(Subject subject)
        {
            Subject findOne = db.Subject.Where(x => (x.SubjectId == subject.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                subject.InDateTime = DateTime.Now;
                subject.ModifyDateTime = DateTime.Now;
                db.Subject.Add(subject);
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
                findOne.SubjectCode = subject.SubjectCode;
            }
            db.SaveChanges();
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
            if (subjectFile.SeqNO == 0)
            {
                SubjectFile findOneMax = db.SubjectFile.Where(x => (x.SubjectId==subjectFile.SubjectId)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
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
                SubjectFile findOne = db.SubjectFile.Where(x => (x.SubjectId==subjectFile.SubjectId&& x.SeqNO == subjectFile.SeqNO)).FirstOrDefault();
                if (findOne == null)
                {
                    subjectFile.InDateTime = DateTime.Now;
                    db.SubjectFile.Add(subjectFile);
                }
                else
                {
                    findOne.FileName = subjectFile.FileName;
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
        public void DeleteSubjectFile(long subjectId,int seqNo)
        {
            SubjectFile findone = db.SubjectFile.Where(x => x.SubjectId==subjectId&&x.SeqNO==seqNo).FirstOrDefault();
            db.SubjectFile.Remove(findone);
            db.SaveChanges();
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
            SubjectInspectionStandard findone = db.SubjectInspectionStandard.Where(x => x.SubjectId == subjectId && x.SeqNO == seqNo).FirstOrDefault();
            db.SubjectInspectionStandard.Remove(findone);
            db.SaveChanges();
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
            SubjectLossResult findone = db.SubjectLossResult.Where(x => x.SubjectId == subjectId && x.SeqNO == seqNo).FirstOrDefault();
            db.SubjectLossResult.Remove(findone);
            db.SaveChanges();
        }
        #endregion
        #region 标签管理
        public List<Label> GetLabel(string brandId,string labelId,string labelType,bool? useChk,string labelCode)
        {
            if (labelCode == null) labelCode = "";
            if (labelType == null) labelType = "";
            if (labelId == null) labelId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@LabelType", labelType),
                                                        new SqlParameter("@BrandId", brandId),
                                                         new SqlParameter("@LabelCode", labelCode),
                                                        new SqlParameter("@LabelId", labelId)};
            Type t = typeof(Label);
            string sql = "";
            sql = @"SELECT * 
                      FROM [Label] A
                    WHERE  BrandId = @BrandId AND 1=1
                    ";
            if (useChk.HasValue)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND A.UseChk = @UseChk";
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
            return db.Database.SqlQuery(t, sql, para).Cast<Label>().ToList();
        }
        public void SaveLabel(Label label)
        {
            Label findOne = db.Label.Where(x => (x.LabelId == label.LabelId)).FirstOrDefault();
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
        public void SaveLabelObject(LabelObject labelObject)
        {
            LabelObject findOne = db.LabelObject.Where(x => (x.LabelId == labelObject.LabelId&&x.ObjectId==labelObject.ObjectId)).FirstOrDefault();
            if (findOne == null)
            {
                labelObject.InDateTime = DateTime.Now;
                db.LabelObject.Add(labelObject);
            }
            db.SaveChanges();
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
    }
}