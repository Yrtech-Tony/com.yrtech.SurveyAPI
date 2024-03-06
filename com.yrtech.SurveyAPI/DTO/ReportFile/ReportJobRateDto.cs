using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class ReportJobRateDto
    {
        public int ProjectId { get; set; }
        public int AreaId { get; set; }
       
        public string AreaCode { get; set; }
       
        public string AreaName { get; set; }
        public string JobName { get; set; }
        public int JobFullCount { get; set; }
        public int JobActualCount { get; set; }
        public string MeetChk { get; set; }
        public decimal MeetRate { get; set; }
        public bool? ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}