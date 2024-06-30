using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System.Linq;
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
        ShopService shopService = new ShopService();

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
                    List<RecheckStatusDto> recheckStatusList_S1 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S1");
                    if (recheckStatusList_S1 != null && recheckStatusList_S1.Count > 0)
                    {
                        throw new Exception("已提交审核，请勿重复提交");
                    }
                    // 获取经销商的试卷类型
                    string labelId = "";
                    List<ProjectShopExamTypeDto> projectShopExamTypeList = shopService.GetProjectShopExamType("", recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString());
                    if (projectShopExamTypeList != null && projectShopExamTypeList.Count > 0)
                    {
                        labelId = projectShopExamTypeList[0].ExamTypeId.ToString();
                    }
                    List<AnswerDto> answerList = answerService.GetShopScoreInfo_NotAnswer(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), labelId);
                    if (answerList != null && answerList.Count > 0)
                    {
                        throw new Exception("存在未打分的题目，请先打分完毕");
                    }

                    // 验证照片是否已经全部上传
                    bool photoUpload = true;
                    // 经销商自检不进行照片上传验证
                    List<ProjectDto> projectList = masterService.GetProject("", "", recheckStatus.ProjectId.ToString(), "", "", "");
                    if (projectList != null && projectList.Count > 0 && projectList[0].SelfTestChk == false)
                    {
                        List<AnswerPhotoLogDto> photoList = answerService.GetShopAnsewrPhotoLog(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString());
                        foreach (AnswerPhotoLogDto answerPhoto in photoList)
                        {
                            if (answerPhoto.UploadStatus == "0")
                            {
                                photoUpload = false;
                                break;
                            }
                        }
                        if (!photoUpload)
                        {
                            throw new Exception("存在未上传的照片，请先在上传管理一键上传");
                        }
                    }
                }
                /*一审复审完毕是按照类型提交的，不在此处进行验证*/
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
                    if (recheckStatusList == null || recheckStatusList.Count == 0 || (recheckStatusList != null && recheckStatusList.Count > 0 && recheckStatusList[0].Status_S3 == ""))
                    {
                        throw new Exception("该经销商还未复审完毕,不能进行提交");
                    }
                    // 验证是否已经提交过复审修改完毕
                    List<RecheckStatusDto> recheckStatusList_S4 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S4");
                    if (recheckStatusList_S4 != null && recheckStatusList_S4.Count > 0)
                    {
                        throw new Exception("已复审修改完毕，请勿重复提交");
                    }
                }
                //一审确认
                if (recheckStatus.StatusCode == "S8")
                {
                    List<RecheckStatusDto> recheckStatusList_S4 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S4");
                    if (recheckStatusList_S4 == null || recheckStatusList_S4.Count == 0)
                    {
                        throw new Exception("该经销商还未复审修改完毕");
                    }
                    List<RecheckStatusDto> recheckStatusList_S8 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S8");
                    if (recheckStatusList_S8 != null && recheckStatusList_S8.Count > 0)
                    {
                        throw new Exception("已一审确认完毕，请勿重复提交");
                    }
                }
                //二审完毕
                if (recheckStatus.StatusCode == "S9")
                {
                    bool modifyFinish = true;
                    List<RecheckDto> notPassRecheckList = recheckModifyService.GetRecheckInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "", null, null);
                    foreach (RecheckDto recheck in notPassRecheckList)
                    {
                        if (recheck.PassReCheck_Sec == null)
                        {
                            modifyFinish = false;
                            break;
                        }
                    }
                    if (!modifyFinish)
                    {
                        throw new Exception("存在未二审的题目,请全部复审完再提交");
                    }
                    List<RecheckStatusDto> recheckStatusList_S9 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S9");
                    if (recheckStatusList_S9 != null && recheckStatusList_S9.Count > 0)
                    {
                        throw new Exception("已二审完毕，请勿重复提交");
                    }
                }
                // 仲裁
                if (recheckStatus.StatusCode == "S5") {
                    List<RecheckStatusDto> recheckStatusList_S4 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S4");
                    if (recheckStatusList_S4 == null || recheckStatusList_S4.Count==0)
                    {
                        throw new Exception("该经销商还未复审修改完毕");
                    }
                    List<RecheckStatusDto> recheckStatusList_S5 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S5");
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
                    List<RecheckStatusDto> recheckStatusList_S6 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S6");
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
                    List<RecheckStatusDto> recheckStatusList_S7 = recheckService.GetShopRecheckStatusInfo(recheckStatus.ProjectId.ToString(), recheckStatus.ShopId.ToString(), "S7");
                    if (recheckStatusList_S7 != null && recheckStatusList_S7.Count > 0)
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
                // 验证所有的题目是否都复审完毕，
                List<AnswerDto> recheckList = recheckService.GetNotRecheckSubject(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), recheckStatusDtl.RecheckTypeId.ToString());
                if (recheckList != null && recheckList.Count != 0)
                {
                    throw new Exception("存在未复审的题目,请先一审完毕");
                }
                // 验证已经提交过复审
                List<RecheckStatusDtlDto> recheckStatusDtlList = recheckService.GetShopRecheckStautsDtl(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), recheckStatusDtl.RecheckTypeId.ToString());
                if (recheckStatusDtlList != null && recheckStatusDtlList.Count != 0)
                {
                    throw new Exception("该经销商此类型已提交复审,请勿重复提交");
                }
                recheckService.SaveRecheckStatusDtl(recheckStatusDtl);

                // 若所有题目都复审完毕，且全部通过，则自动复审修改完毕和自动复审确认完毕
                // 查询该店的复审未通过数据信息
                List<RecheckDto> recheckNotPassList  = recheckModifyService.GetRecheckInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "", false, null);

                // 查询该店当前的状态
                List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatus(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "");

                // 复审完毕，且全部通过审核
                if (recheckStatusList != null && recheckStatusList.Count > 0 && !string.IsNullOrEmpty(recheckStatusList[0].Status_S3)
                    && (recheckNotPassList == null || recheckNotPassList.Count == 0))
                {
                    // 自动复审修改完毕
                    ReCheckStatus recheckStatus = new ReCheckStatus();
                    recheckStatus.ProjectId = recheckStatusDtl.ProjectId;
                    recheckStatus.ShopId = Convert.ToInt32(recheckStatusDtl.ShopId);
                    recheckStatus.StatusCode = "S4";
                    recheckStatus.InUserId = recheckStatusDtl.InUserId;
                    recheckService.SaveRecheckStatus(recheckStatus);
                    // 自动复审确认完毕
                    ReCheckStatus recheckStatus_Confirm = new ReCheckStatus();
                    recheckStatus_Confirm.ProjectId = recheckStatusDtl.ProjectId;
                    recheckStatus_Confirm.ShopId = Convert.ToInt32(recheckStatusDtl.ShopId);
                    recheckStatus_Confirm.StatusCode = "S8";
                    recheckStatus_Confirm.InUserId = recheckStatusDtl.InUserId;
                    recheckService.SaveRecheckStatus(recheckStatus_Confirm);
                }
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
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S8))
                    { recheckStatus.Status_S8 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S5))
                    { recheckStatus.Status_S5 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S9))
                    { recheckStatus.Status_S9 = "√"; }
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
        /// <summary>
        /// 查询当前经销商所有一审类型的审核状态
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetShopRecheckStautsDtlForAllType")]
        public APIResult GetShopRecheckStautsDtlForAllType(string projectId, string shopId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStautsDtlForAllType(projectId, shopId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        // 按状态查询复审信息
        [HttpGet]
        [Route("Recheck/GetShopRecheckStatus")]
        public APIResult GetShopRecheckStatus(string projectId, string shopId,string statusCode)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStatusInfo(projectId,shopId,statusCode)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 复审管理
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Recheck/GetRecheckStatusForFirstRecheck")]
        public APIResult GetRecheckStatusForFirstRecheck(string projectId, string shopId, string shopCode = "")
        {
            try
            {
                // 过滤出需要复审或者需要复审确认的经销商,即，S1不为空且S3为空或者s8为空且S4不为空
                List<RecheckStatusDto> recheckStatusDtoList = recheckService.GetShopRecheckStatus(projectId, shopId, shopCode).Where(x=>(string.IsNullOrEmpty(x.Status_S3)&&!string.IsNullOrEmpty(x.Status_S1))||(string.IsNullOrEmpty(x.Status_S8)&&!string.IsNullOrEmpty(x.Status_S4))).ToList();
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
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S8))
                    { recheckStatus.Status_S8 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S5))
                    { recheckStatus.Status_S5 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S9))
                    { recheckStatus.Status_S9 = "√"; }
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
        public APIResult GetShopRecheckScoreInfo(string projectId, string shopId, string subjectId, string recheckTypeId,bool? passRecheck=null)
        {
            try
            {
                // 在页面默认显示需要复审的经销商，所以验证去掉
                //List<RecheckStatusDto> recheckStatus = recheckService.GetShopRecheckStatus(projectId, shopId,"");
                //if (recheckStatus == null || recheckStatus.Count == 0 || (recheckStatus != null && recheckStatus.Count > 0 && string.IsNullOrEmpty(recheckStatus[0].Status_S1)))
                //{
                //    throw new Exception("该经销商还未提复审");
                //}
                List<RecheckDto> recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, subjectId, recheckTypeId);
                if (passRecheck != null) {
                    if (passRecheck == true)
                    {
                        recheckList = recheckList.Where(x => x.PassRecheck == passRecheck).ToList();
                    }
                    else {
                        recheckList = recheckList.Where(x => x.PassRecheck == passRecheck|| x.PassRecheck==null).ToList();
                    }
                }
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
                //List<SubjectDto> subjectList = masterService.GetSubject("", recheck.SubjectId.ToString(), "", "");
                //string recheckTypeId = "";
                //if (subjectList != null && subjectList.Count > 0)
                //{
                //    recheckTypeId = subjectList[0].LabelId_RecheckType.ToString();
                //}
                //List<RecheckStatusDtlDto> dtlList = recheckService.GetShopRecheckStautsDtl(recheck.ProjectId.ToString(), recheck.ShopId.ToString(), recheckTypeId);
                //if (dtlList != null && dtlList.Count > 0 )
                //{
                //    throw new Exception("已复审完毕不能修改");
                //}
             
                //List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatus(recheck.ProjectId.ToString(), recheck.ShopId.ToString(), "");
                //if (!(recheckStatusList != null && recheckStatusList.Count > 0 && !string.IsNullOrEmpty(recheckStatusList[0].Status_S3)))
                //{
                //    throw new Exception("已复审完毕不能修改");
                //}
                recheckService.SaveShopRecheckInfo(recheck);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveShopRecheckConfirmInfo")]
        public APIResult SaveShopRecheckConfirmInfo(ReCheck recheck)
        {
            try
            {
                recheckService.SaveRecheckConfirmInfo(recheck.RecheckId.ToString(),recheck.PassReCheck_Confirm,recheck.ReCheckContent_Confirm,recheck.ReCheckError_Confirm,recheck.ReCheckScore_Confirm,recheck.ReCheckUserId_Confirm);
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
        #region 二审管理
        [HttpGet]
        [Route("Recheck/GetRecheckStatusForSecondRecheck")]
        public APIResult GetRecheckStatusForSecondRecheck(string projectId, string shopId, string shopCode = "")
        {
            try
            {
                // 过滤出需要二审经销商,即，S8不为空且S9为空
                List<RecheckStatusDto> recheckStatusDtoList = recheckService.GetShopRecheckStatus(projectId, shopId, shopCode).Where(x => !string.IsNullOrEmpty(x.Status_S8) && string.IsNullOrEmpty(x.Status_S9)).ToList();
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
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S8))
                    { recheckStatus.Status_S8 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S5))
                    { recheckStatus.Status_S5 = "√"; }
                    if (!string.IsNullOrEmpty(recheckStatus.Status_S9))
                    { recheckStatus.Status_S9 = "√"; }
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
        [HttpPost]
        [Route("Recheck/SaveShopRecheckSecondInfo")]
        public APIResult SaveShopRecheckSecondInfo(ReCheck recheck)
        {
            try
            {
                recheckService.SaveRecheckSecondInfo(recheck.RecheckId.ToString(), recheck.PassReCheck_Sec, recheck.ReCheckContent_Sec, recheck.ReCheckError_Sec, recheck.ReCheckScore_Sec, recheck.ReCheckUserId_Sec);
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
        #region 运营中心
        [HttpGet]
        [Route("Recheck/GetRecheckStatusList")]
        public APIResult GetRecheckStatusList(string projectId, string shopId)
        {
            try
            {
                /*
                 * 一审各类型和其他审核状态合并到一块输出
                 */
                // 一审各类型的审核信息
                List<RecheckStatusDtlDto> recheckStatusDtlList = new List<RecheckStatusDtlDto>();
                // 其他审核信息
                List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatusInfo(projectId, shopId, "");
                 recheckStatusDtlList= recheckService.GetShopRecheckStautsDtl(projectId, shopId, "");
                foreach (RecheckStatusDto status in recheckStatusList)
                {
                    RecheckStatusDtlDto recheckStatusDtl = new RecheckStatusDtlDto();
                    recheckStatusDtl.ProjectId = status.ProjectId;
                    recheckStatusDtl.ProjectCode = status.ProjectCode;
                    recheckStatusDtl.ProjectName = status.ProjectName;
                    recheckStatusDtl.ShopCode = status.ShopCode;
                    recheckStatusDtl.ShopName = status.ShopName;
                    recheckStatusDtl.ShopId = status.ShopId;
                    recheckStatusDtl.RecheckStatusId = status.RecheckStatusId;
                    recheckStatusDtl.RecheckTypeCode = status.StatusCode;// 复审状态代码
                    recheckStatusDtl.RecheckTypeName = status.StatusName;// 复审状态名称
                    recheckStatusDtl.InDateTime = status.InDateTime;
                    recheckStatusDtlList.Add(recheckStatusDtl);
                }
                recheckStatusDtlList.OrderByDescending(x => x.InDateTime).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckStatusDtlList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/DeleteRecheckStatus")]
        public APIResult DeleteRecheckStatus(RecheckStatusDtlDto recheckStatusDtl)
        {
            try
            {
                if (recheckStatusDtl.RecheckStatusId == 0) // 如果是一审的复审类型
                {
                    List<RecheckStatusDto> recheckStatusList_S4 = recheckService.GetShopRecheckStatusInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "S4");
                    List<RecheckStatusDto> recheckStatusList_S6 = recheckService.GetShopRecheckStatusInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "S6");
                    List<RecheckStatusDto> recheckStatusList_S7 = recheckService.GetShopRecheckStatusInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "S7");
                    if (recheckStatusList_S4 != null || recheckStatusList_S4.Count > 0)
                    {
                        throw new Exception("该经销商已复审修改完毕，请先删除复审修改完毕状态");
                    }
                    if (recheckStatusList_S6 != null || recheckStatusList_S6.Count > 0)
                    {
                        throw new Exception("该经销商督导已抽查完毕，请先删除督导抽查状态");
                    }
                    if (recheckStatusList_S7 != null || recheckStatusList_S7.Count > 0)
                    {
                        throw new Exception("该经销商项目经理已抽查完毕，请先删除项目经理抽查状态");
                    }
                    recheckService.DeleteRecheckStatusDtl(recheckStatusDtl.RecheckStatusDtlId.ToString(), recheckStatusDtl.ModifyUserId.ToString());
                }
                else // 非一审
                {
                    if (recheckStatusDtl.RecheckTypeCode == "S0")
                    {
                        List<RecheckStatusDto> recheckStatusList_S1 = recheckService.GetShopRecheckStatusInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "S1");
                        if (recheckStatusList_S1 != null || recheckStatusList_S1.Count > 0)
                        {
                            throw new Exception("该经销商已提交复审，请先删除复审提交状态");
                        }
                    }
                    else if (recheckStatusDtl.RecheckTypeCode == "S4")
                    {
                        List<RecheckStatusDto> recheckStatusList_S8 = recheckService.GetShopRecheckStatusInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "S8");
                        if (recheckStatusList_S8 != null || recheckStatusList_S8.Count > 0)
                        {
                            throw new Exception("该经销商已一审确认完毕，请先删除一审确认完毕状态");
                        }
                    }
                    else if (recheckStatusDtl.RecheckTypeCode == "S8")
                    {
                        List<RecheckStatusDto> recheckStatusList_S9 = recheckService.GetShopRecheckStatusInfo(recheckStatusDtl.ProjectId.ToString(), recheckStatusDtl.ShopId.ToString(), "S9");
                        if (recheckStatusList_S9 != null || recheckStatusList_S9.Count > 0)
                        {
                            throw new Exception("该经销商已二审完毕，请先删除二审状态");
                        }
                    }
                    recheckService.DeleteRecheckStatus(recheckStatusDtl.RecheckStatusId.ToString(), recheckStatusDtl.ModifyUserId.ToString());
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
