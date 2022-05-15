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
        //#region 不联网版本
        ///// <summary>
        ///// 保存答题信息列表
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //public void SaveAnswerList(List<Answer> lst, string userId)
        //{

        //    if (lst == null) return;
        //    //string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString(),"","")[0].ShopCode;
        //    //string brandId = masterService.GetShop("", "", lst[0].ShopId.ToString(),"","")[0].BrandId.ToString();
        //    //string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString(),"")[0].ProjectCode;
        //    //string accountId = accountService.GetUserInfo("",userId,"","")[0].AccountId;
        //    // if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }
        //    //try
        //    //{
        //    //    /// 保存得分信息
        //    //    foreach (Answer answer in lst)
        //    //    {
        //    //        string subjectCode = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString())[0].SubjectCode;
        //    //        webService.SaveAnswer(projectCode, subjectCode, shopCode, answer.PhotoScore,//score 赋值photoscore,模拟得分在上传的会自动计算覆盖
        //    //            answer.Remark, "", accountId, '0', "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(answer.InDateTime).ToString("yyyy-MM-dd HH:mm:ss"), answer.PhotoScore.ToString());
        //    //        List<InspectionStandardResultDto> inspectionList = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(answer.InspectionStandardResult);
        //    //        List<FileResultDto> fileList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
        //    //        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
        //    //        List<ShopConsultantResultDto> shopConsultantList = CommonHelper.DecodeString<List<ShopConsultantResultDto>>(answer.ShopConsultantResult);
        //    //        //CommonHelper.log(answer.ShopConsultantResult.ToString());
        //    //        if (inspectionList != null)
        //    //        {
        //    //            foreach (InspectionStandardResultDto inspection in inspectionList)
        //    //            {
        //    //                webService.SaveAnswerDtl(projectCode, subjectCode, shopCode, Convert.ToInt32(inspection.SeqNO), accountId, inspection.AnswerResult, "");
        //    //            }
        //    //        }
        //    //        if (fileList != null)
        //    //        {
        //    //            foreach (FileResultDto file in fileList)
        //    //            {
        //    //                webService.SaveAnswerDtl2Stream(projectCode, subjectCode, shopCode, Convert.ToInt32(file.SeqNO), accountId, "", null, "", file.FileName);
        //    //            }
        //    //        }
        //    //        if (lossResultList != null)
        //    //        {
        //    //            foreach (LossResultDto loss in lossResultList)
        //    //            {
        //    //                webService.SaveAnswerDtl3(projectCode, subjectCode, shopCode, Convert.ToInt32(loss.LossId), loss.LossDesc, loss.LossFileNameUrl);
        //    //            }
        //    //        }
        //    //        if (shopConsultantList != null)
        //    //        {
        //    //            foreach (ShopConsultantResultDto shopConsult in shopConsultantList)
        //    //            {
        //    //                CommonHelper.log(shopConsult.ConsultantScore.ToString());
        //    //                // System.Threading.Thread.Sleep(500);
        //    //                webService.SaveSalesConsultant_Upload(projectCode, shopCode, subjectCode, shopConsult.ConsultantName, shopConsult.ConsultantScore.HasValue ? shopConsult.ConsultantScore.ToString() : "", shopConsult.ConsultantLossDesc, accountId, shopConsult.ConsultantType);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    CommonHelper.log(ex.ToString());
        //    //}

        //    foreach (Answer answer in lst)
        //    {
        //        Answer findOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.SubjectId == answer.SubjectId)).FirstOrDefault();

        //        if (findOne == null)
        //        {
        //            db.Answer.Add(answer);
        //        }
        //        else
        //        {
        //            findOne.FileResult = answer.FileResult;
        //            findOne.InspectionStandardResult = answer.InspectionStandardResult;
        //            findOne.LossResult = answer.LossResult;
        //            findOne.LossResutAdd = answer.LossResutAdd;
        //            findOne.ModifyDateTime = answer.ModifyDateTime;
        //            findOne.ModifyUserId = answer.ModifyUserId;
        //            findOne.PhotoScore = answer.PhotoScore;
        //            findOne.Remark = answer.Remark;
        //            findOne.ShopConsultantResult = answer.ShopConsultantResult;
        //            //db.Entry<Answer>(answer).State = System.Data.Entity.EntityState.Modified;
        //        }
        //    }
        //    db.SaveChanges();
        //}

        ///// <summary>
        ///// 保存信息
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //public void SaveAnswerShopInfoList(List<AnswerShopInfo> lst, string userId)
        //{
        //    ////CommonHelper.log("Service" + lst.ToString());
        //    //if (lst == null) return;
        //    ////CommonHelper.log(lst.ToString());
        //    //string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString(),"","")[0].ShopCode;
        //    //string brandId = masterService.GetShop("", "", lst[0].ShopId.ToString(),"","")[0].BrandId.ToString();
        //    //string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString(),"")[0].ProjectCode;
        //    //string accountId = accountService.GetUserInfo("",userId,"","")[0].AccountId;
        //    //if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }
        //    //// 保存数据到原系统
        //    //CommonHelper.log(webService.Url + " " + brandId.ToString());

        //    //foreach (AnswerShopInfo answerShopInfo in lst)
        //    //{
        //    //    webService.AnswerStartInfoSave(projectCode, shopCode, answerShopInfo.TeamLeaderName, accountId, Convert.ToDateTime(answerShopInfo.StartDate).ToString("yyyy-MM-dd HH:mm:ss"));
        //    //}
        //    foreach (AnswerShopInfo answerShopInfo in lst)
        //    {
        //        AnswerShopInfo findOne = db.AnswerShopInfo.Where(x => (x.ProjectId == answerShopInfo.ProjectId && x.ShopId == answerShopInfo.ShopId)).FirstOrDefault();
        //        if (findOne == null)
        //        {
        //            db.AnswerShopInfo.Add(answerShopInfo);
        //        }
        //        else
        //        {
        //            findOne.ModifyDateTime = answerShopInfo.ModifyDateTime;
        //            findOne.ModifyUserId = answerShopInfo.ModifyUserId;
        //            findOne.StartDate = answerShopInfo.StartDate;
        //            findOne.TeamLeader = answerShopInfo.TeamLeader;
        //            //db.Entry<AnswerShopInfo>(findOne).State = System.Data.Entity.EntityState.Modified;
        //        }
        //    }
        //    db.SaveChanges();
        //}

        ///// <summary>
        ///// 保存顾问信息列表
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //public void SaveAnswerShopConsultantList(List<AnswerShopConsultant> lst, string userId)
        //{

        //    if (lst == null || lst.Count == 0) return;
        //    //List<Project> projectList = masterService.GetProject("", "", lst[0].ProjectId.ToString(),"");
        //    //if (projectList == null || projectList.Count == 0)
        //    //{
        //    //    throw new Exception("没有找到对应的期号");
        //    //}
        //    //List<Shop> shopList = masterService.GetShop("", "", lst[0].ShopId.ToString(),"","");
        //    //if (shopList == null || shopList.Count == 0)
        //    //{
        //    //    throw new Exception("没有找到对应的经销商");
        //    //}
        //    //List<UserInfo> userList = accountService.GetUserInfo("",userId,"","");
        //    //if (userList == null || userList.Count == 0)
        //    //{
        //    //    throw new Exception("没有找到对应的用户");
        //    //}

        //    //string shopCode = shopList[0].ShopCode;
        //    //string projectCode = projectList[0].ProjectCode;
        //    //string accountId = userList[0].AccountId;
        //    //foreach (AnswerShopConsultant item in lst)
        //    //{
        //    //    // 不需要了，在保存分数的时候一块保存了
        //    //    // webService.SaveSaleContantInfo(projectCode, shopCode, item.SeqNO.ToString(), item.ConsultantName, item.ConsultantType);
        //    //}
        //    foreach (AnswerShopConsultant item in lst)
        //    {
        //        item.UploadDateTime = DateTime.Now;
        //        item.UploadUserId = Convert.ToInt32(userId);
        //        AnswerShopConsultant findOne = db.AnswerShopConsultant.Where(x => (x.ProjectId == item.ProjectId && x.ShopId == item.ShopId && x.ConsultantName == item.ConsultantName && x.ConsultantType == item.ConsultantType)).FirstOrDefault();
        //        if (findOne == null)
        //        {
        //            db.AnswerShopConsultant.Add(item);
        //        }
        //        else
        //        {
        //            findOne.ModifyDateTime = item.ModifyDateTime;
        //            findOne.ModifyUserId = item.ModifyUserId;
        //            findOne.UploadUserId = item.UploadUserId;
        //            findOne.UploadDateTime = item.UploadDateTime;
        //            findOne.SubjectLinkId = item.SubjectLinkId;
        //            findOne.UseChk = item.UseChk;
        //            // db.Entry<AnswerShopConsultant>(item).State = System.Data.Entity.EntityState.Modified;
        //        }
        //    }
        //    db.SaveChanges();
        //}
        //#endregion
        #region 得分登记时调用
        /// <summary>
        /// 获取当前经销商需要打分的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="examTypeId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopNeedAnswerSubject(string projectId, string shopId, string examTypeId,string subjectType)
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
                            B.FileResult,B.LossResult,B.LossResultAdd,B.Remark,B.Indatetime,B.ModifyDateTime,
                            A.SubjectCode,A.OrderNO,a.[Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId AND A.SubjectId = B.SubjectId AND B.ShopId = @ShopId
                    WHERE A.ProjectId  = @ProjectId AND  A.OrderNO = 
                                                                (SELECT 
                                                                CASE WHEN EXISTS(SELECT 1 FROM Answer X INNER JOIN [Subject] Y ON X.ProjectId = Y.SubjectId 
                                                                                WHERE X.ProjectId = @ProjectId AND X.ShopId = @ShopId AND Y.HiddenCode_SubjectType = @SubjectType) 
                                                                THEN (SELECT MAX(OrderNO) FROM  Answer X INNER JOIN [Subject] Y ON X.ProjectId = Y.ProjectId 
																                                                                AND X.SubjectId = Y.SubjectId 
																                                                                AND X.ProjectId = @ProjectId 
																                                                                AND X.ShopId = @ShopId
																                                                                AND Y.LabelId=@ExamTypeId
                                                                                                                                AND Y.HiddenCode_SubjectType =@SubjectType) 
                                                                ELSE (SELECT ISNULL(MIN(OrderNO),0) FROM [Subject] WHERE ProjectId  = @ProjectId 
                                                                                                                            AND LabelId=@ExamTypeId
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
        public List<AnswerDto> GetShopNextAnswerSubject(string projectId, string shopId, string examTypeId, string orderNO,string subjectType)
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
                            B.FileResult,B.LossResult,B.LossResultAdd,B.Remark,B.Indatetime,B.ModifyDateTime,
                            A.SubjectCode,A.OrderNO,a.[Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                            AND A.SubjectId = B.SubjectId 
                                                            AND B.ShopId =  @ShopId
                    WHERE A.ProjectId  = @ProjectId 
                    AND  A.OrderNO =(SELECT ISNULL(MIN(OrderNO),0) 
                                    FROM [Subject] 
                                    WHERE ProjectId = @ProjectId 
                                    AND LabelId =  @ExamTypeId
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
                            B.FileResult,B.LossResult,B.LossResultAdd,B.Remark,B.Indatetime,B.ModifyDateTime,
                            A.SubjectCode,A.OrderNO,a.[Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
                    FROM  [Subject] A LEFT JOIN Answer B ON A.ProjectId = B.ProjectId 
                                                        AND A.SubjectId = B.SubjectId 
                                                        AND B.ShopId =  @ShopId
                    WHERE A.ProjectId  = @ProjectId 
                    AND  A.OrderNO =(SELECT ISNULL(MAX(OrderNO),0) FROM [Subject] 
                                                                    WHERE ProjectId = @ProjectId 
                                                                    AND LabelId = @ExamTypeId 
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
        public List<AnswerDto> GetShopTransAnswerSubject(string projectId, string shopId, string orderNO,string subjectType)
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
                            B.FileResult,B.LossResult,B.Remark,B.Indatetime,B.ModifyDateTime,
                            A.SubjectCode,A.OrderNO,a.[Desc],a.FullScore,a.LowScore,a.[CheckPoint],a.Implementation,a.Inspectiondesc,A.HiddenCode_SubjectType
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
            sql = @"  SELECT  A.ProjectId,A.LabelId AS ExamTypeId,B.ShopId,A.SubjectId,B.ShopCode,B.ShopName,A.SubjectCode,A.[CheckPoint],A.OrderNO,A.[Desc],A.InspectionDesc,A.HiddenCode_SubjectType
                             ,C.PhotoScore, C.Remark,C.InspectionStandardResult,C.FileResult,C.LossResult,C.InDateTime,C.ModifyDateTime
                    FROM [Subject] A CROSS JOIN 
                                    (SELECT * FROM Shop WHERE ShopId = @ShopId) B 
                        LEFT JOIN Answer C ON A.SubjectId = c.SubjectId AND A.ProjectId = C.ProjectId AND B.ShopId = C.ShopId
                    WHERE A.ProjectId = @ProjectId";
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
            //// 绑定销售顾问打分信息,并计算当前题的得分，模拟得分
            //foreach (AnswerDto answer in answerList)
            //{
            //    answer.ShopConsultantResult = CommonHelper.Encode(GetShopConsultantScore(answer.AnswerId.ToString(), ""));
            //    decimal? consultantScore = AvgConsultantScore(answer.AnswerId.ToString());
            //    if (consultantScore == null && (answer.PhotoScore == null|| Convert.ToInt32(answer.PhotoScore)==9999))
            //    {
            //        answer.Score = Convert.ToDecimal(9999);
            //    }
            //    else if (consultantScore == null)
            //    {
            //        answer.Score = Math.Round(Convert.ToDecimal(answer.PhotoScore),2);
            //    }
            //    else if (answer.PhotoScore == null)
            //    {
            //        answer.Score = Math.Round(Convert.ToDecimal(consultantScore),2);
            //    }
            //    else {
            //        answer.Score = Math.Round(Convert.ToDecimal((answer.PhotoScore + consultantScore) / 2),2);
            //    }
            //    answer.ConsultantScore = consultantScore;
            //}

            return answerList;
        }
        /// <summary>
        /// 获取经销商还未打分的题目
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopScoreInfo_NotAnswer(string projectId, string shopId)
        {
            shopId = shopId == null ? "" : shopId;
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"  SELECT  A.ProjectId,A.LabelId AS ExamTypeId,A.SubjectId,A.SubjectCode,A.[CheckPoint],A.OrderNO,A.[Desc],A.InspectionDesc,A.HiddenCode_SubjectType
                    FROM [Subject] A 
                    WHERE A.SubjectId NOT IN (SELECT SubjectId FROM Answer WHERE ProjectId = A.ProjectId AND ShopId = @ShopId)
                    AND  A.ProjectId = @ProjectId";
            List<AnswerDto> answerList = db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
            return answerList;
        }

        //public decimal? AvgConsultantScore(string answerId)
        //{
        //    decimal? avgScore = null;
        //    decimal? totalScore = Convert.ToDecimal(0);
        //    int count = 0;
        //    List < ShopConsultantResultDto > consultScoreList= GetShopConsultantScore(answerId,"");
        //    {
        //        foreach (ShopConsultantResultDto result in consultScoreList)
        //        {
        //            if (result.ConsultantScore != null && Convert.ToInt32(result.ConsultantScore) != 9999)
        //            {
        //                totalScore += result.ConsultantScore;
        //                count++;
        //            }
        //        }
        //    }
        //    if (count != 0)
        //    {
        //        avgScore = totalScore / count;
        //    }
        //    return avgScore;
        //}
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
            answer.Remark = answerDto.Remark;
            answer.FileResult = answerDto.FileResult;
            answer.InspectionStandardResult = answerDto.InspectionStandardResult;
            answer.LossResult = answerDto.LossResult;
            answer.ShopConsultantResult = answerDto.ShopConsultantResult;
            answer.InDateTime = DateTime.Now;
            answer.InUserId = Convert.ToInt32(answerDto.ModifyUserId);
            answer.ModifyDateTime = DateTime.Now;
            answer.ModifyUserId = Convert.ToInt32(answerDto.ModifyUserId);
          
            Answer findOne = db.Answer.Where(x => (x.ProjectId == answerDto.ProjectId && x.ShopId == answerDto.ShopId && x.SubjectId == answerDto.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.ModifyDateTime = DateTime.Now;
                db.Answer.Add(answer);
            }
            else
            {
                findOne.PhotoScore = answer.PhotoScore;
                findOne.Remark = answer.Remark;
                findOne.FileResult = answer.FileResult;
                findOne.InspectionStandardResult = answer.InspectionStandardResult;
                findOne.LossResult = answer.LossResult;
                findOne.LossResultAdd = answer.LossResultAdd;
                findOne.ShopConsultantResult = answer.ShopConsultantResult;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = answer.ModifyUserId;
            }
            db.SaveChanges();

        }
        ///// <summary>
        ///// 保存销售顾问得分
        ///// </summary>
        ///// <param name="score"></param>
        //public void SaveConsultantScore(AnswerShopConsultantScore score)
        //{
        //    AnswerShopConsultantScore findOne = db.AnswerShopConsultantScore.Where(x => (x.AnswerId == score.AnswerId && x.ConsultantId == score.ConsultantId)).FirstOrDefault();
        //    if (findOne == null)
        //    {
        //        db.AnswerShopConsultantScore.Add(score);
        //    }
        //    else
        //    {
        //        findOne.ConsultantScore = score.ConsultantScore;
        //        findOne.ConsultantLossDesc = score.ConsultantLossDesc;
        //        findOne.ModifyDateTime = DateTime.Now;
        //        findOne.ModifyUserId = score.ModifyUserId;
        //    }
        //    db.SaveChanges();
        //}
        ///// <summary>
        ///// 获取销售顾问成绩
        ///// </summary>
        ///// <param name="answerId"></param>
        ///// <param name="consultantId"></param>
        ///// <returns></returns>
        //public List<ShopConsultantResultDto> GetShopConsultantScore(string answerId, string consultantId)
        //{
        //    consultantId = consultantId == null ? "" : consultantId;
        //    SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AnswerId", answerId),
        //                                               new SqlParameter("@ConsultantId", consultantId) };
        //    Type t = typeof(ShopConsultantResultDto);
        //    string sql = "";
        //    sql = @"SELECT B.AnswerShopConsultantScoreId,B.AnswerId,B.ConsultantId,B.ConsultantScore,B.ConsultantLossDesc
        //                ,C.ConsultantName,C.ConsultantType,C.SeqNO,B.InUserId,B.ModifyUserId
        //            FROM dbo.Answer A INNER JOIN dbo.AnswerShopConsultantScore B ON A.AnswerId = B.AnswerId
        // INNER JOIN dbo.AnswerShopConsultant C ON B.ConsultantId = C.ConsultantId
        //            WHERE A.AnswerId = @AnswerId ";
        //    if (!string.IsNullOrEmpty(consultantId))
        //    {
        //        sql += " AND B.ConsultantId = @ConsultantId";
        //    }
        //    return db.Database.SqlQuery(t, sql, para).Cast<ShopConsultantResultDto>().ToList();
        //}
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
            sql = @"SELECT A.ProjectId,A.ShopId,B.ShopCode,B.ShopName,ISNULL(C.TeamLeader,'') AS TeamLeader,C.StartDate,C.Longitude,C.Latitude,C.InDateTime,C.ModifyDateTime,C.PhotoUrl
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
        #region 外部得分导入
        //public void ImportAnswerResult(string tenantId,string brandId,string projectId,string userId,List<AnswerDto> answerList)
        //{
        //    string sql = "";
        //    int brandIdInt = Convert.ToInt32(brandId);
        //    int tenantIdInt = Convert.ToInt32(tenantId);
        //    int projectIdInt = Convert.ToInt32(projectId);
        //    sql += "DELETE Answer WHERE ProjectId = " + projectIdInt;
        //    SqlParameter[] para = new SqlParameter[] { };
        //    Type t = typeof(int);
        //    foreach (AnswerDto answer in answerList)
        //    {

        //        Shop shop = db.Shop.Where(x => (x.ShopCode == answer.ShopCode&&x.BrandId == brandIdInt && x.TenantId== tenantIdInt)).FirstOrDefault();
        //        Subject subject = db.Subject.Where(x => (x.ProjectId == projectIdInt && x.SubjectCode == answer.SubjectCode)).FirstOrDefault();

        //        sql += "INSERT INTO Answer(ProjectId,SubjectId,ShopId,ImportScore,ImportLossResult,InUserId,InDateTime,ModifyUserId,ModifyDateTime,UploadUserId,UploadDate) VALUES(";
        //        sql += projectId + ",";
        //        sql += subject.SubjectId + ",";
        //        sql += shop.ShopId + ",";
        //        if (answer.ImportScore == null)
        //        {
        //            sql += "null,'";
        //        }
        //        else {
        //            sql += answer.ImportScore+",'";
        //        }
        //        //sql += answer.ImportScore==null?"":answer.ImportScore+ "','";
        //        if (answer.ImportLossResult == null)
        //        {
        //            sql += "',";
        //        }
        //        else {
        //            sql += answer.ImportLossResult + "',";
        //        }
        //        //sql += answer.ImportLossResult == null ? "" : answer.ImportLossResult + "',";
        //        sql += userId + ",GETDATE(),";
        //        sql += userId + ",GETDATE(),";
        //        sql += userId + ",GETDATE())";
        //        sql += " ";
        //    }
        //    db.Database.ExecuteSqlCommand(sql, para);
        //}
        #endregion

    }
}