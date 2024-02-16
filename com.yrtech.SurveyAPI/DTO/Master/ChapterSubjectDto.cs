using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ChapterSubjectDto
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } // 题目代码
        public Nullable<int> ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ChapterId { get; set; }
        public string ChapterCode { get; set; }
        public string ChapterName { get; set; }
        public Nullable<int> OrderNO { get; set; }//序号
        public Nullable<decimal> FullScore { get; set; } // 最高分
        public Nullable<decimal> LowScore { get; set; }// 最低分
        public bool? MustScore { get; set; }
        public int? LabelId { get; set; } // 卷别类型ID
        public int? LabelId_RecheckType { get; set; } // 复审类型ID
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
       
        public List<FileResultDto> PhotoList { get; set; }
        public List<InspectionStandardResultDto> InstandardList { get; set; }
        public List<LossResultDto> LossResultList { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }



    }
}