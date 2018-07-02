using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.Account
{
    [Serializable]
    public class AccountDto
    {
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public string TenantName { get; set;  }
        public string TenantCode { get; set; }
        public string BrandName { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public int UserId { get; set;  }
        public string Password { get; set; }
        public bool UseChk { get; set; }

    }
}