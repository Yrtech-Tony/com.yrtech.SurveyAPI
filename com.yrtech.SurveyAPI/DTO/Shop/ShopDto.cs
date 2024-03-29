﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class ShopDto
    {
        public int ProjectId { get; set; }
        public int ShopId { get; set; }
        public int AreaShopId { get; set; }
        public int? AreaId { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public int? GroupId { get; set; }
        public string Address { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ShopShortName { get; set; }
        public string Province { get; set; }
        public int? ProvinceId { get; set; }
        public string City { get; set; }
        public int SubjectTypeExamId { get; set; }
        public string SubjectTypeExamName { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public bool? UseChk { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}