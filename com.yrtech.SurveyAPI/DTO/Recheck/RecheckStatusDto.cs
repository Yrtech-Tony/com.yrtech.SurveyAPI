using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RecheckStatusDto
    {
        public int RecheckStatusId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Status_S0 { get; set; } // 进店状态
        public string Status_S1 { get; set; } // 提交复审状态
        public string Status_S2 { get; set; }// 复审进行中
        public string Status_S3 { get; set; }// 复审完毕
        public string Status_S0_Name{ get; set; }
        public string Status_S1_Name { get; set; }
        public string Status_S2_Name { get; set; }
        public string Status_S3_Name { get; set; }
        public int? RecheckUserId { get; set; } // 复审人员Id
        public string RecheckUserName { get; set; } // 复审人员名称
        public DateTime? RecheckDateTime { get; set; }// 复审时间
    }
}