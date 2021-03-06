﻿using System.Web.Http;
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
        //#region 不联网
        //[HttpGet]
        //[Route("Master/GetBaseInfo")]
        //public APIResult GetBaseInfo(string projectId)
        //{
        //    try
        //    {
        //        List<object> resultList = new List<object>();
        //        #region 下载基础数据到Mobile本地

        //        // 用户经销商关系信息 ProjectShop
        //        resultList.Add(shopService.GetProjectShop(projectId));
        //        // 经销商试卷类型信息 ShopSubjectTypeExam
        //        resultList.Add(shopService.GetShopSubjectTypeExam(projectId));
        //        // 体系类型信息  Subject
        //        resultList.Add(masterService.GetSubject(projectId, ""));
        //        // 标准照片信息 SubjectFile
        //        resultList.Add(masterService.GetSubjectFile(projectId, ""));
        //        // 检查标准信息 SubjectInspectionStandard
        //        resultList.Add(masterService.GetSubjectInspectionStandard(projectId, ""));
        //        // 失分描述 SubjectLossResult
        //        resultList.Add(masterService.GetSubjectLossResult(projectId, ""));
        //        // 体系类型打分范围信息 SubjectTypeScoreRegion
        //        resultList.Add(masterService.GetSubjectTypeScoreRegion(projectId, "",""));

        //        return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}

        //[HttpPost]
        //[Route("Master/Upload")]
        //public async Task<APIResult> Upload([FromBody]UploadData data)
        //{
        //    try
        //    {

        //        string userId = data.UserId;
        //        data.AnswerShopInfoList = CommonHelper.DecodeString<List<AnswerShopInfo>>(data.AnswerShopInfoListJson);
        //        data.AnswerShopConsultantList = CommonHelper.DecodeString<List<AnswerShopConsultant>>(data.AnswerShopConsultantListJson);
        //        // data.AnswerList = CommonHelper.DecodeString<List<Answer>>(data.AnswerListJson);
        //        //CommonHelper.log(data.sh);
        //        // Thread.Sleep(1000);


        //        // CommonHelper.log(data.AnswerListJson);
        //        answerService.SaveAnswerShopInfoList(data.AnswerShopInfoList, userId);
        //        answerService.SaveAnswerShopConsultantList(data.AnswerShopConsultantList, userId);
        //        //answerService.SaveAnswerList(data.AnswerList, userId);

        //        return new APIResult() { Status = true, Body = "" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        //public async Task<Dictionary<string, string>> PostFormData()
        //{
        //    try
        //    {
        //        if (!Request.Content.IsMimeMultipartContent())
        //        {
        //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //        }

        //        var multipartMemoryStreamProvider = await Request.Content.ReadAsMultipartAsync();
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        foreach (var content in multipartMemoryStreamProvider.Contents)
        //        {
        //            string fileName = content.Headers.ContentDisposition.FileName;
        //            if (!string.IsNullOrEmpty(fileName))
        //            {
        //                using (Stream stream = await content.ReadAsStreamAsync())
        //                {
        //                    string projectId = dic["projectId"];
        //                    string shopId = dic["shopId"];
        //                    string fileKey = "survey/" + projectId + "/" + shopId + "/" + fileName.Replace("\"", "");
        //                    //处理文件
        //                    OSSClientHelper.UploadOSSFile(fileKey, stream, stream.Length);
        //                }
        //            }
        //            else
        //            {
        //                string val = await content.ReadAsStringAsync();
        //                dic.Add(content.Headers.ContentDisposition.Name.Replace("\"", ""), val);
        //            }
        //        }

        //        return dic;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //#endregion
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
        public APIResult GetRoleType(string type)
        {
            try
            {
                List<RoleType> roleTypeList = masterService.GetRoleType(type,"","");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(roleTypeList) };
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
        [HttpGet]
        [Route("Master/GetUserInfo")]
        public APIResult GetUserInfo(string tenantId, string brandId, string userId, string accountId, string accountName, string roleTypeCode, string telNO, string email)
        {
            try
            {
                List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId, brandId, userId, accountId, accountName, roleTypeCode, telNO, email);
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
                List<UserInfo> userInfoList = masterService.GetUserInfo(userInfo.TenantId.ToString(), "", "", userInfo.AccountId, "", "", "", "");
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
                    List<RoleType> roleTypeList = masterService.GetRoleType("","",userInfoDto.RoleTypeName);
                    if (roleTypeList == null || roleTypeList.Count == 0)
                    {
                        userInfoDto.ImportChk = false;
                        userInfoDto.ImportRemark += "权限类型不存在" + ";";
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
         [HttpGet]
        [Route("Master/UserInfoObjectExcelAnalysis")]
        public APIResult UserInfoObjectExcelAnalysis(string tenantId,string brandId,string ossPath)
        {
            try
            {
                List<UserInfoObjectDto> list = excelDataService.UserInfoObjectImport(ossPath);
                foreach (UserInfoObjectDto userInfoObjectDto in list)
                {
                    userInfoObjectDto.ImportChk = true;
                    userInfoObjectDto.ImportRemark = "";
                    List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId,brandId,"", userInfoObjectDto.AccountId, "","","","");
                    if (userInfoList == null || userInfoList.Count == 0)
                    {
                        userInfoObjectDto.ImportChk = false;
                        userInfoObjectDto.ImportRemark += "账号未在系统登记" + ";";
                    }
                    else {
                        string roleTypeCode = userInfoList[0].RoleType;
                        if (roleTypeCode == "B_Group")
                        {
                            List<Group> groupList = masterService.GetGroup(brandId, "", userInfoObjectDto.ObjectCode, "");
                            if (groupList == null || groupList.Count == 0)
                            {
                                userInfoObjectDto.ImportChk = false;
                                userInfoObjectDto.ImportRemark += "集团代码不存在" + ";";
                            }
                        }
                        else if(roleTypeCode == "B_Bussiness")
                        {
                            List<AreaDto> areaList = masterService.GetArea("",brandId,userInfoObjectDto.ObjectCode,"","Bussiness","",null);
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
                            List<ShopDto> shopList = masterService.GetShop(tenantId,brandId,"",userInfoObjectDto.ObjectCode,"");
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
                     List<UserInfo> userInfoList= masterService.GetUserInfo(userInfoObjectDto.TenantId.ToString(), userInfoObjectDto.brandId,"", userInfoObjectDto.AccountId, "", "", "", "");
                    if (userInfoList != null && userInfoList.Count > 0)
                    {
                        userInfo.UserId = userInfoList[0].Id;
                        string roleTypeCode = userInfoList[0].RoleType;
                        if (roleTypeCode == "B_Group")
                        {
                            userInfo.ObjectId = masterService.GetGroup(userInfoObjectDto.brandId, "", userInfoObjectDto.ObjectCode, "")[0].GroupId;
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
                            userInfo.ObjectId = masterService.GetShop(userInfoObjectDto.TenantId.ToString(), userInfoObjectDto.brandId, "", userInfoObjectDto.ObjectCode, "")[0].ShopId;
                            
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
                string downloadPath = excelDataService.UserInfoExport(tenantId,brandId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 区域管理
        [HttpGet]
        [Route("Master/GetArea")]
        public APIResult GetArea(string brandId, string areaId, string areaCode, string areaName, string areaType, string parentId,bool? useChk)
        {
            try
            {
                List<AreaDto> areaList = masterService.GetArea(areaId, brandId, areaCode, areaName, areaType, parentId,useChk);
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
                List<AreaDto> areaList = masterService.GetArea("", area.BrandId.ToString(), area.AreaCode, "", "", "",null);
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
                       area.AreaType != "SmalllArea")
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
                        List<AreaDto> areaList = masterService.GetArea("", area.BrandId.ToString(), area.ParentCode, "", parentAreaType, "",null);
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
                        area.ImportRemark += "上级区域代码在系统和表格中均不存在" + ";";
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
                        area.AreaType != "SmalllArea")
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
                    List<AreaDto> areaList = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.AreaCode, "", areaDto.AreaType, "",null);
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
                        List<AreaDto> areaList = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.ParentCode, "", parentAreaType, "",null);
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
                        List<AreaDto> areaList = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.AreaCode, "", areaDto.AreaType, "",null);
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
                        List<AreaDto> areaList_Parent = masterService.GetArea("", areaDto.BrandId.ToString(), areaDto.ParentCode, "", parentAreaType, "",null);
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
        public APIResult GetShop(string brandId, string shopId, string shopCode, string key)
        {
            try
            {
                List<ShopDto> shopList = masterService.GetShop("", brandId, shopId, shopCode, key);
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
                List<ShopDto> shopList = masterService.GetShop("", shop.BrandId.ToString(), "", shop.ShopCode, "");
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
        /// 获取当前期需要执行的经销商及所属试卷类型
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
                List<ShopDto> projectShopList = new List<ShopDto>();
                List<Shop> shopList = shopService.GetShopByProjectId(projectId);
                List<ShopSubjectTypeExamDto> subjectTypeExamList = shopService.GetShopSubjectTypeExam(projectId, "");
                foreach (Shop shop in shopList)
                {
                    ShopDto shopDto = new ShopDto();
                    shopDto.ProjectId = Convert.ToInt32(projectId);
                    shopDto.ShopId = shop.ShopId;
                    shopDto.ShopCode = shop.ShopCode;
                    shopDto.ShopName = shop.ShopName;
                    shopDto.Province = shop.Province;
                    shopDto.City = shop.City;
                    shopDto.ShopShortName = shop.ShopShortName;

                    ShopSubjectTypeExamDto exam = subjectTypeExamList.Where(x => x.ShopId == shop.ShopId).FirstOrDefault();
                    if (exam != null)
                    {
                        shopDto.SubjectTypeExamId = Convert.ToInt32(exam.ShopSubjectTypeExamId);
                        shopDto.SubjectTypeExamName = exam.SubjectTypeExamName;
                    }

                    projectShopList.Add(shopDto);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectShopList) };
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
                        List<Group> groupList = masterService.GetGroup(brandId, "", shop.GroupCode, "");
                        if (groupList == null || groupList.Count == 0)
                        {
                            shop.ImportChk = false;
                            shop.ImportRemark += "集团代码在系统中不存在" + ";";
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
                        List<Group> groupList = masterService.GetGroup(shop.BrandId.ToString(), "", shop.GroupCode, "");
                        if (groupList == null || groupList.Count == 0)
                        {
                            return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记的集团代码，请检查文件" };
                        }
                    }
                }
                List<Shop> importShopList = new List<Shop>();
                foreach (ShopDto shopDto in list)
                {
                    Shop shop = new Shop();
                    List<Group> groupList = new List<Group>();
                    if (string.IsNullOrEmpty(shopDto.GroupCode))
                    { groupList = masterService.GetGroup(shopDto.BrandId.ToString(), "", shopDto.GroupCode, ""); }
                    if (groupList != null && groupList.Count > 0)
                    {
                        shop.GroupId = groupList[0].GroupId;
                    }
                    // 如果经销商代码在系统已经存在就进行更新操作，可以进行导入的批量更新
                    List<ShopDto> shopList = masterService.GetShop(shopDto.TenantId.ToString(), shopDto.BrandId.ToString(), "", shopDto.ShopCode, "");
                    if (shopList != null && shopList.Count > 0)
                    {
                        shop.ShopId = shopList[0].ShopId;
                    }
                    shop.BrandId = shopDto.BrandId;
                    shop.City = shopDto.City;
                    shop.InUserId = shopDto.InUserId;
                    shop.ModifyUserId = shopDto.ModifyUserId;
                    shop.Province = shopDto.Province;
                    shop.ShopCode = shopDto.ShopCode;
                    shop.ShopName = shopDto.ShopName;
                    shop.ShopShortName = shopDto.ShopShortName;
                    shop.TenantId = shopDto.TenantId;
                    shop.UseChk = shopDto.UseChk;
                    masterService.SaveShop(shop);
                    //importShopList.Add(shop);
                }
               // masterService.ImportShop(importShopList);
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
                    List<ShopDto> shopList = masterService.GetShop(tenantId, brandId, "", shopDto.GroupCode, "");
                    if (shopList == null || shopList.Count == 0)
                    {
                        shopDto.ImportChk = false;
                        shopDto.ImportRemark += "经销商代码在系统中不存在" + ";";
                    }
                    List<AreaDto> areaList = masterService.GetArea("", brandId, shopDto.AreaCode, "", "SmallArea", "",null);
                    if (areaList == null || areaList.Count == 0)
                    {
                        shopDto.ImportChk = false;
                        shopDto.ImportRemark += "小区代码在系统中不存在" + ";";
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
                    List<ShopDto> shopList = masterService.GetShop(shopDto.TenantId.ToString(), shopDto.BrandId.ToString(), "", shopDto.GroupCode, "");
                    if (shopList == null || shopList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记的经销商代码，请检查文件" };
                    }
                    List<AreaDto> areaList = masterService.GetArea("", shopDto.BrandId.ToString(), shopDto.AreaCode, "", "SmallArea", "",null);
                    if (areaList == null || areaList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记的小区代码，请检查文件" };
                    }
                }
                foreach (ShopDto shopDto in list)
                {
                    AreaShop areashop = new AreaShop();
                    List<AreaDto> areaList = masterService.GetArea("", shopDto.BrandId.ToString(), shopDto.AreaCode, "", "SmallArea", "",null);
                    if (areaList != null && areaList.Count > 0)
                    {
                        areashop.AreaId = areaList[0].AreaId;
                    }
                    areashop.InUserId = shopDto.InUserId;
                    areashop.ModifyUserId = shopDto.ModifyUserId;
                    List<ShopDto> shopList = masterService.GetShop(shopDto.TenantId.ToString(), shopDto.BrandId.ToString(), "", shopDto.ShopCode, "");
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
        public APIResult GetGroup(string brandId, string groupId, string groupCode, string groupName)
        {
            try
            {
                List<Group> groupList = masterService.GetGroup(brandId, groupId, groupCode, groupName);
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
                List<Group> groupList = masterService.GetGroup(group.BrandId.ToString(), "", group.GroupCode, "");
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
                List<ProjectDto> projectList = masterService.GetProject("", brandId, projectId, "", year);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveProject")]
        public APIResult SaveProject(Project project)
        {
            try
            {
                List<ProjectDto> projectList = masterService.GetProject("", project.BrandId.ToString(), "", project.ProjectCode, "");
                if (projectList != null && projectList.Count > 0 && projectList[0].ProjectId != project.ProjectId)
                {
                    return new APIResult() { Status = false, Body = "期号代码重复" };
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
        public APIResult GetLabel(string brandId,string labelId, string labelType, bool? useChk)
        {
            try
            {
                List<Label> labelList = masterService.GetLabel(brandId, labelId,labelType, useChk, "");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(labelList) };
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
                List<Label> labelList = masterService.GetLabel(label.BrandId.ToString(),"", label.LabelType, null, label.LabelCode);
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
        public APIResult GetSubject(string projectId, string subjectId)
        {
            try
            {
                List<SubjectDto> subjectList = masterService.GetSubject(projectId, subjectId);
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
                masterService.SaveSubject(subject);
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
        public APIResult SaveLossResult([FromBody]SubjectLossResult subjectlossResult)
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
        #endregion





    }
}
