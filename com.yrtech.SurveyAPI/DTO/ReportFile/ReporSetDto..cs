using System;
using System.Collections.Generic;
using com.yrtech.SurveyDAL;


namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportSetDto
    {
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int InUserId { get; set; }
        public DateTime? InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
    }
}