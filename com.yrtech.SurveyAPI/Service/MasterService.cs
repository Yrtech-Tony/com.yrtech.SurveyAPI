using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyAPI.DTO.Master;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.SurveyAPI.Service
{
    public class MasterService
    {
        Survey db = new Survey();
        AccountService accountService = new AccountService();
        localhost.Service webService = new localhost.Service();
        /// <summary>
        /// 获取体系类型
        /// </summary>
        /// <returns></returns>
        public List<SubjectType> GetSubjectType()
        {
            Type t = typeof(SubjectType);
            string sql = "";
            sql = @"SELECT [SubjectTypeId]
                          ,[SubjectTypeName]
                          ,[InUserId]
                          ,[InDateTime]
                   FROM [SubjectType] ";
            return db.Database.SqlQuery(t, sql, new SqlParameter[] { }).Cast<SubjectType>().ToList();

        }
        /// <summary>
        /// 获取体系试卷类型
        /// </summary>
        /// <returns></returns>
        public List<SubjectTypeExam> GetSubjectTypeExam(string projectId, string subjectTypeExamId)
        {
            if (subjectTypeExamId == null) subjectTypeExamId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@SubjectTypeExamId", subjectTypeExamId)};
            Type t = typeof(SubjectTypeExam);
            string sql = "";

            sql = @"SELECT [SubjectTypeExamId]
                          ,[SubjectTypeExamName]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
                      FROM [SubjectTypeExam] WHERE 1=1 ";
            if (string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (string.IsNullOrEmpty(subjectTypeExamId))
            {
                sql += " AND SubjectTypeExamId = @SubjectTypeExamId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectTypeExam>().ToList();

        }
        /// <summary>
        /// 保存体系试卷类型
        /// </summary>
        /// <param name="subjectTypeExam"></param>
        public void SaveSubjectTypeExam(SubjectTypeExam subjectTypeExam)
        {

            SubjectTypeExam findOne = db.SubjectTypeExam.Where(x => (x.SubjectTypeExamId == subjectTypeExam.SubjectTypeExamId)).FirstOrDefault();
            if (findOne == null)
            {
                subjectTypeExam.InDateTime = DateTime.Now;
                subjectTypeExam.ModifyDateTime = DateTime.Now;
                db.SubjectTypeExam.Add(subjectTypeExam);
            }
            else
            {
                findOne.SubjectTypeExamName = subjectTypeExam.SubjectTypeExamName;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subjectTypeExam.ModifyUserId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 查询租户信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<Tenant> GetTenant(string tenantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId) };
            Type t = typeof(Tenant);
            string sql = "";

            sql = @"SELECT TenantId,TenantName,TenantCode,Email,TelNo,InUserid,InDateTime,ModifyUserId,ModifyDateTime
	                    FROM Tenant WHERE 1=1 ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Tenant>().ToList();

        }
        /// <summary>
        /// 查询品牌信息,
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Brand> GetBrand(string tenantId, string userId, string roleType, string brandId)
        {
            userId = userId == null ? "" : userId;
            brandId = brandId == null ? "" : brandId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                       new SqlParameter("@UserId", userId),
                                                        new SqlParameter("@BrandId", brandId)};
            Type t = typeof(Brand);
            string sql = "";

            sql = @"SELECT DISTINCT A.BrandId,A.TenantId,A.BrandName,A.BrandCode,A.Remark,A.InUserId,A.InDateTime,A.ModifyUserId,A.ModifyDateTime
                    FROM Brand A ";
            if (roleType != "S_Sysadmin"&& roleType!="S") // 不是租户管理员和系统管理员时按照userId查询，如果时租户管理员查询所有
            {
                sql += "INNER JOIN UserInfoBrand B ON A.BrandId = B.BrandId";
            }
            sql += " WHERE 1=1 AND A.TenantId = @TenantId";
            if (!string.IsNullOrEmpty(userId) && roleType != "S_Sysadmin" && roleType != "S")
            {
                sql += " AND B.UserId = @UserId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND A.BrandId = @BrandId";
            }
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
        /// <summary>
        /// 查询品牌下账号信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfoByBrandId(string brandId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@BrandId", brandId) };
            Type t = typeof(UserInfo);
            string sql = "";

            sql = @"SELECT C.Id,C.TenantId,A.BrandId,C.AccountId,C.AccountName,C.Password,C.UserType,C.RoleType,ISNULL(C.UseChk,0) AS UseChk,
                    Email,TelNO,HeadPicUrl,
                    C.InUserId,C.InDateTime,C.ModifyUserId,C.ModifyDateTime
                    FROM Brand A INNER JOIN UserInfoBrand B ON A.BrandId = B.BrandId AND A.BrandId = @BrandId
								 INNER JOIN UserInfo C ON B.UserId = C.Id ";

            return db.Database.SqlQuery(t, sql, para).Cast<UserInfo>().ToList();

        }
        /// <summary>
        /// 获取期号
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<Project> GetProject(string tenantId, string brandId, string projectId, string year)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            projectId = projectId == null ? "" : projectId;
            year = year == null ? "" : year;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@Year", year)};
            Type t = typeof(Project);
            string sql = "";
            sql = @"SELECT [ProjectId]
                          ,[TenantId]
                          ,[BrandId]
                          ,[ProjectCode]
                          ,[ProjectName]
                          ,[Year]
                          ,[Quarter]
                          ,[DataScore]
                          ,AppealStartDate
                          ,[OrderNO]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
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
            if (!string.IsNullOrEmpty(year))
            {
                sql += " AND Year = @Year";
            }
            sql += " ORDER BY OrderNO";
            return db.Database.SqlQuery(t, sql, para).Cast<Project>().ToList();

        }
        /// <summary>
        /// 保存期号信息
        /// </summary>
        /// <param name="project"></param>
        public void SaveProject(Project project)
        {
            List<UserInfo> userList = accountService.GetUserInfo(project.ModifyUserId.ToString());
            if (userList == null || userList.Count == 0)
            {
                throw new Exception("没有找到对应的用户");
            }
            string brandId = project.BrandId.ToString();
            string accountId = userList[0].AccountId;

            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }

            webService.SaveProject('I', project.ProjectCode, project.Year, project.Quarter, Convert.ToInt32(project.OrderNO));

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
                findOne.DataScore = project.DataScore;
                findOne.OrderNO = project.OrderNO;
                findOne.Quarter = project.Quarter;
                findOne.Year = project.Year;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取期号下的复审类型
        /// </summary>
        /// <returns></returns>
        public List<SubjectRecheckType> GetSubjectRecheckType(string projectId, string recheckTypeId)
        {
            if (string.IsNullOrEmpty(recheckTypeId)) recheckTypeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId), new SqlParameter("@RecheckTypeId", recheckTypeId) };
            Type t = typeof(SubjectRecheckType);
            string sql = "";

            sql = @"SELECT [RecheckTypeId]
                  ,[RecheckTypeName]
                  , ProjectId
                  ,UseChk
                  ,[InUserId]
                  ,[InDateTime]
                  ,[ModifyUserId]
                  ,[ModifyDateTime]
              FROM [SubjectRecheckType] WHERE ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(recheckTypeId))
            {
                sql += " AND RecheckTypeId = @RecheckTypeId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectRecheckType>().ToList();
        }
        /// <summary>
        /// 保存期号下的复审类型
        /// </summary>
        /// <param name="subjectRecheckType"></param>
        public void SaveSubjectRecheckType(SubjectRecheckType subjectRecheckType)
        {

            SubjectRecheckType findOne = db.SubjectRecheckType.Where(x => (x.RecheckTypeId == subjectRecheckType.RecheckTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                subjectRecheckType.InDateTime = DateTime.Now;
                subjectRecheckType.ModifyDateTime = DateTime.Now;
                subjectRecheckType.UseChk = true;
                db.SubjectRecheckType.Add(subjectRecheckType);
            }
            else
            {
                findOne.RecheckTypeName = subjectRecheckType.RecheckTypeName;
                findOne.UseChk = subjectRecheckType.UseChk;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subjectRecheckType.ModifyUserId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取期号下的复审错误类型
        /// </summary>
        /// <returns></returns>
        public List<RecheckErrorType> GetRecheckErrorType(string projectId, string recheckErrorTypeId)
        {
            if (recheckErrorTypeId == null) recheckErrorTypeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    , new SqlParameter("@RecheckErrorTypeId", recheckErrorTypeId) };
            Type t = typeof(RecheckErrorType);
            string sql = "";

            sql = @"SELECT * FROM RecheckErrorType WHERE ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(recheckErrorTypeId))
            {
                sql += " AND RecheckErrorTypeId = @RecheckErrorTypeId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<RecheckErrorType>().ToList();
        }
        /// <summary>
        /// 保存期号下的复审错误类型
        /// </summary>
        /// <param name="recheckErrorType"></param>
        public void SaveRecheckErrorType(RecheckErrorType recheckErrorType)
        {

            RecheckErrorType findOne = db.RecheckErrorType.Where(x => (x.RecheckErrorTypeId == recheckErrorType.RecheckErrorTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                recheckErrorType.InDateTime = DateTime.Now;
                recheckErrorType.ModifyDateTime = DateTime.Now;
                recheckErrorType.UseChk = true;
                db.RecheckErrorType.Add(recheckErrorType);
            }
            else
            {
                findOne.RecheckErrorName = recheckErrorType.RecheckErrorName;
                findOne.UseChk = recheckErrorType.UseChk;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = recheckErrorType.ModifyUserId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 期号下获取流程类型
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectLink> GetSubjectLink(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectLink);
            string sql = @"SELECT * FROM [SubjectLink] WHERE ProjectId = @ProjectId";
            return db.Database.SqlQuery(t, sql, para).Cast<SubjectLink>().ToList();
        }
        /// <summary>
        /// 保存期号下的流程类型
        /// </summary>
        /// <param name="project"></param>
        public void SaveSubjectLink(SubjectLink subjectLink)
        {
            SubjectLink findOne = db.SubjectLink.Where(x => (x.SubjectLinkId == subjectLink.SubjectLinkId)).FirstOrDefault();
            if (findOne == null)
            {
                subjectLink.InDateTime = DateTime.Now;
                db.SubjectLink.Add(subjectLink);
            }
            else
            {
                findOne.SubjectLinkCode = subjectLink.SubjectLinkCode;
                findOne.SubjectLinkName = subjectLink.SubjectLinkName;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取经销商
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<Shop> GetShop(string tenantId, string brandId, string shopId,string key)
        {
            tenantId = tenantId == null ? "" : tenantId;
            brandId = brandId == null ? "" : brandId;
            shopId = shopId == null ? "" : shopId;
            key = key == null ? "" : key;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(Shop);
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
                          ,[UseChk]
                          ,[InUserId]
                          ,[InDateTime]
                          ,[ModifyUserId]
                          ,[ModifyDateTime]
                      FROM [Shop]
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
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (ShopCode LIKE '%" + key + "%' OR ShopName LIKE '%"+key+"%' OR ShopShortName LIKE '%"+key+"%')";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Shop>().ToList();
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
                findOne.City = shop.City;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = shop.ModifyUserId;
                findOne.Province = shop.Province;
                findOne.ShopShortName = shop.ShopShortName;
                findOne.UseChk = shop.UseChk;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectDto> GetSubject(string projectId, string subjectId)
        {
            projectId = projectId == null ? "" : projectId;
            subjectId = subjectId == null ? "" : subjectId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(SubjectDto);
            string sql = "";
            sql = @"SELECT A.SubjectId,A.[SubjectCode],A.[ProjectId],A.[SubjectTypeExamId],B.SubjectTypeExamName,A.[SubjectRecheckTypeId]
                          ,A.[SubjectLinkId],C.SubjectLinkName,A.[OrderNO],A.[Implementation],A.[CheckPoint],A.[Desc]
                          ,A.[AdditionalDesc],A.[InspectionDesc],A.InUserId,A.InDateTime,
                          A.ModifyUserId,A.ModifyDateTime
                    FROM Subject A INNER JOIN SubjectTypeExam B ON A.SubjectTypeExamId = B.SubjectTypeExamId
				                   INNER JOIN SubjectLink C ON A.SubjectLinkId= C.SubjectLinkId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId = " + subjectId;
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
            List<UserInfo> userList = accountService.GetUserInfo(subject.ModifyUserId.ToString());
            if (userList == null || userList.Count == 0)
            {
                throw new Exception("没有找到对应的用户");
            }
            string accountId = userList[0].AccountId;
            string brandId = GetBrand("1", subject.ModifyUserId.ToString(),userList[0].RoleType, "")[0].BrandId.ToString();
            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }
            //webService.SaveSubject("U",GetProject("1","",subject.ProjectId.ToString())[0].ProjectCode,subject.SubjectCode,subject.Implementation,subject.CheckPoint,subject.Desc,subject.AdditionalDesc,subject.InspectionDesc
            //    ,subject.Remark,subject.OrderNO,"",null,true,GetSubjectType())
            Subject findOne = db.Subject.Where(x => (x.SubjectId == subject.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                subject.InDateTime = DateTime.Now;
                subject.ModifyDateTime = DateTime.Now;
                db.Subject.Add(subject);
            }
            else
            {
                findOne.AdditionalDesc = subject.AdditionalDesc;
                findOne.CheckPoint = subject.CheckPoint;
                findOne.Desc = subject.Desc;
                findOne.Implementation = subject.Implementation;
                findOne.InspectionDesc = subject.InspectionDesc;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subject.ModifyUserId;
                findOne.OrderNO = subject.OrderNO;
                findOne.Remark = subject.Remark;
                findOne.SubjectCode = subject.SubjectCode;
                findOne.SubjectRecheckTypeId = subject.SubjectRecheckTypeId;
                findOne.SubjectTypeExamId = subject.SubjectTypeExamId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 批量更新SubjectLinkId
        /// </summary>
        /// <param name="subjectIdList"></param>
        /// <param name="subjectLinkId"></param>
        public void SetSubjectLinkId(List<SubjectDto> subjectList)
        {
            Type t = typeof(int);
            string sql = "";
            foreach (SubjectDto subject in subjectList)
            {
                sql += " UPDATE Subject SET SubjectLinkId =" + subject.SubjectLinkId.ToString() + " WHERE SubjectId = " + subject.SubjectId.ToString() + " ";
            }

            db.Database.SqlQuery(t, sql, null).Cast<int>().ToList();
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
            sql = @"SELECT sf.FileId,sf.SubjectId,sf.SeqNO,sf.FileName,sf.FileType,sf.InUserId,sf.InDateTime,sf.ModifyUserId,sf.ModifyDateTime
                   FROM SubjectFile sf,Subject s 
                   WHERE sf.SubjectId=s.SubjectId AND ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            List<SubjectFile> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectFile>().ToList();
            return list;
        }
        /// <summary>
        /// 保存标准照片
        /// </summary>
        /// <param name="subjectFile"></param>
        public void SaveSubjectFile(SubjectFile subjectFile)
        {
            SubjectFile findOne = db.SubjectFile.Where(x => (x.FileId == subjectFile.FileId)).FirstOrDefault();
            if (findOne == null)
            {
                subjectFile.InDateTime = DateTime.Now;
                subjectFile.ModifyDateTime = DateTime.Now;
                db.SubjectFile.Add(subjectFile);
            }
            else
            {
                findOne.FileName = subjectFile.FileName;
                findOne.FileType = subjectFile.FileType;
                findOne.SubjectId = subjectFile.SubjectId;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subjectFile.ModifyUserId;
                findOne.SeqNO = subjectFile.SeqNO;
            }
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
            sql = @"SELECT sis.InspectionStandardId,sis.InspectionStandardName,sis.SubjectId,sis.SeqNO,sis.InUserId,sis.InDateTime,sis.ModifyUserId,sis.ModifyDateTime" +
                  " FROM SubjectInspectionStandard sis,Subject s WHERE sis.SubjectId=s.SubjectId and ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            List<SubjectInspectionStandard> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectInspectionStandard>().ToList();
            return list;
        }
        /// <summary>
        /// 保存检查标准
        /// </summary>
        /// <param name="SubjectInspectionStandard"></param>
        public void SaveSubjectInspectionStandard(SubjectInspectionStandard subjectInspectionStandard)
        {
            SubjectInspectionStandard findOne = db.SubjectInspectionStandard.Where(x => (x.InspectionStandardId == subjectInspectionStandard.InspectionStandardId)).FirstOrDefault();
            if (findOne == null)
            {
                subjectInspectionStandard.InDateTime = DateTime.Now;
                subjectInspectionStandard.ModifyDateTime = DateTime.Now;
                db.SubjectInspectionStandard.Add(subjectInspectionStandard);
            }
            else
            {
                findOne.InspectionStandardName = subjectInspectionStandard.InspectionStandardName;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subjectInspectionStandard.ModifyUserId;
                findOne.SeqNO = subjectInspectionStandard.SeqNO;
                findOne.SubjectId = subjectInspectionStandard.SubjectId;
            }
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
            sql = @"SELECT slr.LossResultId,slr.SubjectId,slr.SeqNO,slr.LossResultName,slr.InUserId,slr.InDateTime,slr.ModifyUserId,slr.ModifyDateTime" +
                  " FROM SubjectLossResult slr,Subject s WHERE slr.SubjectId=s.SubjectId and s.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            List<SubjectLossResult> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectLossResult>().ToList();
            return list;
        }
        /// <summary>
        /// 保存失分说明
        /// </summary>
        /// <param name="subjectLossResult"></param>
        public void SaveSubjectLossResult(SubjectLossResult subjectLossResult)
        {
            SubjectLossResult findOne = db.SubjectLossResult.Where(x => (x.LossResultId == subjectLossResult.LossResultId)).FirstOrDefault();
            if (findOne == null)
            {
                subjectLossResult.InDateTime = DateTime.Now;
                subjectLossResult.ModifyDateTime = DateTime.Now;
                db.SubjectLossResult.Add(subjectLossResult);
            }
            else
            {
                findOne.LossResultName = subjectLossResult.LossResultName;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subjectLossResult.ModifyUserId;
                findOne.SeqNO = subjectLossResult.SeqNO;
                findOne.SubjectId = subjectLossResult.SubjectId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取体系类型打分范围信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<SubjectTypeScoreRegion> GetSubjectTypeScoreRegion(string projectId, string subjectId, string subjectTypeId)
        {
            projectId = projectId == null ? "" : projectId;
            subjectId = subjectId == null ? "" : subjectId;
            subjectTypeId = subjectTypeId == null ? "" : subjectTypeId;
            //CommonHelper.log(projectId + " " + subjectId+" " + subjectTypeId);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@SubjectId", subjectId)
                                                        , new SqlParameter("@SubjectTypeId", subjectTypeId) };
            Type t = typeof(SubjectTypeScoreRegion);
            string sql = "";
            sql = @"SELECT str.Id,str.SubjectId,str.SubjectTypeId,str.LowestScore,str.FullScore,str.InUserId,str.InDateTime,str.ModifyUserId,str.ModifyDateTime" +
                  " FROM SubjectTypeScoreRegion str,Subject s  WHERE str.SubjectId=s.SubjectId";

            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND S.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            if (!string.IsNullOrEmpty(subjectTypeId))
            {
                sql += " AND str.SubjectTypeId = @SubjectTypeId";
            }
            List<SubjectTypeScoreRegion> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectTypeScoreRegion>().ToList();
            return list;
        }
        public List<SubjectTypeScoreRegionDto> GetSubjectTypeScoreRegionDto(string projectId, string subjectId, string subjectTypeId)
        {
            projectId = projectId == null ? "" : projectId;
            subjectId = subjectId == null ? "" : subjectId;
            subjectTypeId = subjectTypeId == null ? "" : subjectTypeId;
            //CommonHelper.log(projectId + " " + subjectId+" " + subjectTypeId);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@SubjectId", subjectId)
                                                        , new SqlParameter("@SubjectTypeId", subjectTypeId) };
            Type t = typeof(SubjectTypeScoreRegionDto);
            string sql = "";
            sql = @"SELECT str.Id,str.SubjectId,str.SubjectTypeId,st.SubjectTypeName,str.LowestScore,str.FullScore,str.InUserId,str.InDateTime,str.ModifyUserId,str.ModifyDateTime" +
                  " FROM SubjectTypeScoreRegion str,Subject s,SubjectType st  WHERE str.SubjectId=s.SubjectId  and st.SubjectTypeId = str.SubjectTypeId";

            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND S.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = @SubjectId";
            }
            if (!string.IsNullOrEmpty(subjectTypeId))
            {
                sql += " AND str.SubjectTypeId = @SubjectTypeId";
            }
            List<SubjectTypeScoreRegionDto> list = db.Database.SqlQuery(t, sql, para).Cast<SubjectTypeScoreRegionDto>().ToList();
            return list;
        }
        public void SaveSubjectTypeScoreRegion(SubjectTypeScoreRegion subjectTypeScoreRegion)
        {
            SubjectTypeScoreRegion findOne = db.SubjectTypeScoreRegion.Where(x => (x.Id == subjectTypeScoreRegion.Id)).FirstOrDefault();
            if (findOne == null)
            {
                subjectTypeScoreRegion.InDateTime = DateTime.Now;
                subjectTypeScoreRegion.ModifyDateTime = DateTime.Now;
                db.SubjectTypeScoreRegion.Add(subjectTypeScoreRegion);
            }
            else
            {
                findOne.FullScore = subjectTypeScoreRegion.FullScore;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = subjectTypeScoreRegion.ModifyUserId;
                findOne.SubjectId = subjectTypeScoreRegion.SubjectId;
                findOne.LowestScore = subjectTypeScoreRegion.LowestScore;
                findOne.SubjectId = subjectTypeScoreRegion.SubjectId;
                findOne.SubjectTypeId = subjectTypeScoreRegion.SubjectTypeId;
            }
            db.SaveChanges();
        }
    }
}