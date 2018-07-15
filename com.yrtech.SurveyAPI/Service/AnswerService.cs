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
                webService.SaveAnswer(projectCode, subjectCode, shopCode, null, answer.Remark, "", accountId, '0', "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(answer.InDateTime).ToString("yyyy-MM-dd HH:mm:ss"), answer.PhotoScore.ToString());
                List<InspectionStandardResultDto> inspectionList = CommonHelper.DecodeString<List<InspectionStandardResultDto>>(answer.InspectionStandardResult);
                List<FileResultDto> fileList = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                List<ShopConsultantResultDto> shopConsultantList = CommonHelper.DecodeString<List<ShopConsultantResultDto>>(answer.ShopConsultantResult);
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
                        webService.SaveAnswerDtl2(projectCode, subjectCode, shopCode, Convert.ToInt32(file.SeqNO), accountId, "", file.FileName);
                    }
                }
                if (lossResultList != null)
                {
                    foreach (LossResultDto loss in lossResultList)
                    {
                        webService.SaveAnswerDtl3(projectCode, subjectCode, shopCode, Convert.ToInt32(loss.SeqNO), loss.LossDesc, loss.LossFIleNameUrl);
                    }
                }
                if (shopConsultantList != null)
                {
                    foreach (ShopConsultantResultDto shopConsult in shopConsultantList)
                    {
                        // 先保存销售顾问得分信息
                        webService.SaveSalesConsultant(projectCode, shopCode, subjectCode, shopConsult.SeqNO, shopConsult.ConsultantName, shopConsult.ConsultantScore, shopConsult.ConsultantLossDesc, accountId, 'I', shopConsult.ConsultantType);
                        // 再更新失分说明，因为以前的系统是分2部来做的
                        webService.UpdateSalesConsultant(projectCode, shopCode, subjectCode, shopConsult.SeqNO, shopConsult.ConsultantLossDesc);
                    }
                }
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
            CommonHelper.log("Service" + lst.ToString());
            if (lst == null) return;
            CommonHelper.log(lst.ToString());
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            // 保存数据到原系统
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
            if (lst == null) return;
            string shopCode = masterService.GetShop("", "", lst[0].ShopId.ToString())[0].ShopCode;
            string projectCode = masterService.GetProject("", "", lst[0].ProjectId.ToString())[0].ProjectCode;
            string accountId = accountService.GetUserInfo(userId)[0].AccountId;
            foreach (AnswerShopConsultant item in lst)
            {
                webService.SaveSaleContantInfo(projectCode, shopCode, item.SeqNO.ToString(), item.ConsultantName, item.ConsultantType);
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
                   // db.Entry<AnswerShopConsultant>(item).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
    }
}