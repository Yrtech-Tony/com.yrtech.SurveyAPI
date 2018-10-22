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
                            webService.SaveSalesConsultant_Upload(projectCode, shopCode, subjectCode, shopConsult.ConsultantName, shopConsult.ConsultantScore, shopConsult.ConsultantLossDesc, accountId, shopConsult.ConsultantType);
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
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
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
                    findOne.SubjectConsultantId = item.SubjectConsultantId;
                    findOne.UseChk = item.UseChk;
                    // db.Entry<AnswerShopConsultant>(item).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
        #endregion
        /// <summary>
        /// 获取当前经销商需要打分的体系信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectTypeId"></param>
        /// <param name="subjectTypeExamId"></param>
        /// <param name="subjectConsultantId"></param>
        /// <returns></returns>
        public List<Subject> GetShopNeedAnswerSubject(string projectId, string shopId, string subjectTypeId, string subjectTypeExamId, string subjectConsultantId)
        {
            #region 获取当前经销商最后一次打分的序号

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId)
                                                                            };
            Type t = typeof(int);
            string sql = "";
            int lastAnswerSubjectOrderNO = 0;// 最后一次的序号
            int answerSubjectId = 0;
            sql = @"SELECT ISNULL(MAX(B.OrderNO),0) AS OrderNO 
                    FROM Answer A JOIN [Subject] B ON A.ProjectId = B.ProjectId
                                                   AND A.SubjectId = B.SubjectId
		            WHERE 1=1
                    AND A.ProjectId = @ProjectId
				    AND A.ShopId = @ShopId
				    AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = B.SubjectId AND SubjectTypeId = @SubjectTypeId)
				    AND (B.SubjectTypeExamId = @SubjectTypeExamId OR B.SubjectTypeExamId = 1) ";
            if (string.IsNullOrEmpty(subjectConsultantId))
            {
                sql += "AND B.SubjectConsultantId = @SubjectConsultantId";
            }
            lastAnswerSubjectOrderNO = db.Database.SqlQuery(t, sql, para).Cast<int>().First();
            #endregion
            #region 查询需要打分体系Id
            SqlParameter[] para1 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@LastAnswerSubjectOrderNO", lastAnswerSubjectOrderNO),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId) };

            sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MIN(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO > @LastAnswerSubjectOrderNO	
		            AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = A.SubjectId AND SubjectTypeId = @SubjectTypeId)
		            AND (A.SubjectTypeExamId = @SubjectTypeExamId OR A.SubjectTypeExamId = 1))";
            if (string.IsNullOrEmpty(subjectConsultantId))
            {
                sql += "AND A.SubjectConsultantId = @SubjectConsultantId";
            }
            answerSubjectId = db.Database.SqlQuery(t, sql, para1).Cast<int>().First();

            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AnswerSubjectId", answerSubjectId) };
            sql = @"SELECT A.ProjectId
					       ,A.SubjectId
					       ,A.SubjectCode
					       ,A.Implementation -- 执行方式
					       ,A.[CheckPoint]-- 检查点
					       ,A.[Desc]--说明
					       ,A.AdditionalDesc-- 补充说明
					       ,A.InspectionDesc-- 检查标准
					       ,A.OrderNO-- 执行顺序 
                    FROM[Subject] A WHERE 1=1 
                                    AND ProjectId = @ProjectId
                                    AND SubjectId = @AnswerSubjectId";
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
        /// <param name="subjectConsultantId"></param>
        /// <returns></returns>
        public List<Subject> GetShopNextAnswerSubject(string projectId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectConsultantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId),
                                                       new SqlParameter("@OrderNO", orderNO),
                                                       new SqlParameter("@SubjectConsultantId", subjectConsultantId)
            };
            Type t = typeof(int);
            #region 查询需要打分体系Id
            string sql = "";
            int answerSubjectId = 0;
            sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT MIN(OrderNO)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO > @OrderNO	
		            AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = A.SubjectId AND SubjectTypeId = @SubjectTypeId)
		            AND (A.SubjectTypeExamId = @SubjectTypeExamId OR A.SubjectTypeExamId = 1))";
            if (string.IsNullOrEmpty(subjectConsultantId))
            {
                sql += "AND A.SubjectConsultantId = @SubjectConsultantId";
            }
            answerSubjectId = db.Database.SqlQuery(t, sql, para).Cast<int>().First();

            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AnswerSubjectId", answerSubjectId) };
            sql = @"SELECT A.ProjectId
					       ,A.SubjectId
					       ,A.SubjectCode
					       ,A.Implementation -- 执行方式
					       ,A.[CheckPoint]-- 检查点
					       ,A.[Desc]--说明
					       ,A.AdditionalDesc-- 补充说明
					       ,A.InspectionDesc-- 检查标准
					       ,A.OrderNO-- 执行顺序 
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
        /// <param name="subjectConsultantId"></param>
        /// <returns></returns>
        public List<Subject> GetShopPreAnswerSubject(string projectId, string subjectTypeId, string subjectTypeExamId, string orderNO, string subjectConsultantId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@SubjectTypeId", subjectTypeId),
                                                       new SqlParameter("@SubjectTypeExamId", subjectTypeExamId),
                                                       new SqlParameter("@OrderNO", orderNO),
                                                       new SqlParameter("@SubjectConsultantId", subjectConsultantId)
            };
            Type t = typeof(int);
            #region 查询需要打分体系Id
            string sql = "";
            int answerSubjectId = 0;
            sql = @"SELECT TOP 1 SubjectId FROM Subject WHERE ProjectId = @ProjectId AND OrderNO = (SELECT ISNULL(Max(OrderNO),0)	
		            FROM [Subject] A 
		            WHERE ProjectId = @ProjectId 
		            AND OrderNO < @OrderNO	
		            AND EXISTS(SELECT 1 FROM SubjectTypeScoreRegion WHERE SubjectId = A.SubjectId AND SubjectTypeId = @SubjectTypeId)
		            AND (A.SubjectTypeExamId = @SubjectTypeExamId OR A.SubjectTypeExamId = 1))";
            if (string.IsNullOrEmpty(subjectConsultantId))
            {
                sql += "AND A.SubjectConsultantId = @SubjectConsultantId";
            }
            answerSubjectId = db.Database.SqlQuery(t, sql, para).Cast<int>().First();

            #endregion
            #region 通过最后一次打分的Id查询需要打分的体系
            SqlParameter[] para2 = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AnswerSubjectId", answerSubjectId) };
            sql = @"SELECT A.ProjectId
					       ,A.SubjectId
					       ,A.SubjectCode
					       ,A.Implementation -- 执行方式
					       ,A.[CheckPoint]-- 检查点
					       ,A.[Desc]--说明
					       ,A.AdditionalDesc-- 补充说明
					       ,A.InspectionDesc-- 检查标准
					       ,A.OrderNO-- 执行顺序 
                    FROM[Subject] A WHERE 1=1 
                                    AND ProjectId = @ProjectId
                                    AND SubjectId = @AnswerSubjectId";
            Type t_subject = typeof(Subject);
            return db.Database.SqlQuery(t_subject, sql, para2).Cast<Subject>().ToList();
            #endregion
        }
        /// <summary>
        /// 获取当期经销商当前体系打分的详细信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<Answer> GetAnswerInfoDetail(string projectId, string shopId, string subjectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@SubjectId", subjectId) };
            Type t = typeof(Answer);
            string sql = "";
            sql = @"SELECT A.PhotoScore
                            ,A.InspectionStandardResult
                            ,A.FileResult
                            ,A.LossResult
                            ,A.ShopConsultantResult
                            ,A.Remark 
		            FROM Answer A 
		            WHERE ProjectId = @ProjectId
		            AND ShopId = @ShopId
		            AND SubjectId = @SubjectId";
            return db.Database.SqlQuery(t, sql, para).Cast<Answer>().ToList();
        }
        /// <summary>
        /// 保存打分信息
        /// </summary>
        /// <param name="answerDto"></param>
        /// <param name="userId"></param>
        public void SaveAnswerInfo(AnswerDto answerDto, string userId)
        {
            answerDto.UploadDate = DateTime.Now;
            answerDto.UploadUserId = Convert.ToInt32(userId);

            Answer answer = new Answer();
            answer.FileResult = answerDto.FileResult;
            answer.InspectionStandardResult = answerDto.InspectionStandardResult;
            answer.LossResult = answerDto.LossResult;
            answer.ModifyDateTime = answerDto.ModifyDateTime;
            answer.ModifyUserId = answerDto.ModifyUserId;
            answer.PhotoScore = answerDto.PhotoScore;
            answer.Remark = answerDto.Remark;
            answer.ShopConsultantResult = answerDto.ShopConsultantResult;
            answer.UploadDate = answerDto.UploadDate;
            answer.UploadUserId = answerDto.UploadUserId;

            Answer findOne = db.Answer.Where(x => (x.ProjectId == answerDto.ProjectId && x.ShopId == answerDto.ShopId && x.SubjectId == answerDto.SubjectId)).FirstOrDefault();
            if (findOne == null)
            {
                db.Answer.Add(answer);
            }
            else
            {
                findOne = answer;
            }
            db.SaveChanges();

            // 保存打分信息
            string shopCode = masterService.GetShop("", "", answer.ShopId.ToString())[0].ShopCode;
            string subjectCode = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString())[0].SubjectCode;
            string brandId = masterService.GetShop("", "", answer.ShopId.ToString())[0].BrandId.ToString();
            string projectCode = masterService.GetProject("", "", answer.ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;

            if (brandId == "3") { webService.Url = "http://123.57.229.128/gacfcaserver1/service.asmx"; }

            List<InspectionStandardResultDto> inspectionList = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(answer.InspectionStandardResult);
            List<FileResultDto> fileList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
            List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
            List<ShopConsultantResultDto> shopConsultantList = CommonHelper.DecodeString<List<ShopConsultantResultDto>>(answer.ShopConsultantResult);

            webService.SaveAnswer(projectCode, subjectCode, shopCode, answer.PhotoScore,//score 赋值photoscore,模拟得分在上传的会自动计算覆盖
                        answer.Remark, "", accountId, '0', "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(answer.InDateTime).ToString("yyyy-MM-dd HH:mm:ss"), answer.PhotoScore.ToString());

            if (inspectionList != null)
            {
                foreach (InspectionStandardResultDto inspection in inspectionList)
                {
                    if (!string.IsNullOrEmpty(inspection.ModifyType))
                        webService.SaveAnswerDtl(projectCode, subjectCode, shopCode, Convert.ToInt32(inspection.SeqNO), accountId, inspection.AnswerResult, "");
                }
            }
            if (fileList != null)
            {
                foreach (FileResultDto file in fileList)
                {
                    if (!string.IsNullOrEmpty(file.ModifyType))
                        webService.SaveAnswerDtl2Stream(projectCode, subjectCode, shopCode, Convert.ToInt32(file.SeqNO), accountId, "", null, "", file.FileName);
                }
            }
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
                }
            }
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
                    webService.SaveSalesConsultant(projectCode, shopCode, subjectCode,shopConsult.SeqNO,shopConsult.ConsultantName, shopConsult.ConsultantScore, shopConsult.ConsultantLossDesc, accountId,type, shopConsult.ConsultantType);
                }
            }

        }

    }
}