using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.SurveyAPI.Service
{
    public class AnswerService
    {
        Entities db = new Entities();
        localhost.Service webService = new localhost.Service();
        MasterService masterService = new MasterService();
        AccountService accountService = new AccountService();
        #region 不联网版本
        /// <summary>
        /// 保存答题信息列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void SaveAnswerList(List<Answer> lst, string userId)
        {

            if (lst == null) return;
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string brandId = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].BrandId.ToString();
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }
            try
            {
                /// 保存得分信息
                foreach (Answer answer in lst)
                {
                    string subjectCode = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString())[0].SubjectCode;
                    webService.SaveAnswer(projectCode, subjectCode, shopCode, answer.PhotoScore,//score 赋值photoscore,模拟得分在上传的会自动计算覆盖
                        answer.Remark, "", accountId, '0', "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(answer.InDateTime).ToString("yyyy-MM-dd HH:mm:ss"), answer.PhotoScore.ToString());
                    List<InspectionStandardResultDto> inspectionList = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(answer.InspectionStandardResult);
                    List<FileResultDto> fileList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                    List<ShopConsultantResultDto> shopConsultantList = CommonHelper.DecodeString<List<ShopConsultantResultDto>>(answer.ShopConsultantResult);
                    //CommonHelper.log(answer.ShopConsultantResult.ToString());
                    if (inspectionList != null)
                    {
                        foreach (InspectionStandardResultDto inspection in inspectionList)
                        {
                            webService.SaveAnswerDtl(projectCode, subjectCode, shopCode, Convert.ToInt32(inspection.SeqNO), accountId, inspection.AnswerResult, "");
                        }
                    }
                    if (fileList != null)
                    {
                        foreach (FileResultDto file in fileList)
                        {
                            webService.SaveAnswerDtl2Stream(projectCode, subjectCode, shopCode, Convert.ToInt32(file.SeqNO), accountId, "", null, "", file.FileName);
                        }
                    }
                    if (lossResultList != null)
                    {
                        foreach (LossResultDto loss in lossResultList)
                        {
                            webService.SaveAnswerDtl3(projectCode, subjectCode, shopCode, Convert.ToInt32(loss.LossId), loss.LossDesc, loss.LossFileNameUrl);
                        }
                    }
                    if (shopConsultantList != null)
                    {
                        foreach (ShopConsultantResultDto shopConsult in shopConsultantList)
                        {
                            CommonHelper.log(shopConsult.ConsultantScore.ToString());
                            // System.Threading.Thread.Sleep(500);
                            webService.SaveSalesConsultant_Upload(projectCode, shopCode, subjectCode, shopConsult.ConsultantName, shopConsult.ConsultantScore.HasValue ? shopConsult.ConsultantScore.ToString() : "", shopConsult.ConsultantLossDesc, accountId, shopConsult.ConsultantType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.log(ex.ToString());
            }

            foreach (Answer answer in lst)
            {
                Answer findOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.SubjectId == answer.SubjectId)).FirstOrDefault();
                answer.UploadDate = DateTime.Now;
                answer.UploadUserId = Convert.ToInt32(userId);

                if (findOne == null)
                {
                    db.Answer.Add(answer);
                }
                else
                {
                    findOne.FileResult = answer.FileResult;
                    findOne.InspectionStandardResult = answer.InspectionStandardResult;
                    findOne.LossResult = answer.LossResult;
                    findOne.ModifyDateTime = answer.ModifyDateTime;
                    findOne.ModifyUserId = answer.ModifyUserId;
                    findOne.PhotoScore = answer.PhotoScore;
                    findOne.Remark = answer.Remark;
                    findOne.ShopConsultantResult = answer.ShopConsultantResult;
                    findOne.UploadDate = answer.UploadDate;
                    findOne.UploadUserId = answer.UploadUserId;
                    //db.Entry<Answer>(answer).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void SaveAnswerShopInfoList(List<AnswerShopInfo> lst, string userId)
        {
            //CommonHelper.log("Service" + lst.ToString());
            if (lst == null) return;
            //CommonHelper.log(lst.ToString());
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string brandId = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].BrandId.ToString();
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }
            // 保存数据到原系统
            CommonHelper.log(webService.Url + " " + brandId.ToString());

            foreach (AnswerShopInfo answerShopInfo in lst)
            {
                webService.AnswerStartInfoSave(projectCode, shopCode, answerShopInfo.TeamLeaderName, accountId, Convert.ToDateTime(answerShopInfo.StartDate).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            foreach (AnswerShopInfo answerShopInfo in lst)
            {
                answerShopInfo.UploadDateTime = DateTime.Now;
                answerShopInfo.UploadUserId = Convert.ToInt32(userId);
                AnswerShopInfo findOne = db.AnswerShopInfo.Where(x => (x.ProjectId == answerShopInfo.ProjectId && x.ShopId == answerShopInfo.ShopId)).FirstOrDefault();
                if (findOne == null)
                {
                    db.AnswerShopInfo.Add(answerShopInfo);
                }
                else
                {
                    findOne.ModifyDateTime = answerShopInfo.ModifyDateTime;
                    findOne.ModifyUserId = answerShopInfo.ModifyUserId;
                    findOne.StartDate = answerShopInfo.StartDate;
                    findOne.TeamLeaderName = answerShopInfo.TeamLeaderName;
                    findOne.UploadDateTime = answerShopInfo.UploadDateTime;
                    findOne.UploadUserId = answerShopInfo.UploadUserId;
                    //db.Entry<AnswerShopInfo>(findOne).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 保存顾问信息列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void SaveAnswerShopConsultantList(List<AnswerShopConsultant> lst, string userId)
        {

            if (lst == null || lst.Count == 0) return;
            List<Project> projectList = masterService.GetProject("", "", lst[0].ProjectId.ToString());
            if (projectList == null || projectList.Count == 0)
            {
                throw new Exception("没有找到对应的期号");
            }
            List<Shop> shopList = masterService.GetShop("", "", lst[0].ShopId.ToString());
            if (shopList == null || shopList.Count == 0)
            {
                throw new Exception("没有找到对应的经销商");
            }
            List<UserInfo> userList = accountService.GetUserInfo(userId);
            if (userList == null || userList.Count == 0)
            {
                throw new Exception("没有找到对应的用户");
            }

            string shopCode = shopList[0].ShopCode;
            string projectCode = projectList[0].ProjectCode;
            string accountId = userList[0].AccountId;
            foreach (AnswerShopConsultant item in lst)
            {
                // 不需要了，在保存分数的时候一块保存了
                // webService.SaveSaleContantInfo(projectCode, shopCode, item.SeqNO.ToString(), item.ConsultantName, item.ConsultantType);
            }
            foreach (AnswerShopConsultant item in lst)
            {
                item.UploadDateTime = DateTime.Now;
                item.UploadUserId = Convert.ToInt32(userId);
                AnswerShopConsultant findOne = db.AnswerShopConsultant.Where(x => (x.ProjectId == item.ProjectId && x.ShopId == item.ShopId && x.ConsultantName == item.ConsultantName && x.ConsultantType == item.ConsultantType)).FirstOrDefault();
                if (findOne == null)
                {
                    db.AnswerShopConsultant.Add(item);
                }
                else
                {
                    findOne.ModifyDateTime = item.ModifyDateTime;
                    findOne.ModifyUserId = item.ModifyUserId;
                    findOne.UploadUserId = item.UploadUserId;
                    findOne.UploadDateTime = item.UploadDateTime;
                    findOne.SubjectLinkId = item.SubjectLinkId;
                    findOne.UseChk = item.UseChk;
                    // db.Entry<AnswerShopConsultant>(item).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
        #endregion
        #region 得分登记时调用
        /// <summary>
        /// 获取当前经销商需要打分的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="subjectLinkId"></param>
        /// <returns></returns>
        public List<Subject> GetShopNeedAnswerSubject(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string subjectLinkId, string consultantId)
        {
            #region 获取当前经销商最后一次打分的序号

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                        new SqlParameter("@ConsultantId", consultantId)};
            Type t = typeof(int);
            string sql = "";
            int lastAnswerSubjectOrderNO = 0;// 最后一次的序号
            int answerSubjectId = 0;
            sql = @"SELECT ISNULL(MAX(B.OrderNO),0) AS OrderNO 
                    FROM Answer A JOIN [Subject] B ON A.ProjectId = B.ProjectId
                                                   AND A.SubjectId = B.SubjectId";
            if (string.IsNullOrEmpty(consultantId))
            {
                sql += " INNER JOIN dbo.AnswerShopConsultantScore B ON A.AnswerId = B.AnswerId AND B.ConsultantId = @ConsultantId";
            }
            sql += " WHERE 1 = 1 ";
            sql += @"AND A.ProjectId = @ProjectId
                    AND A.ShopId = @ShopId
                    AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = B.SubjectId AND SubjectTypeId = @SubjectTypeId)
				    AND(B.SubjectTypeExamId = @SubjectTypeExamId OR B.SubjectTypeExamId = 1) ";
            if (!string.IsNullOrEmpty(subjectLinkId))
            {
                sql += "AND B.SubjectLinkId IN (";
                string[] subjectLinkIdList = subjectLinkId.Split(';');
                sql += string.Join(",", subjectLinkIdList);
                sql += " )";
            }
            lastAnswerSubjectOrderNO = db.Database.SqlQuery(t, sql, para).Cast<int>().FirstOrDefault();
            #endregion
            #region 查询需要打分体系Id
            SqlParameter[] para1 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@LastAnswerSubjectOrderNO", lastAnswerSubjectOrderNO),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId)};

            sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MIN(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO > @LastAnswerSubjectOrderNO	
		            AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = A.SubjectId AND SubjectTypeId = @SubjectTypeId)
		            AND (A.SubjectTypeExamId = @SubjectTypeExamId OR A.SubjectTypeExamId = 1)";
            if (!string.IsNullOrEmpty(subjectLinkId))
            {
                sql += "AND A.SubjectLinkId IN (";
                string[] subjectLinkIdList = subjectLinkId.Split(';');
                sql += string.Join(",", subjectLinkIdList);
                sql += " )";
            }
            sql += ")";
            answerSubjectId = db.Database.SqlQuery(t, sql, para1).Cast<int>().FirstOrDefault();
            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AnswerSubjectId", answerSubjectId),
                                                        new SqlParameter("@OrderNO", lastAnswerSubjectOrderNO)  };
            if (answerSubjectId == 0) // 如果全部打完分查询最后一个题
            {
                sql = @"SELECT * FROM Subject WHERE ProjectId = @ProjectId AND OrderNO =@OrderNO";
            }
            else
            {
                sql = @"SELECT * FROM Subject WHERE ProjectId = @ProjectId AND SubjectId = @AnswerSubjectId";
            }
            Type t_subject = typeof(Subject);
            return db.Database.SqlQuery(t_subject, sql, para2).Cast<Subject>().ToList();
            #endregion
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
        public List<Subject> GetShopNextAnswerSubject(string projectId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectLinkId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId),
                                                       new SqlParameter("@OrderNO", orderNO)
            };
            Type t = typeof(int);
            #region 查询需要打分体系Id
            string sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MIN(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO > @OrderNO	
		            AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = A.SubjectId AND SubjectTypeId = @SubjectTypeId)
		            AND (A.SubjectTypeExamId = @SubjectTypeExamId OR A.SubjectTypeExamId = 1)";
            int answerSubjectId = 0;
            if (!string.IsNullOrEmpty(subjectLinkId))
            {
                sql += "AND A.SubjectLinkId IN (";
                string[] subjectLinkIdList = subjectLinkId.Split(';');
                sql += string.Join(",", subjectLinkIdList);
                sql += " )";
            }
            sql += ")";
            answerSubjectId = db.Database.SqlQuery(t, sql, para).Cast<int>().FirstOrDefault();
            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AnswerSubjectId", answerSubjectId) };
            sql = @"SELECT A.* 
                    FROM[Subject] A WHERE 1=1 
                                    AND ProjectId = @ProjectId
                                    AND SubjectId = @AnswerSubjectId";
            Type t_subject = typeof(Subject);
            return db.Database.SqlQuery(t_subject, sql, para2).Cast<Subject>().ToList();
            #endregion
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
        public List<Subject> GetShopPreAnswerSubject(string projectId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectLinkId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId),
                                                       new SqlParameter("@OrderNO", orderNO)
            };
            Type t = typeof(int);
            #region 查询需要打分体系Id
            string sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT ISNULL(Max(OrderNO),0)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO < @OrderNO	
		            AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = A.SubjectId AND SubjectTypeId = @SubjectTypeId)
		            AND (A.SubjectTypeExamId = @SubjectTypeExamId OR A.SubjectTypeExamId = 1)";
            int answerSubjectId = 0;
            if (!string.IsNullOrEmpty(subjectLinkId))
            {
                sql += "AND A.SubjectLinkId IN (";
                string[] subjectLinkIdList = subjectLinkId.Split(';');
                sql += string.Join(",", subjectLinkIdList);
                sql += " )";
            }
            sql += ")";
            answerSubjectId = db.Database.SqlQuery(t, sql, para).Cast<int>().FirstOrDefault();

            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AnswerSubjectId", answerSubjectId) };
            sql = @"SELECT A.*
                    FROM[Subject] A WHERE 1=1 
                                    AND ProjectId = @ProjectId
                                    AND SubjectId = @AnswerSubjectId";
            Type t_subject = typeof(Subject);
            return db.Database.SqlQuery(t_subject, sql, para2).Cast<Subject>().ToList();
            #endregion
        }
        /// <summary>
        /// 获取当期经销商已经打分的信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopAnswerScoreInfo(string projectId, string shopId, string subjectId)
        {
            // 获取打分的信息
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId) };
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT S.ProjectId,S.SubjectId,S.SubjectCode,S.SubjectTypeExamId,S.SubjectTypeExamId,
                           '' SubjectTypeExamName,S.SubjectLinkId,SL.SubjectLinkName,S.SubjectRecheckTypeId,S.OrderNO,
                           S.Implementation,S.[CheckPoint],S.AdditionalDesc,S.[Desc],S.InspectionDesc,
                           A.ShopId,A.AnswerId,A.InspectionStandardResult,A.FileResult,A.LossResult,A.ShopConsultantResult,
                           A.PhotoScore,A.Remark,A.InUserId,A.InDateTime,A.ModifyUserId,A.ModifyDateTime,A.UploadDate,A.UploadUserId 
                    FROM Subject S 
		            LEFT JOIN Answer A ON S.ProjectId=A.ProjectId AND S.SubjectId = A.SubjectId
                    LEFT JOIN SubjectLink SL ON SL.SubjectLinkId = S.SubjectLinkId 
		            WHERE S.ProjectId = @ProjectId AND A.ShopId = @ShopId";
            if (!string.IsNullOrEmpty(subjectId))
            {
                sql += " AND S.SubjectId = " + subjectId;
            }
            List<AnswerDto> answerList = db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
            // 绑定销售顾问打分信息,并计算当前题的得分，模拟得分
            foreach (AnswerDto answer in answerList)
            {
                answer.ShopConsultantResult = CommonHelper.Encode(GetShopConsultantScore(answer.AnswerId.ToString(), ""));
                decimal? consultantScore = AvgConsultantScore(answer.AnswerId.ToString());
                if (consultantScore == null && (answer.PhotoScore == null|| Convert.ToInt32(answer.PhotoScore)==9999))
                {
                    answer.Score = Convert.ToDecimal(9999);
                }
                else if (consultantScore == null)
                {
                    answer.Score = Math.Round(Convert.ToDecimal(answer.PhotoScore),2);
                }
                else if (answer.PhotoScore == null)
                {
                    answer.Score = Math.Round(Convert.ToDecimal(consultantScore),2);
                }
                else {
                    answer.Score = Math.Round(Convert.ToDecimal((answer.PhotoScore + consultantScore) / 2),2);
                }
                answer.ConsultantScore = consultantScore;
            }

            return answerList;
        }
        public decimal? AvgConsultantScore(string answerId)
        {
            decimal? avgScore = null;
            decimal? totalScore = Convert.ToDecimal(0);
            int count = 0;
            List < ShopConsultantResultDto > consultScoreList= GetShopConsultantScore(answerId,"");
            {
                foreach (ShopConsultantResultDto result in consultScoreList)
                {
                    if (result.ConsultantScore != null && Convert.ToInt32(result.ConsultantScore) != 9999)
                    {
                        totalScore += result.ConsultantScore;
                        count++;
                    }
                }
            }
            if (count != 0)
            {
                avgScore = totalScore / count;
            }
            return avgScore;
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
            answer.ShopId = answerDto.ShopId;
            answer.SubjectId = answerDto.SubjectId;
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
            answer.UploadDate = DateTime.Now;
            answer.UploadUserId = Convert.ToInt32(answerDto.ModifyUserId);
            // 保存打分信息
            List<Project> projectList = masterService.GetProject("", "", answer.ProjectId.ToString());
            if (projectList == null || projectList.Count == 0)
            {
                throw new Exception("没有找到对应的期号");
            }
            List<Shop> shopList = masterService.GetShop("", "", answer.ShopId.ToString());
            if (shopList == null || shopList.Count == 0)
            {
                throw new Exception("没有找到对应的经销商");
            }
            List<UserInfo> userList = accountService.GetUserInfo(answerDto.ModifyUserId.ToString());
            if (userList == null || userList.Count == 0)
            {
                throw new Exception("没有找到对应的用户");
            }
            List<SubjectDto> subjectList = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString());
            if (subjectList == null || subjectList.Count == 0)
            {
                throw new Exception("没有找到对应的体系");
            }
            string projectCode = projectList[0].ProjectCode;
            string shopCode = shopList[0].ShopCode;
            string brandId = shopList[0].BrandId.ToString();
            string accountId = userList[0].AccountId;
            string subjectCode = subjectList[0].SubjectCode;
            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }
            List<InspectionStandardResultDto> inspectionList = new List<InspectionStandardResultDto>();
            List<FileResultDto> fileList = new List<FileResultDto>();
            List<LossResultDto> lossResultList = new List<LossResultDto>();
            List<ShopConsultantResultDto> shopConsultantList = new List<ShopConsultantResultDto>();

            inspectionList = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(answer.InspectionStandardResult);
            fileList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
            lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
            shopConsultantList = CommonHelper.DecodeString<List<ShopConsultantResultDto>>(answer.ShopConsultantResult);


            webService.SaveAnswer(projectCode, subjectCode, shopCode, answer.PhotoScore,//score 赋值photoscore,模拟得分在上传的会自动计算覆盖
                        answer.Remark, "", accountId, '0', "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(answer.InDateTime).ToString("yyyy-MM-dd HH:mm:ss"), answer.PhotoScore.ToString());

            if (inspectionList != null)
            {
                foreach (InspectionStandardResultDto inspection in inspectionList)
                {

                    if (!string.IsNullOrEmpty(inspection.ModifyType))
                    { webService.SaveAnswerDtl(projectCode, subjectCode, shopCode, Convert.ToInt32(inspection.SeqNO), accountId, inspection.AnswerResult, ""); }
                    inspection.ModifyType = null;
                }
            }
            answer.InspectionStandardResult = CommonHelper.Encode(inspectionList);
            if (fileList != null)
            {
                foreach (FileResultDto file in fileList)
                {
                    if (!string.IsNullOrEmpty(file.ModifyType))
                        webService.SaveAnswerDtl2Stream(projectCode, subjectCode, shopCode, Convert.ToInt32(file.SeqNO), accountId, "", null, "", file.FileName);
                    file.ModifyType = null;
                }
            }
            answer.FileResult = CommonHelper.Encode(fileList);
            if (lossResultList != null)
            {
                foreach (LossResultDto loss in lossResultList)
                {
                    char type = 'N';
                    if (loss.ModifyType == "U")
                    { type = 'U'; }
                    else if (loss.ModifyType == "D")
                    {
                        type = 'D';
                    }
                    webService.SaveLossDesc(projectCode, shopCode, "", subjectCode, loss.LossDesc, loss.LossFileNameUrl, Convert.ToInt32(loss.LossId), type, "");
                    loss.ModifyType = null;
                }
            }
            answer.LossResult = CommonHelper.Encode(lossResultList);
            if (shopConsultantList != null)
            {
                foreach (ShopConsultantResultDto shopConsult in shopConsultantList)
                {
                    char type = 'N';
                    if (shopConsult.ModifyType == "U")
                    { type = 'U'; }
                    else if (shopConsult.ModifyType == "D")
                    {
                        type = 'D';
                    }
                    webService.SaveSalesConsultant(projectCode, shopCode, subjectCode, shopConsult.SeqNO.ToString(), shopConsult.ConsultantName,
                        shopConsult.ConsultantScore.HasValue ? shopConsult.ConsultantScore.ToString() : "", shopConsult.ConsultantLossDesc,
                        accountId, type, shopConsult.ConsultantType);
                    shopConsult.ModifyType = null;
                }
            }
            //CommonHelper.log(answer.LossResult);
            answer.LossResult = CommonHelper.Encode(lossResultList);
            answer.FileResult = CommonHelper.Encode(fileList);
            answer.InspectionStandardResult = CommonHelper.Encode(inspectionList);
            answer.ShopConsultantResult = CommonHelper.Encode(shopConsultantList);
            Answer findOne = db.Answer.Where(x => (x.ProjectId == answerDto.ProjectId && x.ShopId == answerDto.ShopId && x.SubjectId == answerDto.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                db.Answer.Add(answer);
            }
            else
            {
                findOne.PhotoScore = answer.PhotoScore;
                findOne.Remark = answer.Remark;
                findOne.FileResult = answer.FileResult;
                findOne.InspectionStandardResult = answer.InspectionStandardResult;
                findOne.LossResult = answer.LossResult;
                findOne.ShopConsultantResult = answer.ShopConsultantResult;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = answer.ModifyUserId;
                findOne.UploadDate = DateTime.Now;
                findOne.UploadUserId = answer.UploadUserId;
            }
            db.SaveChanges();
            // 保存销售顾问得分
            int answerId = db.Answer.Where(x => (x.ProjectId == answerDto.ProjectId && x.ShopId == answerDto.ShopId && x.SubjectId == answerDto.SubjectId)).FirstOrDefault().AnswerId;
            foreach (ShopConsultantResultDto result in shopConsultantList)
            {
                AnswerShopConsultantScore score = new AnswerShopConsultantScore();
                score.AnswerId = answerId;
                score.ConsultantId = result.ConsultantId;
                score.ConsultantScore = result.ConsultantScore;
                score.ConsultantLossDesc = result.ConsultantLossDesc;
                score.InDateTime = DateTime.Now;
                score.InUserId = answer.ModifyUserId;
                score.ModifyDateTime = DateTime.Now;
                score.ModifyUserId = answer.ModifyUserId;
                SaveConsultantScore(score);
            }

        }
        /// <summary>
        /// 保存销售顾问得分
        /// </summary>
        /// <param name="score"></param>
        public void SaveConsultantScore(AnswerShopConsultantScore score)
        {
            AnswerShopConsultantScore findOne = db.AnswerShopConsultantScore.Where(x => (x.AnswerId == score.AnswerId && x.ConsultantId == score.ConsultantId)).FirstOrDefault();
            if (findOne == null)
            {
                db.AnswerShopConsultantScore.Add(score);
            }
            else
            {
                findOne.ConsultantScore = score.ConsultantScore;
                findOne.ConsultantLossDesc = score.ConsultantLossDesc;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = score.ModifyUserId;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取销售顾问成绩
        /// </summary>
        /// <param name="answerId"></param>
        /// <param name="consultantId"></param>
        /// <returns></returns>
        public List<ShopConsultantResultDto> GetShopConsultantScore(string answerId, string consultantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AnswerId", answerId),
                                                       new SqlParameter("@ConsultantId", consultantId) };
            Type t = typeof(ShopConsultantResultDto);
            string sql = "";
            sql = @"SELECT B.AnswerShopConsultantScoreId,B.AnswerId,B.ConsultantId,B.ConsultantScore,B.ConsultantLossDesc
                        ,C.ConsultantName,C.ConsultantType
                    FROM dbo.Answer A INNER JOIN dbo.AnswerShopConsultantScore B ON A.AnswerId = B.AnswerId
						   INNER JOIN dbo.AnswerShopConsultant C ON B.ConsultantId = C.ConsultantId
                    WHERE B.AnswerId = @AnswerId ";
            if (string.IsNullOrEmpty(consultantId))
            {
                sql += " AND B.ConsultantId = @ConsultantId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ShopConsultantResultDto>().ToList();
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
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId)};
            Type t = typeof(AnswerShopInfoDto);
            string sql = "";
            sql = @"SELECT ProjectId,ShopId,TeamLeaderName,StartDate,InUserId,InDateTime,ModifyUserId,ModifyDateTime 
                    FROM AnswerShopInfo  
		            WHERE ProjectId = @ProjectId
		            AND ShopId = @ShopId";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerShopInfoDto>().ToList();
        }
        /// <summary>
        /// 保存经销商进店信息
        /// </summary>
        /// <param name="shopInfo"></param>
        /// <param name="userId"></param>
        public void SaveAnswerShopInfo(AnswerShopInfo shopInfo)
        {
            List<Project> projectList = masterService.GetProject("", "", shopInfo.ProjectId.ToString());
            if (projectList == null || projectList.Count == 0)
            {
                throw new Exception("没有找到对应的期号");
            }
            List<Shop> shopList = masterService.GetShop("", "", shopInfo.ShopId.ToString());
            if (shopList == null || shopList.Count == 0)
            {
                throw new Exception("没有找到对应的经销商");
            }
            List<UserInfo> userList = accountService.GetUserInfo(shopInfo.ModifyUserId.ToString());
            if (userList == null || userList.Count == 0)
            {
                throw new Exception("没有找到对应的用户");
            }
            string projectCode = projectList[0].ProjectCode;
            string shopCode = shopList[0].ShopCode;
            string brandId = shopList[0].BrandId.ToString();
            string accountId = userList[0].AccountId;

            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }

            webService.AnswerStartInfoSave(projectCode, shopCode, shopInfo.TeamLeaderName, accountId, shopInfo.StartDate.ToString());

            AnswerShopInfo findOne = db.AnswerShopInfo.Where(x => (x.ProjectId == shopInfo.ProjectId && x.ShopId == shopInfo.ShopId)).FirstOrDefault();
            if (findOne == null)
            {
                shopInfo.InDateTime = DateTime.Now;
                shopInfo.ModifyDateTime = DateTime.Now;
                db.AnswerShopInfo.Add(shopInfo);
            }
            else
            {
                findOne.TeamLeaderName = shopInfo.TeamLeaderName;
                findOne.StartDate = shopInfo.StartDate;
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
            List<Project> projectList = masterService.GetProject("", "", consultant.ProjectId.ToString());
            if (projectList == null || projectList.Count == 0)
            {
                throw new Exception("没有找到对应的期号");
            }
            List<Shop> shopList = masterService.GetShop("", "", consultant.ShopId.ToString());
            if (shopList == null || shopList.Count == 0)
            {
                throw new Exception("没有找到对应的经销商");
            }
            List<UserInfo> userList = accountService.GetUserInfo(consultant.ModifyUserId.ToString());
            if (userList == null || userList.Count == 0)
            {
                throw new Exception("没有找到对应的用户");
            }
            string shopCode = shopList[0].ShopCode;
            string brandId = shopList[0].BrandId.ToString();
            string projectCode = projectList[0].ProjectCode;
            string accountId = userList[0].AccountId;

            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }

            webService.SaveSaleContantInfo(projectCode, shopCode, consultant.SeqNO.ToString(), consultant.ConsultantName, consultant.ConsultantType);

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
    }
}