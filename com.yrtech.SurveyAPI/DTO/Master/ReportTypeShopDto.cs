using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ReportTypeShopDto
    {
        public int ReportTypeId { get; set; }
        public int ReportShopId { get; set; }
        public string ReportTypeCode { get; set; }
        public string ReportTypeName { get; set; }
        public int ProjectId { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }

        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public int InUserId { get; set; }


    }
}