﻿using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class ImproveDto
    {
        public int ImproveId { get; set; }
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectShortName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string ShopType { get; set; }
        public string CheckPoint { get; set; }
        public string Remark { get; set; }
        public decimal? FullScore { get; set; }
        public decimal? Score { get; set; }
        public bool? AppealStatus { get; set; }
        public string LossResult { get; set; }
        public string LossResultImport { get; set; }
        public string FileResult { get; set; }
        public string LossPhotoCount { get; set; }
        public List<SubjectFile> SubjectFileList { get; set; }
        public string ImproveContent { get; set; }
        public string ImproveCycle { get; set; }
        public bool? ImproveStatus { get; set; }
        public string ImproveStatusStr { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
        
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
    }
}