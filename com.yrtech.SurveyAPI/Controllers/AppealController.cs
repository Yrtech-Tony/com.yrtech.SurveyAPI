using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System.Linq;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class AppealController : ApiController
    {
        AppealService appealService = new AppealService();
        ExcelDataService excelDataService = new ExcelDataService();
        MasterService masterService = new MasterService();

        #region 申诉设置
        [HttpGet]
        [Route("Appeal/GetAppealSet")]
        public APIResult GetAppealSet(string projectId)
        {
            try
            {
                List<AppealSetDto> list = appealService.GetAppealSet(projectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/SaveAppealSet")]
        public APIResult SaveAppealSet(AppealSet appealSet)
        {
            try
            {
                appealService.SaveAppealSet(appealSet);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetAppealShopSet")]
        public APIResult GetAppealShopSet(string projectId)
        {
            try
            {
                List<AppealSetDto> list = appealService.GetAppealShopSet(projectId, "");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        ///  验证单个经销商申诉期是否已过，经销商申诉页面使用
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Appeal/GetAppealShopSetCheck")]
        public APIResult GetAppealShopSetCheck(string projectId,string shopId)
        {
            try
            {
                List<AppealSetDto> list = new List<AppealSetDto>();
                // 如果设置了申诉时间过期，进行提醒。但是数据还可以查询。在web端调用这个接口
                if (!string.IsNullOrEmpty(shopId))
                {
                    list = appealService.GetAppealShopSet(projectId, shopId.Replace(",", ""));
                    if (list != null && list.Count > 0)
                    {
                        if (list[0].AppealEndDate < DateTime.Now)
                        {
                            return new APIResult() { Status = false, Body = "申诉时间已过期，无法申诉" };
                        }
                    }
                }
                
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/SaveAppealShopSet")]
        public APIResult SaveAppealShopSet(AppealShopSet appealSet)
        {
            try
            {
                appealService.SaveAppealShopSet(appealSet);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/AppealShopSetExcelAnalysis")]
        public APIResult AppealShopSetExcelAnalysis(string tenantId, string brandId, string ossPath)
        {
            try
            {
                List<AppealSetDto> list = excelDataService.AppealShopSetImport(ossPath);
                foreach (AppealSetDto appealSet in list)
                {
                    appealSet.ImportChk = true;
                    appealSet.ImportRemark = "";
                    List<ShopDto> shopList = masterService.GetShop(tenantId, brandId, "", appealSet.ShopCode, "", null);
                    if (shopList == null || shopList.Count == 0)
                    {
                        appealSet.ImportChk = false;
                        appealSet.ImportRemark += "经销商不存在" + ";";
                    }
                    if (appealSet.AppealEndDate == null)
                    {
                        appealSet.ImportChk = false;
                        appealSet.ImportRemark += "结束时间不能为空" + ";";
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
        [Route("Appeal/AppealShopSetImport")]
        public APIResult AppealShopSetImport(UploadData uploadData)
        {
            try
            {
                List<AppealSetDto> list = CommonHelper.DecodeString<List<AppealSetDto>>(uploadData.ListJson);
                foreach (AppealSetDto appeal in list)
                {
                    AppealShopSet appealSet = new AppealShopSet();
                    appealSet.ProjectId = appeal.ProjectId;

                    List<ShopDto> shopList = masterService.GetShop(appeal.TenantId.ToString(), appeal.BrandId.ToString(), "", appeal.ShopCode, "", null);
                    if (shopList != null && shopList.Count > 0)
                    {
                        appealSet.ShopId = shopList[0].ShopId;
                    }
                    appealSet.AppealStartDate = appeal.AppealStartDate;
                    appealSet.AppealEndDate = appeal.AppealEndDate;
                    appealSet.InUserId = appeal.InUserId;
                    appealSet.ModifyUserId = appeal.ModifyUserId;
                    appealService.SaveAppealShopSet(appealSet);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        [HttpGet]
        [Route("Appeal/CreateAppeal")]
        public APIResult CreateAppeal(string projectId)
        {
            try
            {
                appealService.CreateAppeal(projectId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetShopAppealInfoByPage")]
        public APIResult GetShopAppealInfoByPage(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount, string roleType = "")
        {
            try
            {
                if (roleType == "B_Shop")
                {
                    // 如果没有设置申诉时间，进行提醒，且不能查询到申诉数据
                    List<AppealSetDto> appealSetlist = appealService.GetAppealShopSet(projectId, shopIdStr.Replace(",", ""));
                    if (appealSetlist == null || appealSetlist.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "未设置申诉时间，请联系管理员" };
                    }
                    else
                    {
                        if (appealSetlist[0].AppealEndDate == null)
                        {
                            return new APIResult() { Status = false, Body = "未设置申诉时间，请联系管理员" };
                        }

                    }
                }
                List<AppealDto> list = appealService.GetShopAppealInfoByPage(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCount);
                foreach (AppealDto appeal in list)
                {
                    if (appeal.AppealEndDate == null)
                    {
                        appeal.AppealDateCheck = true;
                    }
                    else if (appeal.AppealEndDate > DateTime.Now)
                    {
                        appeal.AppealDateCheck = true; // 不能编辑
                    }
                    else if (appeal.AppealEndDate < DateTime.Now)
                    {
                        appeal.AppealDateCheck = false;
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetFeedBackInfoByPage")]
        public APIResult GetFeedBackInfoByPage(string projectId, string keyword, int pageNum, int pageCount)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.GetFeedBackInfoByPage(projectId, keyword, pageNum, pageCount)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetShopSubjectAppeal")]
        public APIResult GetShopSubjectAppeal(string appealId)
        {
            try
            {
                List<AppealDto> list = appealService.GetShopSubjectAppeal(appealId);
                foreach (AppealDto appeal in list)
                {
                    if (appeal.AppealEndDate == null)
                    {
                        appeal.AppealDateCheck = true;
                    }
                    else if (appeal.AppealEndDate > DateTime.Now)
                    {
                        appeal.AppealDateCheck = true; // 
                    }
                    else if (appeal.AppealEndDate < DateTime.Now)
                    {
                        appeal.AppealDateCheck = false;
                    }

                }
                if (list != null && list.Count > 0)
                {
                    list[0].SubjectFileList = masterService.GetSubjectFile(list[0].ProjectId.ToString(), list[0].SubjectId.ToString());

                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/AppealFileSearch")]
        public APIResult AppealFileSearch(string appealId, string fileType)
        {
            try
            {
                List<AppealFileDto> list = appealService.AppealFileSearch(appealId, fileType);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        ///  统计申诉和反馈数量
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Appeal/AppealCountSearch")]
        public APIResult AppealCountSearch(string projectId)
        {
            try
            {
                List<AppealCountDto> list = appealService.AppealCountByShop(projectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealApply")]
        public APIResult AppealApply(UploadData uploadData)
        {
            try
            {
                List<AppealDto> list = CommonHelper.DecodeString<List<AppealDto>>(uploadData.ListJson);
                foreach (AppealDto appealDto in list)
                {
                    Appeal appeal = new Appeal();
                    appeal.AppealId = appealDto.AppealId;
                    appeal.AppealStatus = appealDto.AppealStatus;
                    // 如果选择了不申诉，系统自动生成申诉理由
                    if (appealDto.AppealStatus == false)
                    {
                        appeal.AppealReason = "经销商对检核扣分：无异议/未反馈";
                    }
                    else
                    {
                        appeal.AppealReason = appealDto.AppealReason;
                    }
                    appeal.AppealUserId = appealDto.AppealUserId;

                    appeal.ProjectId = appealDto.ProjectId;
                    appeal.ShopId = appealDto.ShopId;
                    appeal.SubjectId = appealDto.SubjectId;

                    if (appeal.AppealId == 0)// 申诉新增时，文件也进行保存
                    {
                        appeal = appealService.AppealApply(appeal);
                        foreach (AppealFile appealFile in appealDto.AppealFileList)
                        {
                            appealFile.AppealId = appeal.AppealId;
                            appealService.AppealFileSave(appealFile);
                        }
                    }
                    else// 申诉编辑时只保存申诉信息，文件信息在上传时会进行保存
                    {
                        appeal = appealService.AppealApply(appeal);
                    }

                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFeedBack")]
        public APIResult AppealFeedBack(UploadData uploadData)
        {
            try
            {
                List<AppealDto> list = CommonHelper.DecodeString<List<AppealDto>>(uploadData.ListJson);
                foreach (AppealDto appealDto in list)
                {
                    Appeal appeal = new Appeal();
                    appeal.AppealId = appealDto.AppealId;
                    appeal.FeedBackReason = appealDto.FeedBackReason;
                    appeal.FeedBackStatus = appealDto.FeedBackStatus;
                    appeal.FeedBackUserId = appealDto.FeedBackUserId;
                    appealService.AppealFeedBack(appeal);
                    //foreach (AppealFile appealFile in appealDto.AppealFileList)
                    //{
                    //    appealFile.AppealId = appealDto.AppealId;
                    //    appealService.AppealFileSave(appealFile);
                    //}

                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFileDelete")]
        public APIResult AppealFileDelete(AppealFile appealFile)
        {
            try
            {
                appealService.AppealFileDelete(appealFile.FileId.ToString());
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFileSave")]
        public APIResult AppealFileSave(AppealFile appealFile)
        {
            try
            {
                appealService.AppealFileSave(appealFile);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealDelete")]
        public APIResult AppealDelete(Appeal appeal)
        {
            try
            {
                appealService.AppealDelete(appeal.AppealId.ToString());
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        // 申诉导出
        [HttpGet]
        [Route("Appeal/AppealExport")]
        public APIResult AppealExport(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            try
            {
                string downloadPath = excelDataService.AppealExport(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCount);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        // 申诉反馈导出
        [HttpGet]
        [Route("Appeal/AppealFeedbackExport")]
        public APIResult AppealFeedbackExport(string projectId, string keyword, int pageNum, int pageCount)
        {
            try
            {
                string downloadPath = excelDataService.AppealFeedbackExport(projectId, keyword, pageNum, pageCount);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/AppealExcelAnalysis")]
        public APIResult AppealExcelAnalysis(string brandId, string projectId, string ossPath)
        {
            try
            {
                List<AppealDto> list = excelDataService.AppealImport(ossPath);
                foreach (AppealDto appeal in list)
                {
                    appeal.ImportChk = true;
                    appeal.ImportRemark = "";
                    List<ShopDto> shopList = masterService.GetShop("", brandId, "", appeal.ShopCode, "", true);

                    if (shopList == null || shopList.Count == 0)
                    {
                        appeal.ImportChk = false;
                        appeal.ImportRemark += "经销商代码不存在" + ";";
                    }
                    List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", appeal.SubjectCode, "");

                    if (subjectList == null || subjectList.Count == 0)
                    {
                        appeal.ImportChk = false;
                        appeal.ImportRemark += "题目代码不存在" + ";";
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
        [Route("Appeal/AppealImport")]
        public APIResult AppealImport(UploadData uploadData)
        {
            try
            {
                List<AppealDto> list = CommonHelper.DecodeString<List<AppealDto>>(uploadData.ListJson);
                foreach (AppealDto appealDto in list)
                {
                    Appeal appeal = new Appeal();
                    appeal.ProjectId = appealDto.ProjectId;
                    List<ShopDto> shopList = masterService.GetShop("", appealDto.BrandId.ToString(), "", appealDto.ShopCode, "", true);
                    if (shopList != null && shopList.Count > 0)
                    {
                        appeal.ShopId = shopList[0].ShopId;
                    }
                    List<SubjectDto> subjectList = masterService.GetSubject(appealDto.ProjectId.ToString(), "", appealDto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        appeal.SubjectId = Convert.ToInt32(subjectList[0].SubjectId);
                    }
                    appeal.LossResultImport = appealDto.LossResultImport;
                    appealService.SaveAppeal(appeal);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
    }
}
