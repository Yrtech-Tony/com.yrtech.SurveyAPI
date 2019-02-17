using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.Master
{
    public class SubjectTypeScoreRegionDto
    {
        public int Id { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public Nullable<decimal> LowestScore { get; set; }
        public Nullable<decimal> FullScore { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}