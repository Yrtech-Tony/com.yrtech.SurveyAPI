using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;
using System.Collections;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class RecheckController : ApiController
    {
        RecheckService recheckService = new RecheckService();
        RecheckModifService recheckModifyService = new RecheckModifService();
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();

        #region 复审状态
        /// <summary>
        /// 各复审类型完毕时调用
        /// </summary>
        /// <param name="recheckStatusDtl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Recheck/SaveRecheckStatusDtl")]
        public APIResult SaveRecheckStatusDtl([FromBody]RecheckStatusDtl recheckStatusDtl)
        {
            try
            {
                List<AnswerDto> answerList = recheckService.GetNotRecheckSubject(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), recheckStatusDtl.RecheckTypeId.ToString());
                if (answerList != null && answerList.Count > 0)
                {
                    string subjectCode = "";
                    foreach (AnswerDto answer in answerList)
                    {
                        subjectCode += answer.SubjectCode + "\r\n";
                    }
                    throw new Exception("以下体系还未进行审核，请审核完毕后再进行提交: \r\n" + subjectCode);
                }
                recheckService.SaveRecheckStatusDtl(recheckStatusDtl);
                // 复审类型的个数
                int recheckTypeCount = masterService.GetSubjectRecheckType(recheckStatusDtl.ProjectId.ToString(), "").Count;
                // 已经复审的类型的个数
                int comRecheckTypeCount = recheckService.GetShopRecheckStautsDtl(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString()).Count;
                if (recheckTypeCount != 0 && recheckTypeCount == comRecheckTypeCount)
                {
                    ReCheckStatus status = new ReCheckStatus();
                    status.ProjectId = recheckStatusDtl.ProjectId;
                    status.InUserId = recheckStatusDtl.InUserId;
                    status.ShopId = Convert.ToInt32(recheckStatusDtl.ShopId);
                    status.StatusCode = "S3";
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
        /// <param name="recheckStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Recheck/SaveRecheckStatus")]
        public APIResult SaveRecheckStatus([FromBody]ReCheckStatus recheckStatus)
        {
            try
            {
                recheckStatus.StatusCode = "S3";
                recheckService.SaveRecheckStatus(recheckStatus);
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取经销商的复审状态
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetRecheckStatus")]
        public APIResult GetRecheckStatus(string projectId, string shopId, string statusCode)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStauts(projectId, shopId, statusCode)) };
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
        #region 复审详细
        /// <summary>
        /// 获取复审信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetShopRecheckInfo")]
        public APIResult GetShopRecheckInfo(string projectId, string shopId, string subjectId)
        {
            try
            {
                List<object> result = new List<object>();
                List<AnswerDto> answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, subjectId);
                List<RecheckDto> recheckList = recheckService.GetShopRecheckInfo(projectId, shopId, subjectId);
                // answer和recheck默认都只会有一条数据
                if (recheckList != null && recheckList.Count > 0)
                {
                    answerList[0].Recheck = recheckList[0];
                }
                result.Add(masterService.GetRecheckErrorType(projectId, ""));
                result.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(result) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveShopRecheckInfo")]
        public APIResult SaveShopRecheckInfo([FromBody]ReCheck recheck)
        {
            try
            {
                recheckService.SaveShopRecheckInfo(recheck);
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/GetShopNeedRecheckSubject")]
        public APIResult GetShopNeedRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
                List<SubjectFile> subjectFileList = new List<SubjectFile>();
                List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
                //获取体系信息
                List<Subject> subjectList = recheckService.GetShopNeedRecheckSubject(projectId, shopId, subjectRecheckTypeId);
                if (subjectList != null && subjectList.Count > 0)
                {
                    subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
                    subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
                    subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
                    subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "1"));
                    subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "2"));
                }
                resultList.Add(subjectList);
                resultList.Add(subjectInspectionStandardList);
                resultList.Add(subjectFileList);
                resultList.Add(subjectLossResultList);
                resultList.Add(subjectTypeScoreRegionList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/GetShopNextRecheckSubject")]
        public APIResult GetShopNextRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId, string orderNO)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
                List<SubjectFile> subjectFileList = new List<SubjectFile>();
                List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
                //获取体系信息
                List<Subject> subjectList = recheckService.GetShopNextRecheckSubject(projectId, shopId, subjectRecheckTypeId, orderNO);
                if (subjectList == null || subjectList.Count == 0)
                {
                    throw new Exception("已经是最后一题");
                }
                if (subjectList != null && subjectList.Count > 0)
                {
                    subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
                    subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
                    subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
                    subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "1"));
                    subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "2"));
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/GetShopPreRecheckSubject")]
        public APIResult GetShopPreRecheckSubject(string projectId, string shopId, string subjectRecheckTypeId, string orderNO)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
                List<SubjectFile> subjectFileList = new List<SubjectFile>();
                List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
                //获取体系信息
                List<Subject> subjectList = recheckService.GetShopPreRecheckSubject(projectId, shopId, subjectRecheckTypeId, orderNO);
                if (subjectList == null || subjectList.Count == 0)
                {
                    throw new Exception("已经是第一题了");
                }
                if (subjectList != null && subjectList.Count > 0)
                {
                    subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
                    subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
                    subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
                    subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "1"));
                    subjectTypeScoreRegionList.AddRange(masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), "2"));
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 复审修改
        /// <summary>
        /// 查询复审修改的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetRecheckModifyInfo")]
        public APIResult GetRecheckModifyInfo(string projectId, string shopId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckModifyService.GetNeedRecheckkModifyInfo(projectId, shopId, "", "")) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 复审修改保存
        /// </summary>
        /// <param name="recheck"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Recheck/SaveRecheckModifyInfo")]
        public APIResult SaveRecheckModifyInfo([FromBody]ReCheck recheck)
        {
            try
            {
                recheckModifyService.SaveRecheckModifyInfo(recheck.RecheckId.ToString(), recheck.AgreeCheck, recheck.AgreeReason, recheck.AgreeUserId);
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveRecheckModifyStatus")]
        public APIResult SaveRecheckModifyStatus([FromBody]ReCheckStatus recheckStatus)
        {
            try
            {
                List<RecheckDto> recheckModifyNotSave = recheckModifyService.GetNeedRecheckkModifyInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "", null);
                if (recheckModifyNotSave != null && recheckModifyNotSave.Count > 0)
                {
                    string subjectCode = "";
                    foreach (RecheckDto recheckModify in recheckModifyNotSave)
                    {
                        subjectCode += recheckModify.SubjectCode + "\r\n";
                    }
                    throw new Exception("以下体系还未进行同意与否操作，请操作完毕后再进行提交: \r\n" + subjectCode);
                }
                recheckStatus.StatusCode = "S4";
                recheckStatus.InDateTime = DateTime.Now;
                recheckService.SaveRecheckStatus(recheckStatus);
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
