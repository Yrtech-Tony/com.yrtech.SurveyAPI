using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class NoticeDto
    {
        public int NoticeId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string NoticeCode { get; set; }
        public string NoticeContent { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public List<NoticeFile> NoticeFileList { get; set; }
        public int PublishObjectCount { get; set; }
        public int PublishViewCount { get; set; }
        public List<NoticeUserDto> NoticeObjectList { get; set; }
        public List<NoticeUserDto> NoticeViewList { get; set; }
        public string ViewStatus { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }

    }
}