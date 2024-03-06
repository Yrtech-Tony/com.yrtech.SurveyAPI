using System;
using System.Collections.Generic;
using com.yrtech.SurveyDAL;


namespace com.yrtech.SurveyAPI.DTO
{
    public  class ReportSubjectScoreDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AreaId { get; set; }
        public string AreaCode { get; set; } // 区域代码
        public string AreaName { get; set; } // 区域名称
        public int ShopId { get; set; }
        public string ShopCode { get; set; } // 经销商代码
        public string ShopName { get; set; } // 经销商名称
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } // 题目代码
        public string CheckPoint { get; set; } // 二级指标检查点
        public Nullable<decimal> Score { get; set; } // 得分
        public Nullable<decimal> SumScore { get; set; } // 总分
        public Nullable<decimal> FullScore { get; set; } // 标准分
        public Nullable<decimal> CountrySumScore { get; set; } // 全国得分
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}