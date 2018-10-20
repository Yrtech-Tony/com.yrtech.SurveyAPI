using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.Models
{
    public class ShopSubjectTypeExamVM
    {
        public int ShopSubjectTypeExamId { get; set; }
        public string SubjectTypeExamName { get; set; }
        public int ShopId { get; set; }
        public int ProjectId { get; set; }
    }

    public class ShopVM
    {
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }
}