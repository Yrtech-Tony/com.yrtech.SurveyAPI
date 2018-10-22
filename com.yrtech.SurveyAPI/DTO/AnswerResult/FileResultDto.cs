using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class FileResultDto
    {
        public string FileId { get; set; }
        public string SeqNO { get; set; }
        public string FileName { get; set;  }
        public string FileType { get; set; }
        public string Url { get; set; }
        public DateTime LastTime { get; set; }
        public string ModifyType { get; set; }//"U"：修改；"D":删除; 
    }
}