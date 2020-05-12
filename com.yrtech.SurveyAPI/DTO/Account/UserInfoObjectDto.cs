using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class UserInfoObjectDto
    {
        public int Id { get; set; }
        public Nullable<int> TenantId { get; set; }
        public int UserId { get; set; }
        public int ObjectId { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectName { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}