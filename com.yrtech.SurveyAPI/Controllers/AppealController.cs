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
    public class AppealController : ApiController
    {
        AppealService appealService = new AppealService();

        [HttpGet]
        [Route("Appeal/CreateAppealInfoByProject")]
        public APIResult CreateAppealInfoByProject(string projectId)
        {
            try
            {
                // appealService.CreateAppealInfoByProject(Convert.ToInt32(projectId));
                return new APIResult() { Status = true, Body = "申诉阶段开始成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Appeal/GetShopAppealInfoByPage")]
        public APIResult GetShopAppealInfoByPage(string projectId, string businessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.GetShopAppealInfoByPage(projectId, businessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCount)) };
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
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.GetShopSubjectAppeal(appealId)) };
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
                    appeal.AppealReason = appealDto.AppealReason;
                    appeal.AppealUserId = appealDto.AppealUserId;
                    appeal.CheckPoint = appealDto.CheckPoint;
                    appeal.LossResult = appealDto.LossResult;
                    appeal.ProjectId = appealDto.ProjectId;
                    appeal.Score = appealDto.Score;
                    appeal.ShopId = appealDto.ShopId;
                    appeal.SubjectCode = appealDto.SubjectCode;
                    appeal.SubjectId = appealDto.SubjectId;
                    appeal = appealService.AppealApply(appeal);
                    foreach (AppealFile appealFile in appealDto.AppealFileList)
                    {
                        appealFile.AppealId = appealDto.AppealId;
                        appealService.AppealFileSave(appealFile);
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
        public APIResult AppealFeedBack(Appeal appeal)
        {
            try
            {
                // appealService.AppealFeedBack(appeal.AppealId, appeal.FeedBackStatus, appeal.FeedBackReason, appeal.FeedBackUserId);
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
                // appealService.AppealShopAccept(appeal.AppealId, appeal.ShopAcceptStatus, appeal.ShopAcceptReason, appeal.ShopAcceptUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [Route("Appeal/AppealDelete")]
        public APIResult AppealDelete(Appeal appeal)
        {
            try
            {
                //appealService.AppealDelete(appeal.AppealId);
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
                appealService.AppealDelete(appealFile.FileId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
