using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class LabelDto
    {
        public Nullable<int> LabelId_RecheckType { get; set; }
        public Nullable<int> LabelId_SubjectPattern { get; set; }
        public int LabelId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public int ShopId { get; set; }
        public bool? Checked { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }
        public Nullable<bool> UseChk { get; set; }
        public string LabelType { get; set; }
        public string Remark { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }

    }
}