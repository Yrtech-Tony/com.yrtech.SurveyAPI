using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System.Collections;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class RecheckController : ApiController
    {
        RecheckService recheckService = new RecheckService();
        //RecheckModifService recheckModifyService = new RecheckModifService();
        ArbitrationService arbitrationService = new ArbitrationService();
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();

        #region 复审状态
        /// <summary>
        /// 各复审类型完毕时调用
        /// </summary>
        /// <param name="recheckStatusDtl"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("Recheck/SaveRecheckStatusDtl")]
        //public APIResult SaveRecheckStatusDtl([FromBody]RecheckStatusDtl recheckStatusDtl)
        //{
        //    try
        //    {
        //        List<AnswerDto> answerList = recheckService.GetNotRecheckSubject(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), recheckStatusDtl.RecheckTypeId.ToString());
        //        if (answerList != null && answerList.Count > 0)
        //        {
        //            string subjectCode = "";
        //            foreach (AnswerDto answer in answerList)
        //            {
        //                subjectCode += answer.SubjectCode + "\r\n";
        //            }
        //            throw new Exception("以下体系还未进行审核，请审核完毕后再进行提交: \r\n" + subjectCode);
        //        }
        //        recheckService.SaveRecheckStatusDtl(recheckStatusDtl);
        //        // 复审类型的个数
        //        int recheckTypeCount = masterService.GetSubjectRecheckType(recheckStatusDtl.ProjectId.ToString(), "").Count;
        //        // 已经复审的类型的个数
        //        int comRecheckTypeCount = recheckService.GetShopRecheckStautsDtl(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString()).Count;
        //        if (recheckTypeCount != 0 && recheckTypeCount == comRecheckTypeCount)
        //        {
        //            ReCheckStatus status = new ReCheckStatus();
        //            status.ProjectId = recheckStatusDtl.ProjectId;
        //            status.InUserId = recheckStatusDtl.InUserId;
        //            status.ShopId = Convert.ToInt32(recheckStatusDtl.ShopId);
        //            status.StatusCode = "S3";
        //            recheckService.SaveRecheckStatus(status);
        //        }
        //        return new APIResult() { Status = true, Body = "保存成功" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recheckStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Recheck/SaveRecheckStatus")]
        public APIResult SaveRecheckStatus(ReCheckStatus recheckStatus)
        {
            try
            {
                recheckService.SaveRecheckStatus(recheckStatus);
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveRecheckStatusDtl")]
        public APIResult SaveRecheckStatusDtl(RecheckStatusDtl recheckStatusDtl)
        {
            try
            {
                recheckService.SaveRecheckStatusDtl(recheckStatusDtl);
                List<RecheckStatusDtlDto> recheckStatusDtlList = recheckService.GetShopRecheckStautsDtl(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString());
                List<ProjectDto> projectList = masterService.GetProject("", "", recheckStatusDtl.ProjectId.ToString(), "", "", "");
                if (projectList != null && projectList.Count > 0)
                {
                    ReCheckStatus status = new ReCheckStatus();
                    status.InUserId = recheckStatusDtl.InUserId;
                    status.ProjectId = recheckStatusDtl.ProjectId;
                    status.ShopId = recheckStatusDtl.ShopId==null?0:Convert.ToInt32(recheckStatusDtl.ShopId);
                    List<Label> labelList = masterService.GetLabel(projectList[0].BrandId.ToString(), "", "RecheckType", true, "");
                    if (recheckStatusDtlList != null && labelList != null && recheckStatusDtlList.Count == labelList.Count)
                    {
                        status.StatusCode = "S3";
                    }
                    else
                    {
                        status.StatusCode = "S2";
                    }
                    recheckService.SaveRecheckStatus(status);
                }
               
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetRecheckStatus")]
        public APIResult GetRecheckStatus(string projectId, string shopId)
        {
            try
            {
                List<RecheckStatusDto> recheckStatusDtoList = recheckService.GetShopRecheckStatus(projectId, shopId);
                foreach (RecheckStatusDto recheckStatus in recheckStatusDtoList)
                {
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S0))
                    { recheckStatus.Status_S0 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S1))
                    { recheckStatus.Status_S1 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S2))
                    { recheckStatus.Status_S2 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S3))
                    { recheckStatus.Status_S3 = "√"; }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckStatusDtoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取各复审类型的状态
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetRecheckStatusDtl")]
        public APIResult GetRecheckStatusDtl(string projectId, string shopId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStautsDtl(projectId, shopId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 复审管理
        /// <summary>
        /// 复审清单
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <param name="recheckTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetShopRecheckScoreInfo")]
        public APIResult GetShopRecheckScoreInfo(string projectId, string shopId, string subjectId, string recheckTypeId)
        {
            try
            {
                List<RecheckDto> recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, subjectId, recheckTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        //[HttpPost]
        //[Route("Recheck/SaveShopRecheckInfo")]
        //public APIResult SaveShopRecheckInfo([FromBody]ReCheck recheck)
        //{
        //    try
        //    {
        //        recheckService.SaveShopRecheckInfo(recheck);
        //        return new APIResult() { Status = true, Body = "保存成功" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        //[HttpGet]
        //[Route("Recheck/GetShopNeedRecheckSubject")]
        //public APIResult GetShopNeedRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId)
        //{
        //    try
        //    {
        //        List<object> resultList = new List<object>();
        //        List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
        //        List<SubjectFile> subjectFileList = new List<SubjectFile>();
        //        List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
        //        List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
        //        //获取体系信息
        //        List<Subject> subjectList = recheckService.GetShopNeedRecheckSubject(projectId, shopId, subjectRecheckTypeId);
        //        if (subjectList != null && subjectList.Count > 0)
        //        {
        //            subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
        //            subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
        //            subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
        //            subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "1"));
        //            subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "2"));
        //        }
        //        resultList.Add(subjectList);
        //        resultList.Add(subjectInspectionStandardList);
        //        resultList.Add(subjectFileList);
        //        resultList.Add(subjectLossResultList);
        //        resultList.Add(subjectTypeScoreRegionList);
        //        return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        [HttpGet]
        [Route("Recheck/GetShopNextRecheckSubject")]
        public APIResult GetShopNextRecheckSubject(string projectId, string shopId, string recheckTypeId, string orderNO)
        {
            try
            {
                //获取体系信息
                List<AnswerDto> answerList = recheckService.GetShopNextRecheckSubject(projectId, shopId, recheckTypeId, orderNO);
                if (answerList == null || answerList.Count == 0)
                {
                    throw new Exception("已经是最后一题");
                }
                if (answerList != null && answerList.Count > 0)
                {
                    answerList[0].SubjectFileList = masterService.GetSubjectFile(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectLossResultList = masterService.GetSubjectLossResult(projectId, answerList[0].SubjectId.ToString());
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/GetShopPreRecheckSubject")]
        public APIResult GetShopPreRecheckSubject(string projectId, string shopId, string recheckTypeId, string orderNO)
        {
            try
            {
                //获取体系信息
                List<AnswerDto> answerList = recheckService.GetShopPreRecheckSubject(projectId, shopId, recheckTypeId, orderNO);
                if (answerList == null || answerList.Count == 0)
                {
                    throw new Exception("已经是第一题");
                }
                if (answerList != null && answerList.Count > 0)
                {
                    answerList[0].SubjectFileList = masterService.GetSubjectFile(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectLossResultList = masterService.GetSubjectLossResult(projectId, answerList[0].SubjectId.ToString());
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recheck"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Recheck/SaveShopRecheckInfo")]
        public APIResult SaveShopRecheckInfo(ReCheck recheck)
        {
            try
            {
                recheckService.SaveShopRecheckInfo(recheck);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        //#region 复审修改
        ///// <summary>
        ///// 查询复审修改的信息
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <param name="shopId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Recheck/GetRecheckModifyInfo")]
        //public APIResult GetNeedRecheckkModifyInfo(string projectId, string shopId,string subjectId)
        //{
        //    try
        //    {
        //        return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckModifyService.GetNeedRecheckkModifyInfo(projectId, shopId, subjectId, "")) };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        ///// <summary>
        ///// 复审修改保存
        ///// </summary>
        ///// <param name="recheck"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Recheck/SaveRecheckModifyInfo")]
        //public APIResult SaveRecheckModifyInfo([FromBody]ReCheck recheck)
        //{
        //    try
        //    {
        //        List<RecheckStatusDto> statusLogList = recheckService.GetShopRecheckStatusLog(recheck.ProjectId.ToString(), recheck.ShopId.ToString(), "S4");
        //        if (statusLogList != null && statusLogList.Count == 0)
        //        {
        //            recheckModifyService.SaveRecheckModifyInfo(recheck.RecheckId.ToString(), recheck.AgreeCheck, recheck.AgreeReason, recheck.AgreeUserId);
        //        }
        //        else {
        //            throw new Exception("该经销商已经复审修改完毕，不能进行修改");
        //        }
        //        return new APIResult() { Status = true, Body = "保存成功" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        //[HttpPost]
        //[Route("Recheck/SaveRecheckModifyStatus")]
        //public APIResult SaveRecheckModifyStatus([FromBody]ReCheckStatus recheckStatus)
        //{
        //    try
        //    {
        //        List<RecheckDto> recheckModifyNotSave = recheckModifyService.GetNeedRecheckkModifyInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "", null);
        //        if (recheckModifyNotSave != null && recheckModifyNotSave.Count > 0)
        //        {
        //            string subjectCode = "";
        //            foreach (RecheckDto recheckModify in recheckModifyNotSave)
        //            {
        //                subjectCode += recheckModify.SubjectCode + "\r\n";
        //            }
        //            throw new Exception("以下体系还未进行同意与否操作，请操作完毕后再进行提交: \r\n" + subjectCode);
        //        }
        //        recheckStatus.StatusCode = "S4";
        //        recheckStatus.InDateTime = DateTime.Now;
        //        recheckService.SaveRecheckStatus(recheckStatus);
        //        return new APIResult() { Status = true, Body = "保存成功" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        //#endregion
        #region 仲裁
        [HttpGet]
        [Route("Recheck/GetArbitrabitionInfo")]
        public APIResult GetArbitrabitionInfo(string projectId, string shopId, string subjectId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(arbitrationService.GetNeedArbitrationInfo(projectId, shopId, subjectId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveArbitrationInfo")]
        public APIResult SaveArbitrationInfo([FromBody]ReCheck recheck)
        {
            try
            {

                arbitrationService.SaveArbitrationInfo(recheck.RecheckId.ToString(), recheck.LastConfirmCheck, recheck.LastConfirmReason, recheck.LastConfirmUserId);

                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        #endregion
    }
}
