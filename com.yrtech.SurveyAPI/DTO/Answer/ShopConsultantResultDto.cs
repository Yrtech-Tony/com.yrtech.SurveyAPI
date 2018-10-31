using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ShopConsultantResultDto
    {
        public int ConsultantId { get; set; }
        public int SeqNO { get; set; }
        public string ConsultantName { get; set;  }
        public string ConsultantLossDesc { get; set; }
        public decimal? ConsultantScore { get; set; }
        public string ConsultantType { get; set; }
        public int SubjectLinkId { get; set; }
        public bool UseChk { get; set; }
        public DateTime LastTime { get; set; }
        public string ModifyType { get; set; }//"U"：修改；"D":删除; 
        
    }
}