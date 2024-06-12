using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class ShopController : ApiController
    {
        ShopService shopService = new ShopService();
        ExcelDataService excelDataService = new ExcelDataService();
        MasterService masterService = new MasterService();
        RecheckService recheckService = new RecheckService();
        //[HttpGet]
        //[Route("Shop/GetShopByProjectId")]
        //public APIResult GetShopByProjectId(string projectId)
        //{
        //    try
        //    {
        //        var lst = shopService.GetShopByProjectId(projectId);
        //        return new APIResult() { Status = true, Body = CommonHelper.Encode(lst) };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResult() { Status = false, Body = ex.Message.ToString() };
        //    }
        //}
        #region  经销商卷别设置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Shop/GetProjectShopExamType")]
        public APIResult GetProjectShopExamType(string brandId, string projectId, string shopId, string userId = "")
        {
            try
            {
                List<ProjectShopExamTypeDto> result = new List<ProjectShopExamTypeDto>();
                List<ProjectShopExamTypeDto> projectShopExamTypeList = shopService.GetProjectShopExamType(brandId, projectId, shopId);// 查询当前期号下经销商设置的试卷类型
                foreach (ProjectShopExamTypeDto projectShopExamType in projectShopExamTypeList)
                {
                    // 经销商试卷类型可以选择多种，重新绑定赋值，暂时默认是只有一个值
                    // string[] examTypeIdList = null;
                    string examTypeCodeList = "";
                    string examTypeNameList = "";
                    //if (projectShopExamType.ExamTypeId != null)
                    //{
                    //    examTypeIdList = projectShopExamType.ExamTypeId.Split(';');
                    //}
                    //if (examTypeIdList != null)
                    //{
                    //foreach (string examTypeId in examTypeIdList)
                    //{
                    List<Label> labelList = masterService.GetLabel(brandId, projectShopExamType.ExamTypeId.ToString(), "ExamType", true, ""); // 根据ExamTypeId 查询Code和Name
                    if (labelList != null && labelList.Count > 0)
                    {
                        examTypeCodeList += labelList[0].LabelCode;// + ";";
                        examTypeNameList += labelList[0].LabelName;// + ";";
                    }
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(examTypeCodeList))
                    {
                        projectShopExamType.ExamTypeCode = examTypeCodeList;//.Substring(0, examTypeCodeList.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(examTypeNameList))
                    {
                        projectShopExamType.ExamTypeName = examTypeNameList;//.Substring(0, examTypeNameList.Length - 1);
                    }
                }

                // 验证是否设置：如果已经提交审核，执行人员就不允许查看分数
                bool rechckShopShow = true; // true: 复审之后，执行人员还可以查到这家店,false:不能查看
                bool selfTestChk = true;// true:经销商可以自检；false或者null 不能自检
                List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
                if (projectList != null && projectList.Count > 0)
                {
                    rechckShopShow = projectList[0].RechckShopShow == null ? true : Convert.ToBoolean(projectList[0].RechckShopShow);
                    selfTestChk = projectList[0].SelfTestChk == null ? true : Convert.ToBoolean(projectList[0].SelfTestChk);
                }
                List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatus(projectId, "", "");
                if (string.IsNullOrEmpty(userId))
                {
                    result = projectShopExamTypeList;
                }
                else
                {
                    // 如果传入了UserId，若是执行人员或者经销商查询对应权限的经销商，如果是其他角色查询全部
                    List<UserInfo> userInfoList = masterService.GetUserInfo("", "", userId, "", "", "", "", "", null,"");
                    if (userInfoList != null && userInfoList.Count > 0)
                    {
                        if (userInfoList[0].RoleType == "S_Execute")
                        {
                            List<UserInfoObjectDto> userInfoObjectDtoList = masterService.GetUserInfoObject(userInfoList[0].TenantId.ToString(), userId, "", userInfoList[0].RoleType);
                            foreach (UserInfoObjectDto userInfoObjectDto in userInfoObjectDtoList)
                            {
                                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in projectShopExamTypeList)
                                {
                                    if (userInfoObjectDto.ObjectId == projectShopExamTypeDto.ShopId)
                                    {
                                        // 如果未设置了复审后，执行人员不能查看，直接添加
                                        if (rechckShopShow)
                                        {
                                            result.Add(projectShopExamTypeDto);
                                        }
                                        else
                                        {
                                            //如果设置了复审后，执行人员不能查看，先判断是否已经提交审核
                                            // 是否提交审核
                                            //bool recheckStatus_S1 = false;
                                            List<RecheckStatusDto> recheckStatusList_shop = recheckStatusList.Where(x => x.ShopId == userInfoObjectDto.ObjectId && !string.IsNullOrEmpty(x.Status_S1)).ToList();
                                            if (recheckStatusList_shop == null || recheckStatusList_shop.Count == 0)
                                            {
                                                result.Add(projectShopExamTypeDto);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (userInfoList[0].RoleType == "B_Shop")// 允许经销商自检时，经销商也可以使用APP
                        {
                            if (selfTestChk)
                            {
                                List<UserInfoObjectDto> userInfoObjectDtoList = masterService.GetUserInfoObject(userInfoList[0].TenantId.ToString(), userId, "", userInfoList[0].RoleType);
                                foreach (UserInfoObjectDto userInfoObjectDto in userInfoObjectDtoList)
                                {
                                    foreach (ProjectShopExamTypeDto projectShopExamTypeDto in projectShopExamTypeList)
                                    {
                                        if (userInfoObjectDto.ObjectId == projectShopExamTypeDto.ShopId)
                                        {
                                            // 如果未设置了复审后，执行人员不能查看，直接添加
                                            if (rechckShopShow)
                                            {
                                                result.Add(projectShopExamTypeDto);
                                            }
                                            else
                                            {
                                                //如果设置了复审后，执行人员不能查看，先判断是否已经提交审核
                                                // 是否提交审核
                                                //bool recheckStatus_S1 = false;
                                                List<RecheckStatusDto> recheckStatusList_shop = recheckStatusList.Where(x => x.ShopId == userInfoObjectDto.ObjectId && !string.IsNullOrEmpty(x.Status_S1)).ToList();
                                                if (recheckStatusList_shop == null || recheckStatusList_shop.Count == 0)
                                                {
                                                    result.Add(projectShopExamTypeDto);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = projectShopExamTypeList;
                        }
                    }
                }

                return new APIResult() { Status = true, Body = CommonHelper.Encode(result) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Shop/SaveProjectShopExamType")]
        public APIResult SaveProjectShopExamType(ProjectShopExamType projectShopExamType)
        {
            try
            {
                shopService.SaveProjectShopExamType(projectShopExamType);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Shop/ProjectShopExamTypeExcelAnalysis")]
        public APIResult ProjectShopExamTypeExcelAnalysis(string brandId, string ossPath)
        {
            try
            {
                List<ProjectShopExamTypeDto> list = excelDataService.ProjectShopExamTypeImport(ossPath);
                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in list)
                {
                    projectShopExamTypeDto.ImportChk = true;
                    projectShopExamTypeDto.ImportRemark = "";
                    List<ShopDto> shopList = masterService.GetShop("", brandId, "", projectShopExamTypeDto.ShopCode, "", true);
                    if (shopList == null || shopList.Count == 0)
                    {
                        projectShopExamTypeDto.ImportChk = false;
                        projectShopExamTypeDto.ImportRemark += "经销商代码在系统中不存在或不可用" + ";";
                    }
                    else
                    {
                        projectShopExamTypeDto.ShopName = shopList[0].ShopName;
                    }
                    // 经销商执行类型如果是多个的话，使用逗号隔开
                    //string[] examTypeCodeList = null;
                    //if (projectShopExamTypeDto.ExamTypeCode != null)
                    //{
                    //    examTypeCodeList = projectShopExamTypeDto.ExamTypeCode.Split(';');
                    //}
                    //foreach (string examTypeCode in examTypeCodeList)
                    //{
                    //    if (!string.IsNullOrEmpty(examTypeCode))
                    //    {
                    List<Label> labelList = masterService.GetLabel(brandId, "", "ExamType", true, projectShopExamTypeDto.ExamTypeCode);
                    if (labelList == null || labelList.Count == 0)
                    {
                        projectShopExamTypeDto.ImportChk = false;
                        projectShopExamTypeDto.ImportRemark += "卷别代码在系统中不存在或不可用" + ";";
                    }
                    //    }
                    //}
                }
                list = (from projectShopExamType in list orderby projectShopExamType.ImportChk select projectShopExamType).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Shop/ProjectShopExamTypeImport")]
        public APIResult ProjectShopExamTypeImport(UploadData uploadData)
        {
            try
            {
                List<ProjectShopExamTypeDto> list = CommonHelper.DecodeString<List<ProjectShopExamTypeDto>>(uploadData.ListJson);
                //验证Excel中的经销商代码和卷别代码是否在系统存在
                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in list)
                {
                    List<ShopDto> shopList = masterService.GetShop("", projectShopExamTypeDto.BrandId.ToString(), "", projectShopExamTypeDto.ShopCode, "", true);
                    if (shopList == null || shopList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记或不可用的经销商代码，请检查文件" };
                    }
                    //string[] examTypeCodeList = null;
                    //if (projectShopExamTypeDto.ExamTypeCode != null)
                    //{
                    //    examTypeCodeList = projectShopExamTypeDto.ExamTypeCode.Split(';');
                    //}
                    //foreach (string examTypeCode in examTypeCodeList)
                    //{
                    //    if (!string.IsNullOrEmpty(examTypeCode))
                    //    {
                    List<Label> labelList = masterService.GetLabel(projectShopExamTypeDto.BrandId.ToString(), "", "ExamType", true, projectShopExamTypeDto.ExamTypeCode);
                    if (labelList == null || labelList.Count == 0)
                    {
                        return new APIResult() { Status = false, Body = "导入失败,文件中存在在系统未登记或者不可用的卷别代码，请检查文件" };
                    }
                    //    }
                    //}
                }
                foreach (ProjectShopExamTypeDto projectShopExamTypeDto in list)
                {
                    ProjectShopExamType projectShopExamType = new ProjectShopExamType();
                    projectShopExamType.ProjectId = projectShopExamTypeDto.ProjectId;
                    List<ShopDto> shopList = masterService.GetShop("", projectShopExamTypeDto.BrandId.ToString(), "", projectShopExamTypeDto.ShopCode, "", true);
                    if (shopList != null && shopList.Count > 0)
                    {
                        projectShopExamType.ShopId = shopList[0].ShopId;
                    }
                    //string[] examTypeCodeList = null;
                    //string examTypeIdList = "";
                    //if (projectShopExamTypeDto.ExamTypeCode != null)
                    //{
                    //    examTypeCodeList = projectShopExamTypeDto.ExamTypeCode.Split(';');
                    //}
                    //foreach (string examTypeCode in examTypeCodeList)
                    //{
                    List<Label> labelList = masterService.GetLabel(projectShopExamTypeDto.BrandId.ToString(), "", "ExamType", true, projectShopExamTypeDto.ExamTypeCode);
                    //if (labelList != null && labelList.Count > 0)
                    //{
                    //examTypeIdList += labelList[0].LabelId;// + ";";
                    projectShopExamType.ExamTypeId = labelList[0].LabelId;
                    //}
                    //}
                    // 去掉最后一个分号
                    //if (examTypeIdList != null)
                    //{
                    //    projectShopExamType.ExamTypeId = examTypeIdList;//.Substring(0, examTypeIdList.Length - 1);
                    //}
                    projectShopExamType.InUserId = projectShopExamTypeDto.InUserId;
                    projectShopExamType.ModifyUserId = projectShopExamTypeDto.ModifyUserId;

                    shopService.SaveProjectShopExamType(projectShopExamType);
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
