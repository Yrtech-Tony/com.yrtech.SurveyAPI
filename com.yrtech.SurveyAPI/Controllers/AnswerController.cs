using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;

namespace com.yrtech.SurveyAPI.Controllers
{

    [RoutePrefix("survey/api")]
    public class AnswerController : ApiController
    {
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        RecheckService recheckService = new RecheckService();
        #region 得分登记
        ///// <summary>
        ///// 查询经销商需要打分的体系信息
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <param name="shopId"></param>
        ///// <param name="subjectTypeId"></param>
        ///// <param name="subjectTypeExamId"></param>
        ///// <param name="subjectLinkId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("Answer/GetShopNeedAnswerSubjectInfo")]
        public APIResult GetShopNeedAnswerSubjectInfo(string projectId, string shopId, string examTypeId,string subjectType="")
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopNeedAnswerSubject(projectId, shopId, examTypeId,subjectType);
                //List<AnswerDto> result = new List<AnswerDto>();
                //if (!string.IsNullOrEmpty(subjectType))
                //{
                //    foreach (AnswerDto answerDto in answerList)
                //    {
                //        if (answerDto.HiddenCode_SubjectType == subjectType)
                //        {
                //            result.Add(answerDto);
                //        }
                //    }
                //}
                //else { result = answerList; }
                
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
        /// 查询下一个体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="orderNO"></param>
        /// <param name="SubjectLinkId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/GetShopNextAnswerSubjectInfo")]
        public APIResult GetShopNextAnswerSubjectInfo(string projectId, string shopId, string examTypeId, string orderNO,string subjectType="")
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopNextAnswerSubject(projectId, shopId, examTypeId, orderNO,subjectType);
                //List<AnswerDto> result = new List<AnswerDto>();
                //if (!string.IsNullOrEmpty(subjectType))
                //{
                //    foreach (AnswerDto answerDto in answerList)
                //    {
                //        if (answerDto.HiddenCode_SubjectType == subjectType)
                //        {
                //            result.Add(answerDto);
                //        }
                //    }
                //}
                //else { result = answerList; }
                if (answerList != null && answerList.Count > 0)
                {
                    answerList[0].SubjectFileList = masterService.GetSubjectFile(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectLossResultList = masterService.GetSubjectLossResult(projectId, answerList[0].SubjectId.ToString());
                }
                if (answerList == null || answerList.Count == 0)
                {
                    throw new Exception("已经是最后一题");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 查询上一个体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="orderNO"></param>
        /// <param name="subjectLinkId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/GetShopPreAnswerSubjectInfo")]
        public APIResult GetShopPreAnswerSubjectInfo(string projectId, string shopId, string examTypeId, string orderNO,string subjectType="")
        {
            try
            {
                
                List<AnswerDto> answerList = answerService.GetShopPreAnswerSubject(projectId, shopId, examTypeId, orderNO,subjectType);
                //List<AnswerDto> result = new List<AnswerDto>();
                //if (!string.IsNullOrEmpty(subjectType))
                //{
                //    foreach (AnswerDto answerDto in answerList)
                //    {
                //        if (answerDto.HiddenCode_SubjectType == subjectType)
                //        {
                //            result.Add(answerDto);
                //        }
                //    }
                //}
                //else { result = answerList; }
                if (answerList != null && answerList.Count > 0)
                {
                    answerList[0].SubjectFileList = masterService.GetSubjectFile(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectLossResultList = masterService.GetSubjectLossResult(projectId, answerList[0].SubjectId.ToString());
                }
                if (answerList == null || answerList.Count == 0)
                {
                    throw new Exception("已经是第一题");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetShopTransAnswerSubjectInfo")]
        public APIResult GetShopTransAnswerSubjectInfo(string projectId, string shopId, string orderNO,string subjectType="")
        {
            try
            {

                List<AnswerDto> answerList = answerService.GetShopTransAnswerSubject(projectId, shopId, orderNO,subjectType);
                //List<AnswerDto> result = new List<AnswerDto>();
                //if (!string.IsNullOrEmpty(subjectType))
                //{
                //    foreach (AnswerDto answerDto in answerList)
                //    {
                //        if (answerDto.HiddenCode_SubjectType == subjectType)
                //        {
                //            result.Add(answerDto);
                //        }
                //    }
                //}
                //else { result = answerList; }
                if (answerList != null && answerList.Count > 0)
                {
                    answerList[0].SubjectFileList = masterService.GetSubjectFile(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, answerList[0].SubjectId.ToString());
                    answerList[0].SubjectLossResultList = masterService.GetSubjectLossResult(projectId, answerList[0].SubjectId.ToString());
                }
                if (answerList == null || answerList.Count == 0)
                {
                    throw new Exception("该序号不存在或题目类型不匹配");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 保存打分信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Answer/SaveAnswerInfo")]
        public APIResult SaveAnswerInfo(AnswerDto answer)
        {
            try
            {
                //List<RecheckStatusDto> list = recheckService.GetShopRecheckStatus(answer.ProjectId.ToString(), answer.ShopId.ToString());
                //if (list != null && list.Count > 0 && !string.IsNullOrEmpty(list[0].Status_S1))
                //{
                //    throw new Exception("已提交复审，不能进行修改");
                //}
                answerService.SaveAnswerInfo(answer);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 得分查询
        /// <summary>
        /// subjectId="" 查询所有得分，
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/GetShopAnswerScoreInfo")]
        public APIResult GetShopAnswerScoreInfo(string projectId, string shopId, string subjectId, string key)
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, subjectId, key);
                // 在查询特定Subject得分时，返回题目的信息
                if (!string.IsNullOrEmpty(subjectId)&&answerList != null && answerList.Count>0)
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
        #endregion
        #region 进店信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerShopInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Answer/SaveAnswerShopInfo")]
        public APIResult SaveAnswerShopInfo(AnswerShopInfo answerShopInfo)
        {
            try
            {
                answerService.SaveAnswerShopInfo(answerShopInfo);
                // 提交进店信息，同时更新状态
                ReCheckStatus status = new ReCheckStatus();
                status.InUserId = answerShopInfo.InUserId;
                status.ProjectId = answerShopInfo.ProjectId;
                status.ShopId = answerShopInfo.ShopId;
                status.StatusCode = "S0";
                recheckService.SaveRecheckStatus(status);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取进店信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [Route("Answer/GetAnswerShopInfo")]
        public APIResult GetAnswerShopInfo(string projectId, string shopId)
        {
            try
            {
                // 获取进店基本信息
                List<AnswerShopInfoDto> answershopInfoList = answerService.GetAnswerShopInfo(projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answershopInfoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 销售顾问信息
        /// <summary>
        /// 保存销售顾问信息
        /// </summary>
        /// <param name="shopConsultant"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Answer/SaveShopConsultant")]
        public APIResult SaveShopConsultant([FromBody]ShopConsultantDto shopConsultant)
        {
            try
            {
                shopConsultant.ShopConsultantSubjectLinkList = CommonHelper.DecodeString<List<ShopConsultantSubjectLinkDto>>(shopConsultant.ShopConsultantSubjectLinkListJson);
                answerService.SaveShopConsultant(shopConsultant);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取销售顾问信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [Route("Answer/GetShopShopConsultant")]
        public APIResult GetShopShopConsultant(string projectId, string shopId)
        {
            try
            {
                List<ShopConsultantDto> shopContantList = answerService.GetShopConsultant(projectId, shopId);
                foreach (ShopConsultantDto shopContant in shopContantList)
                {
                    shopContant.ShopConsultantSubjectLinkList = answerService.GetShopConsultantSubjectLink(projectId, shopContant.ConsultantId.ToString());
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(shopContantList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region ImportAnswer
        [HttpPost]
        [Route("Answer/ImportAnswer")]
        public APIResult ImportAnswer([FromBody]UploadData data)
        {
            try
            {
                // answerService.ImportAnswerResult(data.AnswerList[0].TenantId.ToString(), data.AnswerList[0].BrandId.ToString(), data.AnswerList[0].ProjectId.ToString(), data.AnswerList[0].InUserId.ToString(), data.AnswerList);
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
