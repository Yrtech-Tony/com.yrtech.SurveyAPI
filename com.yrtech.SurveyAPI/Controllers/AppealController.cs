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
        public APIResult GetShopAppealInfoByPage(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCount)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(appealService.GetShopAppealInfoByPage(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCount)) };
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
                    appeal.AppealReason = appealDto.AppealReason;
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
    }
}
