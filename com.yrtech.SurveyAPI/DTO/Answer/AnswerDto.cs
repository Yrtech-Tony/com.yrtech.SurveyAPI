using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AnswerDto
    {
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public int AnswerId { get; set; }
        public int ProjectId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectTypeCode { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public decimal? PhotoScore { get; set; }
        public decimal? Score { get; set; }
        public decimal? ImportScore { get; set; }
        public decimal? ImportLossResult { get; set; }
        public decimal? ConsultantScore { get; set; }
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
        public string SubjectCode { get; set; }
        public int SubjectTypeExamId { get; set; }
        public string SubjectTypeExamName { get; set; }
        public int SubjectRecheckTypeId { get; set; }
        public int SubjectLinkId { get; set; }
        public string SubjectLinkName { get; set; }
        public int OrderNO { get; set; }
        public string Implementation { get; set; }
        public string CheckPoint { get; set; }
        public string Desc { get; set; }
        public string AdditionalDesc { get; set; }
        public string InspectionDesc { get; set; }
        public char ModifyType { get; set; }//"U"：修改；"D":删除; 

    }
}