using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.Account
{
    public class ChangePasswordDto
    {
        public string UserId { get; set; } // 登录的Id
        public string sOldPassword { get; set; } // 密码
        public string sNewPassword { get; set; }
        public string OpenId { get; set; } // OpenId
        public string AccountId { get; set; }
    }
}