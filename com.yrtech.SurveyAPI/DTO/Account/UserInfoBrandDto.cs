using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class UserInfoBrandDto
    {
        public int Id { get; set; }
        public Nullable<int> TenantId { get; set; }
        public string UserId { get; set; }
        public string BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}