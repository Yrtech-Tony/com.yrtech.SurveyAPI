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
        /// <summary>
        /// 查询经销商需要打分的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="subjectConsultantId"></param>
        /// <returns></returns>
        [Route("Master/GetShopNeedAnswerSubjectInfo")]
        public APIResult GetShopNeedAnswerSubjectInfo(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string subjectConsultantId)
        {
            try
            {
                List<object> resultList = new List<object>();
                //获取体系信息
                List<Subject> subjectList = answerService.GetShopNeedAnswerSubject(projectId, shopId, subjectTypeId, subjectTypeExamId, subjectConsultantId);
                // 获取打分信息
                List<Answer> answerList = answerService.GetAnswerInfoDetail(projectId, shopId, subjectList[0].SubjectId.ToString());
                resultList.Add(subjectList);
                resultList.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [Route("Master/GetShopNextAnswerSubjectInfo")]
        public APIResult GetShopNextAnswerSubjectInfo(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectConsultantId)
        {
            try
            {
                List<object> resultList = new List<object>();
                //获取体系信息
                List<Subject> subjectList = answerService.GetShopNextAnswerSubject(projectId, subjectTypeId, subjectTypeExamId, orderNO, subjectConsultantId);
                // 获取打分信息
                List<Answer> answerList = answerService.GetAnswerInfoDetail(projectId, shopId, subjectList[0].SubjectId.ToString());
                resultList.Add(subjectList);
                resultList.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [Route("Master/GetShopPreAnswerSubjectInfo")]
        public APIResult GetShopPreAnswerSubjectInfo(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectConsultantId)
        {
            try
            {
                List<object> resultList = new List<object>();
                //获取体系信息
                List<Subject> subjectList = answerService.GetShopPreAnswerSubject(projectId, subjectTypeId, subjectTypeExamId, orderNO, subjectConsultantId);
                // 获取打分信息
                List<Answer> answerList = answerService.GetAnswerInfoDetail(projectId, shopId, subjectList[0].SubjectId.ToString());
                resultList.Add(subjectList);
                resultList.Add(answerList);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [Route("Master/SaveAnswerInfo")]
        public async Task<APIResult> SaveAnswerInfo([FromBody]UploadData data)
        {
            try
            {
                string userId = data.UserId;
                data.AnswerList = CommonHelper.DecodeString<List<AnswerDto>>(data.AnswerListJson);
                answerService.SaveAnswerInfo(data.AnswerList[0], userId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
