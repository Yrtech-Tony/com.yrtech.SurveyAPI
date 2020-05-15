using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class ReportFileController : ApiController
    {
        ReportFileService reportFileService = new ReportFileService();
        MasterService masterService = new MasterService();

        #region 文件上传
        [HttpGet]
        [Route("ReportFile/ReportFileListUploadSearch")]
        public APIResult ReportFileListUploadSearch(string brandId,string projectId, string keyword, int pageNum, int pageCount)
        {
            try
            {
                //List<object> resultList = new List<object>();
                // int total = reportFileService.ReportFileListUploadALLSearch(projectId, keyword).Count;
                //resultList.Add(total);
                // resultList.Add(reportFileService.ReportFileListUploadALLByPageSearch(projectId, keyword, pageNum, pageCount));
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileListUploadALLByPageSearch(brandId,projectId, keyword, pageNum, pageCount)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("ReportFile/ReportFileSearch")]
        public APIResult ReportFileSearch(string projectId, string shopId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileSearch(projectId, shopId)) };
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
                    List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "");
                    if (shopList == null || shopList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "上传的文件中存在未知的经销商代码，请确认命名" };
                    }
                }
                foreach (ReportFileDto reportFileDto in list)
                {
                    ReportFile reportFile = new ReportFile();
                    reportFile.ProjectId = reportFileDto.ProjectId;
                    List<ShopDto> shopList = masterservice.GetShop(reportFileDto.TenantId.ToString(), reportFileDto.BrandId.ToString(), "", reportFileDto.ShopCode, "");
                    if (shopList != null && shopList.Count > 0)
                    {
                        reportFile.ShopId = shopList[0].ShopId;
                    }
                    reportFile.InUserId = reportFileDto.InUserId;
                    reportFile.ReportFileName = reportFileDto.ReportFileName;
                    reportFile.ReportFileType = reportFileDto.ReportFileType;
                    reportFile.Url_OSS = reportFileDto.Url_OSS;
                    reportFileService.ReportFileSave(reportFile);
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
            MasterService masterservice = new MasterService();
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
        [Route("ReportFile/ReportFileDelete")]
        public APIResult ReportFileDelete(UploadData upload)
        {
            try
            {
                List<ReportFile> list = CommonHelper.DecodeString<List<ReportFile>>(upload.ListJson);
                foreach (ReportFile reportFile in list)
                {
                    reportFileService.ReportFileDelete(reportFile.ProjectId.ToString(), reportFile.ShopId.ToString(), reportFile.SeqNO.ToString());
                }
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
        public APIResult ReportFileListSearch(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            try
            {

                List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "");
                if (projectList != null && projectList.Count > 0 && !projectList[0].ReportDeployChk)
                {
                    return new APIResult() { Status = false, Body = "该期报告还未发布，请耐心等待通知" };
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileDownloadAllByPageSearch(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCount)) };
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
        public APIResult ReportFileCountYear()
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(reportFileService.ReportFileCountYear()) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion

    }
}
