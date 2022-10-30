using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ProjectShopExamTypeDto
    {
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ShopShortName { get; set; }
        public int? ExamTypeId { get; set; }
        public string Address { get; set; }
        public string ExamTypeCode { get; set; }
        public string ExamTypeName { get; set; }
        public int InUserId { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime? InDateTime { get; set; }
        public DateTime? ModifyDateTime { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
    }
}