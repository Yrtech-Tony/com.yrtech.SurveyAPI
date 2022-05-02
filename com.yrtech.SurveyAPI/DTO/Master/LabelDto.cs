using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class LabelDto
    {
        public long Id { get; set; }
        public Nullable<int> LabelId_RecheckType { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }
        public string LabelType { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }

    }
}