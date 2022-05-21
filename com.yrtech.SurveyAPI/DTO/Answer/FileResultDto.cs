using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class FileResultDto
    {
        public string ProjectId { get; set; }
        public string ShopId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public int SeqNO { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public DateTime InDateTime { get; set; }
        public int InUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
    }
}