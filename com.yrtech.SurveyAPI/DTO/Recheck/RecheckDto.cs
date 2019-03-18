using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RecheckDto
    {
        public int RecheckId { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public bool? PassRecheck { get; set; }
        public string RecheckContent { get; set; }
        public string RecheckError { get; set; }
        public int? RecheckUserId { get; set; }
        public string RecheckUserName { get; set; }
        public DateTime? RecheckDateTime { get; set; }
        public bool? AgreeCheck { get; set; }
        public string AgreeReason { get; set; }
        public int? AgreeUserId { get; set; }
        public string AgreeUserName { get; set; }
        public DateTime? AgreeDateTime { get; set; }
        public bool? LastConfirmCheck { get; set; }
        public int? LastConfirmUserId { get; set; }
        public string LastConfirmUserName { get; set; }
        public DateTime? LastConfirmDateTime { get; set; }
         
    }
}