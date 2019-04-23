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
    public class AppealController : ApiController
    {
        AppealService appealService = new AppealService();

        [HttpPost]
        [Route("Appeal/CreateAppealInfoByProject")]
        public APIResult CreateAppealInfoByProject([FromBody]string projectId)
        {
            try
            {
                appealService.CreateAppealInfoByProject(Convert.ToInt32(projectId));
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
                int total = appealService.GetShopAppealInfoByAll(projectId, businessType, wideArea, bigArea, middleArea, smallArea, shopIdStr,keyword).Count;
                resultList.Add(total);
                resultList.Add(appealService.GetShopAppealInfoByPage(projectId, businessType, wideArea, bigArea, middleArea, smallArea, shopIdStr,keyword, pageNum, pageCount));
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
        public APIResult AppealApply([FromBody]Appeal appeal)
        {
            try
            {
                appealService.AppealApply(appeal.AppealId, appeal.AppealReason, appeal.AppealUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFeedBack")]
        public APIResult AppealFeedBack([FromBody]Appeal appeal)
        {
            try
            {
                appealService.AppealFeedBack(appeal.AppealId, appeal.FeedBackStatus, appeal.FeedBackReason, appeal.FeedBackUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealShopAccept")]
        public APIResult AppealShopAccept([FromBody]Appeal appeal)
        {
            try
            {
                appealService.AppealShopAccept(appeal.AppealId, appeal.ShopAcceptStatus, appeal.ShopAcceptReason, appeal.ShopAcceptUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFileSave")]
        public APIResult AppealFileSave([FromBody]AppealFile appealFile)
        {
            try
            {
                appealService.AppealFileSave(Convert.ToInt32(appealFile.AppealId),appealFile.FileType,appealFile.FileName,appealFile.ServerFileName,Convert.ToInt32(appealFile.InUserId));
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Appeal/AppealFileDelete")]
        public APIResult AppealFileDelete([FromBody]AppealFile appeal)
        {
            try
            {
                appealService.AppealFileDelete(Convert.ToInt32(appeal.FileId));
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
