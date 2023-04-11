using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RecheckStatusDtlDto
    {
        public int RecheckStatusDtlId { get; set; }
        public int RecheckStatusId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int RecheckTypeId { get; set; } // 复审类型Id
        public string RecheckTypeCode { get; set; } // 复审类型代码
        public string RecheckTypeName { get; set; }// 复审类型名称
        public int? ModifyUserId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
        public int? InUserId { get; set; }
        public DateTime? InDateTime { get; set; }
    }
}