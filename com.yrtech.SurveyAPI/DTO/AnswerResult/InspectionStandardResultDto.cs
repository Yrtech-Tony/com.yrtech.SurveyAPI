﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.AnswerResult
{
    [Serializable]
    public class InspectionStandardResultDto
    {
        public string InspectionStandardId { get; set; }
        public string SeqNO { get; set; }
        public string InspectionStandardName { get; set;  }
        public string AnswerResult { get; set; }
        public DateTime LastTime { get; set; }
    }
}