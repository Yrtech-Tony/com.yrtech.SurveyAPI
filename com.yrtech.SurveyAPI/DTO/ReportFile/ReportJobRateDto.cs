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
        public int JobFullCount { get; set; }// 标准人数
        public int JobActualCount { get; set; }//到港人数
        public string MeetChk { get; set; }// 人数是否达标
        public decimal MeetRate { get; set; } // 岗位满足率
        public bool? ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}