//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace com.yrtech.SurveyDAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Subject
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ExamTypeId { get; set; }
        public Nullable<int> RecheckTypeId { get; set; }
        public Nullable<int> OrderNO { get; set; }
        public Nullable<decimal> FullScore { get; set; }
        public Nullable<decimal> LowScore { get; set; }
        public string CheckPoint { get; set; }
        public string Implementation { get; set; }
        public string InspectionDesc { get; set; }
        public string Desc { get; set; }
        public string Remark { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}
