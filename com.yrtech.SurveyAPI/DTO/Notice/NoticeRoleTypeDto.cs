﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class NoticeRoleTypeDto
    {
        public int NoticeId { get; set; }
        public int RoleTypeId { get; set; }
        public string RoleTypeCode { get; set; }
        public string RoleTypeName { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public bool ImportChk { get; set; }
        public string ImportRemark { get; set; }

    }
}