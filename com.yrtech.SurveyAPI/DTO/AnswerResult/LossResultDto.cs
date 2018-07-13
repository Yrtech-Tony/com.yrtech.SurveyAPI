using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO.AnswerResult
{
    [Serializable]
    public class LossResultDto
    {
        public string LossId { get; set; }
        public string SeqNO { get; set; }
        public string LossDesc { get; set;  }
        public string LossFIleNameUrl { get; set; }
        public DateTime LastTime { get; set; }
    }
}