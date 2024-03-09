using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class LossResultDto
    {
        public string ProjectId { get; set; }
        public string ShopId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public int SeqNO { get; set; }
        public string LossId { get; set; }
        public string LossCode { get; set; }
        public string LossDesc { get; set; }
        public string LossDesc2 { get; set; }
        public string LossFileNameUrl { get; set; }
        public string LossPhotoCount { get; set; }
        public DateTime InDateTime { get; set; }
        public int InUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime LastTime { get; set; }
        public int ModifyUserId { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
    }
}