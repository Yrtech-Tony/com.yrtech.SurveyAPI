using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AppealSetDto
    {
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public DateTime? AppealStartDate { get; set; }
        public DateTime? AppealEndDate { get; set; }
        public string HiddenCode { get; set; }
        public string HiddenName { get; set; }
        public DateTime? InDateTime { get; set; }
        public DateTime? ModifyDateTime { get; set; }
    }
}