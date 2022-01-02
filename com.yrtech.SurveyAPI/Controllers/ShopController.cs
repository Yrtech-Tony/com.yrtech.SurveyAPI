using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class ShopController : ApiController
    {
        ShopService shopService = new ShopService();
        ExcelDataService excelDataService = new ExcelDataService();
        MasterService masterService = new MasterService();
        //[HttpGet]
        //[Route("Shop/GetShopByProjectId")]
        //public APIResult GetShopByProjectId(string projectId)
        //{
        //    try
        //    {
        //        var lst = shopService.GetShopByProjectId(projectId);
        //        return new APIResult() { Status = true, Body = CommonHelper.Encode(lst) };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        #region  经销商卷别设置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Shop/GetProjectShopExamType")]
        public APIResult GetProjectShopExamType(string brandId,string projectId, string shopId)
        {
            try
            {
                List<ProjectShopExamTypeDto> projectShopExamTypeList= shopService.GetProjectShopExamType(brandId,projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectShopExamTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Shop/SaveProjectShopExamType")]
        public APIResult SaveProjectShopExamType(ProjectShopExamType projectShopExamType)
        {
            try
            {
                shopService.SaveProjectShopExamType(projectShopExamType);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Shop/ProjectShopExamTypeExcelAnalysis")]
        public APIResult ProjectShopExamTypeExcelAnalysis(string brandId, string ossPath)
        {
            try
            {
                List<ProjectShopExamTypeDto> list = excelDataService.ProjectShopExamTypeImport(ossPath);
                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in list)
                {
                    projectShopExamTypeDto.ImportChk = true;
                    projectShopExamTypeDto.ImportRemark = "";
                    List<ShopDto> shopList = masterService.GetShop("", brandId, "", projectShopExamTypeDto.ShopCode, "");
                    if (shopList == null || shopList.Count == 0)
                    {
                        projectShopExamTypeDto.ImportChk = false;
                        projectShopExamTypeDto.ImportRemark += "经销商代码在系统中不存在" + ";";
                    }
                    List<Label> labelList = masterService.GetLabel(brandId,"","ExamType",true,projectShopExamTypeDto.ExamTypeCode);
                    if (labelList == null || labelList.Count == 0)
                    {
                        projectShopExamTypeDto.ImportChk = false;
                        projectShopExamTypeDto.ImportRemark += "卷别代码在系统中不存在或不可用" + ";";
                    }
                }
                list = (from projectShopExamType in list orderby projectShopExamType.ImportChk select projectShopExamType).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Shop/ProjectShopExamTypeImport")]
        public APIResult ProjectShopExamTypeImport(UploadData uploadData)
        {
            try
            {
                List<ProjectShopExamTypeDto> list = CommonHelper.DecodeString<List<ProjectShopExamTypeDto>>(uploadData.ListJson);
                //验证Excel中的经销商代码和卷别代码是否在系统存在
                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in list)
                {
                    List<ShopDto> shopList = masterService.GetShop("", projectShopExamTypeDto.BrandId.ToString(), "", projectShopExamTypeDto.ShopCode, "");
                    if (shopList == null || shopList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记的经销商代码，请检查文件" };
                    }
                    List<Label> labelList = masterService.GetLabel(projectShopExamTypeDto.BrandId.ToString(), "", "ExamType", true, projectShopExamTypeDto.ExamTypeCode); 
                    if (labelList == null || labelList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记或者不可用的卷别代码，请检查文件" };
                    }
                }
                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in list)
                {
                    ProjectShopExamType projectShopExamType = new ProjectShopExamType();
                    projectShopExamType.ProjectId = projectShopExamTypeDto.ProjectId;
                    List<ShopDto> shopList = masterService.GetShop("", projectShopExamTypeDto.BrandId.ToString(), "", projectShopExamTypeDto.ShopCode, "");
                    if (shopList != null && shopList.Count > 0)
                    {
                        projectShopExamType.ShopId = shopList[0].ShopId;
                    }
                    List<Label> labelList = masterService.GetLabel(projectShopExamTypeDto.BrandId.ToString(), "", "ExamType", true, projectShopExamTypeDto.ExamTypeCode);
                    if (labelList != null && labelList.Count > 0)
                    {
                        projectShopExamType.ExamTypeId = labelList[0].LabelId;
                    }
                    projectShopExamType.InUserId = projectShopExamTypeDto.InUserId;
                    projectShopExamType.ModifyUserId = projectShopExamTypeDto.ModifyUserId;
                    
                    shopService.SaveProjectShopExamType(projectShopExamType);
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
