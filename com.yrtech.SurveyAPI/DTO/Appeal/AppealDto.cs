using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AppealDto
    {
        public int AppealId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string CheckPoint { get; set; }
        public decimal ImportScore { get; set; }
        public string ImportLossResult { get; set; }
        public string AppealReason { get; set; }
        public string AppealUserName { get; set; }
        public string AppealUserId { get; set; }
        public DateTime AppealDateTime { get; set; }
        public string FeedBackStatusStr { get; set; }
        public string FeedBackStatus { get; set; }
        public string FeedBackReason { get; set; }
        public string FeedBackUserName { get; set; }
        public int FeedBackUserId { get; set; }
        public DateTime FeedBackDateTime { get; set; }
        public string ShopAcceptStatusStr { get; set; }
        public string ShopAcceptStatus { get; set; }
        public string ShopAcceptReason { get; set; }
        public string ShopAcceptUserName { get; set; }
        public int ShopAcceptUserId { get; set; }
        public DateTime ShopAcceptDateTime { get; set; }
    }
}