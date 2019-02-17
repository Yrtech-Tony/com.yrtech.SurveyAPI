using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.Account
{
    public class ChangePasswordDto
    {
        public string UserId { get; set; }
        public string sOldPassword { get; set; }
        public string sNewPassword { get; set; }
    }
}