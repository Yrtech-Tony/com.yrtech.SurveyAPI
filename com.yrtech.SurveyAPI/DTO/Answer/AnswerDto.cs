﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AnswerDto
    {
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public long? AnswerId { get; set; }
        public int ProjectId { get; set; }
        public long? SubjectId { get; set; }
        public int? ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public decimal? PhotoScore { get; set; }// 照片得分
        public decimal? Score { get; set; }
        public decimal? ConsultantScore { get; set; }
        public string InspectionStandardResult { get; set; }// 检查标准结果Json
        public string FileResult { get; set; }// 标准照片结果Json
        public string LossResult { get; set; }// 失分描述结果Json
        public List<InspectionStandardResultDto> InspectionStandardResultList { get; set; }// 检查标准结果DTO，用于同步数据使用
        public List<FileResultDto> FileResultList { get; set; }// 标准照片结果DTO，用于同步数据使用
        public List<LossResultDto> LossResultList { get; set; }// 失分描述结果DTO，用于同步数据使用
        public string ShopConsultantResult { get; set; }
        public string Remark { get; set; }
        public int? InUserId { get; set; }
        public DateTime? InDateTime { get; set; }
        public int? ModifyUserId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
        public string SubjectCode { get; set; }//题目代码
        public int? OrderNO { get; set; }//题目序号
        public string Implementation { get; set; }//执行方式
        public string CheckPoint { get; set; }//检查点
        public decimal? FullScore { get; set; }
        public decimal? LowScore { get; set; }
        public string Desc { get; set; }//说明
        public string InspectionDesc { get; set; }//检查标准说明
        public RecheckDto Recheck { get; set; }

    }
}