using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ShopSubjectTypeExamDto
    {
        public int ProjectId { get; set; }
        public int ShopId { get; set; }
        public int ShopSubjectTypeExamId { get; set; }
        public string SubjectTypeExamName { get; set; } 
    }
}