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
    public class ReportFileController : ApiController
    {
        ReportFileService reportFileService = new ReportFileService();
        MasterService masterService = new MasterService();
        ExcelDataService excelDataService = new ExcelDataService();
        #region 报告设置
        [HttpGet]
        [Route("ReportFile/GetReportSet")]
        public APIResult GetReportSet(string projectId)
        {
            try
            {
                List<ReportSetDto> list = reportFileService.GetReportSet(projectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("ReportFile/SaveReportSet")]
        public APIResult SaveReportSet(ReportSet reportSet)
        {
            try
            {
                reportFileService.SaveReportSet(reportSet);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 文件上传
        [HttpGet]
        [Route("ReportFile/ReportFileListUploadSearch")]
        public APIResult ReportFileListUploadSearch(string brandId, string projectId, string bussinessTypeId, string keyword, int pageNum, int pageCount)
        {
            try
            {
                //List<object> resultList = new List<object>();
                // int total = reportFileService.ReportFileListUploadALLSearch(projectId, keyword).Count;
                //resultList.Add(total);
                // resultList.Add(reportFileService.ReportFileListUploadALLByPageSearch(projectId, keyword, pageNum, pageCount));
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileListUploadALLByPageSearch(brandId, projectId, bussinessTypeId, keyword, pageNum, pageCount)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileSearch")]
        public APIResult ReportFileSearch(string projectId, string bussinessTypeId, string shopId, string reportFileType)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileSearch(projectId, bussinessTypeId, shopId, reportFileType)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("ReportFile/ReportFileListSaveCheck")]
        public APIResult ReportFileListSaveCheck(UploadData upload)
        {
            MasterService masterservice = new MasterService();
            try
            {
                List<ReportFileDto> list = CommonHelper.DecodeString<List<ReportFileDto>>(upload.ListJson);
                foreach (ReportFileDto reportFileDto in list)
                {
                    if (!string.IsNullOrEmpty(reportFileDto.ShopCode))
                    {
                        List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "",null);
                        if (shopList != null && shopList.Count > 0)
                        {
                            reportFileDto.ShopCodeCheck = true;
                        }
                        else
                        {
                            reportFileDto.ShopCodeCheck = false;
                        }
                    }
                    else
                    {
                        reportFileDto.ShopCodeCheck = false;
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
        [Route("ReportFile/ReportFileListSave")]
        public APIResult ReportFileListSave(UploadData upload)
        {
            MasterService masterservice = new MasterService();
            try
            {
                List<ReportFileDto> list = CommonHelper.DecodeString<List<ReportFileDto>>(upload.ListJson);
                foreach (ReportFileDto reportFileDto in list)
                {
                    List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "",null);
                    if (shopList == null || shopList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "上传的文件中存在未知的经销商代码，请确认品牌和经销商代码" };
                    }
                }
                foreach (ReportFileDto reportFileDto in list)
                {
                    ReportFile reportFile = new ReportFile();
                    reportFile.ProjectId = reportFileDto.ProjectId;
                    reportFile.BussinessTypeId = reportFileDto.BussinessTypeId;
                    List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "",null);
                    if (shopList != null && shopList.Count > 0)
                    {
                        reportFile.ShopId = shopList[0].ShopId;
                        reportFile.InUserId = reportFileDto.InUserId;
                        reportFile.ReportFileName = reportFileDto.ReportFileName;
                        reportFile.ReportFileType = reportFileDto.ReportFileType;
                        reportFile.Url_OSS = reportFileDto.Url_OSS;
                        reportFileService.ReportFileSave(reportFile);
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
        [Route("ReportFile/ReportFileSave")]
        public APIResult ReportFileSave(ReportFile reportFile)
        {
            try
            {
                reportFileService.ReportFileSave(reportFile);

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileDelete")]
        public APIResult ReportFileDelete(string projectId, string shopId, string seqNO)
        {
            try
            {
                reportFileService.ReportFileDelete(projectId, shopId, seqNO);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        #endregion
        #region 文件下载
        [HttpGet]
        [Route("ReportFile/ReportFileListSearch")]
        public APIResult ReportFileListSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType, int pageNum, int pageCount)
        {
            try
            {
                List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                if (projectList != null && projectList.Count > 0 && !projectList[0].ReportDeployChk)
                {
                    return new APIResult() { Status = false, Body = "该期报告还未发布，请耐心等待通知" };
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileDownloadAllByPageSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, reportFileType, pageNum, pageCount)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileDownLoad")]
        public APIResult ReportFileDownLoad(string userId, string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, string reportFileType, int pageNum, int pageCount)
        {
            try
            {
                string downloadPath = reportFileService.ReportFileDownLoad(userId, projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, reportFileType, pageNum, pageCount);
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
        #endregion
        #region 得分查询
        [HttpGet]
        [Route("ReportFile/ShopAnswerSearch")]
        public APIResult ShopAnswerSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            try
            {
                List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                if (projectList != null && projectList.Count > 0 && !projectList[0].ReportDeployChk)
                {
                    return new APIResult() { Status = false, Body = "该期报告还未发布，请耐心等待通知" };
                }
                List<AnswerDto> answerList = reportFileService.ShopAnswerSearchByPageSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCount);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion

        #region 首页统计
        [HttpGet]
        [Route("ReportFile/ReportFileCountYear")]
        public APIResult ReportFileCountYear(string tenantId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileCountYear(tenantId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 报告上传下载记录
        [HttpPost]
        [Route("ReportFile/ReportFileActionLogSave")]
        public APIResult ReportFileActionLogSave(ReportFileActionLog reportFileActionLog)
        {
            try
            {
                reportFileService.ReportFileActionLogSave(reportFileActionLog);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileActionLogSearch")]
        public APIResult ReportFileActionLogSearch(string userId, string action, string account, string project, string reportFileName, string startDate, string endDate)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileActionLogSearch(userId, action, account, project, reportFileName, startDate, endDate)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportLogExport")]
        public APIResult ReportLogExport(string project, string reportFileName, string startDate, string endDate)
        {
            try
            {
                string downloadPath = excelDataService.ReportLogExport(project, reportFileName, startDate, endDate);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 数据报告
        #region 获取区域和全国执行进度
        [HttpGet]
        [Route("ReportFile/ReportShopCompleteCountSearch")]
        public APIResult ReportShopCompleteCountSearch(string projectId, string areaId, string shopType = "")
        {
            try
            {
                List<ReportShopCompleteCountDto> list = new List<ReportShopCompleteCountDto>();
                // 为空查询全国的数量，不为空查询当前区域的数量
                if (string.IsNullOrEmpty(areaId))
                {
                    list = reportFileService.ReportShopCompleteCountCountrySearch(projectId, shopType);
                }
                else
                {
                    list = reportFileService.ReportShopCompleteCountSearch(projectId, areaId, shopType);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 获取一级指标和二级指标的得分
        [HttpGet]
        [Route("ReportFile/ReportChapterScoreSearch")]
        public APIResult ReportChapterScoreSearch(string projectId, string areaId, string shopId, string shopType = "")
        {
            try
            {
                List<ReportChapterScoreDto> list = new List<ReportChapterScoreDto>();
                // 如果经销商不为空，按经销商查询
                if (!string.IsNullOrEmpty(shopId))
                {

                    ShopService shopservice = new ShopService();
                    // 专门针对极狐项目的特殊处理
                    List<ProjectShopExamTypeDto> shopList = shopservice.GetProjectShopExamType("", projectId, shopId);
                    if (shopList != null && shopList.Count > 0)
                    {
                        if (shopList[0].ExamTypeId == 18)
                        {
                            shopType = "A";
                        }
                        else if (shopList[0].ExamTypeId == 19)
                        {
                            shopType = "B";
                        }
                        else {
                            shopType = "C";
                        }
                    }
                    list = reportFileService.ReportShopChapterScoreSearch(projectId, shopId,shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportShopSubjectScoreSearch(projectId, shopId, chapterScore.ChapterId.ToString());
                    }
                    if (list == null || list.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "申诉流程暂未结束，检核快报暂未生成" };
                    }
                }
                // 区域为空查询全国的数量，不为空查询当前区域的数量
                else if (!string.IsNullOrEmpty(areaId))
                {
                    list = reportFileService.ReportAreaChapterScoreSearch(projectId, areaId, shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportAreaSubjectScoreSearch(projectId, areaId, chapterScore.ChapterId.ToString(),shopType);
                    }
                }
                else
                {
                    list = reportFileService.ReportCountryChapterScoreSearch(projectId,shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportCountrySubjectScoreSearch(projectId, chapterScore.ChapterId.ToString(), shopType);
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 扣分细项
        [HttpGet]
        [Route("ReportFile/ReportShopLossResult")]
        public APIResult ReportShopLossResult(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
        {
            try
            {
                //List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                //if (projectList != null && projectList.Count > 0 && !projectList[0].ReportDeployChk)
                //{
                //    return new APIResult() { Status = false, Body = "该期报告还未发布，请耐心等待通知" };
                //}
                List<AnswerDto> answerList = reportFileService.ReportShopLossResult(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword);
                if (answerList == null || answerList.Count == 0)
                {
                    return new APIResult() { Status = false, Body = "申诉流程暂未结束，扣分细节页暂未开放" };
                }
                foreach (AnswerDto answer in answerList)
                {
                    //失分说明
                    string lossResultStr = "";
                    if (!string.IsNullOrEmpty(answer.LossResult))
                    {
                        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                        // 去掉重复项，有可能2条失分说明中勾选了重复的失分说明
                        lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            if (!string.IsNullOrEmpty(lossResult.LossDesc))
                            {
                                lossResultStr += lossResult.LossDesc + ";";
                            }
                            if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                            {
                                lossResultStr += lossResult.LossDesc2 + ";";
                            }
                        }
                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStr))
                    {
                        lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                    }
                    answer.LossResultStr = lossResultStr;
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 数据报告生成
        [HttpGet]
        [Route("ReportFile/ReportDataCreate")]
        public APIResult ReportDataCreate(string brandId,string projectId)
        {
            try
            {
                reportFileService.ReportDataCreate(brandId, projectId);
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
