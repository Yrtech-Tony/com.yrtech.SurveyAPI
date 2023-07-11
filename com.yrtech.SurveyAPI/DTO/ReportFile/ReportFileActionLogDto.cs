using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportFileActionLogDto
    {
        public long LogId { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> InUserId { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ReportFileName { get; set; }
    }
}