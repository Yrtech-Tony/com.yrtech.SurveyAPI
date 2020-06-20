using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class LabelObjectDto
    {
        public long Id { get; set; }
        public Nullable<int> LabelId { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }
        public string LabelType { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectName { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }

    }
}