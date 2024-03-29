﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AreaDto
    {
        public int AreaId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string AreaType { get; set; }
        public string AreaTypeName { get; set; }
        public int? ParentId { get; set; }
        public string ParentCode { get; set; }
        public string ParentName { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public bool? UseChk { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}