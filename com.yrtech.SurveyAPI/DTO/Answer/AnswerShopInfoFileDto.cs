using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AnswerShopInfoFileDto
    {
        public int FileId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int AnswerShopInfoId { get; set; }
        public int SeqNO { get; set; }
        public string FileType { get; set; }
        public string FileTypeName { get; set; }
        public string FileName { get; set; }
        public string ServerFileName { get; set; }
        public int InUserId { get; set; }
        public string InUserName { get; set; }
        public DateTime InDateTime { get; set; }
    }
}