using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ChapterReportTypeDto
    {
        public int ReportTypeId { get; set; }
        public string ReportTypeCode { get; set; }
        public string ReportTypeName { get; set; }
        public decimal? FullScore { get; set; }
        public int ProjectId { get; set; }
        public int ChapterId { get; set; }
        public string ChapterCode { get; set; }
        public string ChapterName { get; set; }

        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public Nullable<int> InUserId { get; set; }


    }
}