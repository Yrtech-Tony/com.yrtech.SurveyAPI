using System;
using System.Collections.Generic;
using com.yrtech.SurveyDAL;


namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportChapterScoreDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AreaId { get; set; }
        public string AreaCode { get; set; } // 区域代码
        public string AreaName { get; set; }// 区域名称
        public int ShopId { get; set; }
        public string ShopCode { get; set; } // 经销商代码
        public string ShopName { get; set; } // 经销商名称
        public string ShopType { get; set; } // 经销商类型代码
        public int ChapterId { get; set; }
        public string ChapterCode { get; set; } //一级指标代码
        public string ChapterName { get; set; } // 一级指标名称
        public Nullable<decimal> Score { get; set; } // 分数
        public Nullable<decimal> FullScore { get; set; } // 标准分
        public Nullable<decimal> SumScore { get; set; } // 总得分
        public Nullable<decimal> MaxScore { get; set; } // 最高分
        public Nullable<decimal> MinScore { get; set; } // 最低分
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
        public List<ReportSubjectScoreDto> ReportSubjectScoreList { get; set; } // 二级指标List
    }
}