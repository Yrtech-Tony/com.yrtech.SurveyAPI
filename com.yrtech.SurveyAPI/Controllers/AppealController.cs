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
        PhotoService photoService = new PhotoService();
        ImproveService improveService = new ImproveService();
        #region 申诉
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
        public APIResult GetAppealShopSet(string projectId, string shopCode = "")
        {
            try
            {
                List<AppealSetDto> list = new List<AppealSetDto>();
                if (!string.IsNullOrEmpty(shopCode))
                {
                    string[] shopCodeStr = shopCode.Replace("，",",").Split(',');
                    foreach (string shop in shopCodeStr)
                    {
                        list.AddRange(appealService.GetAppealShopSet(projectId, "", shop));
                    }
                }
                else
                {
                    list = appealService.GetAppealShopSet(projectId, "", shopCode);
                }
               
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
                    list = appealService.GetAppealShopSet(projectId, shopId.Replace(",", ""),"");
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
        public APIResult GetShopAppealInfoByPage(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount, string roleType = "",string shopType="")
        {
            try
            {
                if (roleType == "B_Shop")
                {
                    // 如果没有设置申诉时间，进行提醒，且不能查询到申诉数据
                    List<AppealSetDto> appealSetlist = appealService.GetAppealShopSet(projectId, shopIdStr.Replace(",", ""),"");
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
                if (!string.IsNullOrEmpty(shopType))
                {
                    list = list.Where(x => x.ShopType == shopType).ToList().OrderBy(x => x.ShopType).ThenBy(x=>x.SubjectCode).ToList();
                }
                else {
                    list = list.OrderBy(x => x.SubjectCode).ToList();
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
        public APIResult GetFeedBackInfoByPage(string projectId, string keyword, string userId="",string appealStatus="",string feedbackStatus="",int pageNum=1, int pageCount=30000)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.GetFeedBackInfoByPage(projectId, keyword,userId, appealStatus, feedbackStatus,pageNum, pageCount)) };
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
                    // 失分照片信息数量绑定
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(appeal.LossResult);

                    foreach (LossResultDto lossResultDto in lossResultList)
                    {
                        int lossPhotoCount = 0;// 失分照片数量
                        if (!string.IsNullOrEmpty(lossResultDto.LossFileNameUrl))
                        {
                            lossPhotoCount += lossResultDto.LossFileNameUrl.Split(';').Length;
                        }
                        if (lossPhotoCount == 0)
                        {
                            lossResultDto.LossPhotoCount = "0";
                        }
                        else
                        {
                            lossResultDto.LossPhotoCount = lossPhotoCount.ToString();
                        }
                    }
                    appeal.LossResult = CommonHelper.EncodeDto<string>(lossResultList);

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
        [HttpGet]
        [Route("Appeal/AppealFileDownLoad")]
        public APIResult AppealFileDownLoad(string projectId, string shopId)
        {
            try
            {
                string downloadPath = photoService.AppealFileDownLoad(projectId, shopId);
                if (string.IsNullOrEmpty(downloadPath))
                {
                    return new APIResult() { Status = false, Body = "没有可下载文件" };
                }

                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
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
        [Route("Appeal/AppealFeedBackDelete")]
        public APIResult AppealFeedBackDelete(string projectId,string shopId,string userId)
        {
            try
            {
                string[] shop = shopId.Split(',');
                foreach (string shopIdStr in shop)
                {
                    appealService.AppealFeedBackDelete(projectId, shopId,userId);
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
        // 目前Web端未使用到-20230709
        //[HttpPost]
        //[Route("Appeal/AppealDelete")]
        //public APIResult AppealDelete(Appeal appeal)
        //{
        //    try
        //    {
        //        appealService.AppealDelete(appeal.AppealId.ToString());
        //        return new APIResult() { Status = true, Body = "" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}

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
        public APIResult AppealFeedbackExport(string projectId, string keyword, string userId,string appealStatus="",int pageNum=1, int pageCount=30000)
        {
            try
            {
                string downloadPath = excelDataService.AppealFeedbackExport(projectId, keyword,userId, appealStatus,pageNum, pageCount);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        // 申诉导入
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
                // 导入之前先删除当前经销商的申诉信息
                foreach (AppealDto appealDto in list)
                {
                    string shopId = "";
                    List<ShopDto> shopList = masterService.GetShop("", appealDto.BrandId.ToString(), "", appealDto.ShopCode, "", true);
                    if (shopList != null && shopList.Count > 0)
                    {
                        shopId = shopList[0].ShopId.ToString();
                    }
                    if (!string.IsNullOrEmpty(shopId))
                    {
                        appealService.AppealDeleteByShopId(appealDto.ProjectId.ToString(), shopId,appealDto.ModifyUserId.ToString());
                    }
                }
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
                    
                    appealService.SaveAppeal(appeal, appealDto.ModifyUserId);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        // 申诉反馈导入
        [HttpGet]
        [Route("Appeal/AppealFeedBackExcelAnalysis")]
        public APIResult AppealFeedBackExcelAnalysis(string brandId, string projectId, string ossPath)
        {
            try
            {
                List<AppealDto> list = excelDataService.AppealFeedBackImport(ossPath);
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
        [Route("Appeal/AppealFeedBackImport")]
        public APIResult AppealFeedBackImport(UploadData uploadData)
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
                    appeal.FeedBackStatus = appealDto.FeedBackStatus;
                    appeal.FeedBackReason = appealDto.FeedBackReason;
                    appeal.FeedBackUserId = appealDto.FeedBackUserId;
                    appealService.AppealFeedBackBySubjectId(appeal);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 改善
        #region 导入改善
        [HttpGet]
        [Route("Appeal/ImproveExcelAnalysis")]
        public APIResult ImproveExcelAnalysis(string brandId, string projectId, string ossPath)
        {
            try
            {
                List<ImproveDto> list = excelDataService.ImproveImport(ossPath);
                foreach (ImproveDto improve in list)
                {
                    improve.ImportChk = true;
                    improve.ImportRemark = "";
                    List<ShopDto> shopList = masterService.GetShop("", brandId, "", improve.ShopCode, "", true);
                    if (shopList == null || shopList.Count == 0)
                    {
                        improve.ImportChk = false;
                        improve.ImportRemark += "经销商代码不存在" + ";";
                    }
                    List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", improve.SubjectCode, "");
                    if (subjectList == null || subjectList.Count == 0)
                    {
                        improve.ImportChk = false;
                        improve.ImportRemark += "题目代码不存在" + ";";
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
        [Route("Appeal/ImproveImport")]
        public APIResult ImproveImport(UploadData uploadData)
        {
            try
            {
                List<ImproveDto> list = CommonHelper.DecodeString<List<ImproveDto>>(uploadData.ListJson);
                // 导入之前先删除当前经销商的申诉信息
                foreach (ImproveDto improveDto in list)
                {
                    string shopId = "";
                    List<ShopDto> shopList = masterService.GetShop("", improveDto.BrandId.ToString(), "", improveDto.ShopCode, "", true);
                    if (shopList != null && shopList.Count > 0)
                    {
                        shopId = shopList[0].ShopId.ToString();
                    }
                    if (!string.IsNullOrEmpty(shopId))
                    {
                        improveService.DeleteImproveByShopId(improveDto.ProjectId.ToString(), shopId, improveDto.ModifyUserId.ToString());
                    }
                }
                foreach (ImproveDto improveDto in list)
                {
                    Improve improve = new Improve();
                    improve.ProjectId = improveDto.ProjectId;
                    List<ShopDto> shopList = masterService.GetShop("", improveDto.BrandId.ToString(), "", improveDto.ShopCode, "", true);
                    if (shopList != null && shopList.Count > 0)
                    {
                        improve.ShopId = shopList[0].ShopId;
                    }
                    List<SubjectDto> subjectList = masterService.GetSubject(improveDto.ProjectId.ToString(), "", improveDto.SubjectCode, "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        improve.SubjectId = Convert.ToInt32(subjectList[0].SubjectId);
                    }
                    improve.ImproveContent = improveDto.ImproveContent;
                    improve.ImproveCycle = improveDto.ImproveCycle;
                    improve.ImproveStatus = improveDto.ImproveStatus;
                    improve.InUserId = improveDto.InUserId;
                    improve.ModifyUserId = improveDto.ModifyUserId;
                    improveService.ImproveSave(improve);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 改善查询
        [HttpGet]
        [Route("Appeal/GetImprove")]
        public APIResult GetImprove(string projectId, string keyword, string improveStatus,string userId = "")
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(improveService.GetImprove(projectId, keyword, improveStatus, userId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetShopImprove")]
        public APIResult GetShopImprove(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(improveService.GetShopImprove(projectId,bussinessType,wideArea,bigArea,middleArea,smallArea,shopIdStr,"")) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetImproveDetail")]
        public APIResult GetImproveDetail(string improveId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(improveService.GetShopImproveDetail(improveId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetImproveDetailFile")]
        public APIResult GetImproveDetailFile(string improveId,string seqNO)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(improveService.GetShopImproveDetailFile(improveId,seqNO)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 改善保存
        [HttpPost]
        [Route("Appeal/SaveImprove")]
        public APIResult SaveImprove(Improve improve)
        {
            try
            {
                improveService.ImproveSave(improve);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        //[HttpPost]
        //[Route("Appeal/SaveImproveDetail")]
        //public APIResult SaveImproveDetail(UploadData uploadData)
        //{
        //    try
        //    {
        //        List<ImproveDetailDto> list = CommonHelper.DecodeString<List<ImproveDetailDto>>(uploadData.ListJson);
        //        foreach (ImproveDetailDto improveDetailDto in list)
        //        {
        //            ImproveDetail improveDetail = new ImproveDetail();
        //            improveDetail.ImproveId = improveDetailDto.ImproveId;
        //            improveDetail.SeqNO = improveDetailDto.SeqNO;
        //            improveDetail.CommitDateTime = improveDetailDto.CommitDateTime;
        //            improveDetail.CommitUserId = improveDetailDto.CommitUserId;
        //            improveDetail.EndDate = improveDetailDto.EndDate;
        //            improveDetail.ImproveDesc = improveDetailDto.ImproveDesc;
        //            improveDetail.ImproveFeedBackDateTime = improveDetailDto.ImproveFeedBackDateTime;
        //            improveDetail.ImproveFeedBackDesc = improveDetailDto.ImproveFeedBackDesc;
        //            improveDetail.ImproveFeedBackStatus = improveDetailDto.ImproveFeedBackStatus;
        //            improveDetail.ImproveFeedBackUserId = improveDetailDto.ImproveFeedBackUserId;
        //            improveDetail.InUserId = improveDetailDto.InUserId;

        //            if (improveDetail.SeqNO == 0)
        //            {
        //                improveDetail = improveService.ImproveDetailSave(improveDetail);
        //                foreach (AppealFile appealFile in appealDto.AppealFileList)
        //                {
        //                    appealFile.AppealId = appeal.AppealId;
        //                    appealService.AppealFileSave(appealFile);
        //                }
        //            }
        //            else// 申诉编辑时只保存申诉信息，文件信息在上传时会进行保存
        //            {
        //                appeal = appealService.AppealApply(appeal);
        //            }
        //            return new APIResult() { Status = true, Body = "" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        [HttpPost]
        [Route("Appeal/SaveImproveDetailFile")]
        public APIResult SaveImproveDetailFile(ImproveFile improveFile)
        {
            try
            {
                improveService.ImproveFileSave(improveFile);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #endregion
    }

}
