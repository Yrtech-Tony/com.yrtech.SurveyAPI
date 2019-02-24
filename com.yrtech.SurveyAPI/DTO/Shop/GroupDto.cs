using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class GroupDto
    {
        public int GroupId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandCode { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}