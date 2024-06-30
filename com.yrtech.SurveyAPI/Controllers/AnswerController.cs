using System.Web.Http;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.Common;
using System.Collections.Generic;
using System;
using System.Linq;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System.IO;
using System.Threading;

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
        public List<AnswerDto> AnswerScoreReset(List<AnswerDto> answerList)
        {
            if (answerList == null || answerList.Count == 0) return new List<AnswerDto>();
            string projectId = answerList[0].ProjectId.ToString();
            // 打分默认显示
            string scoreShowType = "";
            List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "");
            if (projectList != null && projectList.Count > 0)
            {
                scoreShowType = projectList[0].ScoreShowType;
            }
            foreach (AnswerDto answer in answerList)
            {
                if (answer.PhotoScore == null)
                {
                    List<SubjectDto> subjectList = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString(), "", "");
                    if (subjectList != null && subjectList.Count > 0)
                    {
                        if (scoreShowType == "L")
                        {
                            answer.PhotoScore = subjectList[0].LowScore;
                        }
                        else if (scoreShowType == "F")
                        {
                            answer.PhotoScore = subjectList[0].FullScore;
                        }
                    }
                }
            }
            return answerList;
        }
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
                List<AnswerDto> answerList = AnswerScoreReset(answerService.GetShopNeedAnswerSubject(projectId, shopId, examTypeId, subjectType));

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
                List<AnswerDto> answerList = AnswerScoreReset(answerService.GetShopNextAnswerSubject(projectId, shopId, examTypeId, orderNO, subjectType));
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

                List<AnswerDto> answerList = AnswerScoreReset(answerService.GetShopPreAnswerSubject(projectId, shopId, examTypeId, orderNO, subjectType));
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

                List<AnswerDto> answerList = AnswerScoreReset(answerService.GetShopTransAnswerSubject(projectId, shopId, orderNO, subjectType));
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
                #region 获取操作人权限
                string roleTypeCode = "";
                List<UserInfo> userInfoList = masterService.GetUserInfo("", "", answer.ModifyUserId.ToString(), "", "", "", "", "", null, "");
                if (userInfoList != null && userInfoList.Count > 0)
                {
                    roleTypeCode = userInfoList[0].RoleType;
                }
                #endregion
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
                        else if (!string.IsNullOrEmpty(list[0].Status_S1) && string.IsNullOrEmpty(list[0].Status_S3))
                        {
                            throw new Exception("已提交复审，不能进行修改");
                        }
                    }
                }
                else if (roleTypeCode == "B_Shop") // 允许经销商自检的情况
                {
                    bool selfTestChk = false;
                    List<ProjectDto> projectList = masterService.GetProject("", "", answer.ProjectId.ToString(), "", "", "");
                    if (projectList != null && projectList.Count > 0)
                    {
                        selfTestChk = projectList[0].SelfTestChk == null ? false : Convert.ToBoolean(projectList[0].SelfTestChk);
                    }
                    if (selfTestChk)
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
                    else
                    {
                        throw new Exception("该项目不允许经销商自检，请联系管理员");
                    }
                }
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
                List<AnswerPhotoLogDto> photoList = new List<AnswerPhotoLogDto>();
                photoList = answerService.GetShopAnsewrPhotoLog(projectId, shopId);
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
        public APIResult GetFolderName(string projectId, string shopCode, string shopName, string subectCode, string photoName, string photoOrder, string photoType, string subjectOrder)
        {
            try
            {
                // photoName:（标准照片：照片名称；失分照片：失分描述）
                // photoOrder:（标准照片：标准照片SeqNO；失分照片：失分描述Id）
                // photoType:(标准照片：1; 失分照片：2)
                string folder4 = photoService.GetFolderName(projectId, "4", shopCode, shopName, subectCode, photoName, photoOrder, "", photoType, subjectOrder, "");
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
                // 因特殊原因导致失分描述json中，有字段但无对应的数据
                // 对于无失分说明，无补充失分说明，且无照片的失分描述的数据排除在外，不显示在页面上
                foreach (AnswerDto answer in answerList)
                {
                    List<LossResultDto> lossResultList_Answer = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                    if (lossResultList_Answer != null && lossResultList_Answer.Count > 0)
                    {
                        List<LossResultDto> lossResultList = new List<LossResultDto>();
                        foreach (LossResultDto lossResult in lossResultList_Answer)
                        {
                            if (!string.IsNullOrEmpty(lossResult.LossDesc)
                                || !string.IsNullOrEmpty(lossResult.LossDesc2)
                                || !string.IsNullOrEmpty(lossResult.LossFileNameUrl))
                            {
                                lossResultList.Add(lossResult);
                            }
                        }
                        answer.LossResult = CommonHelper.EncodeDto<string>(lossResultList);
                    }
                }
                // 1. 统计标准照片的拍照状态及具体数量
                // 2. 统计失分描述填写状态，失分照片状态及具体数量
                foreach (AnswerDto answer in answerList)
                {
                    #region 标准照片
                    List<SubjectFile> subjectFileList = masterService.GetSubjectFile(projectId, answer.SubjectId.ToString());
                    List<FileResultDto> fileResultList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                    if (subjectFileList == null || subjectFileList.Count == 0)
                    {
                        answer.PhotoCount = "0/0";
                        answer.PhotoStatus = "1";
                    }
                    else
                    {
                        int fileResutlCount = 0;
                        if (fileResultList != null)
                        {
                            foreach (FileResultDto fileResult in fileResultList)
                            {
                                if (!string.IsNullOrEmpty(fileResult.Url))
                                {
                                    fileResutlCount = fileResutlCount + 1;
                                }
                            }
                        }
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
                    #endregion
                    #region 失分照片信息
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);

                    int lossPhotoCount = 0;// 失分照片数量
                    if (lossResultList == null || lossResultList.Count == 0)
                    {
                        answer.LossPhotoCount = "0";
                        answer.LossPhotoStatus = "0";
                        answer.LossResultStatus = "0";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(lossResultList[0].LossDesc) || !string.IsNullOrEmpty(lossResultList[0].LossDesc2))
                        {
                            answer.LossResultStatus = "1";
                        }
                        else
                        {
                            answer.LossResultStatus = "0";
                        }
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
                    #endregion 
                }
                // 在查询特定Subject得分时，返回题目的信息
                if (!string.IsNullOrEmpty(subjectId) && answerList != null && answerList.Count > 0)
                {
                    List<SubjectFile> subjectFileList = masterService.GetSubjectFile(projectId, answerList[0].SubjectId.ToString());
                    foreach (SubjectFile subjectFile in subjectFileList)
                    {
                        if (string.IsNullOrEmpty(subjectFile.FileDemo))
                        {
                            subjectFile.FileDemo = "Survey/ImportTemplate/示例.jpg";
                        }
                    }
                    answerList[0].SubjectFileList = subjectFileList;
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
        public APIResult ShopAnswerScoreInfoExportL(string projectId, string shopId, string columnList = "")
        {
            try
            {
                string downloadPath = excelDataService.ShopAnsewrScoreInfoExport_L(projectId, shopId, columnList);
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
        /// <summary>
        /// 经销商标准照片上传信息,暂时未用到
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="uploadStatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/GetShopAnswerPhotoInfo")]
        public APIResult GetShopAnswerPhotoInfo(string projectId, string shopId)
        {
            try
            {
                List<FileResultDto> fileResultList = new List<FileResultDto>();
                List<SubjectFile> subjectList = masterService.GetSubjectFile(projectId, "");
                List<AnswerDto> answerList = answerService.GetShopAnswerScoreInfo(projectId, shopId, "", "");
                foreach (SubjectFile subjectFile in subjectList)
                {
                    FileResultDto fileResult = new FileResultDto();
                    fileResult.ProjectId = projectId;
                    fileResult.ShopId = shopId;
                    fileResult.SubjectId = subjectFile.SubjectId.ToString();
                    fileResult.FileName = subjectFile.FileName;
                    fileResult.SeqNO = subjectFile.SeqNO;
                    if (answerList == null || answerList.Count == 0)
                    {
                        fileResult.Url = "";
                        fileResult.Status = "未上传";
                    }
                    else
                    {
                        List<AnswerDto> answerListSubject = answerList.Where(x => x.SubjectId == subjectFile.SubjectId).ToList();
                        if (answerListSubject == null || answerListSubject.Count == 0 || answerListSubject[0].FileResult == null)
                        {
                            fileResult.Url = "";
                            fileResult.Status = "未上传";
                        }
                        else
                        {
                            List<FileResultDto> fileResultSubjectList = CommonHelper.DecodeString<List<FileResultDto>>(answerListSubject[0].FileResult);

                            foreach (FileResultDto fileResultSubject in fileResultSubjectList)
                            {
                                if (subjectFile.SubjectId.ToString() == fileResultSubject.SubjectId
                                    && subjectFile.SeqNO == fileResultSubject.SeqNO
                                    && !string.IsNullOrEmpty(fileResultSubject.Url))
                                {
                                    fileResult.Url = fileResultSubject.Url;
                                    fileResult.Status = "已上传";
                                    break;
                                }
                            }
                        }
                    }
                    fileResultList.Add(fileResult);
                }


                return new APIResult() { Status = true, Body = CommonHelper.Encode(fileResultList) };
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
        public APIResult GetAnswerShopInfo(string projectId, string shopId, string shopkey = "", string userId = "")
        {
            try
            {
                List<AnswerShopInfoDto> result = new List<AnswerShopInfoDto>();

                List<AnswerShopInfoDto> answershopInfoList = answerService.GetAnswerShopInfo(projectId, shopId, shopkey);
                List<UserInfo> userInfoList = masterService.GetUserInfo("", "", userId, "", "", "", "", "", null, "");
                if (string.IsNullOrEmpty(userId)
                    || (userInfoList != null && userInfoList.Count > 0 && userInfoList[0].RoleType != "S_Execute"
                    && userInfoList[0].RoleType != "B_Shop")
                    )
                {
                    result = answershopInfoList;
                }
                else
                {
                    List<UserInfoObjectDto> userInfoObjectDtoList = masterService.GetUserInfoObject(userInfoList[0].TenantId.ToString(), userId, "", "S_Execute");
                    if (userInfoList != null && userInfoList.Count > 0 && userInfoList[0].RoleType == "S_Execute")
                    {
                        foreach (AnswerShopInfoDto answerShopInfoDto in answershopInfoList)
                        {
                            List<UserInfoObjectDto> userInfoObjectList = userInfoObjectDtoList.Where(x => x.ObjectId == answerShopInfoDto.ShopId).ToList();
                            if (userInfoObjectList != null && userInfoObjectList.Count > 0)
                            {
                                result.Add(answerShopInfoDto);
                            }
                        }
                    }
                    else if(userInfoList != null && userInfoList.Count > 0 && userInfoList[0].RoleType == "B_Shop"){
                        result = answershopInfoList;
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(result) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/AnswerShopInfoFileSearch")]
        public APIResult AnswerShopInfoFileSearch(string answerShopInfoFileId, string fileType)
        {
            try
            {
                List<AnswerShopInfoFileDto> list = answerService.AnswerShopInfoFileSearch(answerShopInfoFileId, fileType);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Answer/AnswerShopInfoFileDelete")]
        public APIResult AnswerShopInfoFileDelete(AnswerShopInfoFile answerShopInfoFile)
        {
            try
            {
                answerService.AnswerShopInfoFileDelete(answerShopInfoFile.FileId.ToString());
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Answer/AnswerShopInfolFileSave")]
        public APIResult AnswerShopInfolFileSave(AnswerShopInfoFile answerShopInfoFile)
        {
            try
            {
                answerService.AnswerShopInfoFileSave(answerShopInfoFile);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/AnswerShpoInfoFileDownLoad")]
        public APIResult AnswerShpoInfoFileDownLoad(string projectId, string shopId)
        {
            try
            {
                string downloadPath = photoService.AnswerShopInfoFileDownLoad(projectId, shopId);
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
        [Route("Answer/AnswerShopInfoExport")]
        public APIResult AnswerShopInfoExport(string projectId, string shopId)
        {
            try
            {
                string downloadPath = excelDataService.AnswerShopInfoExport(projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
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
        #region 运营中心
        [HttpPost]
        [Route("Answer/DeleteAnswer")]
        public APIResult DeleteAnswer(string answerId, string projectId, string shopId, string userId)
        {
            try
            {
                // 删除前判断是否已经提交复审，如果已经提交复审需要手动回滚状态
                // 只有未提交复审的打分数据才能删除
                // 复审状态回滚后，如存在复审信息，则复审信息同时删除 ,在Service层执行

                List<RecheckStatusDto> list = recheckService.GetShopRecheckStatus(projectId, shopId, "");
                if (list != null && list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(list[0].Status_S1))
                    {
                        throw new Exception("该经销商已提交复审，请先回滚状态后再删除数据");
                    }
                }
                answerService.DeleteAnswer(answerId, userId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 自检
        // 任务查询
        // taskType = 1
        [HttpGet]
        [Route("Answer/GetTaskProject")]
        public APIResult GetTaskProject(string shopId, string projectId = "", string taskType="")
        {
            try
            {
                List<ProjectDto> projectList = answerService.GetTaskProject(projectId, shopId,taskType);
                foreach (ProjectDto project in projectList)
                {
                    #region 计算拍照点完成数量
                    // 已拍照或者无照片都认为已完成
                    int subjectCompleteCount = 0;
                    // 当前任务，当前经销商下所有拍照点
                    List<AnswerDto> answerList = answerService.GetShopAnswerScoreInfo(project.ProjectId.ToString(), shopId, "", "");
                    foreach (AnswerDto answer in answerList)
                    {
                        if (answer.PhotoScore == 9999)// 无照片
                        {
                            subjectCompleteCount = subjectCompleteCount + 1;
                        }
                        else
                        {
                            List<FileResultDto> fileResult = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                            if (fileResult != null && fileResult.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(fileResult[0].Url))
                                {
                                    subjectCompleteCount = subjectCompleteCount + 1;
                                }
                            }
                        }
                    }
                    project.SubjectCompleteCount = subjectCompleteCount;
                    #endregion
                    #region 剩余时间
                    if (project.EndDate == null)
                    {
                        project.LeftTime = "未设置";
                    }
                    else
                    {
                        if (DateTime.Now >= project.EndDate)
                        {
                            project.LeftTime = "0";
                        }
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(project.EndDate) - DateTime.Now;
                            project.LeftTime = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分";
                        }
                    }

                    #endregion
                    #region 任务状态
                    List<RecheckStatusDto> statusList = recheckService.GetShopRecheckStatusInfo(project.ProjectId.ToString(), shopId, "S1");
                    if (statusList != null && statusList.Count > 0)
                    {
                        project.Status = "已提交";
                    }
                    else if (project.SubjectCompleteCount == project.SubjectCount)
                    {
                        project.Status = "已完成";
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        if (now > project.StartDate)
                        {
                            project.Status = "待开始";
                        }
                        if (project.SubjectCompleteCount != 0
                           && project.SubjectCompleteCount < project.SubjectCount)
                        {
                            project.Status = "进行中";
                        }
                        if (now > project.EndDate)
                        {
                            project.Status = "超时";
                        }
                    }
                    #endregion
                }
                projectList = projectList.OrderByDescending(x => x.StartDate).ToList();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        // 子任务查询
        [HttpGet]
        [Route("Answer/GetSubtaskChapter")]
        public APIResult GetSubtaskChapter(string shopId, string projectId = "")
        {
            try
            {
                List<ChapterDto> chapterList = answerService.GetSubtaskChapter(projectId, shopId);
                if (chapterList != null && chapterList.Count > 0)
                {

                    foreach (ChapterDto chapter in chapterList)
                    {
                        #region 计算拍照点完成数量
                        // 已拍照或者无照片都认为已完成
                        int answerCount = 0;
                        List<AnswerDto> answerList = answerService.GetShopAnswerByChapterId(chapter.ProjectId.ToString(), shopId, chapter.ChapterId.ToString());
                        foreach (AnswerDto answer in answerList)
                        {
                            if (answer.PhotoScore == 9999) // 无照片
                            {
                                answerCount = answerCount + 1;
                            }
                            else
                            {
                                List<FileResultDto> fileResult = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                                if (fileResult != null && fileResult.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(fileResult[0].Url))
                                    {
                                        answerCount = answerCount + 1;
                                    }
                                }
                            }
                        }
                        chapter.SubjectCompleteCount = answerCount;
                        #endregion
                        #region 剩余时间
                        if (chapter.EndDate == null)
                        {
                            chapter.LeftTime = "未设置";
                        }
                        else
                        {
                            if (DateTime.Now >= chapter.EndDate)
                            {
                                chapter.LeftTime = "0";
                            }
                            else
                            {
                                TimeSpan ts = Convert.ToDateTime(chapter.EndDate) - DateTime.Now;
                                chapter.LeftTime = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分";
                            }
                        }
                        #endregion
                        #region 任务状态
                        if (chapter.SubjectCompleteCount == chapter.SubjectCount)
                        {
                            chapter.Status = "已完成";
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            if (now > chapter.StartDate)
                            {
                                chapter.Status = "待开始";
                            }
                            if (chapter.SubjectCompleteCount != 0
                               && chapter.SubjectCompleteCount < chapter.SubjectCount)
                            {
                                chapter.Status = "进行中";
                            }
                            if (now > chapter.EndDate)
                            {
                                chapter.Status = "超时";
                            }
                        }
                        #endregion
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(chapterList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        // 拍照点查询
        [HttpGet]
        [Route("Answer/GetPhotoPoint")]
        public APIResult GetPhotoPoint(string projectId, string shopId, string chapterId)
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerByChapterId(projectId, shopId, chapterId);
                foreach (AnswerDto answer in answerList)
                {
                    // 标准照片信息
                    List<SubjectFile> subjectFileList = masterService.GetSubjectFile(projectId, answer.SubjectId.ToString());
                    List<FileResultDto> fileResultList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                    if (subjectFileList == null || subjectFileList.Count == 0)
                    {
                        answer.PhotoStatus = "已上传";// 未设置标准照片默认已上传
                    }
                    else
                    {
                        int fileResutlCount = fileResultList == null ? 0 : fileResultList.Count;
                        if (fileResutlCount != 0 && !string.IsNullOrEmpty(fileResultList[0].Url))
                        {
                            answer.PhotoStatus = "已上传";
                        }
                        else
                        {
                            answer.PhotoStatus = "未上传";
                        }
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #region 待办
        // 数据采集
        [HttpGet]
        [Route("Answer/GetTaskProjectStat")]
        public APIResult GetTaskProjectStat(string shopId, string projectId = "",string taskType="")
        {
            try
            {
                CountDto count = new CountDto();
                List<ProjectDto> projectList = answerService.GetTaskProject(projectId, shopId, taskType);
                foreach (ProjectDto project in projectList)
                {
                    #region 已完成和未完成数量统计
                    List<RecheckStatusDto> statusList = recheckService.GetShopRecheckStatusInfo(project.ProjectId.ToString(), shopId, "S1");
                    if (statusList != null && statusList.Count > 0)
                    {
                        count.CompleteCount = count.CompleteCount + 1;
                    }
                    else
                    {
                        count.UnCompleteCount = count.UnCompleteCount + 1;
                    }

                    #endregion
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(count) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        // 特殊案例
        [HttpGet]
        [Route("Answer/GetSpecialCaseStat")]
        public APIResult GetSpecialCaseStat(string shopId)
        {
            try
            {
                CountDto count = new CountDto();
                List<SpecialCaseDto> specialCaseList = answerService.GetSpecialCase("", shopId, "", "","");
                foreach (SpecialCaseDto specialCase in specialCaseList)
                {
                    if (!string.IsNullOrEmpty(specialCase.SpecialFeedBack))
                    {
                        count.CompleteCount = count.CompleteCount + 1;
                    }
                    else {
                        count.UnCompleteCount = count.UnCompleteCount + 1;
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(count) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #endregion
        #region 特殊案例
        [HttpGet]
        [Route("Answer/GetSpecialCase")]
        public APIResult GetSpecialCase(string projectId, string shopId, string subjectId, string content,string shopkey="")
        {
            try
            {
                List<SpecialCaseDto> specialCaseList = answerService.GetSpecialCase(projectId, shopId, subjectId, content,shopkey);
                foreach (SpecialCaseDto special in specialCaseList)
                {
                    special.SpecialCaseFileList = answerService.SpecailCaseFileSearch(special.SpecialCaseId.ToString(), "");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(specialCaseList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Answer/SaveSpecialCase")]
        public APIResult SaveSpecialCase(SpecialCase specialCase)
        {
            try
            {
                answerService.SaveSpecialCase(specialCase);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #region 暂时不使用
        [HttpGet]
        [Route("Answer/SpecialCaseFileSearch")]
        public APIResult SpecialCaseFileSearch(string specialCaseId, string fileType)
        {
            try
            {
                List<SpecialCaseFileDto> list = answerService.SpecailCaseFileSearch(specialCaseId, fileType);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Answer/SpecialCaseFileDelete")]
        public APIResult SpecialCaseFileDelete(SpecialCaseFile specialCaseFile)
        {
            try
            {
                answerService.SpecialCaseFileDelete(specialCaseFile.SpecialCaseId.ToString(), specialCaseFile.SeqNO.ToString());
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Answer/SpecialCaseFileSave")]
        public APIResult SpecialCaseFileSave(SpecialCaseFile specialCaseFile)
        {
            try
            {
                answerService.SpecailCaseFileSave(specialCaseFile);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion

        #endregion
        #region 图片上传
        public byte[] Base64ToBytes(string base64Img)
        {
            if (!string.IsNullOrEmpty(base64Img))
            {
                byte[] bytes = Convert.FromBase64String(base64Img);
                return bytes;
            }
            return null;
        }
        public Stream BytesToStream(byte[] dataBytes)
        {
            if (dataBytes == null)
            {
                return null;
            }
            MemoryStream ms = new MemoryStream(dataBytes);
            return ms;
        }
        public string UploadBase64Pic(string filePath, string base64Img)
        {
            if (!string.IsNullOrEmpty(base64Img) && base64Img.Contains("data:image"))
            {
                base64Img = base64Img.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                base64Img = base64Img.Substring(base64Img.IndexOf("base64") + 6);
                if (base64Img.Length % 4 > 0)
                {
                    base64Img = base64Img.PadRight(base64Img.Length + 4 - base64Img.Length % 4, '=');
                }
                Stream stream = BytesToStream(Base64ToBytes(base64Img));
                OSSClientHelper.UploadOSSFile(filePath, stream, stream.Length);
                Thread.Sleep(10);
            }
            return filePath;
        }
        [HttpGet]
        [Route("Answer/UploadFilePic")]
        public APIResult UploadFilePic(string projectId, string shopId, string subjectCode, string base64Img)
        {
            try
            {
                string filePath = @"Survey/" + projectId + @"/" + shopId + @"/" + subjectCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                filePath = UploadBase64Pic(filePath, base64Img);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(filePath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
    }
}
