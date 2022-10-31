using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AppealSetDto
    {
        public int ProjectId { get; set; }
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public string ProjectCode { get; set; }// 期号代码
        public string ProjectName { get; set; } // 期号名称
        public int ShopId { get; set; }
        public string ShopCode { get; set; } //经销商代码
        public string ShopName { get; set; }// 经销商名称
        public DateTime? AppealStartDate { get; set; } // 申诉开始时间
        public DateTime? AppealEndDate { get; set; } // 申诉结束时间
        public DateTime? AppealCreateDateTime { get; set; }
        public string HiddenCode { get; set; }
        public string HiddenName { get; set; }
        public DateTime? InDateTime { get; set; } //登记时间
        public DateTime? ModifyDateTime { get; set; } // 修改时间
        public int InUserId { get; set; }
        public int ModifyUserId { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
    }
}