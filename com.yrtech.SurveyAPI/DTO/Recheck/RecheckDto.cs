using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RecheckDto
    {
        public int? RecheckId { get; set; } // 复审Id
        public int? ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public int? OrderNO { get; set; }//题目序号
        public string Implementation { get; set; }//执行方式
        public string CheckPoint { get; set; }//检查点
        public decimal? PhotoScore { get; set; }// 照片得分
        public string Remark { get; set; } // 得分备注
        public string LossResult { get; set; } // 失分说明
        public decimal? RecheckScore { get; set; }// 照片得分
        public bool? PassRecheck { get; set; } // 是否通过复审
        public string PassRecheckName { get; set; }
        public string RecheckContent { get; set; } // 复审意见
        public string RecheckError { get; set; }// 错误类型，json
        public int? RecheckUserId { get; set; } // 复审人员Id
        public string RecheckUserName { get; set; } //复审人员名称
        public DateTime? RecheckDateTime { get; set; }// 复审时间
        public Nullable<bool> PassReCheck_Confirm { get; set; } // 复审确认结果
        public string PassReCheckName_Confirm { get; set; } // 复审确认结果名称
        public string ReCheckContent_Confirm { get; set; }// 复审确认意见
        public string ReCheckError_Confirm { get; set; }// 复审确认错误类型
        public Nullable<decimal> ReCheckScore_Confirm { get; set; }// 复审确认建议得分
        public Nullable<System.DateTime> ReCheckDateTime_Confirm { get; set; } // 复审确认时间
        public Nullable<int> ReCheckUserId_Confirm { get; set; }// 复审确认人员
        public Nullable<bool> PassReCheck_Sec { get; set; }// 二审结果
        public string PassReCheckName_Sec { get; set; }// 二审结果名称
        public string ReCheckContent_Sec { get; set; }// 二审意见
        public string ReCheckError_Sec { get; set; }// 二审错误
        public Nullable<decimal> ReCheckScore_Sec { get; set; }// 二审建议得分
        public Nullable<System.DateTime> ReCheckDateTime_Sec { get; set; }// 二审时间
        public Nullable<int> ReCheckUserId_Sec { get; set; }// 二审人员
        public int RecheckTypeId { get; set; }
        public string RecheckTypeName { get; set; }
        public Nullable<bool> AgreeCheck { get; set; }
        public string AgreeReason { get; set; }
        public Nullable<int> AgreeUserId { get; set; }
        public Nullable<System.DateTime> AgreeDateTime { get; set; }
        public string LastConfirmCheck { get; set; }
        public string LastConfirmCheckName { get; set; }
        public string LastConfirmReason { get; set; }
        public Nullable<int> LastConfirmUserId { get; set; }
        public Nullable<System.DateTime> LastConfirmDate { get; set; }
        public string SupervisionSpotCheckContent { get; set; }
        public Nullable<int> SupervisionSpotCheckUserId { get; set; }
        public Nullable<System.DateTime> SupervisionSpotCheckDateTime { get; set; }
        public string PMSpotCheckContent { get; set; }
        public Nullable<int> PMSpotCheckUserId { get; set; }
        public Nullable<System.DateTime> PMSpotCheckDateTime { get; set; }

    }
}