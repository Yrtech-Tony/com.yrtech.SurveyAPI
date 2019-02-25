using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class ShopDto
    {
        public int ProjectId { get; set; }
        public int ShopId { get; set; }
        public int AreaId { get; set; }
        public int GroupId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ShopShortName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public int SubjectTypeExamId { get; set; }
        public string SubjectTypeExamName { get; set; }
        public bool? UseChk { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}