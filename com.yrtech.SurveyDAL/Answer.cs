//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Survey.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Answer
    {
        public int AnswerId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public int SubjectId { get; set; }
        public int ShopId { get; set; }
        public Nullable<decimal> PhotoScore { get; set; }
        public string InspectionStandardResult { get; set; }
        public string FileResult { get; set; }
        public string LossResult { get; set; }
        public string ShopConsultantResult { get; set; }
        public string Remark { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}
