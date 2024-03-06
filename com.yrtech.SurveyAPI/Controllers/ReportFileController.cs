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
                        List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "", null);
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
                    List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "", null);
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
                    List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "", null);
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
        #region 文件上传-区域
        [HttpGet]
        [Route("ReportFile/ReportFileListUploadSearchArea")]
        public APIResult ReportFileListUploadSearchArea(string brandId, string projectId, string keyword, int pageNum, int pageCount)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileListUploadALLByPageSearch_Area(brandId, projectId, keyword, pageNum, pageCount)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileSearchArea")]
        public APIResult ReportFileSearchArea(string projectId, string areaId, string reportFileType)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileSearch_Area(projectId, areaId, reportFileType)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("ReportFile/ReportFileListSaveCheckArea")]
        public APIResult ReportFileListSaveCheckArea(UploadData upload)
        {
            MasterService masterservice = new MasterService();
            try
            {
                List<ReportFileAreaDto> list = CommonHelper.DecodeString<List<ReportFileAreaDto>>(upload.ListJson);
                foreach (ReportFileAreaDto reportFileDto in list)
                {
                    if (!string.IsNullOrEmpty(reportFileDto.AreaCode))
                    {
                        List<AreaDto> areaList = masterservice.GetArea("", reportFileDto.BrandId.ToString(), reportFileDto.AreaCode, "", "", "", null);
                        if (areaList != null && areaList.Count > 0)
                        {
                            reportFileDto.AreaCodeCheck = true;
                        }
                        else
                        {
                            reportFileDto.AreaCodeCheck = false;
                        }
                    }
                    else
                    {
                        reportFileDto.AreaCodeCheck = false;
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
        [Route("ReportFile/ReportFileListSaveArea")]
        public APIResult ReportFileListSaveArea(UploadData upload)
        {
            MasterService masterservice = new MasterService();
            try
            {
                List<ReportFileAreaDto> list = CommonHelper.DecodeString<List<ReportFileAreaDto>>(upload.ListJson);
                foreach (ReportFileAreaDto reportFileDto in list)
                {
                    List<AreaDto> areaList = masterservice.GetArea("", reportFileDto.BrandId.ToString(), reportFileDto.AreaCode, "", "", "", null);
                    if (areaList == null || areaList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "上传的文件中存在未知的区域代码，请确认品牌和区域代码" };
                    }
                }
                foreach (ReportFileAreaDto reportFileDto in list)
                {
                    ReportFileArea reportFile = new ReportFileArea();
                    reportFile.ProjectId = reportFileDto.ProjectId;
                    List<AreaDto> areaList = masterservice.GetArea("", reportFileDto.BrandId.ToString(), reportFileDto.AreaCode, "", "", "", null);
                    if (areaList != null && areaList.Count > 0)
                    {
                        reportFile.AreaId = areaList[0].AreaId;
                        reportFile.InUserId = reportFileDto.InUserId;
                        reportFile.ReportFileName = reportFileDto.ReportFileName;
                        reportFile.ReportFileType = reportFileDto.ReportFileType;
                        reportFile.Url_OSS = reportFileDto.Url_OSS;
                        reportFileService.ReportFileSave_Area(reportFile);
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
        [Route("ReportFile/ReportFileSaveArea")]
        public APIResult ReportFileSaveArea(ReportFileArea reportFile)
        {
            try
            {
                reportFileService.ReportFileSave_Area(reportFile);

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileDeleteArea")]
        public APIResult ReportFileDeleteArea(string projectId, string areaId, string seqNO)
        {
            try
            {
                reportFileService.ReportFileDelete_Area(projectId, areaId, seqNO);
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
        #region 文件下载-区域
        [HttpGet]
        [Route("ReportFile/ReportFileListSearchArea")]
        public APIResult ReportFileListSearchArea(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string keyword, string reportFileType, int pageNum, int pageCount)
        {
            try
            {
                List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                if (projectList != null && projectList.Count > 0 && !projectList[0].ReportDeployChk)
                {
                    return new APIResult() { Status = false, Body = "该期报告还未发布，请耐心等待通知" };
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileDownloadAllByPageSearch_Area(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, keyword, reportFileType, pageNum, pageCount)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileDownLoadArea")]
        public APIResult ReportFileDownLoadArea(string userId, string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string keyword, string reportFileType, int pageNum, int pageCount)
        {
            try
            {
                string downloadPath = reportFileService.ReportFileDownLoad_Area(userId, projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, keyword, reportFileType, pageNum, pageCount);
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
        [HttpGet]
        [Route("ReportFile/ReportShopCompleteCountAppealSearch")]
        public APIResult ReportShopCompleteCountAppealSearch(string projectId, string areaId, string shopType = "")
        {
            try
            {
                List<ReportShopCompleteCountDto> list = new List<ReportShopCompleteCountDto>();
                // 为空查询全国的数量，不为空查询当前区域的数量
                if (string.IsNullOrEmpty(areaId))
                {
                    list = reportFileService.ReportShopCompleteCountCountrySearch_Appeal(projectId, shopType);
                }
                else
                {
                    list = reportFileService.ReportShopCompleteCountSearch_Appeal(projectId, areaId, shopType);
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
                        else
                        {
                            shopType = "C";
                        }
                    }
                    list = reportFileService.ReportShopChapterScoreSearch(projectId, shopId, shopType);
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
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportAreaSubjectScoreSearch(projectId, areaId, chapterScore.ChapterId.ToString(), shopType);
                    }
                }
                else
                {
                    list = reportFileService.ReportCountryChapterScoreSearch(projectId, shopType);
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
        #region 数据报告-标准版
        // 本期得分和经销商排名情况
        [HttpGet]
        [Route("ReportFile/ReportProjectScoreSearch")]
        public APIResult ReportProjectScoreSearch(string year, string brandId, string projectId, string areaId, string provinceId, string shopId, int count, string shopType = "")
        {
            try
            {
                // 查询当年上期期号Id，用于查询上期得分
                string preProjectId = "";
                List<ProjectDto> preProjectList = masterService.GetPreProjectByProjectId(brandId, projectId, year);
                if (preProjectList != null && preProjectList.Count > 0)
                {
                    preProjectId = preProjectList[0].ProjectId.ToString();
                }
                List<ReportChapterScoreDto> list = new List<ReportChapterScoreDto>();
                // 查询经销商得分以及对应的区域得分和全国得分
                if (!string.IsNullOrEmpty(shopId))
                {
                    list = reportFileService.ReportShopSumScore(projectId, preProjectId, "", "", shopId, shopType);
                }
                // 查询省份得分及此省份内得分最高和最低的经销商得分
                else if (!string.IsNullOrEmpty(provinceId))
                {
                    list = reportFileService.ReportProvinceSumScore(projectId, preProjectId, provinceId, shopType);
                    List<ReportChapterScoreDto> reportChapterScoreDtoList = reportFileService.ReportShopSumScore(projectId, preProjectId, provinceId, "", "", shopType);
                    if (reportChapterScoreDtoList != null && reportChapterScoreDtoList.Count > 0)
                    {
                        ReportChapterScoreDto reportChapterScoreDtoMax = reportChapterScoreDtoList.OrderByDescending(x => x.SumScore).FirstOrDefault();
                        ReportChapterScoreDto reportChapterScoreDtoMin = reportChapterScoreDtoList.OrderByDescending(x => x.SumScore).LastOrDefault();
                        foreach (ReportChapterScoreDto dto in list)
                        {
                            dto.MaxScore = new ReportScoreMaxAndMin();
                            dto.MaxScore.ShopCode = reportChapterScoreDtoMax.ShopCode;
                            dto.MaxScore.ShopName = reportChapterScoreDtoMax.ShopName;
                            dto.MaxScore.SumScore = reportChapterScoreDtoMax.SumScore;
                            dto.MaxScore.PreSumScore = reportChapterScoreDtoMax.PreSumScore == null ? 0 : reportChapterScoreDtoMax.PreSumScore;
                            dto.MinScore = new ReportScoreMaxAndMin();
                            dto.MinScore.ShopCode = reportChapterScoreDtoMin.ShopCode;
                            dto.MinScore.ShopName = reportChapterScoreDtoMin.ShopName;
                            dto.MinScore.SumScore = reportChapterScoreDtoMin.SumScore;
                            dto.MinScore.PreSumScore = reportChapterScoreDtoMax.PreSumScore == null ? 0 : reportChapterScoreDtoMax.PreSumScore;
                        }
                    }
                }

                // 查询区域得分以及区域内的经销商最高分和最低分
                else if (!string.IsNullOrEmpty(areaId))
                {
                    list = reportFileService.ReportAreaSumScore(projectId, preProjectId, areaId, shopType);
                    List<ReportChapterScoreDto> reportChapterScoreDtoList_Shop = reportFileService.ReportShopSumScore(projectId, preProjectId, "", areaId, "", shopType);
                    if (reportChapterScoreDtoList_Shop != null && reportChapterScoreDtoList_Shop.Count > 0)
                    {
                        // 区域内最高分和最低分经销商
                        ReportChapterScoreDto reportChapterScoreDtoMax = reportChapterScoreDtoList_Shop.OrderByDescending(x => x.SumScore).FirstOrDefault();
                        ReportChapterScoreDto reportChapterScoreDtoMin = reportChapterScoreDtoList_Shop.OrderByDescending(x => x.SumScore).LastOrDefault();
                        foreach (ReportChapterScoreDto dto in list)
                        {
                            dto.MaxScore = new ReportScoreMaxAndMin();
                            dto.MaxScore.ShopCode = reportChapterScoreDtoMax.ShopCode;
                            dto.MaxScore.ShopName = reportChapterScoreDtoMax.ShopName;
                            dto.MaxScore.SumScore = reportChapterScoreDtoMax.SumScore;
                            dto.MaxScore.PreSumScore = reportChapterScoreDtoMax.PreSumScore == null ? 0 : reportChapterScoreDtoMax.PreSumScore;
                            dto.MinScore = new ReportScoreMaxAndMin();
                            dto.MinScore.ShopCode = reportChapterScoreDtoMin.ShopCode;
                            dto.MinScore.ShopName = reportChapterScoreDtoMin.ShopName;
                            dto.MinScore.SumScore = reportChapterScoreDtoMin.SumScore;
                            dto.MinScore.PreSumScore = reportChapterScoreDtoMin.PreSumScore == null ? 0 : reportChapterScoreDtoMin.PreSumScore;
                            // 区域内经销商排名
                            dto.ShopRankListTop = reportChapterScoreDtoList_Shop.OrderByDescending(x => x.SumScore).Take(count).ToList();
                            dto.ShopRankListLast = reportChapterScoreDtoList_Shop.Skip(reportChapterScoreDtoList_Shop.Count - count).Take(count).ToList();
                        }

                    }
                }
                // 查询全国得分最高以及最低的区域
                else
                {
                    list = reportFileService.ReportCountrySumScore(projectId, preProjectId, shopType);
                    List<ReportChapterScoreDto> reportChapterScoreDtoList_Area = reportFileService.ReportAreaSumScore(projectId, preProjectId, "", shopType);
                    List<ReportChapterScoreDto> reportChapterScoreDtoList_Shop = reportFileService.ReportShopSumScore(projectId, preProjectId, "", "", "", shopType);
                    // 得分最高和最低区域
                    if (reportChapterScoreDtoList_Area != null && reportChapterScoreDtoList_Area.Count > 0)
                    {
                        ReportChapterScoreDto reportChapterScoreDtoMax = reportChapterScoreDtoList_Area.OrderByDescending(x => x.SumScore).FirstOrDefault();
                        ReportChapterScoreDto reportChapterScoreDtoMin = reportChapterScoreDtoList_Area.OrderByDescending(x => x.SumScore).LastOrDefault();
                        foreach (ReportChapterScoreDto dto in list)
                        {
                            dto.MaxScore = new ReportScoreMaxAndMin();
                            dto.MaxScore.AreaCode = reportChapterScoreDtoMax.AreaCode;
                            dto.MaxScore.AreaName = reportChapterScoreDtoMax.AreaName;
                            dto.MaxScore.SumScore = reportChapterScoreDtoMax.SumScore;
                            dto.MaxScore.PreSumScore = reportChapterScoreDtoMax.PreSumScore == null ? 0 : reportChapterScoreDtoMax.PreSumScore;
                            dto.MinScore = new ReportScoreMaxAndMin();
                            dto.MinScore.AreaCode = reportChapterScoreDtoMin.AreaCode;
                            dto.MinScore.AreaName = reportChapterScoreDtoMin.AreaName;
                            dto.MinScore.SumScore = reportChapterScoreDtoMin.SumScore;
                            dto.MinScore.PreSumScore = reportChapterScoreDtoMin.PreSumScore == null ? 0 : reportChapterScoreDtoMin.PreSumScore;
                        }
                    }

                    if (reportChapterScoreDtoList_Shop != null && reportChapterScoreDtoList_Shop.Count > 0)
                    {
                        foreach (ReportChapterScoreDto dto in list)
                        {
                            // 全国经销商排名
                            dto.ShopRankListTop = reportChapterScoreDtoList_Shop.OrderByDescending(x => x.SumScore).Take(count).ToList();
                            dto.ShopRankListLast = reportChapterScoreDtoList_Shop.OrderByDescending(x => x.SumScore).Skip(reportChapterScoreDtoList_Shop.Count - count).Take(count).ToList();
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
        // 当前趋势表现
        [HttpGet]
        [Route("ReportFile/ReportYearTrend")]
        public APIResult ReportYearTrend(string year, string brandId, string areaId, string provinceId, string shopId, string shopType = "")
        {
            try
            {
                List<ReportYearTrendDto> yearTrendList = new List<ReportYearTrendDto>();
                List<ProjectDto> projectList = masterService.GetProject("", brandId, "", "", year, "");
                List<AreaDto> areaList = new List<AreaDto>();
                if (projectList != null)
                {
                    yearTrendList[0].ProjectList = projectList;
                }
                List<ReportChapterScoreDto> list = new List<ReportChapterScoreDto>();
                list = reportFileService.ReportYearCountryAreaTrend(year, brandId, areaId, shopType); // 全国和特定区域得分
                if (!string.IsNullOrEmpty(shopId)) // 选择经销商时查询：经销商、经销商所属区域、全国的趋势图
                {
                    list.AddRange(reportFileService.ReportYearShopTrend(year, brandId, shopId));
                }
                else if (!string.IsNullOrEmpty(provinceId)) // 选择省份时查询：省份、省份所属区域、全国趋势图
                {
                    list.AddRange(reportFileService.ReportYearProvinceTrend(year, brandId, provinceId, shopType));
                }
                else
                {
                    // 选择全国或区域的时候都显示全国及所有区域趋势图,
                    list = reportFileService.ReportYearCountryAreaTrend(year, brandId, "", shopType);
                }
                yearTrendList[0].YearTrendDataList = list;
                foreach (ReportChapterScoreDto reportChapterScoreDto in list)
                {
                    AreaDto area = new AreaDto();
                    area.AreaCode = reportChapterScoreDto.AreaCode;
                    area.AreaName = reportChapterScoreDto.AreaName;
                    areaList.Add(area);
                }
                areaList.GroupBy(d => new { d.AreaCode, d.AreaName }).Select(d => d.FirstOrDefault()).ToList(); // 去重
                yearTrendList[0].AreaList = areaList;
                return new APIResult() { Status = true, Body = CommonHelper.Encode(yearTrendList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        // 一级指标和二级指标得分
        [HttpGet]
        [Route("ReportFile/ReportChapterScoreSearchByProject")]
        public APIResult ReportChapterScoreSearchByProject(string projectId, string areaId, string provinceId, string shopId, string shopType = "")
        {
            try
            {
                List<ReportChapterScoreDto> list = new List<ReportChapterScoreDto>();
                // 经销商得分
                if (!string.IsNullOrEmpty(shopId))
                {
                    list = reportFileService.ReportShopChapterScoreSearch(projectId, shopId, shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportShopSubjectScoreSearch(projectId, shopId, chapterScore.ChapterId.ToString());
                    }
                }
                // 省份得分
                else if (!string.IsNullOrEmpty(provinceId))
                {
                    list = reportFileService.ReportProvinceChapterScoreSearch(projectId, provinceId, shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportProvinceSubjectScoreSearch(projectId, provinceId, chapterScore.ChapterId.ToString(), shopType);
                    }
                }
                // 区域得分
                else if (!string.IsNullOrEmpty(areaId))
                {
                    list = reportFileService.ReportAreaChapterScoreSearch(projectId, areaId, shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportAreaSubjectScoreSearch(projectId, areaId, chapterScore.ChapterId.ToString(), shopType);
                    }
                }
                else
                {
                    list = reportFileService.ReportCountryChapterScoreSearch(projectId, shopType);
                    foreach (ReportChapterScoreDto chapterScore in list)
                    {
                        chapterScore.ReportSubjectScoreList = reportFileService.ReportCountrySubjectScoreSearch(projectId, chapterScore.ChapterId.ToString(), shopType);
                    }
                }
                // 重新绑定全国得分
                foreach (ReportChapterScoreDto chapterScore in list)
                {
                    List<ReportChapterScoreDto> list_CountryChapter = reportFileService.ReportCountryChapterScoreSearch(projectId, shopType).Where(x => x.ChapterId == chapterScore.ChapterId).ToList();
                    if (list_CountryChapter != null && list_CountryChapter.Count > 0)
                    {
                        chapterScore.CountrySumScore = list_CountryChapter[0].Score;
                    }
                    foreach (ReportSubjectScoreDto subjectScore in chapterScore.ReportSubjectScoreList)
                    {
                        List<ReportSubjectScoreDto> list_CountrySubject = reportFileService.ReportCountrySubjectScoreSearch(projectId, chapterScore.ChapterId.ToString(), shopType).Where(x => x.SubjectId == subjectScore.SubjectId).ToList();
                        if (list_CountrySubject != null && list_CountrySubject.Count > 0)
                        {
                            subjectScore.CountrySumScore = list_CountrySubject[0].Score;
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
        #endregion
        #region 扣分细项
        [HttpGet]
        [Route("ReportFile/ReportShopLossResult")]
        public APIResult ReportShopLossResult(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword)
        {
            try
            {
                List<AnswerDto> answerList = reportFileService.ReportShopLossResult(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword);
                if (answerList == null || answerList.Count == 0)
                {
                    // kavos极狐项目使用，其他项目不使用
                    //  return new APIResult() { Status = false, Body = "申诉流程暂未结束，扣分细节页暂未开放" };
                }
                foreach (AnswerDto answer in answerList)
                {
                    //失分说明
                    string lossResultStr = "";
                    string lossResultFileNameUrl = "";
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
                            //if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                            //{
                            //    lossResultStr += lossResult.LossDesc2 + ";";
                            //}
                            if (!string.IsNullOrEmpty(lossResult.LossFileNameUrl))
                            {
                                lossResultFileNameUrl += lossResult.LossFileNameUrl + ";";
                            }
                        }
                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStr))
                    {
                        lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(lossResultFileNameUrl))
                    {
                        lossResultFileNameUrl = lossResultFileNameUrl.Substring(0, lossResultFileNameUrl.Length - 1);
                    }
                    answer.LossResultStr = lossResultStr;
                    answer.LossResultPicStr = lossResultFileNameUrl;
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
        public APIResult ReportDataCreate(string brandId, string projectId)
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
        [HttpGet]
        [Route("ReportFile/DBbak")]
        public APIResult DBbak()
        {
            try
            {
                FileService fs = new FileService();
                fs.DBFileBak();
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #endregion
        #region 岗位满足率
        [HttpGet]
        [Route("ReportFile/ReportBaseJobRateSearch")]
        public APIResult ReportBaseJobRateSearch(string projectId, string smallArea)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportBaseJobRateSearcht(projectId, smallArea)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportJobRateSearch")]
        public APIResult ReportJobRateSearch(string projectId, string smallArea)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportJobRateSearcht(projectId, smallArea)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportJobRateExcelAnalysis")]
        public APIResult ReportJobRateExcelAnalysis(string projectId, string ossPath)
        {
            try
            {
                List<ReportJobRateDto> list = excelDataService.ReportJobRateImport(ossPath);
                foreach (ReportJobRateDto reportJobRateDto in list)
                {
                    reportJobRateDto.ImportChk = true;
                    reportJobRateDto.ImportRemark = "";

                    List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                    List<AreaDto> areaList = masterService.GetArea("", projectList[0].BrandId.ToString(), reportJobRateDto.AreaCode, "", "", "", true);
                    if (areaList == null || areaList.Count == 0)
                    {
                        reportJobRateDto.ImportChk = false;
                        reportJobRateDto.ImportRemark += "区域代码未在系统登记" + ";";
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
        [Route("ReportFile/ReportJobRateImport")]
        public APIResult ReportJobRateImport(UploadData uploadData)
        {
            try
            {
                List<ReportJobRateDto> list = CommonHelper.DecodeString<List<ReportJobRateDto>>(uploadData.ListJson);
                // 先按区域删除数据
                foreach (ReportJobRateDto reportJobRateDto in list)
                {
                    reportFileService.ReportJobRateDelete(reportJobRateDto.ProjectId.ToString(), reportJobRateDto.AreaId.ToString());
                }
                // 导入数据
                foreach (ReportJobRateDto reportJobRateDto in list)
                {
                    ReportJobRate reportJobRate = new ReportJobRate();
                    reportJobRate.ProjectId = reportJobRateDto.ProjectId;
                    List<ProjectDto> projectList = masterService.GetProject("", "", reportJobRateDto.ProjectId.ToString(), "", "", "");
                    List<AreaDto> areaList = masterService.GetArea("", projectList[0].BrandId.ToString(), reportJobRateDto.AreaCode, "", "", "", true);
                    if (areaList != null && areaList.Count > 0)
                    {
                        reportJobRate.AreaId = areaList[0].AreaId;
                    }
                    reportJobRate.JobName = reportJobRateDto.JobName;
                    reportJobRate.JobFullCount = reportJobRateDto.JobFullCount;
                    reportJobRate.JobActualCount = reportJobRateDto.JobActualCount;
                    reportJobRate.InUserId = reportJobRateDto.InUserId;
                    reportFileService.SaveReportJobRate(reportJobRate);
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
