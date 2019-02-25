using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class AppealControl : ApiController
    {
        AppealService appealService = new AppealService();

        [HttpPost]
        [Route("Appeal/CreateAppealInfoByProject")]
        public APIResult CreateAppealInfoByProject(string projectId)
        {
            try
            {
                appealService.CreateAppealInfoByProject(projectId);
                return new APIResult() { Status = true, Body = "申诉阶段开始成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetShopAppealInfoByPage")]
        public APIResult GetShopAppealInfoByPage(string projectId,string businessType,string wideArea,string bigArea,string middleArea,string smallArea,string shopIdStr,string keyword, int pageNum, int pageCount)
        {
            try
            {
                List<object> resultList = new List<object>();
                int total = appealService.GetShopAppealInfoByAll(projectId, businessType, wideArea, bigArea, middleArea, smallArea, keyword, shopIdStr).Count;
                resultList.Add(total);
                resultList.Add(appealService.GetShopAppealInfoByPage(projectId, businessType, wideArea, bigArea, middleArea, smallArea, keyword, shopIdStr, pageNum, pageCount));
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
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
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.GetShopSubjectAppeal(appealId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/AppealFileSearch")]
        public APIResult AppealFileSearch(string appealId,string fileType)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.AppealFileSearch(appealId, fileType)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealApply")]
        public APIResult AppealApply(string appealId, string appealReason, int appealUserId)
        {
            try
            {
                appealService.AppealApply(appealId, appealReason, appealUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFeedBack")]
        public APIResult AppealFeedBack(string appealId, string feedbackStatus, string feedbackReason, int feedbackUserId)
        {
            try
            {
                appealService.AppealFeedBack(appealId, feedbackStatus, feedbackReason, feedbackUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealShopAccept")]
        public APIResult AppealShopAccept(string appealId, string shopAcceptStatus, string shopAcceptReason, int shopAcceptUserId)
        {
            try
            {
                appealService.AppealShopAccept(appealId, shopAcceptStatus, shopAcceptReason, shopAcceptUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFileSave")]
        public APIResult AppealFileSave(string appealId, int seqNo, string fileType, string fileName, string serverFileName, int userId)
        {
            try
            {
                appealService.AppealFileSave(appealId, seqNo, fileType, fileName, serverFileName, userId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFileSave")]
        public APIResult AppealFileDelete(string fileId)
        {
            try
            {
                appealService.AppealFileDelete(fileId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
