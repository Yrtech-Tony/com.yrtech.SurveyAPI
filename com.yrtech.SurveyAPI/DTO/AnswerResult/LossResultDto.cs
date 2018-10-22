using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class LossResultDto
    {
        public string LossId { get; set; }
        public string SeqNO { get; set; }
        public string LossDesc { get; set;  }
        public string LossFileNameUrl { get; set; }
        public DateTime LastTime { get; set; }
        public string ModifyType { get; set; }//"U"：修改；"D":删除; 
    }
}