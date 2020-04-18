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


        [HttpGet]
        [Route("ReportFile/ReportFileListUploadSearch")]
        public APIResult ReportFileListUploadSearch(string projectId, string shopId, int pageNum, int pageCount)
        {
            try
            {
                List<object> resultList = new List<object>();
                int total = reportFileService.ReportFileListUploadALLSearch(projectId, shopId).Count;
                resultList.Add(total);
                resultList.Add(reportFileService.ReportFileListUploadALLByPageSearch(projectId, shopId, pageNum, pageCount));
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
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
        [Route("ReportFile/ReportFileSave")]
        public APIResult ReportFileSave(UploadData upload)
        {
            try
            {
                List<ReportFile> list = CommonHelper.DecodeString<List<ReportFile>>(upload.ListJson);
                foreach (ReportFile reportFile in list)
                {
                    reportFileService.ReportFileSave(reportFile);
                }
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
    }
}
