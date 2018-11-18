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
        //#region 不联网
        //[HttpGet]
        //[Route("Master/GetBaseInfo")]
        //public APIResult GetBaseInfo(string projectId)
        //{
        //    try
        //    {
        //        List<object> resultList = new List<object>();
        //        #region 下载基础数据到Mobile本地

        //        // 用户经销商关系信息 ProjectShop
        //        resultList.Add(shopService.GetProjectShop(projectId));
        //        // 经销商试卷类型信息 ShopSubjectTypeExam
        //        resultList.Add(shopService.GetShopSubjectTypeExam(projectId));
        //        // 体系类型信息  Subject
        //        resultList.Add(masterService.GetSubject(projectId, ""));
        //        // 标准照片信息 SubjectFile
        //        resultList.Add(masterService.GetSubjectFile(projectId, ""));
        //        // 检查标准信息 SubjectInspectionStandard
        //        resultList.Add(masterService.GetSubjectInspectionStandard(projectId, ""));
        //        // 失分描述 SubjectLossResult
        //        resultList.Add(masterService.GetSubjectLossResult(projectId, ""));
        //        // 体系类型打分范围信息 SubjectTypeScoreRegion
        //        resultList.Add(masterService.GetSubjectTypeScoreRegion(projectId, "",""));

        //        return new APIResult() { Status = true, Body = CommonHelper.Encode(resultList) };
        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}

        //[HttpPost]
        //[Route("Master/Upload")]
        //public async Task<APIResult> Upload([FromBody]UploadData data)
        //{
        //    try
        //    {

        //        string userId = data.UserId;
        //        data.AnswerShopInfoList = CommonHelper.DecodeString<List<AnswerShopInfo>>(data.AnswerShopInfoListJson);
        //        data.AnswerShopConsultantList = CommonHelper.DecodeString<List<AnswerShopConsultant>>(data.AnswerShopConsultantListJson);
        //        // data.AnswerList = CommonHelper.DecodeString<List<Answer>>(data.AnswerListJson);
        //        //CommonHelper.log(data.sh);
        //        // Thread.Sleep(1000);


        //        // CommonHelper.log(data.AnswerListJson);
        //        answerService.SaveAnswerShopInfoList(data.AnswerShopInfoList, userId);
        //        answerService.SaveAnswerShopConsultantList(data.AnswerShopConsultantList, userId);
        //        //answerService.SaveAnswerList(data.AnswerList, userId);

        //        return new APIResult() { Status = true, Body = "" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        //public async Task<Dictionary<string, string>> PostFormData()
        //{
        //    try
        //    {
        //        if (!Request.Content.IsMimeMultipartContent())
        //        {
        //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //        }

        //        var multipartMemoryStreamProvider = await Request.Content.ReadAsMultipartAsync();
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        foreach (var content in multipartMemoryStreamProvider.Contents)
        //        {
        //            string fileName = content.Headers.ContentDisposition.FileName;
        //            if (!string.IsNullOrEmpty(fileName))
        //            {
        //                using (Stream stream = await content.ReadAsStreamAsync())
        //                {
        //                    string projectId = dic["projectId"];
        //                    string shopId = dic["shopId"];
        //                    string fileKey = "survey/" + projectId + "/" + shopId + "/" + fileName.Replace("\"", "");
        //                    //处理文件
        //                    OSSClientHelper.UploadOSSFile(fileKey, stream, stream.Length);
        //                }
        //            }
        //            else
        //            {
        //                string val = await content.ReadAsStringAsync();
        //                dic.Add(content.Headers.ContentDisposition.Name.Replace("\"", ""), val);
        //            }
        //        }

        //        return dic;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //#endregion
        /// <summary>
        /// 根据租户信息查询品牌信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetBrand")]
        public APIResult GetBrand(string tenantId, string brandId)
        {
            try
            {
                List<Brand> brandList = masterService.GetBrand(tenantId, "", brandId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(brandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 获取品牌下的账号信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetUserInfoByBrandId")]
        public APIResult GetUserInfoByBrandId(string brandId)
        {
            try
            {
                List<UserInfo> userInfoBrandList = masterService.GetUserInfoByBrandId(brandId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoBrandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 获取品牌下期号的信息，也可以获取单个期号的信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetProject")]
        public APIResult GetProject(string brandId,string projectId)
        {
            try
            {
                List<Project> projectList = masterService.GetProject("",brandId,projectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveProject")]
        public APIResult SaveProject(Project project)
        {
            try
            {
                masterService.SaveProject(project);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取体系的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubject")]
        public APIResult GetSubject(string projectId, string subjectId)
        {
            try
            {
                List<SubjectDto> subjectList = masterService.GetSubject(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(subjectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存体系信息
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubject")]
        public async Task<APIResult> SaveSubject([FromBody]UploadData uploadData)
        {
            try
            {
                List<Subject> subjectList = CommonHelper.DecodeString<List<Subject>>(uploadData.ListJson);
                foreach (Subject subject in subjectList)
                {
                    masterService.SaveSubject(subject);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取流程类型
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectLink")]
        public APIResult GetSubjectLink(string projectId)
        {
            try
            {
                List<SubjectLink> subjectLinkList = masterService.GetSubjectLink(projectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(subjectLinkList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 新增或者更新流程类型
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubjectLink")]
        public async Task<APIResult> SaveSubjectLink([FromBody]UploadData uploadData)
        {
            try
            {
                List<SubjectLink> subjectLinkList = CommonHelper.DecodeString<List<SubjectLink>>(uploadData.ListJson);
                foreach (SubjectLink subjectLink in subjectLinkList)
                {
                    masterService.SaveSubjectLink(subjectLink);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 更新SubjectLinkId
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SetSubjectLinkId")]
        public async Task<APIResult> SetSubjectLinkId([FromBody]UploadData uploadData)
        {
            try
            {
                List<SubjectDto> subjectList = CommonHelper.DecodeString<List<SubjectDto>>(uploadData.ListJson);
                masterService.SetSubjectLinkId(subjectList);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取检查标准
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectInspectionStandard")]
        public APIResult GetSubjectInspectionStandard(string projectId,string subjectId)
        {
            try
            {
                List<SubjectInspectionStandard> inspectionStandardList = masterService.GetSubjectInspectionStandard(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(inspectionStandardList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存检查标准
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubjectInspectionStandard")]
        public async Task<APIResult> SaveSubjectInspectionStandard([FromBody]UploadData uploadData)
        {
            try
            {
                List<SubjectInspectionStandard> subjectInspectionStandardList = CommonHelper.DecodeString<List<SubjectInspectionStandard>>(uploadData.ListJson);
                foreach (SubjectInspectionStandard subjectInspectionStandard in subjectInspectionStandardList)
                {
                    masterService.SaveSubjectInspectionStandard(subjectInspectionStandard);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取标准照片列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectFile")]
        public APIResult GetSubjectFile(string projectId, string subjectId)
        {
            try
            {
                List<SubjectFile> fileList = masterService.GetSubjectFile(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(fileList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存标准照片
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveSubjectFile")]
        public async Task<APIResult> SaveSubjectFile([FromBody]UploadData uploadData)
        {
            try
            {
                List<SubjectFile> subjectFileList = CommonHelper.DecodeString<List<SubjectFile>>(uploadData.ListJson);
                foreach (SubjectFile subjectFile in subjectFileList)
                {
                    masterService.SaveSubjectFile(subjectFile);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取失分描述
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetSubjectLossResult")]
        public APIResult GetSubjectLossResult(string projectId, string subjectId)
        {
            try
            {
                List<SubjectLossResult> lossResultList = masterService.GetSubjectLossResult(projectId, subjectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lossResultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 保存失分描述
        /// </summary>
        /// <param name="uploadData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveLossResult")]
        public async Task<APIResult> SaveLossResult([FromBody]UploadData uploadData)
        {
            try
            {
                List<SubjectLossResult> subjectLossResultList = CommonHelper.DecodeString<List<SubjectLossResult>>(uploadData.ListJson);
                foreach (SubjectLossResult subjectlossResult in subjectLossResultList)
                {
                    masterService.SaveSubjectLossResult(subjectlossResult);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetSubjectTypeScoreRegion")]
        public APIResult GetSubjectTypeScoreRegion(string subjectId,string subjectTypeId)
        {
            try
            {
                List<SubjectTypeScoreRegion> lossResultList = masterService.GetSubjectTypeScoreRegion("", subjectId,subjectTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lossResultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveSubjectTypeScoreRegion")]
        public async Task<APIResult> SaveSubjectTypeScoreRegion([FromBody]UploadData uploadData)
        {
            try
            {
                List<SubjectTypeScoreRegion> subjectTypeScoreRegionList = CommonHelper.DecodeString<List<SubjectTypeScoreRegion>>(uploadData.ListJson);
                foreach (SubjectTypeScoreRegion subjectTypeScoreRegion in subjectTypeScoreRegionList)
                {
                    masterService.SaveSubjectTypeScoreRegion(subjectTypeScoreRegion);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 获取经销商信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetShop")]
        public APIResult GetShop(string projectId, string shopId)
        {
            try
            {
                List<Shop> shopList = masterService.GetShop("",projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(shopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 获取当前期需要执行的经销商及所属试卷类型
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetShopByProjectId")]
        public APIResult GetShopByProjectId(string projectId)
        {
            try
            {
                List<ShopDto> projectShopList = new List<ShopDto>();
                List<Shop> shopList = shopService.GetShopByProjectId(projectId);
                List<ShopSubjectTypeExamDto> subjectTypeExamList = shopService.GetShopSubjectTypeExam(projectId,"");
                foreach (Shop shop in shopList)
                {
                    foreach (ShopSubjectTypeExamDto exam in subjectTypeExamList)
                    {
                        ShopDto shopDto = new ShopDto();
                        shopDto.ProjectId = Convert.ToInt32(projectId);
                        shopDto.ShopId = shop.ShopId;
                        shopDto.ShopCode = shop.ShopCode;
                        shopDto.ShopName = shop.ShopName;
                        shopDto.Province = shop.Province;
                        shopDto.City = shop.City;
                        shopDto.ShopShortName = shop.ShopShortName;
                        shopDto.SubjectTypeExamId = Convert.ToInt32(exam.ShopSubjectTypeExamId);
                        shopDto.SubjectTypeExamName = exam.SubjectTypeExamName;
                        projectShopList.Add(shopDto);
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectShopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
    }
}
