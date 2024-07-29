using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class FileResultDto
    {
        public string ProjectId { get; set; }
        public string ShopId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string Date { get; set; }
        public int SeqNO { get; set; }
        public string FileName { get; set; }
        public string FileDemo { get; set; }
        public string FileDemoDesc { get; set; }
        public string FileRemark { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DateTime InDateTime { get; set; }
        public int InUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public string BrandId { get; set; } // 品牌Id
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNO { get; set; }//序号
        public Nullable<decimal> FullScore { get; set; } // 最高分
        public Nullable<decimal> LowScore { get; set; }// 最低分
        public bool? MustScore { get; set; }
        public int? LabelId { get; set; } // 卷别类型ID
        public int? LabelId_RecheckType { get; set; } // 复审类型ID
        public int? LabelId_SubjectPattern { get; set; }
        public string SubjectPatternCode { get; set; } // 题目类型代码
        public string SubjectPatternName { get; set; }// 题目类型名称
        public string ExamTypeCode { get; set; } // 卷别类型代码
        public string ExamTypeName { get; set; } // 卷别类型名称
        public string RecheckTypeCode { get; set; } // 复审类型代码
        public string RecheckTypeName { get; set; } // 复审类型名称
        public string HiddenCode_SubjectType { get; set; } // 题目类型
        public string HiddenCode_SubjectTypeName { get; set; } // 题目类型名称
        public string CheckPoint { get; set; }// 检查点
        public string Implementation { get; set; }// 执行方式
        public string InspectionDesc { get; set; }//检查标准说明
        public string Remark { get; set; }//备注
        public string Desc { get; set; }//备注
        public string ImproveAdvice { get; set; } // 改善建议
    }
}