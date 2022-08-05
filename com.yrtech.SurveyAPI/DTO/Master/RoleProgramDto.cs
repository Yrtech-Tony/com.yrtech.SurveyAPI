using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    public class RoleProgramDto
    {
        public int tenantId { get; set; }
        public string RoleTypeCode{ get; set; }
        public int ProgramId { get; set; }
        public string ProgramCode { get; set; }
        public int? UpperProgramId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public int ShowOrder { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public List<RoleProgramDto> ChildMenu { get; set; }

    }
}