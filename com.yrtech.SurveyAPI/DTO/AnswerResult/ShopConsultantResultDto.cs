using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.AnswerResult
{
    [Serializable]
    public class ShopConsultantResultDto
    {
        public string ConsultantId { get; set; }
        public string SeqNO { get; set; }
        public string ConsultantName { get; set;  }
        public string ConsultantLossDesc { get; set; }
        public string ConsultantScore { get; set; }
        public string ConsultantType { get; set; }
        public DateTime LastTime { get; set; }
    }
}