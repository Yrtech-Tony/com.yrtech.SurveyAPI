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
        RecheckModifService recheckModifyService = new RecheckModifService();
        ArbitrationService arbitrationService = new ArbitrationService();
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        ExcelDataService excelDataService = new ExcelDataService();

        #region 复审状态
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
                // 提交审核时，验证
                if (recheckStatus.StatusCode == "S1")
                {
                    List<ReCheckStatus> recheckStatusList_S1 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S1");
                    if (recheckStatusList_S1 != null && recheckStatusList_S1.Count > 0)
                    {
                        throw new Exception("已提交审核，请勿重复提交");
                    }
                    List<AnswerDto> answerList = answerService.GetShopScoreInfo_NotAnswer(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString());
                    if (answerList != null && answerList.Count > 0)
                    {
                        throw new Exception("存在未打分的题目，请先打分完毕");
                    }
                }
                // 复审修改完毕时验证
                if (recheckStatus.StatusCode == "S4")
                {
                    // 验证是否所有题目都进行了同意与否操作
                    bool modifyFinish = true;
                    List<RecheckDto> notPassRecheckList = recheckModifyService.GetRecheckInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "",false, null);
                    foreach (RecheckDto recheck in notPassRecheckList)
                    {
                        if (recheck.AgreeCheck == null)
                        {
                            modifyFinish = false;
                            break;
                        }
                    }
                    if (!modifyFinish) {
                        throw new Exception("存在未进行同意与否操作的题目");
                    }
                    // 验证是否已经复审完毕
                    List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatus(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(),"");
                    if (recheckStatusList == null || (recheckStatusList != null && recheckStatusList.Count > 0 && recheckStatusList[0].Status_S3 == ""))
                    {
                        throw new Exception("该经销商还未复审完毕,不能进行提交");
                    }
                    // 验证是否已经提交过复审修改完毕
                    List<ReCheckStatus> recheckStatusList_S4 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S4");
                    if (recheckStatusList_S4 != null && recheckStatusList_S4.Count > 0)
                    {
                        throw new Exception("已复审修改完毕，请勿重复提交");
                    }
                }
                // 仲裁
                if (recheckStatus.StatusCode == "S5") {
                    List<ReCheckStatus> recheckStatusList_S4 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S4");
                    if (recheckStatusList_S4 == null || recheckStatusList_S4.Count==0)
                    {
                        throw new Exception("该经销商还未复审修改完毕");
                    }
                    List<ReCheckStatus> recheckStatusList_S5 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S5");
                    if (recheckStatusList_S5 != null && recheckStatusList_S5.Count > 0)
                    {
                        throw new Exception("已仲裁完毕，请勿重复提交");
                    }
                }
                // 督导抽查
                if (recheckStatus.StatusCode == "S6")
                {
                    // 验证是否已经复审完毕
                    List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatus(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(),"");
                    if (recheckStatusList == null || (recheckStatusList != null && recheckStatusList.Count > 0 && recheckStatusList[0].Status_S3 == ""))
                    {
                        throw new Exception("该经销商还未复审完毕,不能进行提交");
                    }
                    List<ReCheckStatus> recheckStatusList_S6 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S6");
                    if (recheckStatusList_S6 != null && recheckStatusList_S6.Count > 0)
                    {
                        throw new Exception("该经销商已复审抽查完毕，请勿重复提交");
                    }
                }
                // 项目经理抽查
                if (recheckStatus.StatusCode == "S7")
                {
                    // 验证是否已经复审完毕
                    List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatus(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(),"");
                    if (recheckStatusList == null || (recheckStatusList != null && recheckStatusList.Count > 0 && recheckStatusList[0].Status_S3 == ""))
                    {
                        throw new Exception("该经销商还未复审完毕,不能进行提交");
                    }
                    List<ReCheckStatus> recheckStatusList_S6 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S7");
                    if (recheckStatusList_S6 != null && recheckStatusList_S6.Count > 0)
                    {
                        throw new Exception("该经销商已复审抽查完毕，请勿重复提交");
                    }
                }
                recheckService.SaveRecheckStatus(recheckStatus);
                return new APIResult() { Status = true, Body = "" };
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
                // 验证所有的题目是否都复审完毕，需要补充
                List<AnswerDto> recheckList = recheckService.GetNotRecheckSubject(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), recheckStatusDtl.RecheckTypeId.ToString());
                if (recheckList != null && recheckList.Count != 0)
                {
                    throw new Exception("存在未复审的题目,请先复审完毕");
                }
                // 验证已经提交过的验证
                List<RecheckStatusDtlDto> recheckStatusDtlList = recheckService.GetShopRecheckStautsDtl(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), recheckStatusDtl.RecheckTypeId.ToString());
                if (recheckStatusDtlList != null && recheckStatusDtlList.Count != 0)
                {
                    throw new Exception("该经销商已提交复审,请勿重复提交");
                }
                recheckService.SaveRecheckStatusDtl(recheckStatusDtl);
                return new APIResult() { Status = true, Body = "" };
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
        public APIResult GetRecheckStatus(string projectId, string shopId,string shopCode="")
        {
            try
            {
                List<RecheckStatusDto> recheckStatusDtoList = recheckService.GetShopRecheckStatus(projectId, shopId, shopCode);
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
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S4))
                    { recheckStatus.Status_S4 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S5))
                    { recheckStatus.Status_S5 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S6))
                    { recheckStatus.Status_S6 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S7))
                    { recheckStatus.Status_S7 = "√"; }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckStatusDtoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/RecheckStatusExport")]
        public APIResult RecheckStatusExport(string projectId, string shopId, string shopCode = "")
        {
            try
            {
                string downloadPath = excelDataService.RecheckStatusExport(projectId, shopId, shopCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
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
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStautsDtl(projectId, shopId,"")) };
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
                List<RecheckStatusDto> recheckStatus = recheckService.GetShopRecheckStatus(projectId, shopId,"");
                if (recheckStatus == null || recheckStatus.Count == 0 || (recheckStatus != null && recheckStatus.Count > 0 && string.IsNullOrEmpty(recheckStatus[0].Status_S1)))
                {
                    throw new Exception("该经销商还未提复审");
                }
                List<RecheckDto> recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, subjectId, recheckTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
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
                List<SubjectDto> subjectList = masterService.GetSubject("", recheck.SubjectId.ToString(), "", "");
                string recheckTypeId = "";
                if (subjectList != null && subjectList.Count > 0)
                {
                    recheckTypeId = subjectList[0].LabelId_RecheckType.ToString();
                }
                List<RecheckStatusDtlDto> dtlList = recheckService.GetShopRecheckStautsDtl(recheck.ProjectId.ToString(), recheck.ShopId.ToString(), recheckTypeId);

                if (dtlList != null && dtlList.Count > 0 )
                {
                    throw new Exception("已复审完毕不能修改");
                }
                recheckService.SaveShopRecheckInfo(recheck);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 复审修改
        /// <summary>
        /// 查询到修改的数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetRecheckNotPass")]
        public APIResult GetRecheckNotPass(string projectId, string shopId, string subjectId)
        {
            try
            {
                List<RecheckStatusDto> recheckStatus = recheckService.GetShopRecheckStatus(projectId, shopId,"");
                if (recheckStatus == null|| recheckStatus.Count==0 || (recheckStatus != null && recheckStatus.Count > 0 && recheckStatus[0].Status_S3 == ""))
                {
                    throw new Exception("该经销商还未复审完毕");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckModifyService.GetRecheckInfo(projectId, shopId, subjectId,false, null)) };
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
        public APIResult SaveRecheckModifyInfo(ReCheck recheck)
        {
            try
            {

                List<RecheckStatusDto> recheckStatus = recheckService.GetShopRecheckStatus(recheck.ProjectId.ToString(), recheck.ShopId.ToString(),"");
                if (recheckStatus != null && recheckStatus.Count > 0 && !string.IsNullOrEmpty(recheckStatus[0].Status_S4))
                {
                    throw new Exception("该经销商已经复审修改完毕，不能进行修改");
                }
                recheckModifyService.SaveRecheckModifyInfo(recheck.RecheckId.ToString(), recheck.AgreeCheck, recheck.AgreeReason, recheck.AgreeUserId);

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 仲裁
        [HttpGet]
        [Route("Recheck/GetRecheckNotPassArbitrationInfo")]
        public APIResult GetRecheckNotPassArbitrationInfo(string projectId, string shopId, string subjectId)
        {
            try
            {
                List<RecheckStatusDto> recheckStatus = recheckService.GetShopRecheckStatus(projectId, shopId,"");
                if (recheckStatus == null || (recheckStatus != null && recheckStatus.Count > 0 && recheckStatus[0].Status_S4 == ""))
                {
                    throw new Exception("该经销商还未复审修改完毕");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckModifyService.GetRecheckInfo(projectId, shopId, subjectId, false,false)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveArbitrationInfo")]
        public APIResult SaveArbitrationInfo(ReCheck recheck)
        {
            try
            {
                arbitrationService.SaveArbitrationInfo(recheck.RecheckId.ToString(), recheck.LastConfirmCheck, recheck.LastConfirmReason, recheck.LastConfirmUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        #endregion
        #region 抽查
        [HttpGet]
        [Route("Recheck/GetSupervisionSpotCheck")]
        public APIResult GetSupervisionSpotCheck(string projectId, string shopId, string subjectId)
        {
            try
            {
                List<RecheckStatusDto> recheckStatus = recheckService.GetShopRecheckStatus(projectId, shopId,"");
                if (recheckStatus == null || (recheckStatus != null && recheckStatus.Count > 0 && recheckStatus[0].Status_S3 == ""))
                {
                    throw new Exception("该经销商还未复审完毕");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckModifyService.GetRecheckInfo(projectId, shopId, subjectId, null, null)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveSupervisionSpotCheck")]
        public APIResult SaveSupervisionSpotCheck(ReCheck recheck)
        {
            try
            {
                recheckModifyService.SaveSupervisionSpotCheck(recheck.RecheckId.ToString(), recheck.SupervisionSpotCheckContent, recheck.SupervisionSpotCheckUserId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SavePMSpotCheck")]
        public APIResult SavePMSpotCheck(ReCheck recheck)
        {
            try
            {
                recheckModifyService.SavePMSpotCheck(recheck.RecheckId.ToString(), recheck.PMSpotCheckContent, recheck.PMSpotCheckUserId);
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
