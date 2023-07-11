using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AppealDto
    {
        public long AppealId { get; set; }
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectShortName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string CheckPoint { get; set; }
        public string Remark { get; set; }
        public decimal? FullScore { get; set; }
        public decimal? Score { get; set; }
        public bool? AppealStatus { get; set; }
        public string LossResult { get; set; }
        public string LossResultImport { get; set; }
        public string FileResult { get; set; }
        public List<SubjectFile> SubjectFileList { get; set; }
        public string AppealReason { get; set; }
        public string AppealUserName { get; set; }
        public int? AppealUserId { get; set; }
        public string AppealDateTime { get; set; }
        public string FeedBackStatusStr { get; set; }
        public bool? FeedBackStatus { get; set; }
        public string FeedBackReason { get; set; }
        public string FeedBackUserName { get; set; }
        public int? FeedBackUserId { get; set; }
        public string FeedBackDateTime { get; set; }
        public string ShopAcceptStatusStr { get; set; }
        public bool? ShopAcceptStatus { get; set; }
        public string ShopAcceptReason { get; set; }
        public string ShopAcceptUserName { get; set; }
        public int? ShopAcceptUserId { get; set; }
        public string ShopAcceptDateTime { get; set; }
        public List<AppealFile> AppealFileList { get; set; }
        public DateTime? AppealEndDate { get; set; }
        public bool AppealDateCheck { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public bool AppealFileChk_Apply { get; set; }
        public bool AppealFileChk_FeedBack { get; set; }
        public int? ModifyUserId { get; set; }
    }
}