using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class ReportFileDto
    {
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public int ProjectId { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ReportFileType { get; set; }
        public string ReportFileName { get; set; }
        public string Url_OSS { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}