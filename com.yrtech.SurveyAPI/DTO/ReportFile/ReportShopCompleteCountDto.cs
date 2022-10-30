using System;
using System.Collections.Generic;
using com.yrtech.SurveyDAL;


namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportShopCompleteCountDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AreaId { get; set; }
        public string ShopType { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public Nullable<int> Count_Complete { get; set; }
        public Nullable<int> Count_UnComplete { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}