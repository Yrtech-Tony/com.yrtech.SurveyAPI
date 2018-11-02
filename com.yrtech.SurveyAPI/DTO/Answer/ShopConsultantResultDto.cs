using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ShopConsultantResultDto
    {
        public int AnswerShopConsultantScoreId { get; set; }
        public int ConsultantId { get; set; }
        public string ConsultantName { get; set;  }
        public int SeqNO { get; set; }
        public string ConsultantLossDesc { get; set; }
        public decimal? ConsultantScore { get; set; }
        public string ConsultantType { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public string ModifyType { get; set; }//"U"：修改；"D":删除; 
        
    }
}