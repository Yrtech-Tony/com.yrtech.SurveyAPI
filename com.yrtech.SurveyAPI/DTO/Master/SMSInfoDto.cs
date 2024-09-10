using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class SMSInfoDto
    {
        public int SMSId { get; set; }
        public int BrandId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string SMSBussinessType { get; set; }
        public string SMSBussinessName { get; set; }
        public Nullable<System.DateTime> SMSSendDate { get; set; }
        public string TelNO { get; set; }
        public string RequestId { get; set; }
        public string BizId { get; set; }
        public string ErrCode { get; set; }
        public string SendStatus { get; set; }
        public string SendStatusName { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }

    }
}