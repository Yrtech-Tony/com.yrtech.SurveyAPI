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
    public class MasterController : ApiController
    {
        AnswerService answerService = new AnswerService();
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
                resultList.Add(masterService.GetSubject(projectId, ""));
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
        public async Task<APIResult> Upload()
        {
            try
            {
                Dictionary<string, string> formData = await PostFormData();
                string userId = formData["UserId"];

                UploadData data = new UploadData();
                data.AnswerShopInfoList = CommonHelper.DecodeString<List<AnswerShopInfo>>(formData["AnswerShopInfoListJson"]);
                data.AnswerShopConsultantList = CommonHelper.DecodeString<List<AnswerShopConsultant>>(formData["AnswerShopConsultantListJson"]);
                data.AnswerList = CommonHelper.DecodeString<List<Answer>>(formData["AnswerListJson"]);

                answerService.SaveAnswerShopInfoList(data.AnswerShopInfoList, userId);
                answerService.SaveAnswerShopConsultantList(data.AnswerShopConsultantList, userId);
                answerService.SaveAnswerList(data.AnswerList, userId);
                
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        public async Task<Dictionary<string, string>> PostFormData()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var multipartMemoryStreamProvider = await Request.Content.ReadAsMultipartAsync();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var content in multipartMemoryStreamProvider.Contents)
                {
                    string fileName = content.Headers.ContentDisposition.FileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        using (Stream stream = await content.ReadAsStreamAsync())
                        {
                            string projectId = dic["projectId"];
                            string shopId = dic["shopId"];
                            string fileKey = "survey/" + projectId + "/" + shopId + "/" + fileName.Replace("\"","");
                            //处理文件
                            OSSClientHelper.UploadOSSFile(fileKey, stream, stream.Length);
                        }
                    }
                    else
                    {
                        string val = await content.ReadAsStringAsync();
                        dic.Add(content.Headers.ContentDisposition.Name.Replace("\"", ""), val);
                    }
                }

                return dic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
