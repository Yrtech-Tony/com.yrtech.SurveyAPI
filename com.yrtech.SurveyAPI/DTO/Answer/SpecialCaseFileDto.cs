﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class SpecialCaseFileDto
    {
        public int SpecialCaseId { get; set; }
        public int SeqNO { get; set; }
        public string FileType { get; set; }
        public string FileTypeName { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public int InUserId { get; set; }
        public string InUserName { get; set; }
        public DateTime InDateTime { get; set; }
    }
}