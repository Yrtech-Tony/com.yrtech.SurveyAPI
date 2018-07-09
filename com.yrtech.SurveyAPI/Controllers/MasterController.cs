using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.SurveyAPI.DTO;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class MasterController : ApiController
    {
        MasterService masterService = new MasterService();
        ShopService shopService = new ShopService();

        [HttpGet]
        [Route("Master/GetBaseInfo")]
        public APIResult GetBaseInfo(string projectId)
        {
            try
            {
                List<object> resultList = new List<object>();
                #region 下载基础数据到Mobile本地

                // 用户经销商关系信息 ProjectShop
                resultList.Add(shopService.GetProjectShop(projectId));
                // 经销商试卷类型信息 ShopSubjectTypeExam
                resultList.Add(shopService.GetShopSubjectTypeExam(projectId));
                // 体系类型信息  Subject
                resultList.Add(masterService.GetSubject(projectId));
                // 标准照片信息 SubjectFile
                resultList.Add(masterService.GetSubjectFile(projectId));
                // 检查标准信息 SubjectInspectionStandard
                resultList.Add(masterService.GetSubjectInspectionStandard(projectId));
                // 失分描述 SubjectLossResult
                resultList.Add(masterService.GetSubjectLossResult(projectId));
                // 体系类型打分范围信息 SubjectTypeScoreRegion
                resultList.Add(masterService.GetSubjectTypeScoreRegion(projectId));

                return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
                #endregion

            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        [HttpPost]
        [Route("Master/Upload")]
        public APIResult Upload(string userId, string data)
        {
            try
            {
                UploadData uploadData = CommonHelper.DecodeString<UploadData>(data);
                masterService.InserAnswerList(uploadData.AnswerList);
                masterService.InserAnswerShopInfoList(uploadData.AnswerShopInfoList);
                masterService.InserAnswerShopConsultantList(uploadData.AnswerShopConsultantList);

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
