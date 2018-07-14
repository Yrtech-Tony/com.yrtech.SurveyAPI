using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class UploadData
    {
        public string UserId { get; set; }
        public List<Answer> AnswerList{get;set;}
        public List<AnswerShopInfo> AnswerShopInfoList { get; set; }
        public List<AnswerShopConsultant> AnswerShopConsultantList { get; set; }
    }
}