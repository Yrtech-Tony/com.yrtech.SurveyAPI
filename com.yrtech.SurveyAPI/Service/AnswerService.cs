using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO.AnswerResult;
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
        /// <summary>
        /// 保存答题信息列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public void SaveAnswerList(List<Answer> lst, string userId)
        {
            if (lst == null) return;
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            /// 保存得分信息
            foreach (Answer answer in lst)
            {
                string subjectCode = masterService.GetSubject(answer.ProjectId.ToString(), answer.SubjectId.ToString())[0].SubjectCode;
                webService.SaveAnswer(projectCode, subjectCode, shopCode, null, answer.Remark, "", accountId,'0', "", DateTime.Now.ToString(), answer.InDateTime.ToString(), answer.PhotoScore.ToString());
                List<InspectionStandardResultDto> inspectionList = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(answer.InspectionStandardResult);
                List<FileResultDto> fileList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                List<ShopConsultantResultDto> shopConsultantList = CommonHelper.DecodeString<List<ShopConsultantResultDto>>(answer.ShopConsultantResult);
                foreach (InspectionStandardResultDto inspection in inspectionList)
                {
                    webService.SaveAnswerDtl(projectCode, subjectCode, shopCode, Convert.ToInt32(inspection.SeqNO), accountId, inspection.AnswerResult, "");
                }
                foreach (FileResultDto file in fileList)
                {
                    webService.SaveAnswerDtl2(projectCode, subjectCode, shopCode, Convert.ToInt32(file.SeqNO), accountId,"",file.FileName);
                }
                foreach (LossResultDto loss in lossResultList)
                {
                    webService.SaveAnswerDtl3(projectCode, subjectCode, shopCode, Convert.ToInt32(loss.SeqNO),loss.LossDesc,loss.LossFIleNameUrl);
                }
                foreach (ShopConsultantResultDto shopConsult in shopConsultantList)
                { 
                    webService.SaveSalesConsultant(projectCode,shopCode, subjectCode,shopConsult.SeqNO,shopConsult.ConsultantName,shopConsult.ConsultantScore,shopConsult.ConsultantLossDesc,accountId,'I',shopConsult.ConsultantType);
                }
            }

            foreach (Answer answer in lst)
            {
                answer.UploadDate = DateTime.Now;
                answer.UploadUserId = Convert.ToInt32(userId);
                Answer findOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.SubjectId == answer.SubjectId)).FirstOrDefault();
                if (findOne == null)
                {
                    db.Answer.Add(answer);
                }
                else
                {
                    db.Entry<Answer>(answer).State = System.Data.Entity.EntityState.Modified;
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
            CommonHelper.log("Service"+lst.ToString());
            if (lst == null) return;
            CommonHelper.log(lst.ToString());
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            // 保存数据到原系统
            foreach (AnswerShopInfo answerShopInfo in lst)
            {
                webService.AnswerStartInfoSave(projectCode, shopCode, answerShopInfo.TeamLeaderName, accountId, answerShopInfo.StartDate.ToString());
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
                    db.Entry<AnswerShopInfo>(findOne).State = System.Data.Entity.EntityState.Modified;
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
            if (lst == null) return;
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            foreach (AnswerShopConsultant item in lst)
            {
                SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectCode", projectCode),
                                                       new SqlParameter("@ShopCode", shopCode),
                                                       new SqlParameter("@SeqNO",item.SeqNO),
                                                        new SqlParameter("@SalesConsultant",item.ConsultantName),
                                                        new SqlParameter("@MemberType",item.ConsultantType)};
                db.Database.ExecuteSqlCommand("EXEC up_Upload_ShopConsultant_S @ProjectCode,@ShopCode,@SeqNO,@SalesConsultant,@MemberType", para);
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
                    db.Entry<AnswerShopConsultant>(findOne).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
    }
}