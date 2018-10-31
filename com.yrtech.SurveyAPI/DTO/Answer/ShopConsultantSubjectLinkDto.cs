using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ShopConsultantSubjectLinkDto
    {
        public int AnswerShopConsultantSubjectId { get; set; }
        public int ConsultantId { get; set; }
        public int SubjectLinkId { get; set; }
        public string SubjectLinkName { get; set; }
        public string SubjectLinkCode { get; set; }
        public string ConsultantName { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public string ModifyType { get; set; }//"U"：修改；"D":删除; 
    }
}