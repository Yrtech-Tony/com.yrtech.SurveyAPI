using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class CountDto
    {
        public int Id { get; set; }
        public int CompleteCount { get; set; }
        public int UnCompleteCount { get; set; }
    }
}