using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AnswerShopInfoDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ShopId { get; set; }
        public string TeamLeaderName { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public int InUserId { get; set;  }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}