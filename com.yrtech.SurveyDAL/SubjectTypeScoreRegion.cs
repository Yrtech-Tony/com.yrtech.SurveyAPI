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
    
    public partial class SubjectTypeScoreRegion
    {
        public int Id { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> SubjectTypeId { get; set; }
        public Nullable<decimal> LowestScore { get; set; }
        public Nullable<decimal> FullScore { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}
