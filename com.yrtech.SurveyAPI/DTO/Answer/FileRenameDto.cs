using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class FileRenameDto
    {
        public int FileNameId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string FileTypeCode { get; set; }
        public string FileTypeName { get; set; }
        public Nullable<int> PhotoType { get; set; }
        public string PhotoTypeName { get; set; }
        public Nullable<int> SeqNO { get; set; }
        public string OptionCode { get; set; }
        public string OptionName { get; set; }
        public string OtherName { get; set; }
        public Nullable<int> StartIndex { get; set; }
        public Nullable<int> EndIndex { get; set; }
        public string ConnectStr { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
    }
}