using System;
using System.Collections.Generic;
using com.yrtech.SurveyDAL;


namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportYearTrendDto
    {
        public List<ProjectDto> ProjectList { get; set; }
        public List<AreaDto> AreaList { get; set; }
        public List<ReportChapterScoreDto> YearTrendDataList { get; set; }
    }
}