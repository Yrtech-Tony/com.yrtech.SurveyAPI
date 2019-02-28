using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AreaDto
    {
        public int AreaId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string AreaType { get; set; }
        public int? ParentId { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}