using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class GTMC365Dto
    {
        //public long Id { get; set; }
        //public int ProjectId { get; set; }
        //public string ProjectCode { get; set; }
        //public string ProjectName { get; set; }
        //public int ShopId { get; set; }
        public string DEALERCODE { get; set; }
        public string organizename { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string area { get; set; }
       // public string TeamLeader { get; set; }//执行组长
        //public string SalesName { get; set; } // 岗位
        //public string SalesNameCheckMode { get; set; }// 销售顾问姓名确认方式
        public string answer_state_translate { get; set; }
        public string attendance_rate { get; set; }
        public string operate_type { get; set; }
        public string extract_time { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        //public int InUserId { get; set; }
        //public DateTime? InDateTime { get; set; }
        //public int ModifyUserId { get; set; }
        //public DateTime? ModifyDateTime { get; set; }
    }
}