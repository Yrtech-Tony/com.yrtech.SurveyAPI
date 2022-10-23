﻿using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using System.Linq;
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
        ExcelDataService excelDataService = new ExcelDataService();
        PhotoService photoService = new PhotoService();
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
        public APIResult GetShopNeedAnswerSubjectInfo(string projectId, string shopId, string examTypeId, string subjectType = "")
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopNeedAnswerSubject(projectId, shopId, examTypeId, subjectType);
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
        public APIResult GetShopNextAnswerSubjectInfo(string projectId, string shopId, string examTypeId, string orderNO, string subjectType = "")
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopNextAnswerSubject(projectId, shopId, examTypeId, orderNO, subjectType);
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
        public APIResult GetShopPreAnswerSubjectInfo(string projectId, string shopId, string examTypeId, string orderNO, string subjectType = "")
        {
            try
            {

                List<AnswerDto> answerList = answerService.GetShopPreAnswerSubject(projectId, shopId, examTypeId, orderNO, subjectType);
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
        public APIResult GetShopTransAnswerSubjectInfo(string projectId, string shopId, string orderNO, string subjectType = "")
        {
            try
            {

                List<AnswerDto> answerList = answerService.GetShopTransAnswerSubject(projectId, shopId, orderNO, subjectType);
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
                string roleTypeCode = "";
                List<UserInfo> userInfoList = masterService.GetUserInfo("", "", answer.ModifyUserId.ToString(), "", "", "", "", "");
                if (userInfoList != null && userInfoList.Count > 0)
                {
                    roleTypeCode = userInfoList[0].RoleType;
                }
                if (roleTypeCode == "S_SurperVision" || roleTypeCode == "S_Customer")
                {
                    throw new Exception("无修改得分权限");
                }
                else if (roleTypeCode == "S_Execute")
                {
                    List<RecheckStatusDto> list = recheckService.GetShopRecheckStatus(answer.ProjectId.ToString(), answer.ShopId.ToString(), "");
                    if (list != null && list.Count > 0)
                    {

                        if (!string.IsNullOrEmpty(list[0].Status_S4))
                        {
                            throw new Exception("已复审修改完毕，不能进行修改");
                        }
                        else if (!string.IsNullOrEmpty(list[0].Status_S1))
                        {
                            throw new Exception("已提交复审，不能进行修改");
                        }
                    }
                }

                //List<SubjectDto> subjectList = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString(), "", "");
                //if (subjectList != null && subjectList.Count > 0)
                //{
                //    decimal fullScore = subjectList[0].FullScore==null?0:Convert.ToDecimal(subjectList[0].FullScore);
                //    decimal lowScore = subjectList[0].LowScore == null ? 0 : Convert.ToDecimal(subjectList[0].LowScore);
                //    decimal photoScore = answer.PhotoScore == null ? 0 : Convert.ToDecimal(answer.PhotoScore);
                //    if (photoScore < lowScore|| photoScore> fullScore) {
                //        throw new Exception("输入的分数不在分值范围内");
                //    }
                //}
                answerService.SaveAnswerInfo(answer);

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        // 照片上传记录
        [HttpPost]
        [Route("Answer/SaveAnswerPhotoLog")]
        public APIResult SaveAnswerPhotoLog(AnswerPhotoLog answerPhotoLog)
        {
            try
            {
                photoService.SaveAnswerPhotoLog(answerPhotoLog);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetShopAnswerPhotoLog")]
        public APIResult GetShopAnswerPhotoLog(string projectId, string shopId, string uploadStatus)
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, "", "");
                List<AnswerPhotoLog> uploadLogList = photoService.GetAnswerPhoto(projectId, shopId);
                List<AnswerPhotoLogDto> photoList = new List<AnswerPhotoLogDto>();
                foreach (AnswerDto answer in answerList)
                {
                    // 标准照片信息
                    List<FileResultDto> fileResultList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                    if (fileResultList != null && fileResultList.Count > 0)
                    {
                        foreach (FileResultDto fileResult in fileResultList)
                        {
                            if (!string.IsNullOrEmpty(fileResult.Url))
                            {
                                string[] strUrl = fileResult.Url.Split(';');
                                foreach (string url in strUrl)
                                {
                                    AnswerPhotoLogDto photo = new AnswerPhotoLogDto();
                                    photo.ProjectId = answer.ProjectId;
                                    photo.ShopId = answer.ShopId.ToString();
                                    photo.ShopCode = answer.ShopCode;
                                    photo.ShopName = answer.ShopName;
                                    photo.SubjectCode = answer.SubjectCode;
                                    photo.SubjectId = answer.SubjectId;
                                    photo.OrderNO = answer.OrderNO;
                                    photo.PhotoUrl = url;
                                    photo.PhotoType = "标准照片";
                                    List<AnswerPhotoLog> uploadLogUrl = uploadLogList.Where(x => x.FileUrl == url).ToList();
                                    if (uploadLogUrl == null || uploadLogUrl.Count == 0)
                                    {
                                        photo.UploadStatus = "0";
                                    }
                                    else
                                    {
                                        photo.UploadStatus = "1";
                                        photo.InDateTime = uploadLogUrl[0].InDateTime;
                                    }
                                    photoList.Add(photo);
                                }
                            }
                        }
                    }
                    // 失分照片信息
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                    if(lossResultList!=null&& lossResultList.Count>0)
                    {
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            if (!string.IsNullOrEmpty(lossResult.LossFileNameUrl))
                            {
                                string[] strUrl = lossResult.LossFileNameUrl.Split(';');
                                foreach (string url in strUrl)
                                {
                                    AnswerPhotoLogDto photo = new AnswerPhotoLogDto();
                                    photo.ProjectId = answer.ProjectId;
                                    photo.ShopId = answer.ShopId.ToString();
                                    photo.ShopCode = answer.ShopCode;
                                    photo.ShopName = answer.ShopName;
                                    photo.SubjectCode = answer.SubjectCode;
                                    photo.SubjectId = answer.SubjectId;
                                    photo.OrderNO = answer.OrderNO;
                                    photo.PhotoUrl = url;
                                    photo.PhotoType = "失分照片";
                                    List<AnswerPhotoLog> uploadLogUrl = uploadLogList.Where(x => x.FileUrl == url).ToList();
                                    if (uploadLogUrl == null || uploadLogUrl.Count == 0)
                                    {
                                        photo.UploadStatus = "0";
                                    }
                                    else
                                    {
                                        photo.UploadStatus = "1";
                                        photo.InDateTime = uploadLogUrl[0].InDateTime;
                                    }
                                    photoList.Add(photo);
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(uploadStatus))
                {
                    photoList = photoList.Where(x => x.UploadStatus == uploadStatus).ToList();
                }
                
                return new APIResult() { Status = true, Body = CommonHelper.Encode(photoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetFolderName")]
        public APIResult GetFolderName(string projectId, string shopCode, string shopName, string subectCode, string photoName, string photoOrder,  string photoType, string subjectOrder)
        {
            try
            {
                // photoName:（标准照片：照片名称；失分照片：失分描述）
                // photoOrder:（标准照片：标准照片SeqNO；失分照片：失分描述Id）
                // photoType:(标准照片：1; 失分照片：2)
                string folder4 =photoService.GetFolderName(projectId, "4", shopCode, shopName, subectCode, photoName, photoOrder, "", photoType, subjectOrder,"");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(folder4) };
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
                foreach (AnswerDto answer in answerList)
                {
                    // 标准照片信息
                    List<SubjectFile> subjectFileList = masterService.GetSubjectFile(projectId, answer.SubjectId.ToString());
                    List<FileResultDto> fileResultList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                    if (subjectFileList == null || subjectFileList.Count == 0)
                    {
                        answer.PhotoCount = "0/0";
                        answer.PhotoStatus = "1";
                    }
                    else
                    {
                        int fileResutlCount = fileResultList == null ? 0 : fileResultList.Count;
                        answer.PhotoCount = fileResutlCount.ToString() + "/" + subjectFileList.Count.ToString();
                        if (fileResutlCount != 0)
                        {
                            answer.PhotoStatus = "1";
                        }
                        else
                        {
                            answer.PhotoStatus = "0";
                        }
                    }
                    // 失分照片信息
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                    int lossPhotoCount = 0;
                    if (lossResultList == null || lossResultList.Count == 0)
                    {
                        answer.LossPhotoCount = "0";
                        answer.LossPhotoStatus = "0";
                    }
                    else
                    {
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            if (!string.IsNullOrEmpty(lossResult.LossFileNameUrl))
                            {
                                lossPhotoCount += lossResult.LossFileNameUrl.Split(';').Length;
                            }
                        }
                        if (lossPhotoCount == 0)
                        {
                            answer.LossPhotoCount = "0";
                            answer.LossPhotoStatus = "0";
                        }
                        else
                        {
                            answer.LossPhotoCount = lossPhotoCount.ToString();
                            answer.LossPhotoStatus = "1";
                        }
                    }
                }
                // 在查询特定Subject得分时，返回题目的信息
                if (!string.IsNullOrEmpty(subjectId) && answerList != null && answerList.Count > 0)
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
        [Route("Answer/ShopAnswerScoreInfoExport")]
        public APIResult ShopAnswerScoreInfoExport(string projectId, string shopId)
        {
            try
            {
                string downloadPath = excelDataService.ShopAnsewrScoreInfoExport(projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/ShopAnswerScoreInfoExportL")]
        public APIResult ShopAnswerScoreInfoExportL(string projectId, string shopId)
        {
            try
            {
                string downloadPath = excelDataService.ShopAnsewrScoreInfoExport_L(projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/ShopAnswerFileResultDownLoad")]
        public APIResult ShopAnswerFileResultDownLoad(string projectId, string shopId)
        {
            try
            {
                string downloadPath = photoService.FileResultDownLoad(projectId, shopId);
                if (string.IsNullOrEmpty(downloadPath))
                {
                    return new APIResult() { Status = false, Body = "没有可下载文件" };
                }

                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/ShopAnswerLossResultDownLoad")]
        public APIResult ShopAnswerLossResultDownLoad(string projectId, string shopId)
        {
            try
            {
                string downloadPath = photoService.LossResultDownLoad(projectId, shopId);
                if (string.IsNullOrEmpty(downloadPath))
                {
                    return new APIResult() { Status = false, Body = "没有可下载文件" };
                }

                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
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
