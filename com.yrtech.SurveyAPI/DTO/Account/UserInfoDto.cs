using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class UserInfoDto
    {
        public int Id { get; set; }
        public Nullable<int> TenantId { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string UserType { get; set; }
        public string RoleType { get; set; }
        public Nullable<bool> UseChk { get; set; }
        public string Email { get; set; }
        public string TelNO { get; set; }
        public string HeadPicUrl { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}