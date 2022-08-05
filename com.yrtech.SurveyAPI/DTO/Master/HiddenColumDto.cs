using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.Master
{
    public class HiddenColumDto
    {
        public int Id { get; set; }
        public string HiddenCodeGroup { get; set; }
        public string HiddenCode { get; set; }
        public string HiddenCode_SubjectType { get; set; }
        public string HiddenName { get; set; }
        public string Remark { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}