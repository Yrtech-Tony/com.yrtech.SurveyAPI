using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RecheckStatusDtlDto
    {
        public int RecheckStatusDtlId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int RecheckTypeId { get; set; }
        public string RecheckTypeCode { get; set; }
        public string RecheckTypeName { get; set; }
        public string StatusName { get; set; }
        public int? RecheckUserId { get; set; }
        public string RecheckUserName { get; set; }
        public DateTime? RecheckDateTime { get; set; }
    }
}