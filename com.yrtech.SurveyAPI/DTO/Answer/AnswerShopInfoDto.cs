using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AnswerShopInfoDto
    {
        public long Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string TeamLeader { get; set; }//执行组长
        public string PhotoUrl { get; set; } //照片地址
        public string Longitude { get; set; } // 经度
        public string Latitude { get; set; } // 纬度
        public int UserId { get; set; }
        public DateTime? StartDate { get; set; }
      
        public string InShopMode { get; set; } // 进店方式
        public string InShopAddress { get; set; }//进店地址
        public string AddressCheck { get; set; }//实际地址和列表是否一致
        public string SalesName { get; set; } // 销售顾问姓名/
        public string SalesNameCheckMode { get; set; }// 销售顾问姓名确认方式
        public string SakesNameCheckReason { get; set; }// 如无法确认，请注明原因
        public string ExecuteName { get; set; }// 评估员姓名
        public string ExcuteAddress { get; set; }// 籍贯
        public string ExcuteCity { get; set; }// 居住城市
        public string ExcuteJob { get; set; }// 虚拟职业
        public string CarBuyPurpose { get; set; }// 购车用途
        public decimal? CarBuyBudget { get; set; }// 购车预算
        public string CarBuyType { get; set; }// 目标购买车型
        public string CarCompetitor { get; set; }// 对比竞品车型
        public string ExcuteHomeAddress { get; set; }// 虚拟家庭住址
        public string ExcutePhone { get; set; }// 评估员留店电话
        public DateTime? InShopStartDate { get; set; } //进店开始时间（包括日期，小时，分）
        public DateTime? InShopEndDate { get; set; } // 进店结束时间（包括日期，小时，分）
        public int InShopM { get; set; } // 进店时长（分钟）
        public string TestDriverCheck { get; set; } // 是否试乘试驾
        public DateTime? TestDriverStartDate { get; set; } // 试乘试驾开始时间（包括日期，小时，分）
        public DateTime? TestDriverEndDate { get; set; }// 试乘试驾结束时间（包括日期，小时，分）
        public int TestDriverM { get; set; } // 试驾时长（分钟）
        public string WeatherCondition { get; set; } // 天气异常情况
        public string OutShopCondition { get; set; }// 店外异常情况
        public string InShopCondition { get; set; }// 店内异常情况
        public string VideoComplete { get; set; }// 录音录像是否完成
        public string ExecuteRecogniz { get; set; }// 执行是否被识别
        public int InUserId { get; set; }
        public DateTime? InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
    }
}