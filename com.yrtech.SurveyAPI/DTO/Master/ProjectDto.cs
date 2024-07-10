using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public int? PreProjectId { get; set; }
        public long? SubjectId { get; set; }
        public int? ShopId { get; set; }
        public string ShopCode { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectShortName { get; set; }
        public DateTime? ReportDeployDate { get; set; }
        public bool ReportDeployChk { get; set; }
        public string Year { get; set; }
        public string Quarter { get; set; }
        public Nullable<int> OrderNO { get; set; }
        public bool? RechckShopShow { get; set; }
        public bool? PhotoUploadMode { get; set; }
        public string PhotoSize { get; set; } // 未设置默认300万像素
        public string DataScore { get; set; }
        public string ProjectGroup { get; set; } // 任务组别，用于自检同组数据统计
        public DateTime? AppealStartDate { get; set; }
        public bool? AppealShow { get; set; } // 是否在
        public string ProjectType { get; set; } // 明检、密采
        public string ScoreShowType { get; set; } // 默认显示得分：L：最低分；F:最高分；空：默认不显示分数
        public bool? SelfTestChk { get; set; } // true:可以自检 false和null:不能自检
        public bool? LossCopyToSupplyChk { get; set; } // true 失分说明自动生成补充失分说明，false和null 不生成
        public DateTime? StartDate { get; set; } //经销商自检开始时间
        public DateTime? EndDate { get; set; }// 经销商自检结束时间
        public int ChapterCount { get; set; }// 章节（子任务）总量
        public int SubjectCount { get; set; } // 拍照点总量
        public int SubjectCompleteCount { get; set; }  // 拍照点完成数量
        public string Status { get; set; } // 状态
        public string StatusCode { get; set; } // 状态代码：
        public string LeftTime { get; set; } // 剩余时间
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }

    }
}