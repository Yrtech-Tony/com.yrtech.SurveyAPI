using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public int ProjectId { get; set; }
        public int SubjectId { get; set; }
        public int ShopId { get; set; }
        public decimal PhotoScore { get; set; }
        public string InspectionStandardResult { get; set; }
        public string FileResult { get; set; }
        public string LossResult { get; set; }
        public string ShopConsultantResult { get; set; }
        public string Remark { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime UploadDate { get; set; }
        public int UploadUserId { get; set; }
        public char ModifyType { get; set; }//"U"：修改；"D":删除; 
    }
}