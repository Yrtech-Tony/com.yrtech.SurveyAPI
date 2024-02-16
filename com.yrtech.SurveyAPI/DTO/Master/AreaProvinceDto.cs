using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class AreaProvinceDto
    {
        public int AreaProvinceId { get; set; }
        public int AreaId { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceCode { get; set; }
        public string ProviceName { get; set; }
    }
}