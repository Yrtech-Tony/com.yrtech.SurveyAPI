//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Purchase.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class TenantMemberTypeCharge
    {
        public int ChargeId { get; set; }
        public int TenantId { get; set; }
        public string MemberType { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<decimal> Charge { get; set; }
        public Nullable<System.DateTime> IndateTime { get; set; }
        public Nullable<int> InUserId { get; set; }
    }
}
