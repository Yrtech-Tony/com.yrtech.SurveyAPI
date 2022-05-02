using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class SubjectDto
    {
        public long SubjectId { get; set; } 
        public string SubjectCode { get; set; } // 题目代码
        public Nullable<int> ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNO { get; set; }//序号
        public Nullable<decimal> FullScore { get; set; } // 最高分
        public Nullable<decimal> LowScore { get; set; }// 最低分
        public int? LabelId { get; set; } // 试卷类型ID
        public int? LabelId_RecheckType { get; set; } // 复审类型ID
        public string ExamTypeCode { get; set; } // 试卷类型代码
        public string ExamTypeName { get; set; } // 试卷类型名称
        public string RecheckTypeCode { get; set; } // 复审类型代码
        public string RecheckTypeName { get; set; } // 复审类型名称
        public string CheckPoint { get; set; }// 检查点
        public string Implementation { get; set; }// 执行方式
        public string InspectionDesc { get; set; }//检查标准说明
        public string Remark { get; set; }//备注
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }

    }
}