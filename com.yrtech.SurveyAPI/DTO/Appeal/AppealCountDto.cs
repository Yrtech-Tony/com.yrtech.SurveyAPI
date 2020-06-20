using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AppealCountDto
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopCode { get; set; }
        public int ApplyCount { get; set; }
        public int FeedBackCount { get; set; }
    }
}