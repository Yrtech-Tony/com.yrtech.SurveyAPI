using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.SurveyAPI.DTO;
using System.Threading;
using com.yrtech.SurveyDAL;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using com.yrtech.SurveyAPI.DTO.Master;

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
        #region 权限类型
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetRoleType")]
        public APIResult GetRoleType(string type)
        {
            try
            {
                List<RoleType> roleTypeList = masterService.GetRoleType(type);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(roleTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 品牌
        /// <summary>
        /// 根据租户信息查询品牌信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetBrand")]
        public APIResult GetBrand(string tenantId, string brandId,string brandCode)
        {
            try
            {
                List<Brand> brandList = masterService.GetBrand(tenantId,brandId,brandCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(brandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveBrand")]
        public APIResult SaveBrand(Brand brand)
        {
            try
            {
                masterService.SaveBrand(brand);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 账号信息管理
        [HttpGet]
        [Route("Master/GetUserInfo")]
        public APIResult GetUserInfo(string tenantId, string brandId,string userId, string accountId, string accountName, string roleTypeCode, string telNO, string email)
        {
            try
            {
                List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId, brandId, userId, accountId, accountName, roleTypeCode, telNO, email);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveUserInfo")]
        public APIResult SaveUserInfo(UserInfo userInfo)
        {
            try
            {
                masterService.SaveUserInfo(userInfo);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetUserInfoBrand")]
        public APIResult GetUserInfoBrand(string tenantId, string brandId, string userId)
        {
            try
            {
                List<UserInfoBrandDto> userInfoBrandList = masterService.GetUserInfoBrand(tenantId, brandId, userId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoBrandList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveUserInfoBrand")]
        public APIResult SaveUserInfoBrand(UserInfoBrand userInfoBrand)
        {
            try
            {
                masterService.SaveUserInfoBrand(userInfoBrand);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteUserInfoBrand")]
        public APIResult DeleteUserInfoBrand(UserInfoBrand userInfoBrand)
        {
            try
            {
                masterService.DeleteUserInfoBrand(userInfoBrand.Id);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetUserInfoObject")]
        public APIResult GetUserInfoObject(string tenantId,  string userId,string objectId,string roleTypeCode)
        {
            try
            {
                List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject(tenantId,userId,objectId,roleTypeCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoObjectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveUserInfoObject")]
        public APIResult SaveUserInfoObject(UserInfoObject userInfoObject)
        {
            try
            {
                masterService.SaveUserInfoObject(userInfoObject);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteUserInfoObject")]
        public APIResult DeleteUserInfoObject(UserInfoObject userInfoObject)
        {
            try
            {
                masterService.DeleteUserInfoObject(userInfoObject.Id);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 区域管理
        [HttpGet]
        [Route("Master/GetArea")]
        public APIResult GetArea(string brandId, string areaId,string areaCode,string areaName,string areaType,string parentId)
        {
            try
            {
                List<AreaDto> areaList = masterService.GetArea(areaId,brandId, areaCode, areaName, areaType,parentId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(areaList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveArea")]
        public APIResult SaveArea(Area area)
        {
            try
            {
                masterService.SaveArea(area);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 区域经销商管理
        [HttpGet]
        [Route("Master/GetAreaShop")]
        public APIResult GetAreaShop(string tenantId,string brandId, string areaId, string shopId)
        {
            try
            {
                List<ShopDto> areaShopList = masterService.GetAreaShop(tenantId,brandId,shopId,areaId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(areaShopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveAreaShop")]
        public APIResult SaveAreaShop(AreaShop areaShop)
        {
            try
            {
                masterService.SaveAreaShop(areaShop);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteAreaShop")]
        public APIResult DeleteAreaShop(AreaShop areaShop)
        {
            try
            {
                masterService.DeleteAreaShop(areaShop.AreaShopId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 集团管理
        [HttpGet]
        [Route("Master/GetGroup")]
        public APIResult GetGroup(string brandId, string groupId,string groupCode, string groupName)
        {
            try
            {
                List<Group> groupList = masterService.GetGroup(brandId,groupId,groupCode,groupName);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(groupList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveGroup")]
        public APIResult SaveGroup(Group group)
        {
            try
            {
                masterService.SaveGroup(group);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 期号


        /// <summary>
        /// 获取品牌下期号的信息，也可以获取单个期号的信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetProject")]
        public APIResult GetProject(string brandId, string projectId, string year)
        {
            try
            {
                List<Project> projectList = masterService.GetProject("", brandId, projectId, year);
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
        #endregion
        #region 体系


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
        public APIResult SaveSubject([FromBody]Subject subject)
        {
            try
            {
                masterService.SaveSubject(subject);
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
        public APIResult GetSubjectInspectionStandard(string projectId, string subjectId)
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
        public APIResult SaveSubjectInspectionStandard(SubjectInspectionStandard subjectInspectionStandard)
        {
            try
            {
                masterService.SaveSubjectInspectionStandard(subjectInspectionStandard);
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
        public APIResult SaveSubjectFile(SubjectFile subjectFile)
        {
            try
            {
                masterService.SaveSubjectFile(subjectFile);
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
        public APIResult SaveLossResult([FromBody]SubjectLossResult subjectlossResult)
        {
            try
            {
                masterService.SaveSubjectLossResult(subjectlossResult);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetSubjectTypeScoreRegion")]
        public APIResult GetSubjectTypeScoreRegion(string projectId, string subjectId, string subjectTypeId)
        {
            try
            {
                List<SubjectTypeScoreRegionDto> lossResultList = masterService.GetSubjectTypeScoreRegionDto(projectId, subjectId, subjectTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lossResultList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveSubjectTypeScoreRegion")]
        public APIResult SaveSubjectTypeScoreRegion([FromBody]SubjectTypeScoreRegion subjectTypeScoreRegion)
        {
            try
            {
                masterService.SaveSubjectTypeScoreRegion(subjectTypeScoreRegion);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region HiddenCode
        [HttpGet]
        [Route("Master/GetHiddenCode")]
        public APIResult GetHiddenCode(string hiddenCodeGroup, string hiddenCode)
        {
            try
            {
                List<HiddenCode> hiddenCodeList = masterService.GetHiddenCode(hiddenCodeGroup,hiddenCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(hiddenCodeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 流程类型
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
        public APIResult SaveSubjectLink([FromBody]SubjectLink subjectLink)
        {
            try
            {
                masterService.SaveSubjectLink(subjectLink);
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
        public APIResult SetSubjectLinkId([FromBody]UploadData uploadData)
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
        #endregion
        #region 经销商


        /// <summary>
        /// 
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetShop")]
        public APIResult GetShop(string brandId, string shopId,string shopCode, string key)
        {
            try
            {
                List<ShopDto> shopList = masterService.GetShop("", brandId, shopId,shopCode, key);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(shopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveShop")]
        public APIResult SaveShop(Shop shop)
        {
            try
            {
                masterService.SaveShop(shop);
                return new APIResult() { Status = true, Body = "" };
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
                List<ShopSubjectTypeExamDto> subjectTypeExamList = shopService.GetShopSubjectTypeExam(projectId, "");
                foreach (Shop shop in shopList)
                {
                    ShopDto shopDto = new ShopDto();
                    shopDto.ProjectId = Convert.ToInt32(projectId);
                    shopDto.ShopId = shop.ShopId;
                    shopDto.ShopCode = shop.ShopCode;
                    shopDto.ShopName = shop.ShopName;
                    shopDto.Province = shop.Province;
                    shopDto.City = shop.City;
                    shopDto.ShopShortName = shop.ShopShortName;

                    ShopSubjectTypeExamDto exam = subjectTypeExamList.Where(x => x.ShopId == shop.ShopId).FirstOrDefault();
                    if (exam != null)
                    {
                        shopDto.SubjectTypeExamId = Convert.ToInt32(exam.ShopSubjectTypeExamId);
                        shopDto.SubjectTypeExamName = exam.SubjectTypeExamName;
                    }

                    projectShopList.Add(shopDto);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectShopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        #endregion
        #region 试卷类型
        [HttpGet]
        [Route("Master/GetSubjectTypeExam")]
        public APIResult GetSubjectTypeExam(string projectId, string subjectTypeExamId)
        {
            try
            {
                List<SubjectTypeExam> examList = masterService.GetSubjectTypeExam(projectId, subjectTypeExamId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(examList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveSubjectTypeExam")]
        public APIResult SaveSubjectTypeExam([FromBody]SubjectTypeExam subjectTypeExam)
        {
            try
            {
                masterService.SaveSubjectTypeExam(subjectTypeExam);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 体系类型
        [HttpGet]
        [Route("Master/GetSubjectType")]
        public APIResult GetSubjectType()
        {
            try
            {
                List<SubjectType> examList = masterService.GetSubjectType();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(examList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 复审类型
        [HttpGet]
        [Route("Master/GetSubjectRecheckType")]
        public APIResult GetSubjectRecheckType(string projectId, string subjectRecheckTypeId)
        {
            try
            {
                List<SubjectRecheckType> subjectRecheckTypeList = masterService.GetSubjectRecheckType(projectId, subjectRecheckTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(subjectRecheckTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveSubjectRecheckType")]
        public APIResult SaveSubjectRecheckType([FromBody]SubjectRecheckType subjectRecheckType)
        {
            try
            {
                masterService.SaveSubjectRecheckType(subjectRecheckType);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 复审错误类型
        [HttpGet]
        [Route("Master/GetRecheckErrorType")]
        public APIResult GetRecheckErrorType(string projectId, string recheckErrorTypeId)
        {
            try
            {
                List<RecheckErrorType> recheckErrorTypeList = masterService.GetRecheckErrorType(projectId, recheckErrorTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckErrorTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/SaveRecheckErrorType")]
        public APIResult SaveRecheckErrorType([FromBody]RecheckErrorType recheckErrorType)
        {
            try
            {
                masterService.SaveRecheckErrorType(recheckErrorType);
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
