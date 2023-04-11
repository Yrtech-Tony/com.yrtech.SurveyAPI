using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.SurveyAPI.DTO;
using System.Threading;
using com.yrtech.SurveyDAL;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using com.yrtech.SurveyAPI.DTO.Master;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class MasterController : ApiController
    {
        MasterService masterService = new MasterService();
        ShopService shopService = new ShopService();
        ExcelDataService excelDataService = new ExcelDataService();
        PhotoService photoService = new PhotoService();
        #region 租户管理
        [HttpGet]
        [Route("Master/GetTenant")]
        public APIResult GetTenant(string tenantId, string tenantCode, string tenantName)
        {
            try
            {
                List<Tenant> tenantList = masterService.GetTenant("", tenantCode, "");
                if (tenantList == null || tenantList.Count == 0)
                {
                    return new APIResult() { Status = false, Body = "租户代码不存在" };
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(tenantList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 权限类型
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetRoleType")]
        public APIResult GetRoleType(string type, string roleTypeCode)
        {
            try
            {
                List<RoleType> roleTypeListReturn = new List<RoleType>();
                List<RoleType> roleTypeList = masterService.GetRoleType(type, "", "");
                if (roleTypeCode == "S_Sysadmin")
                { roleTypeListReturn = roleTypeList; }
                // 品牌管理员不能设置租户和品牌管理员的账号
                else if (roleTypeCode == "S_BrandSysadmin")
                {
                    foreach (RoleType roleType in roleTypeList)
                    {
                        if (roleType.RoleTypeCode != "S_Sysadmin" && roleType.RoleTypeCode != "S_BrandSysadmin")
                        {
                            roleTypeListReturn.Add(roleType);
                        }
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(roleTypeListReturn) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region HiddenCode
        [HttpGet]
        [Route("Master/GetHiddenCode")]
        public APIResult GetHiddenCode(string hiddenCodeGroup, string hiddenCode)
        {
            try
            {
                List<HiddenColumn> hiddenCodeList = masterService.GetHiddenCode(hiddenCodeGroup, hiddenCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(hiddenCodeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetHiddenCodeSubjectTye")]
        public APIResult GetHiddenCodeSubjectTye(string hiddenCodeGroup, string hiddenCode)
        {
            try
            {
                List<HiddenColumn> hiddenCodeList = masterService.GetHiddenCode(hiddenCodeGroup, hiddenCode);
                List<HiddenColumDto> hiddenColumDtoList = new List<HiddenColumDto>();
                foreach (HiddenColumn hiddenColumn in hiddenCodeList)
                {
                    HiddenColumDto hiddenColumDto = new HiddenColumDto();
                    hiddenColumDto.HiddenCode_SubjectType = hiddenColumn.HiddenCode;
                    hiddenColumDto.HiddenName = hiddenColumn.HiddenName;
                    hiddenColumDtoList.Add(hiddenColumDto);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(hiddenColumDtoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 品牌
        /// <summary>
        /// 根据租户信息查询品牌信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetBrand")]
        public APIResult GetBrand(string tenantId, string brandId, string brandCode)
        {
            try
            {
                List<Brand> brandList = masterService.GetBrand(tenantId, brandId, brandCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(brandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveBrand")]
        public APIResult SaveBrand(Brand brand)
        {
            try
            {
                if (string.IsNullOrEmpty(brand.BrandCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "品牌代码不能为空" };
                }
                if (string.IsNullOrEmpty(brand.BrandName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "品牌名称不能为空" };
                }
                List<Brand> brandList = masterService.GetBrand(brand.TenantId.ToString(), "", brand.BrandCode);
                if (brandList != null && brandList.Count > 0 && brandList[0].BrandId != brand.BrandId)
                {
                    return new APIResult() { Status = false, Body = "品牌代码重复" };
                }
                masterService.SaveBrand(brand);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 账号信息管理
        #region 账号管理
        [HttpGet]
        [Route("Master/GetUserInfo")]
        public APIResult GetUserInfo(string tenantId, string brandId, string userId, string accountId, string accountName, string roleTypeCode, string telNO, string email, bool? useChk = true)
        {
            try
            {
                List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId, brandId, userId, accountId, accountName, roleTypeCode, telNO, email, useChk);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveUserInfo")]
        public APIResult SaveUserInfo(UserInfo userInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(userInfo.AccountId.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "账号不能为空" };
                }
                if (string.IsNullOrEmpty(userInfo.AccountName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "姓名不能为空" };
                }
                if (string.IsNullOrEmpty(userInfo.RoleType.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "权限不能为空" };
                }
                List<UserInfo> userInfoList = masterService.GetUserInfo(userInfo.TenantId.ToString(), "", "", userInfo.AccountId, "", "", "", "",null);
                if (userInfoList != null && userInfoList.Count > 0 && userInfoList[0].Id != userInfo.Id)
                {
                    return new APIResult() { Status = false, Body = "账号重复" };
                }
                masterService.SaveUserInfo(userInfo);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/UserInfoImport")]
        public APIResult UserInfoImport(UploadData uploadData)
        {
            try
            {
                List<UserInfoDto> list = CommonHelper.DecodeString<List<UserInfoDto>>(uploadData.ListJson);
                foreach (UserInfoDto userInfoDto in list)
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.AccountId = userInfoDto.AccountId;
                    List<UserInfo> userInfoList = masterService.GetUserInfo(userInfoDto.TenantId.ToString(), "", "", userInfoDto.AccountId, "", "", "", "",null);
                    if (userInfoList != null && userInfoList.Count > 0)
                    {
                        userInfo.Id = userInfoList[0].Id;
                    }
                    userInfo.AccountName = userInfoDto.AccountName;
                    userInfo.BrandId = userInfoDto.BrandId;
                    userInfo.Email = userInfoDto.Email;
                    userInfo.InUserId = userInfoDto.InUserId;
                    userInfo.ModifyUserId = userInfoDto.ModifyUserId;
                    userInfo.Password = userInfoDto.Password;

                    userInfo.RoleType = masterService.GetRoleType("", "", userInfoDto.RoleTypeName)[0].RoleTypeCode;
                    userInfo.TelNO = userInfoDto.TelNO;
                    userInfo.TenantId = userInfoDto.TenantId;
                    userInfo.UseChk = userInfoDto.UseChk;
                    masterService.SaveUserInfo(userInfo);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 账号关联品牌
        [HttpGet]
        [Route("Master/GetUserInfoBrand")]
        public APIResult GetUserInfoBrand(string tenantId, string brandId, string userId)
        {
            try
            {
                List<UserInfoBrandDto> userInfoBrandList = masterService.GetUserInfoBrand(tenantId, userId, brandId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoBrandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveUserInfoBrand")]
        public APIResult SaveUserInfoBrand(UserInfoBrand userInfoBrand)
        {
            try
            {
                masterService.SaveUserInfoBrand(userInfoBrand);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteUserInfoBrand")]
        public APIResult DeleteUserInfoBrand(UserInfoBrand userInfoBrand)
        {
            try
            {
                masterService.DeleteUserInfoBrand(userInfoBrand.Id);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 厂商账号设置所属
        [HttpGet]
        [Route("Master/GetUserInfoObject")]
        public APIResult GetUserInfoObject(string tenantId, string userId, string objectId, string roleTypeCode)
        {
            try
            {
                List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject(tenantId, userId, objectId, roleTypeCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoObjectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveUserInfoObject")]
        public APIResult SaveUserInfoObject(UserInfoObject userInfoObject)
        {
            try
            {
                masterService.SaveUserInfoObject(userInfoObject);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/UserInfoExcelAnalysis")]
        public APIResult UserInfoExcelAnalysis(string tenantId, string brandId, string ossPath)
        {
            try
            {
                List<UserInfoDto> list = excelDataService.UserInfoImport(ossPath);
                foreach (UserInfoDto userInfoDto in list)
                {
                    userInfoDto.ImportChk = true;
                    userInfoDto.ImportRemark = "";
                    List<RoleType> roleTypeList = masterService.GetRoleType("", "", userInfoDto.RoleTypeName);
                    if (roleTypeList == null || roleTypeList.Count == 0)
                    {
                        userInfoDto.ImportChk = false;
                        userInfoDto.ImportRemark += "权限类型不存在" + ";";
                    }
                    foreach (UserInfoDto userInfoDto1 in list)
                    {
                        if (userInfoDto1 != userInfoDto && userInfoDto1.AccountId == userInfoDto.AccountId)
                        {
                            userInfoDto.ImportChk = false;
                            userInfoDto.ImportRemark += "表格中有重复的账号" + ";";
                        }
                    }
                }
                list = (from shop in list orderby shop.ImportChk select shop).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/UserInfoObjectExcelAnalysis")]
        public APIResult UserInfoObjectExcelAnalysis(string tenantId, string brandId, string ossPath)
        {
            try
            {
                List<UserInfoObjectDto> list = excelDataService.UserInfoObjectImport(ossPath);
                foreach (UserInfoObjectDto userInfoObjectDto in list)
                {
                    userInfoObjectDto.ImportChk = true;
                    userInfoObjectDto.ImportRemark = "";
                    List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId, brandId, "", userInfoObjectDto.AccountId, "", "", "", "",true);
                    if (userInfoList == null || userInfoList.Count == 0)
                    {
                        userInfoObjectDto.ImportChk = false;
                        userInfoObjectDto.ImportRemark += "账号未在系统登记或不可用" + ";";
                    }
                    else
                    {
                        string roleTypeCode = userInfoList[0].RoleType;
                        if (roleTypeCode == "B_Group")
                        {
                            List<Group> groupList = masterService.GetGroup(brandId, "", userInfoObjectDto.ObjectCode, "",true);
                            if (groupList == null || groupList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "集团代码不存在或不可用" + ";";
                            }
                        }
                        else if (roleTypeCode == "B_Bussiness")
                        {
                            List<AreaDto> areaList = masterService.GetArea("", brandId, userInfoObjectDto.ObjectCode, "", "Bussiness", "",null);
                            if (areaList == null || areaList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "业务类型代码不存在" + ";";
                            }
                        }
                        else if (roleTypeCode == "B_WideArea")
                        {
                            List<AreaDto> areaList = masterService.GetArea("", brandId, userInfoObjectDto.ObjectCode, "", "WideArea", "", null);
                            if (areaList == null || areaList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "广域区域代码不存在" + ";";
                            }
                        }
                        else if (roleTypeCode == "B_BigArea")
                        {
                            List<AreaDto> areaList = masterService.GetArea("", brandId, userInfoObjectDto.ObjectCode, "", "BigArea", "", null);
                            if (areaList == null || areaList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "大区代码不存在" + ";";
                            }
                        }
                        else if (roleTypeCode == "B_MiddleArea")
                        {
                            List<AreaDto> areaList = masterService.GetArea("", brandId, userInfoObjectDto.ObjectCode, "", "MiddleArea", "", null);
                            if (areaList == null || areaList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "中区代码不存在" + ";";
                            }
                        }
                        else if (roleTypeCode == "B_SmallArea")
                        {
                            List<AreaDto> areaList = masterService.GetArea("", brandId, userInfoObjectDto.ObjectCode, "", "SmallArea", "", null);
                            if (areaList == null || areaList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "小区代码不存在" + ";";
                            }
                        }
                        else if (roleTypeCode == "B_Shop")
                        {
                            List<ShopDto> shopList = masterService.GetShop(tenantId, brandId, "", userInfoObjectDto.ObjectCode, "",null);
                            if (shopList == null || shopList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "经销商代码不存在" + ";";
                            }
                        }

                    }
                }
                list = (from shop in list orderby shop.ImportChk select shop).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/UserInfoObjectImport")]
        public APIResult UserInfoObjectImport(UploadData uploadData)
        {
            try
            {
                List<UserInfoObjectDto> list = CommonHelper.DecodeString<List<UserInfoObjectDto>>(uploadData.ListJson);
                foreach (UserInfoObjectDto userInfoObjectDto in list)
                {
                    UserInfoObject userInfo = new UserInfoObject();
                    List<UserInfo> userInfoList = masterService.GetUserInfo(userInfoObjectDto.TenantId.ToString(), userInfoObjectDto.brandId, "", userInfoObjectDto.AccountId, "", "", "", "",null);
                    if (userInfoList != null && userInfoList.Count > 0)
                    {
                        userInfo.UserId = userInfoList[0].Id;
                        string roleTypeCode = userInfoList[0].RoleType;
                        if (roleTypeCode == "B_Group")
                        {
                            userInfo.ObjectId = masterService.GetGroup(userInfoObjectDto.brandId, "", userInfoObjectDto.ObjectCode, "",true)[0].GroupId;
                        }
                        else if (roleTypeCode == "B_Bussiness")
                        {
                            userInfo.ObjectId = masterService.GetArea("", userInfoObjectDto.brandId, userInfoObjectDto.ObjectCode, "", "Bussiness", "", null)[0].AreaId;

                        }
                        else if (roleTypeCode == "B_WideArea")
                        {
                            userInfo.ObjectId = masterService.GetArea("", userInfoObjectDto.brandId, userInfoObjectDto.ObjectCode, "", "WideArea", "", null)[0].AreaId;
                        }
                        else if (roleTypeCode == "B_BigArea")
                        {
                            userInfo.ObjectId = masterService.GetArea("", userInfoObjectDto.brandId, userInfoObjectDto.ObjectCode, "", "BigArea", "", null)[0].AreaId;

                        }
                        else if (roleTypeCode == "B_MiddleArea")
                        {
                            userInfo.ObjectId = masterService.GetArea("", userInfoObjectDto.brandId, userInfoObjectDto.ObjectCode, "", "MiddleArea", "", null)[0].AreaId;

                        }
                        else if (roleTypeCode == "B_SmallArea")
                        {
                            userInfo.ObjectId = masterService.GetArea("", userInfoObjectDto.brandId, userInfoObjectDto.ObjectCode, "", "SmallArea", "", null)[0].AreaId;

                        }
                        else if (roleTypeCode == "B_Shop")
                        {
                            userInfo.ObjectId = masterService.GetShop(userInfoObjectDto.TenantId.ToString(), userInfoObjectDto.brandId, "", userInfoObjectDto.ObjectCode, "",null)[0].ShopId;
                        }
                        userInfo.InUserId = userInfoObjectDto.InUserId;
                    }
                    masterService.SaveUserInfoObject(userInfo);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/UserInfoExport")]
        public APIResult UserInfoExport(string tenantId, string brandId)
        {
            try
            {
                string downloadPath = excelDataService.UserInfoExport(tenantId, brandId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 执行人员管理经销商
        [HttpGet]
        [Route("Master/UserInfoObjectExcuteShopExcelAnalysis")]
        public APIResult UserInfoObjectExcuteShopExcelAnalysis(string tenantId, string brandId, string ossPath)
        {
            try
            {
                List<UserInfoObjectDto> list = excelDataService.UserInfoObjectImport(ossPath);
                foreach (UserInfoObjectDto userInfoObjectDto in list)
                {
                    userInfoObjectDto.ImportChk = true;
                    userInfoObjectDto.ImportRemark = "";
                    List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId, "", "", userInfoObjectDto.AccountId, "", "", "", "",null);
                    if (userInfoList == null || userInfoList.Count == 0)
                    {
                        userInfoObjectDto.ImportChk = false;
                        userInfoObjectDto.ImportRemark += "账号未在系统登记" + ";";
                    }
                    else
                    {
                        string roleTypeCode = userInfoList[0].RoleType;
                        if (roleTypeCode != "S_Execute")
                        {
                            userInfoObjectDto.ImportChk = false;
                            userInfoObjectDto.ImportRemark += "该账号非执行人员账号" + ";";
                        }
                    }
                    List<ShopDto> shopList = masterService.GetShop(tenantId, brandId, "", userInfoObjectDto.ObjectCode, "",null);
                    if (shopList == null || shopList.Count == 0)
                    {
                        userInfoObjectDto.ImportChk = false;
                        userInfoObjectDto.ImportRemark += "经销商代码不存在" + ";";
                    }
                }
                list = (from shop in list orderby shop.ImportChk select shop).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/UserInfoObjectExcuteShopImport")]
        public APIResult UserInfoObjectExcuteShopImport(UploadData uploadData)
        {
            try
            {
                List<UserInfoObjectDto> list = CommonHelper.DecodeString<List<UserInfoObjectDto>>(uploadData.ListJson);
                foreach (UserInfoObjectDto userInfoObjectDto in list)
                {
                    UserInfoObject userInfo = new UserInfoObject();
                    List<UserInfo> userInfoList = masterService.GetUserInfo(userInfoObjectDto.TenantId.ToString(), userInfoObjectDto.brandId, "", userInfoObjectDto.AccountId, "", "", "", "",null);
                    if (userInfoList != null && userInfoList.Count > 0)
                    {
                        userInfo.UserId = userInfoList[0].Id;
                        userInfo.ObjectId = masterService.GetShop(userInfoObjectDto.TenantId.ToString(), userInfoObjectDto.brandId, "", userInfoObjectDto.ObjectCode, "",null)[0].ShopId;
                        userInfo.InUserId = userInfoObjectDto.InUserId;
                    }
                    masterService.SaveUserInfoObject(userInfo);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 删除厂商设置所属及执行关联经销商
        [HttpPost]
        [Route("Master/DeleteUserInfoObject")]
        public APIResult DeleteUserInfoObject(UserInfoObject userInfoObject)
        {
            try
            {
                masterService.DeleteUserInfoObject(userInfoObject.Id);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #endregion
        #region 区域管理
        [HttpGet]
        [Route("Master/GetArea")]
        public APIResult GetArea(string brandId, string areaId, string areaCode, string areaName, string areaType, string parentId, bool? useChk)
        {
            try
            {
                List<AreaDto> areaList = masterService.GetArea(areaId, brandId, areaCode, areaName, areaType, parentId, useChk);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(areaList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveArea")]
        public APIResult SaveArea(Area area)
        {
            try
            {
                if (string.IsNullOrEmpty(area.AreaCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "区域代码不能为空" };
                }
                if (string.IsNullOrEmpty(area.AreaName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "区域名称不能为空" };
                }
                List<AreaDto> areaList = masterService.GetArea("", area.BrandId.ToString(), area.AreaCode, "", "", "", null);
                if (areaList != null && areaList.Count > 0 && areaList[0].AreaId != area.AreaId)
                {
                    return new APIResult() { Status = false, Body = "区域代码重复" };
                }
                masterService.SaveArea(area);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/AreaExcelAnalysis")]
        public APIResult AreaExcelAnalysis(string ossPath)
        {
            try
            {
                List<AreaDto> list = excelDataService.AreaImport(ossPath);
                foreach (AreaDto area in list)
                {
                    area.ImportChk = true;
                    area.ImportRemark = "";
                    // 验证表格中的区域类型是否正确
                    if (area.AreaType != "Bussiness" &&
                       area.AreaType != "WideArea" &&
                       area.AreaType != "BigArea" &&
                       area.AreaType != "MiddleArea" &&
                       area.AreaType != "SmallArea")
                    {
                        area.ImportChk = false;
                        area.ImportRemark += "区域类型填写错误" + ";";
                    }
                    bool parentAreaCodeChk_DB = true;// 数据库中是否有对应的上级区域代码
                    bool parentAreaCodeChk_Excel = false;//Excel中是否有对应的上级区域代码
                    if (area.AreaType != "Bussiness")// 业务类型不验证上级区域
                    {
                        string parentAreaType = "";
                        if (area.AreaType == "WideArea") { parentAreaType = "Bussiness"; }
                        else if (area.AreaType == "BigArea") { parentAreaType = "WideArea"; }
                        else if (area.AreaType == "MiddleArea") { parentAreaType = "BigArea"; }
                        else if (area.AreaType == "SmallArea") { parentAreaType = "MiddleArea"; }
                        List<AreaDto> areaList = masterService.GetArea("", area.BrandId.ToString(), area.ParentCode, "", parentAreaType, "", true);
                        if (areaList == null || areaList.Count == 0)
                        {
                            parentAreaCodeChk_DB = false;
                        }
                    }
                    foreach (AreaDto area1 in list)
                    {
                        // 验证表格中是否有重复的区域代码
                        if (area != area1 && area.AreaCode == area1.AreaCode)
                        {
                            area.ImportChk = false;
                            area.ImportRemark += "表格中有重复的区域代码" + ";";
                        }
                        // 验证上级区域代码在表格中是否存在，如果表格中和DB中都不存在提示错误
                        if (area.AreaType != "Bussiness" && area != area1 && area.ParentCode == area1.AreaCode)
                        {
                            parentAreaCodeChk_Excel = true;
                        }
                    }
                    if (!parentAreaCodeChk_DB && !parentAreaCodeChk_Excel)
                    {
                        area.ImportChk = false;
                        area.ImportRemark += "上级区域代码在系统和表格中均不存在或不可用" + ";";
                    }

                }
                list = (from area in list orderby area.ImportChk select area).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/AreaImport")]
        public APIResult AreaImport(UploadData uploadData)
        {
            try
            {
                List<AreaDto> list = CommonHelper.DecodeString<List<AreaDto>>(uploadData.ListJson);
                //验证Excel是否有重复的区域代码
                foreach (AreaDto area in list)
                {
                    if (area.AreaType != "Bussiness" &&
                        area.AreaType != "WideArea" &&
                        area.AreaType != "BigArea" &&
                        area.AreaType != "MiddleArea" &&
                        area.AreaType != "SmallArea")
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中有错误的区域类型，请检查文件" };
                    }
                    foreach (AreaDto area1 in list)
                    {
                        if (area != area1 && area.AreaCode == area1.AreaCode)
                        {
                            return new APIResult() { Status = false, Body = "导入失败,文件中存在重复的区域代码，请检查文件" };
                        }
                    }
                }
                // 先保存区域的信息
                foreach (AreaDto areaDto in list)
                {
                    Area area = new Area();
                    area.BrandId = areaDto.BrandId;
                    List<AreaDto> areaList = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.AreaCode, "", areaDto.AreaType, "", null);
                    if (areaList != null && areaList.Count > 0)
                    {
                        area.AreaId = areaList[0].AreaId;
                    }
                    area.AreaCode = areaDto.AreaCode;
                    area.AreaName = areaDto.AreaName;
                    area.AreaType = areaDto.AreaType;
                    area.InUserId = areaDto.InUserId;
                    area.ModifyUserId = areaDto.ModifyUserId;
                    area.UseChk = areaDto.UseChk;
                    masterService.SaveArea(area);
                }
                // 更新上级区域之前先验证上级区域是否存在
                foreach (AreaDto areaDto in list)
                {
                    if (areaDto.AreaType != "Bussiness")// 业务类型不验证上级区域
                    {
                        string parentAreaType = "";
                        if (areaDto.AreaType == "WideArea") { parentAreaType = "Bussiness"; }
                        else if (areaDto.AreaType == "BigArea") { parentAreaType = "WideArea"; }
                        else if (areaDto.AreaType == "MiddleArea") { parentAreaType = "BigArea"; }
                        else if (areaDto.AreaType == "SmallArea") { parentAreaType = "MiddleArea"; }
                        List<AreaDto> areaList = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.ParentCode, "", parentAreaType, "", true);
                        if (areaList == null || areaList.Count == 0)
                        {
                            return new APIResult() { Status = false, Body = "区域导入成功，但上级区域导入失败,请确认上级区域代码" };
                        }
                    }
                }
                //更新区域的上级区域
                foreach (AreaDto areaDto in list)
                {
                    if (areaDto.AreaType != "Bussiness") // 业务类型不需要更新上级区域
                    {
                        Area area = new Area();
                        List<AreaDto> areaList = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.AreaCode, "", areaDto.AreaType, "", true);
                        if (areaList != null && areaList.Count > 0)
                        {
                            area.AreaId = areaList[0].AreaId;
                        }
                        area.BrandId = areaDto.BrandId;
                        area.AreaCode = areaDto.AreaCode;
                        area.AreaName = areaDto.AreaName;
                        area.AreaType = areaDto.AreaType;
                        string parentAreaType = "";
                        if (areaDto.AreaType == "WideArea") { parentAreaType = "Bussiness"; }
                        else if (areaDto.AreaType == "BigArea") { parentAreaType = "WideArea"; }
                        else if (areaDto.AreaType == "MiddleArea") { parentAreaType = "BigArea"; }
                        else if (areaDto.AreaType == "SmallArea") { parentAreaType = "MiddleArea"; }
                        List<AreaDto> areaList_Parent = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.ParentCode, "", parentAreaType, "", true);
                        if (areaList_Parent != null && areaList_Parent.Count > 0)
                        {
                            area.ParentId = areaList_Parent[0].AreaId;
                        }
                        area.InUserId = areaDto.InUserId;
                        area.ModifyUserId = areaDto.ModifyUserId;
                        area.UseChk = areaDto.UseChk;

                        masterService.SaveArea(area);
                    }
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 经销商
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetShop")]
        public APIResult GetShop(string brandId, string shopId, string shopCode, string key,bool? useChk=true)
        {
            try
            {
                List<ShopDto> shopList = masterService.GetShop("", brandId, shopId, shopCode, key, useChk);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(shopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveShop")]
        public APIResult SaveShop(Shop shop)
        {
            try
            {
                if (string.IsNullOrEmpty(shop.ShopCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "经销商代码不能为空" };
                }
                if (string.IsNullOrEmpty(shop.ShopName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "经销商名称不能为空" };
                }
                List<ShopDto> shopList = masterService.GetShop("", shop.BrandId.ToString(), "", shop.ShopCode, "",null);
                if (shopList != null && shopList.Count > 0 && shopList[0].ShopId != shop.ShopId)
                {
                    return new APIResult() { Status = false, Body = "经销商代码重复" };
                }
                masterService.SaveShop(shop);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取当前期需要执行的经销商及所属卷别类型
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetShopByProjectId")]
        public APIResult GetShopByProjectId(string projectId)
        {
            try
            {
                //    List<ShopDto> projectShopList = new List<ShopDto>();
                //    List<Shop> shopList = shopService.GetShopByProjectId(projectId);
                //    List<ShopSubjectTypeExamDto> subjectTypeExamList = shopService.GetShopSubjectTypeExam(projectId, "");
                //    foreach (Shop shop in shopList)
                //    {
                //        ShopDto shopDto = new ShopDto();
                //        shopDto.ProjectId = Convert.ToInt32(projectId);
                //        shopDto.ShopId = shop.ShopId;
                //        shopDto.ShopCode = shop.ShopCode;
                //        shopDto.ShopName = shop.ShopName;
                //        shopDto.Province = shop.Province;
                //        shopDto.City = shop.City;
                //        shopDto.ShopShortName = shop.ShopShortName;

                //        ShopSubjectTypeExamDto exam = subjectTypeExamList.Where(x => x.ShopId == shop.ShopId).FirstOrDefault();
                //        if (exam != null)
                //        {
                //            shopDto.SubjectTypeExamId = Convert.ToInt32(exam.ShopSubjectTypeExamId);
                //            shopDto.SubjectTypeExamName = exam.SubjectTypeExamName;
                //        }

                //        projectShopList.Add(shopDto);
                //}
                return new APIResult() { Status = true, Body = CommonHelper.Encode("") };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/ShopExcelAnalysis")]
        public APIResult ShopExcelAnalysist(string brandId, string ossPath)
        {
            try
            {
                List<ShopDto> list = excelDataService.ShopImport(ossPath);
                //验证Excel是否有重复的经销商代码
                foreach (ShopDto shop in list)
                {
                    shop.ImportChk = true;
                    shop.ImportRemark = "";
                    if (IsChineseLetter(shop.ShopCode))
                    {
                        shop.ImportChk = false;
                        shop.ImportRemark += "经销商代码只能为字母或数字" + ";";
                    }
                    foreach (ShopDto shop1 in list)
                    {
                        if (shop != shop1 && shop.ShopCode == shop1.ShopCode)
                        {
                            shop.ImportChk = false;
                            shop.ImportRemark += "表格中存在重复的经销商代码" + ";";
                        }
                    }
                    // 验证Excel中的集团信息是否已经登记
                    if (!string.IsNullOrEmpty(shop.GroupCode.Trim()))
                    {
                        List<Group> groupList = masterService.GetGroup(brandId, "", shop.GroupCode, "",true);
                        if (groupList == null || groupList.Count == 0)
                        {
                            shop.ImportChk = false;
                            shop.ImportRemark += "集团代码在系统中不存在或不可用" + ";";
                        }
                    }
                }
                list = (from shop in list orderby shop.ImportChk select shop).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        protected bool IsChineseLetter(string input)
        {
            bool isChinese = true;
            for (int i = 0; i < input.Length; i++)
            {
                int code = 0;
                int chfrom = Convert.ToInt32("4e00", 16); //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
                int chend = Convert.ToInt32("9fff", 16);
                if (input != "")
                {
                    code = Char.ConvertToUtf32(input, i); //获得字符串input中指定索引index处字符unicode编码

                    if (code >= chfrom && code <= chend)
                    {
                        isChinese = true; ; //当code在中文范围内返回true

                    }
                    else
                    {
                        isChinese = false; //当code不在中文范围内返回false
                    }
                }
            }
            return isChinese;
        }
        [HttpPost]
        [Route("Master/ShopImport")]
        public APIResult ShopImport(UploadData uploadData)
        {
            try
            {
                List<ShopDto> list = CommonHelper.DecodeString<List<ShopDto>>(uploadData.ListJson);
                //验证Excel是否有重复的经销商代码
                foreach (ShopDto shop in list)
                {
                    foreach (ShopDto shop1 in list)
                    {
                        if (shop != shop1 && shop.ShopCode == shop1.ShopCode)
                        {
                            return new APIResult() { Status = false, Body = "导入失败,文件中存在重复的经销商代码，请检查文件" };
                        }
                    }
                    // 验证Excel中的集团信息是否已经登记
                    if (!string.IsNullOrEmpty(shop.GroupCode.Trim()))
                    {
                        List<Group> groupList = masterService.GetGroup(shop.BrandId.ToString(), "", shop.GroupCode, "",true);
                        if (groupList == null || groupList.Count == 0)
                        {
                            return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记或不可用的集团代码，请检查文件" };
                        }
                    }
                }
                foreach (ShopDto shopDto in list)
                {
                    Shop shop = new Shop();
                    List<Group> groupList = new List<Group>();
                    if (string.IsNullOrEmpty(shopDto.GroupCode))
                    { groupList = masterService.GetGroup(shopDto.BrandId.ToString(), "", shopDto.GroupCode, "",true); }
                    if (groupList != null && groupList.Count > 0)
                    {
                        shop.GroupId = groupList[0].GroupId;
                    }
                    // 如果经销商代码在系统已经存在就进行更新操作，可以进行导入的批量更新
                    List<ShopDto> shopList = masterService.GetShop(shopDto.TenantId.ToString(), shopDto.BrandId.ToString(), "", shopDto.ShopCode, "",null);
                    if (shopList != null && shopList.Count > 0)
                    {
                        shop.ShopId = shopList[0].ShopId;
                    }
                    shop.BrandId = shopDto.BrandId;
                    shop.City = shopDto.City;
                    shop.Address = shopDto.Address;
                    shop.InUserId = shopDto.InUserId;
                    shop.ModifyUserId = shopDto.ModifyUserId;
                    shop.Province = shopDto.Province;
                    shop.ShopCode = shopDto.ShopCode;
                    shop.ShopName = shopDto.ShopName;
                    shop.ShopShortName = shopDto.ShopShortName;
                    shop.TenantId = shopDto.TenantId;
                    shop.UseChk = shopDto.UseChk;
                    masterService.SaveShop(shop);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteShop")]
        public APIResult DeleteShop(Shop shop)
        {
            try
            {
                masterService.DeleteShop(shop.ShopId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 区域经销商管理
        [HttpGet]
        [Route("Master/GetAreaShop")]
        public APIResult GetAreaShop(string tenantId, string brandId, string areaId, string shopId)
        {
            try
            {
                List<ShopDto> areaShopList = masterService.GetAreaShop(tenantId, brandId, shopId, areaId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(areaShopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveAreaShop")]
        public APIResult SaveAreaShop(AreaShop areaShop)
        {
            try
            {
                masterService.SaveAreaShop(areaShop);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/AreaShopExcelAnalysis")]
        public APIResult AreaShopExcelAnalysis(string tenantId, string brandId, string ossPath)
        {
            try
            {
                List<ShopDto> list = excelDataService.AreaShopImport(ossPath);
                foreach (ShopDto shopDto in list)
                {
                    shopDto.ImportChk = true;
                    shopDto.ImportRemark = "";
                    List<ShopDto> shopList = masterService.GetShop(tenantId, brandId, "", shopDto.ShopCode, "",true);
                    if (shopList == null || shopList.Count == 0)
                    {
                        shopDto.ImportChk = false;
                        shopDto.ImportRemark += "经销商代码在系统中不存在或不可用" + ";";
                    }
                    List<AreaDto> areaList = masterService.GetArea("", brandId, shopDto.AreaCode, "", "SmallArea", "", true);
                    if (areaList == null || areaList.Count == 0)
                    {
                        shopDto.ImportChk = false;
                        shopDto.ImportRemark += "小区代码在系统中不存在或不可用" + ";";
                    }
                }
                list = (from shop in list orderby shop.ImportChk select shop).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/AreaShopImport")]
        public APIResult AreaShopImport(UploadData uploadData)
        {
            try
            {
                List<ShopDto> list = CommonHelper.DecodeString<List<ShopDto>>(uploadData.ListJson);
                //验证Excel中的经销商代码和区域代码是否在系统存在
                foreach (ShopDto shopDto in list)
                {
                    List<ShopDto> shopList = masterService.GetShop(shopDto.TenantId.ToString(), shopDto.BrandId.ToString(), "", shopDto.ShopCode, "",true);
                    if (shopList == null || shopList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记或不可用的经销商代码，请检查文件" };
                    }
                    List<AreaDto> areaList = masterService.GetArea("", shopDto.BrandId.ToString(), shopDto.AreaCode, "", "SmallArea", "", true);
                    if (areaList == null || areaList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记或不可用的小区代码，请检查文件" };
                    }
                }
                foreach (ShopDto shopDto in list)
                {
                    AreaShop areashop = new AreaShop();
                    List<AreaDto> areaList = masterService.GetArea("", shopDto.BrandId.ToString(), shopDto.AreaCode, "", "SmallArea", "", null);
                    if (areaList != null && areaList.Count > 0)
                    {
                        areashop.AreaId = areaList[0].AreaId;
                    }
                    areashop.InUserId = shopDto.InUserId;
                    areashop.ModifyUserId = shopDto.ModifyUserId;
                    List<ShopDto> shopList = masterService.GetShop(shopDto.TenantId.ToString(), shopDto.BrandId.ToString(), "", shopDto.ShopCode, "",null);
                    if (shopList != null && shopList.Count > 0)
                    {
                        areashop.ShopId = shopList[0].ShopId;
                    }
                    masterService.SaveAreaShop(areashop);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/DeleteAreaShop")]
        public APIResult DeleteAreaShop(AreaShop areaShop)
        {
            try
            {
                masterService.DeleteAreaShop(areaShop.AreaShopId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 集团管理
        [HttpGet]
        [Route("Master/GetGroup")]
        public APIResult GetGroup(string brandId, string groupId, string groupCode, string groupName, bool? useChk = true)
        {
            try
            {
                List<Group> groupList = masterService.GetGroup(brandId, groupId, groupCode, groupName, useChk);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(groupList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveGroup")]
        public APIResult SaveGroup(Group group)
        {
            try
            {
                if (string.IsNullOrEmpty(group.GroupCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "集团代码不能为空" };
                }
                if (string.IsNullOrEmpty(group.GroupName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "集团名称不能为空" };
                }
                List<Group> groupList = masterService.GetGroup(group.BrandId.ToString(), "", group.GroupCode, "",null);
                if (groupList != null && groupList.Count > 0 && groupList[0].GroupId != group.GroupId)
                {
                    return new APIResult() { Status = false, Body = "集团代码重复" };
                }
                masterService.SaveGroup(group);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 期号


        /// <summary>
        /// 获取品牌下期号的信息，也可以获取单个期号的信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetProject")]
        public APIResult GetProject(string brandId, string projectId, string year)
        {
            try
            {
                List<ProjectDto> projectList = masterService.GetProject("", brandId, projectId, "", year, "");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
                // CommonHelper.log(ex.ToString());
            }

        }
        [HttpPost]
        [Route("Master/SaveProject")]
        public APIResult SaveProject(Project project)
        {
            try
            {
                if (string.IsNullOrEmpty(project.ProjectCode))
                {
                    return new APIResult() { Status = false, Body = "期号代码不能为空" };
                }
                if (string.IsNullOrEmpty(project.ProjectName))
                {
                    return new APIResult() { Status = false, Body = "期号名称不能为空" };
                }
                if (project.OrderNO == null || project.OrderNO == 0)
                {
                    return new APIResult() { Status = false, Body = "序号不能为空或者0" };
                }
                List<ProjectDto> projectList_ProjectCode = masterService.GetProject("", project.BrandId.ToString(), "", project.ProjectCode, "", "");
                if (projectList_ProjectCode != null && projectList_ProjectCode.Count > 0 && projectList_ProjectCode[0].ProjectId != project.ProjectId)
                {
                    return new APIResult() { Status = false, Body = "期号代码重复" };
                }
                List<ProjectDto> projectList_OrderNO = masterService.GetProject("", project.BrandId.ToString(), "", "", project.Year, project.OrderNO.ToString());
                if (projectList_OrderNO != null && projectList_OrderNO.Count > 0 && projectList_OrderNO[0].ProjectId != project.ProjectId)
                {
                    return new APIResult() { Status = false, Body = "序号重复" };
                }
                masterService.SaveProject(project);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 标签管理
        [HttpGet]
        [Route("Master/GetLabel")]
        public APIResult GetLabel(string brandId, string labelId, string labelType, bool? useChk)
        {
            try
            {
                List<Label> labelList = masterService.GetLabel(brandId, labelId, labelType, useChk, "");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(labelList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetLabelRecheckType")]
        public APIResult GetLabelRecheckType(string brandId, string labelId, string labelType, bool? useChk)
        {
            try
            {
                List<Label> labelList = masterService.GetLabel(brandId, labelId, labelType, useChk, "");
                List<LabelDto> lableDtoList = new List<LabelDto>();
                foreach (Label label in labelList)
                {
                    LabelDto labelDto = new LabelDto();
                    labelDto.LabelId_RecheckType = label.LabelId;
                    labelDto.LabelCode = label.LabelCode;
                    labelDto.LabelName = label.LabelName;
                    labelDto.LabelType = label.LabelType;
                    lableDtoList.Add(labelDto);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lableDtoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveLabel")]
        public APIResult SaveLabel(Label label)
        {
            try
            {
                if (string.IsNullOrEmpty(label.LabelCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "标签代码不能为空" };
                }
                if (string.IsNullOrEmpty(label.LabelName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "标签名称不能为空" };
                }
                List<Label> labelList = masterService.GetLabel(label.BrandId.ToString(), "", label.LabelType, null, label.LabelCode);
                if (labelList != null && labelList.Count > 0 && labelList[0].LabelId != label.LabelId)
                {
                    return new APIResult() { Status = false, Body = "标签代码重复" };
                }
                masterService.SaveLabel(label);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 体系

        /// <summary>
        /// 获取体系的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubject")]
        public APIResult GetSubject(string projectId, string subjectId, string subjectCode, string orderNO)
        {
            try
            {
                List<SubjectDto> subjectList = masterService.GetSubject(projectId, subjectId, subjectCode, orderNO);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(subjectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存体系信息
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubject")]
        public APIResult SaveSubject(Subject subject)
        {
            try
            {
                if (string.IsNullOrEmpty(subject.SubjectCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "题目代码不能为空" };
                }

                if (subject.OrderNO == null || subject.OrderNO == 0)
                {
                    return new APIResult() { Status = false, Body = "序号不能为空或者为0" };
                }
                //if (subject.LabelId == null || subject.LabelId == 0)
                //{
                //    return new APIResult() { Status = false, Body = "卷别类型不能为空" };
                //}
                if (subject.LabelId_RecheckType == null || subject.LabelId_RecheckType == 0)
                {
                    return new APIResult() { Status = false, Body = "复审类型不能为空" };
                }
                if (string.IsNullOrEmpty(subject.HiddenCode_SubjectType))
                {
                    return new APIResult() { Status = false, Body = "题目类型不能为空" };
                }
                List<SubjectDto> subjectList_SubjectCode = masterService.GetSubject(subject.ProjectId.ToString(), "", subject.SubjectCode, "");
                if (subjectList_SubjectCode != null && subjectList_SubjectCode.Count > 0 && subjectList_SubjectCode[0].SubjectId != subject.SubjectId)
                {
                    return new APIResult() { Status = false, Body = "题目代码重复" };
                }
                List<SubjectDto> subjectList_OrderNO = masterService.GetSubject(subject.ProjectId.ToString(), "", "", subject.OrderNO.ToString());
                if (subjectList_OrderNO != null && subjectList_OrderNO.Count > 0 && subjectList_OrderNO[0].SubjectId != subject.SubjectId)
                {
                    return new APIResult() { Status = false, Body = "题目序号重复" };
                }
                masterService.SaveSubject(subject);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 删除体系信息
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/DeleteSubject")]
        public APIResult DeleteSubject(Subject subject)
        {
            try
            {
                List<SubjectFile> subjectFileList = masterService.GetSubjectFile(subject.ProjectId.ToString(), subject.SubjectId.ToString());
                if (subjectFileList != null && subjectFileList.Count > 0)
                {
                    return new APIResult() { Status = false, Body = "请先删除标准照片" };
                }
                List<SubjectInspectionStandard> subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(subject.ProjectId.ToString(), subject.SubjectId.ToString());
                if (subjectInspectionStandardList != null && subjectInspectionStandardList.Count > 0)
                {
                    return new APIResult() { Status = false, Body = "请先删除检查标准" };
                }
                List<SubjectLossResult> subjectLossResultList = masterService.GetSubjectLossResult(subject.ProjectId.ToString(), subject.SubjectId.ToString());
                if (subjectLossResultList != null && subjectLossResultList.Count > 0)
                {
                    return new APIResult() { Status = false, Body = "请先删除失分说明" };
                }
                masterService.DeleteSubject(subject.SubjectId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取检查标准
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectInspectionStandard")]
        public APIResult GetSubjectInspectionStandard(string projectId, string subjectId)
        {
            try
            {
                List<SubjectInspectionStandard> inspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(inspectionStandardList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存检查标准
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubjectInspectionStandard")]
        public APIResult SaveSubjectInspectionStandard(SubjectInspectionStandard subjectInspectionStandard)
        {
            try
            {
                masterService.SaveSubjectInspectionStandard(subjectInspectionStandard);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 删除检查标准
        /// </summary>
        /// <param name="subjectInspectionStandard"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/DeleteSubjectInspectionStandard")]
        public APIResult DeleteSubjectInspectionStandard(SubjectInspectionStandard subjectInspectionStandard)
        {
            try
            {
                masterService.DeleteSubjectInspectionStandard(subjectInspectionStandard.SubjectId, subjectInspectionStandard.SeqNO);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取标准照片列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectFile")]
        public APIResult GetSubjectFile(string projectId, string subjectId)
        {
            try
            {
                List<SubjectFile> fileList = masterService.GetSubjectFile(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(fileList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存标准照片
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubjectFile")]
        public APIResult SaveSubjectFile(SubjectFile subjectFile)
        {
            try
            {
                masterService.SaveSubjectFile(subjectFile);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 删除标准照片
        /// </summary>
        /// <param name="subjectInspectionStandard"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/DeleteSubjectFile")]
        public APIResult DeleteSubjectFile(SubjectFile subjectFile)
        {
            try
            {
                masterService.DeleteSubjectFile(subjectFile.SubjectId, subjectFile.SeqNO);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取失分描述
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectLossResult")]
        public APIResult GetSubjectLossResult(string projectId, string subjectId)
        {
            try
            {
                List<SubjectLossResult> lossResultList = masterService.GetSubjectLossResult(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lossResultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存失分描述
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveLossResult")]
        public APIResult SaveLossResult(SubjectLossResult subjectlossResult)
        {
            try
            {
                masterService.SaveSubjectLossResult(subjectlossResult);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 删除失分描述
        /// </summary>
        /// <param name="subjectInspectionStandard"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/DeleteSubjectLossResult")]
        public APIResult DeleteSubjectLossResult(SubjectLossResult subjectLossResult)
        {
            try
            {
                masterService.DeleteSubjectLossResult(subjectLossResult.SubjectId, subjectLossResult.SeqNO);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        [HttpGet]
        [Route("Master/SubjectExcelAnalysis")]
        public APIResult SubjectExcelAnalysis(string brandId, string ossPath)
        {
            try
            {
                List<SubjectDto> list = excelDataService.SubjectImport(ossPath);

                foreach (SubjectDto subject in list)
                {
                    subject.ImportChk = true;
                    subject.ImportRemark = "";
                    if (subject.OrderNO == null)
                    {
                        subject.ImportChk = false;
                        subject.ImportRemark += "执行顺序有为空的数据" + ";";
                    }
                    //if (string.IsNullOrEmpty(subject.ExamTypeCode))
                    //{
                    //    subject.ImportChk = false;
                    //    subject.ImportRemark += "卷别类型代码有为空的数据" + ";";
                    //}
                    if (string.IsNullOrEmpty(subject.RecheckTypeCode))
                    {
                        subject.ImportChk = false;
                        subject.ImportRemark += "复审类型代码有为空的数据" + ";";
                    }
                    if (string.IsNullOrEmpty(subject.HiddenCode_SubjectTypeName))
                    {
                        subject.ImportChk = false;
                        subject.ImportRemark += "题目类型有为空的数据" + ";";
                    }
                    foreach (SubjectDto subject1 in list)
                    {
                        if (subject != subject1 && subject.SubjectCode == subject1.SubjectCode)
                        {
                            subject.ImportChk = false;
                            subject.ImportRemark += "表格中存在重复的题目代码" + ";";
                        }
                        if (subject != subject1 && subject.OrderNO == subject1.OrderNO)
                        {
                            subject.ImportChk = false;
                            subject.ImportRemark += "表格中存在重复的执行顺序" + ";";
                        }
                    }
                    // 验证Excel中的卷别类型、复审类型是否存在
                    if (!string.IsNullOrEmpty(subject.ExamTypeCode.Trim()))
                    {
                        List<Label> labelList = masterService.GetLabel(brandId, "", "ExamType", true, subject.ExamTypeCode);
                        if (labelList == null || labelList.Count == 0)
                        {
                            subject.ImportChk = false;
                            subject.ImportRemark += "卷别类型在系统中未登记" + ";";
                        }
                    }
                    // 验证Excel中的卷别类型、复审类型是否存在
                    if (!string.IsNullOrEmpty(subject.RecheckTypeCode.Trim()))
                    {
                        List<Label> labelList = masterService.GetLabel(brandId, "", "RecheckType", true, subject.RecheckTypeCode);
                        if (labelList == null || labelList.Count == 0)
                        {
                            subject.ImportChk = false;
                            subject.ImportRemark += "复审类型在系统中未登记" + ";";
                        }
                    }
                }
                list = (from subject in list orderby subject.ImportChk select subject).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SubjectImport")]
        public APIResult SubjectImport(UploadData uploadData)
        {
            try
            {
                List<SubjectDto> list = CommonHelper.DecodeString<List<SubjectDto>>(uploadData.ListJson);
                foreach (SubjectDto dto in list)
                {
                    Subject subject = new Subject();
                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        subject.SubjectId = subjectList[0].SubjectId;
                    }
                    subject.CheckPoint = dto.CheckPoint;
                    subject.FullScore = dto.FullScore;
                    if (dto.HiddenCode_SubjectTypeName == "照片")
                    {
                        subject.HiddenCode_SubjectType = "Photo";
                    }
                    else { subject.HiddenCode_SubjectType = "Process"; }
                    subject.Implementation = dto.Implementation;
                    subject.InspectionDesc = dto.InspectionDesc;
                    subject.InUserId = dto.InUserId;
                    if (!string.IsNullOrEmpty(dto.ExamTypeCode))
                    {
                        List<Label> labelList = masterService.GetLabel(masterService.GetProject("", "", dto.ProjectId.ToString(), "", "", "")[0].BrandId.ToString(), "", "ExamType", true, dto.ExamTypeCode);
                        if (labelList != null && labelList.Count > 0)
                        {
                            subject.LabelId = labelList[0].LabelId;
                        }
                    }
                    else
                    {
                        subject.LabelId = null;
                    }

                    List<Label> labelList_Recheck = masterService.GetLabel(masterService.GetProject("", "", dto.ProjectId.ToString(), "", "", "")[0].BrandId.ToString(), "", "RecheckType", true, dto.RecheckTypeCode);
                    if (labelList_Recheck != null && labelList_Recheck.Count > 0)
                    {
                        subject.LabelId_RecheckType = labelList_Recheck[0].LabelId;
                    }
                    subject.LowScore = dto.LowScore;

                    subject.ModifyUserId = dto.ModifyUserId;
                    subject.OrderNO = dto.OrderNO;
                    subject.ProjectId = dto.ProjectId;
                    subject.Remark = dto.Remark;
                    subject.SubjectCode = dto.SubjectCode;
                    masterService.SaveSubject(subject);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/SubjectFileExcelAnalysis")]
        public APIResult SubjectFileExcelAnalysis(string projectId, string ossPath)
        {
            try
            {
                List<FileResultDto> list = excelDataService.SubjectFileImport(ossPath);

                foreach (FileResultDto fileResult in list)
                {
                    fileResult.ImportChk = true;
                    fileResult.ImportRemark = "";
                    List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", fileResult.SubjectCode, "");
                    if (subjectList == null || subjectList.Count == 0)
                    {
                        fileResult.ImportChk = false;
                        fileResult.ImportRemark += "题目代码未登记" + ";";
                    }
                }
                list = (from fileResult in list orderby fileResult.ImportChk select fileResult).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SubjectFileImport")]
        public APIResult SubjectFileImport(UploadData uploadData)
        {
            try
            {
                List<FileResultDto> list = CommonHelper.DecodeString<List<FileResultDto>>(uploadData.ListJson);
                // 先删除
                foreach (FileResultDto dto in list)
                {

                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        masterService.DeleteSubjectFile(subjectList[0].SubjectId, 0);
                    }
                }
                // 插入
                foreach (FileResultDto dto in list)
                {
                    SubjectFile subjectFile = new SubjectFile();
                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        subjectFile.SubjectId = subjectList[0].SubjectId;
                    }
                    subjectFile.FileName = dto.FileName;
                    subjectFile.InUserId = dto.InUserId;
                    subjectFile.ModifyUserId = dto.ModifyUserId;
                    masterService.SaveSubjectFile(subjectFile);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/SubjectInspectionStandardExcelAnalysis")]
        public APIResult SubjectInspectionStandardExcelAnalysis(string projectId, string ossPath)
        {
            try
            {
                List<InspectionStandardResultDto> list = excelDataService.SubjectInspectionStandardImport(ossPath);

                foreach (InspectionStandardResultDto inspectionStandardResult in list)
                {
                    inspectionStandardResult.ImportChk = true;
                    inspectionStandardResult.ImportRemark = "";
                    List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", inspectionStandardResult.SubjectCode, "");
                    if (subjectList == null || subjectList.Count == 0)
                    {
                        inspectionStandardResult.ImportChk = false;
                        inspectionStandardResult.ImportRemark += "题目代码未登记" + ";";
                    }
                }
                list = (from inspectionStandardResult in list orderby inspectionStandardResult.ImportChk select inspectionStandardResult).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SubjectInspectionStandardImport")]
        public APIResult SubjectInspectionStandardImport(UploadData uploadData)
        {
            try
            {
                List<InspectionStandardResultDto> list = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(uploadData.ListJson);

                // 先删除
                foreach (InspectionStandardResultDto dto in list)
                {

                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        masterService.DeleteSubjectInspectionStandard(subjectList[0].SubjectId, 0);
                    }
                }
                foreach (InspectionStandardResultDto dto in list)
                {
                    SubjectInspectionStandard subjectInspectionStandard = new SubjectInspectionStandard();
                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        subjectInspectionStandard.SubjectId = subjectList[0].SubjectId;
                    }
                    subjectInspectionStandard.InspectionStandardName = dto.InspectionStandardName;
                    subjectInspectionStandard.InUserId = dto.InUserId;
                    subjectInspectionStandard.ModifyUserId = dto.ModifyUserId;
                    masterService.SaveSubjectInspectionStandard(subjectInspectionStandard);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/SubjectLossResultExcelAnalysis")]
        public APIResult SubjectLossResultExcelAnalysis(string projectId, string ossPath)
        {
            try
            {
                List<LossResultDto> list = excelDataService.SubjectLossImport(ossPath);

                foreach (LossResultDto lossResult in list)
                {
                    lossResult.ImportChk = true;
                    lossResult.ImportRemark = "";
                    List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", lossResult.SubjectCode, "");
                    if (subjectList == null || subjectList.Count == 0)
                    {
                        lossResult.ImportChk = false;
                        lossResult.ImportRemark += "题目代码未登记" + ";";
                    }
                }
                list = (from lossResult in list orderby lossResult.ImportChk select lossResult).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SubjectLossResultImport")]
        public APIResult SubjectLossResultImport(UploadData uploadData)
        {
            try
            {
                List<LossResultDto> list = CommonHelper.DecodeString<List<LossResultDto>>(uploadData.ListJson);
                foreach (LossResultDto dto in list)
                {

                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        masterService.DeleteSubjectLossResult(subjectList[0].SubjectId, 0);
                    }
                }
                foreach (LossResultDto dto in list)
                {
                    SubjectLossResult subjectLossResult = new SubjectLossResult();
                    List<SubjectDto> subjectList = masterService.GetSubject(dto.ProjectId.ToString(), "", dto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        subjectLossResult.SubjectId = subjectList[0].SubjectId;
                    }
                    subjectLossResult.LossResultName = dto.LossDesc;
                    subjectLossResult.LossResultCode = dto.LossCode;
                    subjectLossResult.InUserId = dto.InUserId;
                    subjectLossResult.ModifyUserId = dto.ModifyUserId;
                    masterService.SaveSubjectLossResult(subjectLossResult);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 章节管理
        [HttpGet]
        [Route("Master/GetChapter")]
        public APIResult GetChapter(string projectId, string chapterId = "", string chapterCode = "")
        {
            try
            {
                List<Chapter> chapterList = masterService.GetChapter(projectId, chapterId, chapterCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(chapterList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 下载文件照片命名
        [HttpGet]
        [Route("Master/GetFileType")]
        public APIResult GetFileType()
        {
            try
            {
                List<FileType> fileTypeList = masterService.GetFileType();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(fileTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/GetFileNameOption")]
        public APIResult GetFileNameOption(string fileTypeCode)
        {
            try
            {
                List<FileNameOption> fileNameOptionList = masterService.GetFileNameOption(fileTypeCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(fileNameOptionList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/GetFileRename")]
        public APIResult GetFileRename(string projectId, string fileTypeCode, string photoType)
        {
            try
            {
                List<FileRenameDto> fileRenameList = masterService.GetFileRename(projectId, fileTypeCode, photoType);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(fileRenameList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveFileRename")]
        public APIResult SaveFileRename(FileRename fileRename)
        {
            try
            {
                masterService.SaveFileRename(fileRename);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteFileRename")]
        public APIResult DeleteFileRename(FileRename fileRename)
        {
            try
            {
                masterService.DeleteFileRename(fileRename);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 版本管理
        [HttpGet]
        [Route("Master/GetAppVersion")]
        public APIResult GetAppVersion()
        {
            try
            {
                List<AppVersion> versionList = masterService.GetAppVersion();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(versionList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 删除临时文件
        [HttpPost]
        [Route("Master/DeleteCacheFile")]
        public APIResult DeleteCacheFile()
        {
            try
            {
                List<HiddenColumn> list = masterService.GetHiddenCode("缓存文件信息", "");
                if (list != null && list.Count > 0)
                {
                    string SurveyPhotoPath = "";
                    string SurveyPhotoDay = "";
                    string EasyPhotoPath = "";
                    string EasyPhotoDay = "";
                    string DBLogPath = "";
                    string DBLogDay = "";
                    foreach (HiddenColumn column in list)
                    {
                        if (column.HiddenCode == "SurveyPhotoPath")
                        {
                            SurveyPhotoPath = column.HiddenName;
                        }
                        else if (column.HiddenCode == "SurveyPhotoDay") {
                            SurveyPhotoDay= column.HiddenName;
                        }
                        else if (column.HiddenCode == "EasyPhotoPath")
                        {
                            EasyPhotoPath = column.HiddenName;
                        }
                        else if (column.HiddenCode == "EasyPhotoDay")
                        {
                            EasyPhotoDay = column.HiddenName;
                        }
                        else if (column.HiddenCode == "DBLogPath")
                        {
                            DBLogPath = column.HiddenName;
                        }
                        else if (column.HiddenCode == "DBLogDay")
                        {
                            DBLogDay = column.HiddenName;
                        }
                    }
                    photoService.DeleteCacheFile(SurveyPhotoPath, Convert.ToInt32(SurveyPhotoDay));
                    photoService.DeleteCacheFile(EasyPhotoPath, Convert.ToInt32(EasyPhotoDay));
                    photoService.DeleteCacheFile(DBLogPath, Convert.ToInt32(DBLogDay));
                }
                
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
    }
}
