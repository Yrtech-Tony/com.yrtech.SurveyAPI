using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class SpecialCaseDto
    {
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int ChapterId { get; set; }
        public string ChapterCode { get; set; }
        public string ChapterName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string InspectionDesc { get; set; }
        public string CheckPoint { get; set; }
        public int SpecialCaseId { get; set; }
        public string SpecialCaseCode { get; set; }
        public string SpecialCaseContent { get; set; }
        public string SpecialCaseFile { get; set; }
        public string SpecialFeedBack { get; set; }
        public int? SpecialFeedBackUserId { get; set; }
        public DateTime? SpecialFeedBackDateTime { get; set; }
        public DateTime? InDateTime { get; set; }
        public List<SpecialCaseFileDto> SpecialCaseFileList { get; set; }
        public int? InUserId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
        public int? ModifyUserId { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
    }
}