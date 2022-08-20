using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AnswerPhotoLogDto
    {
        public int ProjectId { get; set; }
        public string ShopId { get; set; }
        public string ShopCode { get; set; }
        public long? SubjectId { get; set; }
        public int? OrderNO { get; set; }
        
        public string SubjectCode { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ShopName { get; set; }
        public string PhotoUrl { get; set; } //照片地址
        public string PhotoType { get; set; }
        public string PhotoStatus { get; set; } //"1": 已上传 "0":未上传 
        public string UploadStatus { get; set; } //"1": 已上传 "0":未上传 
        public string UploadProcess { get; set; }
        public int UploadCount { get; set; }
        public int TotalCount { get; set; }
        public int UserId { get; set; }
        public int InUserId { get; set;  }
        public DateTime? InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
    }
}