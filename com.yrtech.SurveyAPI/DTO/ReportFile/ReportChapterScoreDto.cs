using System;
using System.Collections.Generic;
using com.yrtech.SurveyDAL;


namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportChapterScoreDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int AreaId { get; set; }
        public string AreaCode { get; set; } // 区域代码
        public string AreaName { get; set; }// 区域名称
        public int ProvinceId { get; set; }
        public string ProvinceCode { get; set; } // 省份代码
        public string ProvinceName { get; set; }// 省份名称
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
        public Nullable<decimal> PreSumScore { get; set; } // 上期得分
        public Nullable<decimal> AreaSumScore { get; set; } // 区域总得分
        public Nullable<decimal> CountryChapterScore { get; set; } // 全国章节得分
        public Nullable<decimal> CountrySumScore { get; set; } // 全国总得分
        public Nullable<decimal> PreAreaSumScore { get; set; } // 上期区域总得分
        public Nullable<decimal> PreCountrySumScore { get; set; } // 上期全国总得分
        public ReportScoreMaxAndMin MaxScore { get; set; } // 最大值信息
        public ReportScoreMaxAndMin MinScore { get; set; } // 最小值信息
        public List<ReportChapterScoreDto> ShopRankListTop { get; set; } // 排名前
        public List<ReportChapterScoreDto> ShopRankListLast { get; set; } // 排名后
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
        public List<ReportSubjectScoreDto> ReportSubjectScoreList { get; set; } // 二级指标List
    }
    public class ReportScoreMaxAndMin {
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int AreaId { get; set; }
        public string AreaCode { get; set; } // 区域代码
        public string AreaName { get; set; }// 区域名称
        public Nullable<decimal> PreSumScore { get; set; } // 上期得分
        public int ProvinceId { get; set; }
        public string ProvinceCode { get; set; } // 省份代码
        public string ProvinceName { get; set; }// 省份名称
        public int ShopId { get; set; }
        public string ShopCode { get; set; } // 经销商代码
        public string ShopName { get; set; } // 经销商名称
        public string ShopType { get; set; } // 经销商类型代码
        public Nullable<decimal> SumScore { get; set; } // 总得分
    }
}