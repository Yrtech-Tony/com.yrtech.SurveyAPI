using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class UploadData
    {
        public string UserId { get; set; }
        public string AnswerListJson { get; set; }
        public string AnswerShopInfoListJson { get; set; }
        public string AnswerShopConsultantListJson { get; set; }
        public string ListJson { get; set; }
        public string BrandId { get; set; }
        public string TenantId { get; set; }

        public List<AnswerDto> AnswerList{get;set;}
        public List<AnswerShopInfo> AnswerShopInfoList { get; set; }
    }
}