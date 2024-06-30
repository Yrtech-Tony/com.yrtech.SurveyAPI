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
        public int SubjectCount { get; set; } // 拍照点数量
        public int SubjectCompleteCount { get; set; } // 已完成拍照数量
        public int ChapterCount { get; set; }
        public int ChapterCompleteCount { get; set; }
        public string Status { get; set; }// 状态
        public string LeftTime { get; set; }// 剩余时间
        public DateTime? StartDate { get; set; }// 子任务开始时间
        public DateTime? EndDate { get; set; }// 子任务结束时间
        public Nullable<int> ReportTypeId { get; set; }
        public Nullable<decimal> FullScore { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}