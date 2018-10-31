using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class ShopConsultantDto
    {
        public int ConsultantId { get; set; }
        public int ShopId { get; set; }
        public int ProjectId { get; set; }
        public int SeqNO { get; set; }
        public string ConsultantName { get; set;  }
        public string ConsultantType { get; set; }
        //public int SubjectLinkId { get; set; }
        //public string SubjectLinkName { get; set; }
        public List<ShopConsultantSubjectLinkDto> ShopConsultantSubjectLinkList { get; set; }
        public bool UseChk { get; set; }
        public int InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public int? UploadUserId { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public string ModifyType { get; set; }//"U"：修改；"D":删除; 
        
    }
}