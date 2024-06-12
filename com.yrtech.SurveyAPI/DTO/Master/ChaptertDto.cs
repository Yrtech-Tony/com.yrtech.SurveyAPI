using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ChapterDto
    {
        public Nullable<int> ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public int ChapterId { get; set; }
        public string ChapterCode { get; set; }
        public string ChapterName { get; set; }
        public int SubjectCount { get; set; }
        public int SubjectAnswerCount { get; set; }
        public string Status { get; set; }
        public Nullable<int> ReportTypeId { get; set; }
        public Nullable<decimal> FullScore { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}