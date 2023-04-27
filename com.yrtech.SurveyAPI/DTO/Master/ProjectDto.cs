﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ProjectDto
    {
        public int ProjectId { get; set; }
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
        public DateTime? AppealStartDate { get; set; }
        public bool? AppealShow { get; set; } // 是否在
        public string ProjectType { get; set; } // 明检、密采
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }

    }
}