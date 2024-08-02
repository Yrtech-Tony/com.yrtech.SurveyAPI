using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class WxToken
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string openid { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string grant_type { get; set; }
        public WxTelNO phone_info { get; set; }

    }
    public class WxTelNO
    {
        public string phoneNumber { get; set; }
        public string purePhoneNumber { get; set; }
        public string countryCode { get; set; }
    }
    public class GTMCToken
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string openid { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string grant_type { get; set; }
        public WxTelNO phone_info { get; set; }

    }
}