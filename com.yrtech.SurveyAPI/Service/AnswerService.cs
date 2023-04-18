using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.SurveyAPI.Service
{
    public class AnswerService
    {
        Survey db = new Survey();
        localhost.Service webService = new localhost.Service();
        MasterService masterService = new MasterService();
        AccountService accountService = new AccountService();
        #region 得分登记时调用
        /// <summary>
        /// 获取当前经销商需要打分的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="examTypeId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopNeedAnswerSubject(string projectId, string shopId, string examTypeId, string subjectType)
        {
            projectId = projectId == null ? "" : projectId;
            shopId = shopId == null ? "" : shopId;
            examTypeId = examTypeId == null ? "" : examTypeId;
            subjectType = subjectType == null ? "" : subjectType;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@ExamTypeId", examTypeId),
                                                        new SqlParameter("@SubjectType", subjectType)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT B.AnswerId,A.ProjectId,CAST(@ShopId AS INT) AS ShopId,A.SubjectId,B.PhotoScore,B.InspectionStandardResult,
                            B.FileResult,B.LossResult,B.LossResultAdd,B.Remark,B.Indatetime,B.ModifyDateTime,ISNULL(A.MustScore,0) AS MustScore
                            ,A.SubjectCode,A.OrderNO,a.Remark AS [Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId AND A.SubjectId = B.SubjectId AND B.ShopId = @ShopId
                    WHERE A.ProjectId  = @ProjectId AND  A.OrderNO = 
                                                                (SELECT 
                                                                CASE WHEN EXISTS(SELECT 1 FROM Answer X INNER JOIN [Subject] Y ON X.SubjectId = Y.SubjectId 
                                                                                WHERE X.ProjectId = @ProjectId AND X.ShopId = @ShopId AND Y.HiddenCode_SubjectType = @SubjectType) 
                                                                THEN (SELECT MAX(OrderNO) FROM  Answer X INNER JOIN [Subject] Y ON X.ProjectId = Y.ProjectId 
																                                                                AND X.SubjectId = Y.SubjectId 
																                                                                AND X.ProjectId = @ProjectId 
																                                                                AND X.ShopId = @ShopId
																                                                                AND (Y.LabelId=@ExamTypeId OR Y.LabelId=0 OR Y.LabelId IS NULL) 
                                                                                                                                AND Y.HiddenCode_SubjectType =@SubjectType) 
                                                                ELSE (SELECT ISNULL(MIN(OrderNO),0) FROM [Subject] WHERE ProjectId  = @ProjectId 
                                                                                                                            AND (LabelId=@ExamTypeId OR LabelId=0 OR LabelId IS NULL)
                                                                                                                            AND HiddenCode_SubjectType =@SubjectType )
                                                                END AS OrderNO)";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        /// <summary>
        /// 查询经销商下一个体系的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="orderNO"></param>
        /// <param name="subjectLinkId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopNextAnswerSubject(string projectId, string shopId, string examTypeId, string orderNO, string subjectType)
        {
            projectId = projectId == null ? "" : projectId;
            shopId = shopId == null ? "" : shopId;
            examTypeId = examTypeId == null ? "" : examTypeId;
            subjectType = subjectType == null ? "" : subjectType;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@ExamTypeId", examTypeId),
                                                        new SqlParameter("@SubjectType", subjectType),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT B.AnswerId,A.ProjectId,CAST(@ShopId AS INT) AS ShopId,A.SubjectId,B.PhotoScore,B.InspectionStandardResult,
                            B.FileResult,B.LossResult,B.LossResultAdd,B.Remark,B.Indatetime,B.ModifyDateTime,ISNULL(A.MustScore,0) AS MustScore,
                            A.SubjectCode,A.OrderNO,a.Remark AS [Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                            AND A.SubjectId = B.SubjectId 
                                                            AND B.ShopId =  @ShopId
                    WHERE A.ProjectId  = @ProjectId 
                    AND  A.OrderNO =(SELECT ISNULL(MIN(OrderNO),0) 
                                    FROM [Subject] 
                                    WHERE ProjectId = @ProjectId 
                                    AND (LabelId =  @ExamTypeId OR LabelId=0 OR LabelId IS NULL)
                                    AND HiddenCode_SubjectType = @SubjectType
                                    AND OrderNO > @OrderNO)";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        /// <summary>
        /// 查询经销商上一个体系的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="orderNO"></param>
        /// <param name="subjectLinkId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopPreAnswerSubject(string projectId, string shopId, string examTypeId, string orderNO, string subjectType)
        {
            projectId = projectId == null ? "" : projectId;
            shopId = shopId == null ? "" : shopId;
            examTypeId = examTypeId == null ? "" : examTypeId;
            subjectType = subjectType == null ? "" : subjectType;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                        new SqlParameter("@ExamTypeId", examTypeId),
                                                        new SqlParameter("@SubjectType", subjectType),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT B.AnswerId,A.ProjectId,CAST(@ShopId AS INT) AS ShopId,A.SubjectId,B.PhotoScore,B.InspectionStandardResult,
                            B.FileResult,B.LossResult,B.LossResultAdd,B.Remark,B.Indatetime,B.ModifyDateTime,ISNULL(A.MustScore,0) AS MustScore,
                            A.SubjectCode,A.OrderNO,a.Remark AS [Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                        AND A.SubjectId = B.SubjectId 
                                                        AND B.ShopId =  @ShopId
                    WHERE A.ProjectId  = @ProjectId 
                    AND  A.OrderNO =(SELECT ISNULL(MAX(OrderNO),0) FROM [Subject] 
                                                                    WHERE ProjectId = @ProjectId 
                                                                    AND (LabelId = @ExamTypeId OR LabelId=0 OR LabelId IS NULL)
                                                                    AND HiddenCode_SubjectType = @SubjectType
                                                                    AND OrderNO < @OrderNO)";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        /// <summary>
        /// 按照需要查询题目
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="orderNO"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopTransAnswerSubject(string projectId, string shopId, string orderNO, string subjectType)
        {
            projectId = projectId == null ? "" : projectId;
            shopId = shopId == null ? "" : shopId;
            orderNO = orderNO == null ? "" : orderNO;
            subjectType = subjectType == null ? "" : subjectType;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectType", subjectType),
                                                        new SqlParameter("@OrderNO", orderNO)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT B.AnswerId,A.ProjectId,CAST(@ShopId AS INT) AS ShopId,A.SubjectId,B.PhotoScore,B.InspectionStandardResult,
                            B.FileResult,B.LossResult,B.Remark,B.Indatetime,B.ModifyDateTime,ISNULL(A.MustScore,0) AS MustScore,
                            A.SubjectCode,A.OrderNO,a.Remark AS [Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                        AND A.SubjectId = B.SubjectId 
                                                        AND B.ShopId =  @ShopId
                    WHERE A.ProjectId  = @ProjectId 
                    AND  A.OrderNO = @OrderNO
                    AND A.HiddenCode_SubjectType = @SubjectType";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }

        /// <summary>
        /// 获取单个经销商打分信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopAnswerScoreInfo(string projectId, string shopId, string subjectId, string key)
        {
            // 获取打分的信息
            shopId = shopId == null ? "" : shopId;
            subjectId = subjectId == null ? "" : subjectId;
            key = key == null ? "" : key;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectId", subjectId)
                                                       };
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"  SELECT  A.ProjectId,A.LabelId AS ExamTypeId,B.ShopId,A.SubjectId,B.ShopCode,B.ShopName,A.SubjectCode,A.[CheckPoint],A.OrderNO,A.Remark AS [Desc],A.InspectionDesc,A.HiddenCode_SubjectType
                             ,ISNULL(C.AnswerId,0) AS AnswerId,C.PhotoScore, C.Remark,C.InspectionStandardResult,C.FileResult,C.LossResult,C.InDateTime,C.ModifyDateTime
                             ,a.FullScore,a.LowScore
                    FROM [Subject] A CROSS JOIN 
                                    (SELECT * FROM Shop WHERE ShopId = @ShopId ) B 
							INNER JOIN ProjectShopExamType D ON B.ShopId = D.ShopId AND D.ProjectId=@ProjectId
                           LEFT JOIN Answer C ON A.SubjectId = c.SubjectId AND A.ProjectId = C.ProjectId AND B.ShopId = C.ShopId
                    WHERE A.ProjectId = @ProjectId AND ( A.LabelId=0 OR A.LabelId IS NULL OR A.LabelId = D.ExamTypeId) ";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND A.SubjectId =@SubjectId ";
            }
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (B.ShopCode LIKE '%" + key + "%' OR B.ShopName LIKE '%" + key + "%' OR B.ShopShortName LIKE '%" + key + "%')";
            }
            sql += " ORDER BY A.ProjectId,B.ShopCode,A.OrderNO,A.SubjectId";
            List<AnswerDto> answerList = db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
            return answerList;
        }
        /// <summary>
        /// 获取经销商还未打分的题目
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopScoreInfo_NotAnswer(string projectId, string shopId, string labelId)
        {
            shopId = shopId == null ? "" : shopId;
            labelId = labelId == null ? "" : labelId;

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),new SqlParameter("@LabelId", labelId)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"  SELECT  A.ProjectId,A.LabelId AS ExamTypeId,A.SubjectId,A.SubjectCode,A.[CheckPoint],A.OrderNO,A.[Desc],A.InspectionDesc,A.HiddenCode_SubjectType
                    FROM [Subject] A 
                    WHERE A.SubjectId NOT IN (SELECT SubjectId FROM Answer WHERE ProjectId = A.ProjectId AND ShopId = @ShopId)
                    AND  A.ProjectId = @ProjectId
                    AND (A.LabelId = @LabelId  OR A.LabelId=0 OR A.LabelId IS NULL)";

            List<AnswerDto> answerList = db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
            return answerList;
        }
        /// <summary>
        /// 获取照片上传的状态
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AnswerPhotoLogDto> GetShopAnsewrPhotoLog(string projectId, string shopId)
        {
            PhotoService photoService = new PhotoService();
            List<AnswerDto> answerList = GetShopAnswerScoreInfo(projectId, shopId, "", "");
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
                if (lossResultList != null && lossResultList.Count > 0)
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
            return photoList;
        }
        /// <summary>
        /// 保存打分信息
        /// </summary>
        /// <param name="answerDto"></param>
        /// <param name="userId"></param>
        public void SaveAnswerInfo(AnswerDto answerDto)
        {
            Answer answer = new Answer();
            answer.ProjectId = answerDto.ProjectId;
            answer.ShopId = Convert.ToInt32(answerDto.ShopId);
            answer.SubjectId = Convert.ToInt32(answerDto.SubjectId);
            answer.PhotoScore = answerDto.PhotoScore;
            answer.PhotoScoreResult = answerDto.PhotoScoreResult;
            answer.Remark = answerDto.Remark;
            answer.FileResult = answerDto.FileResult;
            answer.InspectionStandardResult = answerDto.InspectionStandardResult;
            answer.LossResult = answerDto.LossResult;
            answer.LossResultAdd = answerDto.LossResultAdd;
            answer.ShopConsultantResult = answerDto.ShopConsultantResult;
            answer.InDateTime = DateTime.Now;
            answer.InUserId = Convert.ToInt32(answerDto.InUserId);
            answer.ModifyDateTime = DateTime.Now;
            answer.ModifyUserId = Convert.ToInt32(answerDto.ModifyUserId);
            Answer findOne = db.Answer.Where(x => (x.ProjectId == answerDto.ProjectId && x.ShopId == answerDto.ShopId && x.SubjectId == answerDto.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.ModifyDateTime = DateTime.Now;
                db.Answer.Add(answer);
                db.SaveChanges();
                SaveAnswerLogInfo(answer, "I");
            }
            else
            {
                findOne.PhotoScore = answer.PhotoScore;
                findOne.PhotoScoreResult = answer.PhotoScoreResult;
                findOne.Remark = answer.Remark;
                findOne.FileResult = answer.FileResult;
                findOne.InspectionStandardResult = answer.InspectionStandardResult;
                findOne.LossResult = answer.LossResult;
                findOne.LossResultAdd = answer.LossResultAdd;
                findOne.ShopConsultantResult = answer.ShopConsultantResult;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = answer.ModifyUserId;
                db.SaveChanges();
                SaveAnswerLogInfo(findOne, "U");
            }
            
            //if (answerDto.AnswerId == null || answerDto.AnswerId == 0) // 插入
            //{
               
            //}
            //else
            //{
                
            //}
        }
        public void SaveAnswerLogInfo(Answer answer, string dataStatus)
        {
            AnswerLog answerLog = new AnswerLog();
            answerLog.AnswerId = answer.AnswerId;
            answerLog.ProjectId = answer.ProjectId;
            answerLog.ShopId = Convert.ToInt32(answer.ShopId);
            answerLog.SubjectId = Convert.ToInt32(answer.SubjectId);
            answerLog.PhotoScore = answer.PhotoScore;
            answerLog.PhotoScoreResult = answer.PhotoScoreResult;
            answerLog.Remark = answer.Remark;
            answerLog.FileResult = answer.FileResult;
            answerLog.InspectionStandardResult = answer.InspectionStandardResult;
            answerLog.LossResult = answer.LossResult;
            answerLog.ShopConsultantResult = answer.ShopConsultantResult;
            answerLog.InDateTime = DateTime.Now;
            answerLog.InUserId = Convert.ToInt32(answer.ModifyUserId);
            answerLog.DataStatus = dataStatus;
            db.AnswerLog.Add(answerLog);
            db.SaveChanges();
        }

        #endregion
        #region 经销商进店信息
        /// <summary>
        /// 查询经销商进店信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AnswerShopInfoDto> GetAnswerShopInfo(string projectId, string shopId)
        {
            shopId = shopId == null ? "" : shopId;
            projectId = projectId == null ? "" : projectId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(AnswerShopInfoDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,B.Province,B.City,ISNULL(C.TeamLeader,'') AS TeamLeader
                    ,C.StartDate,C.Longitude,C.Latitude,C.InDateTime,C.ModifyDateTime,C.PhotoUrl
                    ,[InShopMode]
                      ,B.Address AS InShopAddress
                      ,[AddressCheck]
                      ,[SalesName]
                      ,[SalesNameCheckMode]
                      ,[SakesNameCheckReason]
                      ,[ExecuteName]
                      ,[ExcuteAddress]
                      ,[ExcuteCity]
                      ,[ExcuteJob]
                      ,[CarBuyPurpose]
                      ,[CarBuyBudget]
                      ,[CarBuyType]
                      ,[CarCompetitor]
                      ,[ExcuteHomeAddress]
                      ,[ExcutePhone]
                      ,[InShopStartDate]
                      ,[InShopEndDate]
                      ,[TestDriverCheck]
                      ,[TestDriverStartDate]
                      ,[TestDriverEndDate]
                      ,[WeatherCondition]
                      ,[OutShopCondition]
                      ,[InShopCondition]
                      ,[VideoComplete]
                      ,[ExecuteRecogniz]
                       FROM ProjectShopExamType A INNER JOIN Shop B ON A.ShopId  = B.ShopId
							                      LEFT JOIN AnswerShopInfo C ON A.ProjectId = C.ProjectId AND A.ShopId = C.ShopId
                            WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId =@ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerShopInfoDto>().ToList();
        }
        /// <summary>
        /// 保存经销商进店信息
        /// </summary>
        /// <param name="shopInfo"></param>
        /// <param name="userId"></param>
        public void SaveAnswerShopInfo(AnswerShopInfo shopInfo)
        {
            AnswerShopInfo findOne = db.AnswerShopInfo.Where(x => (x.ProjectId == shopInfo.ProjectId && x.ShopId == shopInfo.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                shopInfo.InDateTime = DateTime.Now;
                shopInfo.ModifyDateTime = DateTime.Now;
                db.AnswerShopInfo.Add(shopInfo);
            }
            else
            {
                findOne.TeamLeader = shopInfo.TeamLeader;
                findOne.StartDate = shopInfo.StartDate;
                findOne.PhotoUrl = shopInfo.PhotoUrl;
                findOne.Latitude = shopInfo.Latitude;
                findOne.Longitude = shopInfo.Longitude;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = shopInfo.ModifyUserId;
                findOne.InShopMode = shopInfo.InShopMode;
                findOne.InShopAddress = shopInfo.InShopAddress;
                findOne.AddressCheck = shopInfo.AddressCheck;
                findOne.SalesName = shopInfo.SalesName;
                findOne.SalesNameCheckMode = shopInfo.SalesNameCheckMode;
                findOne.SakesNameCheckReason = shopInfo.SakesNameCheckReason;
                findOne.ExecuteName = shopInfo.ExecuteName;
                findOne.ExcuteAddress = shopInfo.ExcuteAddress;
                findOne.ExcuteCity = shopInfo.ExcuteCity;
                findOne.ExcuteJob = shopInfo.ExcuteJob;
                findOne.CarBuyPurpose = shopInfo.CarBuyPurpose;
                findOne.CarBuyBudget = shopInfo.CarBuyBudget;
                findOne.CarBuyType = shopInfo.CarBuyType;
                findOne.CarCompetitor = shopInfo.CarCompetitor;
                findOne.ExcuteHomeAddress = shopInfo.ExcuteHomeAddress;
                findOne.ExcutePhone = shopInfo.ExcutePhone;
                findOne.InShopStartDate = shopInfo.InShopStartDate;
                findOne.InShopEndDate = shopInfo.InShopEndDate;
                findOne.TestDriverCheck = shopInfo.TestDriverCheck;
                findOne.TestDriverStartDate = shopInfo.TestDriverStartDate;
                findOne.TestDriverEndDate = shopInfo.TestDriverEndDate;
                findOne.WeatherCondition = shopInfo.WeatherCondition;
                findOne.OutShopCondition = shopInfo.OutShopCondition;
                findOne.InShopCondition = shopInfo.InShopCondition;
                findOne.VideoComplete = shopInfo.VideoComplete;
                findOne.ExecuteRecogniz = shopInfo.ExecuteRecogniz;
            }
            db.SaveChanges();
        }
        #endregion
        #region 销售顾问信息
        public List<ShopConsultantDto> GetShopConsultant(string projectId, string shopId)
        {

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(ShopConsultantDto);
            string sql = "";
            sql = @"SELECT A.*
                    FROM AnswerShopConsultant A  
		            WHERE A.ProjectId = @ProjectId
		            AND A.ShopId = @ShopId 
                    ORDER BY UseChk DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<ShopConsultantDto>().ToList();
        }
        public List<ShopConsultantSubjectLinkDto> GetShopConsultantSubjectLink(string projectId, string consultantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ConsultantId", consultantId)};
            Type t = typeof(ShopConsultantSubjectLinkDto);
            string sql = "";
            sql = @"SELECT B.AnswerShopConsultantSubjectId,A.ConsultantId,A.ConsultantName
                            ,B.SubjectLinkId,C.SubjectLinkCode,C.SubjectLinkName
                            ,B.InUserId,B.InDateTime
                    FROM dbo.AnswerShopConsultant A INNER JOIN dbo.AnswerShopConsultantSubjectLink B ON A.ConsultantId = B.ConsultantId
                                                    INNER JOIN dbo.SubjectLink C ON A.ProjectId = C.ProjectId AND B.SubjectLinkId = C.SubjectLInkId";
            sql += " WHERE 1=1 AND A.ProjectId = @ProjectId AND A.ConsultantId = @ConsultantId";
            return db.Database.SqlQuery(t, sql, para).Cast<ShopConsultantSubjectLinkDto>().ToList();

        }
        public void SaveShopConsultant(ShopConsultantDto consultantDto)
        {
            // 保存顾问信息
            AnswerShopConsultant consultant = new AnswerShopConsultant();
            consultant.ConsultantId = consultantDto.ConsultantId;
            consultant.ConsultantName = consultantDto.ConsultantName;
            consultant.ConsultantType = consultantDto.ConsultantType;
            consultant.InUserId = consultantDto.InUserId;
            consultant.ModifyUserId = consultantDto.ModifyUserId;
            consultant.ProjectId = consultantDto.ProjectId;
            consultant.SeqNO = consultantDto.SeqNO;
            consultant.ShopId = consultantDto.ShopId;
            consultant.UseChk = consultantDto.UseChk;
            //List<Project> projectList = masterService.GetProject("", "", consultant.ProjectId.ToString(),"");
            //if (projectList == null || projectList.Count == 0)
            //{
            //    throw new Exception("没有找到对应的期号");
            //}
            //List<Shop> shopList = masterService.GetShop("", "", consultant.ShopId.ToString(),"","");
            //if (shopList == null || shopList.Count == 0)
            //{
            //    throw new Exception("没有找到对应的经销商");
            //}
            //List<UserInfo> userList = accountService.GetUserInfo("",consultant.ModifyUserId.ToString(),"","");
            //if (userList == null || userList.Count == 0)
            //{
            //    throw new Exception("没有找到对应的用户");
            //}
            //string shopCode = shopList[0].ShopCode;
            //string brandId = shopList[0].BrandId.ToString();
            //string projectCode = projectList[0].ProjectCode;
            //string accountId = userList[0].AccountId;

            //if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }

            //webService.SaveSaleContantInfo(projectCode, shopCode, consultant.SeqNO.ToString(), consultant.ConsultantName, consultant.ConsultantType);

            AnswerShopConsultant findOne = db.AnswerShopConsultant.Where(x => (x.ProjectId == consultant.ProjectId && x.ShopId == consultant.ShopId && x.SeqNO == consultant.SeqNO)).FirstOrDefault();
            if (findOne == null)
            {
                consultant.InDateTime = DateTime.Now;
                consultant.ModifyDateTime = DateTime.Now;
                db.AnswerShopConsultant.Add(consultant);
            }
            else
            {
                findOne.ConsultantName = consultant.ConsultantName;
                findOne.ConsultantType = consultant.ConsultantType;
                findOne.UseChk = consultant.UseChk;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = consultant.ModifyUserId;
            }
            db.SaveChanges();
            int consultantId = db.AnswerShopConsultant.Where(x => (x.ProjectId == consultant.ProjectId && x.ShopId == consultant.ShopId && x.SeqNO == consultant.SeqNO)).FirstOrDefault().ConsultantId;
            foreach (ShopConsultantSubjectLinkDto subjectLink in consultantDto.ShopConsultantSubjectLinkList)
            {
                AnswerShopConsultantSubjectLink consultantSubjectLink = new AnswerShopConsultantSubjectLink();
                consultantSubjectLink.ConsultantId = consultantId;
                consultantSubjectLink.InUserId = subjectLink.InUserId;
                consultantSubjectLink.SubjectLinkId = subjectLink.SubjectLinkId;
                SaveShopConsultantSubjectLink(consultantSubjectLink);
            }
        }
        public void SaveShopConsultantSubjectLink(AnswerShopConsultantSubjectLink subjectLink)
        {
            AnswerShopConsultantSubjectLink findOne = db.AnswerShopConsultantSubjectLink.Where(x => (x.ConsultantId == subjectLink.ConsultantId && x.SubjectLinkId == subjectLink.SubjectLinkId)).FirstOrDefault();
            if (findOne == null)// 只会执行添加操作，不能修改
            {
                subjectLink.InDateTime = DateTime.Now;
                db.AnswerShopConsultantSubjectLink.Add(subjectLink);
            }
            db.SaveChanges();
        }
        #endregion
        #region 运营中心
        public void DeleteAnswer(string answerId, string userId)
        {
            SqlParameter[] para = new SqlParameter[] { };
            string sql = "";
            string[] answerIdList = answerId.Split(',');
            foreach (string answerIdStr in answerIdList)
            {
                sql += @" INSERT INTO AnswerLog
                          SELECT[AnswerId]
                              ,[ProjectId]
                              ,[SubjectId]
                              ,[ShopId]
                              ,[PhotoScore]
                              ,[PhotoScoreResult]
                              ,[InspectionStandardResult]
                              ,[FileResult]
                              ,[LossResult]
                              ,[LossResultAdd]
                              ,[ShopConsultantResult]
                              ,[Remark]
                              ,'D'
                              ,''
                              ," + userId +
                              @",GETDATE()
                              FROM [Answer] WHERE AnswerId = " + answerIdStr;
                sql += @" DELETE Recheck
                         FROM Recheck A INNER JOIN Answer B ON A.ProjectId = B.ProjectId
                                                            AND A.ShopId = B.ShopId
                                                            AND A.SubjectId = B.SubjectId
                        WHERE B.AnswerId =" + answerIdStr;

                sql += " DELETE Answer WHERE AnswerId = " + answerIdStr;
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public void DeleteAnswerShopInfo(string projectId, string shopId)
        {
            SqlParameter[] para = new SqlParameter[] { };
            string sql = "";
            sql += " DELETE AnswerShopInfo WHERE ProjectId = " + projectId;
            sql += " AND ShopId = " + shopId;
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion
    }
}