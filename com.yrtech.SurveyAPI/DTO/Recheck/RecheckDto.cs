using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RecheckDto
    {
        public int RecheckId { get; set; } // 复审Id
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
        public decimal? ReCheckScore { get; set; }// 照片得分
        public bool? PassRecheck { get; set; } // 是否通过复审
        public string RecheckContent { get; set; } // 复审意见
        public string RecheckError { get; set; }// 错误类型，json
        public int? RecheckUserId { get; set; } // 复审人员Id
        public string RecheckUserName { get; set; } //复审人员名称
        public DateTime? RecheckDateTime { get; set; }// 复审时间
        //public bool? AgreeCheck { get; set; }
        //public string AgreeReason { get; set; }
        //public int? AgreeUserId { get; set; }
        //public string AgreeUserName { get; set; }
        //public DateTime? AgreeDateTime { get; set; }
        //public string LastConfirmCheck { get; set; }
        //public int? LastConfirmUserId { get; set; }
        //public string LastConfirmUserName { get; set; }
        //public DateTime? LastConfirmDateTime { get; set; }
         
    }
}