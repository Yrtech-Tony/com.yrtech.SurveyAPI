using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.SurveyAPI.DTO;
using System.Threading;
using Purchase.DAL;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace com.yrtech.SurveyAPI.Controllers
{

    [RoutePrefix("survey/api")]
    public class AnswerController : ApiController
    {
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        #region 得分登记
        /// <summary>
        /// 查询经销商需要打分的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="subjectLinkId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/GetShopNeedAnswerSubjectInfo")]
        public APIResult GetShopNeedAnswerSubjectInfo(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string subjectLinkId,string consultantId)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
                List<SubjectFile> subjectFileList = new List<SubjectFile>();
                List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
                //获取体系信息
                List<Subject> subjectList = answerService.GetShopNeedAnswerSubject(projectId, shopId, subjectTypeId, subjectTypeExamId, subjectLinkId,consultantId);
                if (subjectList == null || subjectList.Count == 0)
                {
                    throw new Exception("已经是最后一题");
                }
                if (subjectList != null && subjectList.Count > 0)
                {
                    subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
                    subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
                    subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
                    subjectTypeScoreRegionList = masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), subjectTypeId);
                    //Thread.Sleep(1000);
                    //CommonHelper.log(subjectTypeScoreRegionList[0].LowestScore + "  " + subjectTypeScoreRegionList[0].FullScore);
                }
                // 获取打分信息
                List<AnswerDto> answerList = new List<AnswerDto>();
                if (subjectList != null && subjectList.Count > 0)
                {
                    answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, subjectList[0].SubjectId.ToString());
                    if (answerList != null && answerList.Count > 0)
                    {
                        answerList[0].ShopConsultantResult = CommonHelper.Encode(answerService.GetShopConsultantScore(answerList[0].AnswerId.ToString(),consultantId));
                    }
                }
                resultList.Add(subjectList);
                resultList.Add(subjectInspectionStandardList);
                resultList.Add(subjectFileList);
                resultList.Add(subjectLossResultList);
                resultList.Add(subjectTypeScoreRegionList);
                resultList.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
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
        public APIResult GetShopNextAnswerSubjectInfo(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectLinkId)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
                List<SubjectFile> subjectFileList = new List<SubjectFile>();
                List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
                //获取体系信息
                List<Subject> subjectList = answerService.GetShopNextAnswerSubject(projectId, subjectTypeId, subjectTypeExamId, orderNO, subjectLinkId);
                if (subjectList == null || subjectList.Count == 0)
                {
                    throw new Exception("已经是最后一题");
                }
                if (subjectList != null && subjectList.Count > 0)
                {
                    subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
                    subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
                    subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
                    subjectTypeScoreRegionList = masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), subjectTypeId);

                }
                // 获取打分信息
                List<AnswerDto> answerList = new List<AnswerDto>();
                if (subjectList != null && subjectList.Count > 0)
                {
                    answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, subjectList[0].SubjectId.ToString());
                }
                resultList.Add(subjectList);
                resultList.Add(subjectInspectionStandardList);
                resultList.Add(subjectFileList);
                resultList.Add(subjectLossResultList);
                resultList.Add(subjectTypeScoreRegionList);
                resultList.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
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
        public APIResult GetShopPreAnswerSubjectInfo(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectLinkId)
        {
            try
            {
                List<object> resultList = new List<object>();
                List<SubjectInspectionStandard> subjectInspectionStandardList = new List<SubjectInspectionStandard>();
                List<SubjectFile> subjectFileList = new List<SubjectFile>();
                List<SubjectLossResult> subjectLossResultList = new List<SubjectLossResult>();
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = new List<SubjectTypeScoreRegion>();
                //获取体系信息
                List<Subject> subjectList = answerService.GetShopPreAnswerSubject(projectId, subjectTypeId, subjectTypeExamId, orderNO, subjectLinkId);
                if (subjectList == null || subjectList.Count == 0)
                {
                    throw new Exception("已经是第一题");
                }
                if (subjectList != null && subjectList.Count > 0)
                {
                    //CommonHelper.log("subjectTypeID" + subjectTypeId);
                    //Thread.Sleep(1000);
                    subjectInspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectList[0].SubjectId.ToString());
                    subjectFileList = masterService.GetSubjectFile(projectId, subjectList[0].SubjectId.ToString());
                    subjectLossResultList = masterService.GetSubjectLossResult(projectId, subjectList[0].SubjectId.ToString());
                    subjectTypeScoreRegionList = masterService.GetSubjectTypeScoreRegion(projectId, subjectList[0].SubjectId.ToString(), subjectTypeId);
                    //Thread.Sleep(1000);
                    //CommonHelper.log(subjectTypeScoreRegionList[0].LowestScore + "  " + subjectTypeScoreRegionList[0].FullScore);
                }
                // 获取打分信息
                List<AnswerDto> answerList = new List<AnswerDto>();
                if (subjectList != null && subjectList.Count > 0)
                {
                    answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, subjectList[0].SubjectId.ToString());
                }
                resultList.Add(subjectList);
                resultList.Add(subjectInspectionStandardList);
                resultList.Add(subjectFileList);
                resultList.Add(subjectLossResultList);
                resultList.Add(subjectTypeScoreRegionList);
                resultList.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [Route("Answer/GetShopPreAnswerSubjectInfo")]
        /// <summary>
        /// 保存打分信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Answer/SaveAnswerInfo")]
        public async Task<APIResult> SaveAnswerInfo([FromBody]AnswerDto answer)
        {
            try
            {
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
        public APIResult GetShopAnswerScoreInfo(string projectId, string shopId, string subjectId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerService.GetShopAnswerScoreInfo(projectId, shopId, subjectId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 进店信息
        /// <summary>
        /// 保存经销商进店信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Answer/SaveShopAnswerInfo")]
        public async Task<APIResult> SaveShopAnswerInfo([FromBody]AnswerShopInfo answerShopInfo)
        {
            try
            {
                answerService.SaveAnswerShopInfo(answerShopInfo);
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
        [Route("Answer/GetShopAnswerInfo")]
        public APIResult GetShopAnswerInfo(string projectId, string shopId)
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
        public async Task<APIResult> SaveShopConsultant([FromBody]ShopConsultantDto shopConsultant)
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
    }
}
