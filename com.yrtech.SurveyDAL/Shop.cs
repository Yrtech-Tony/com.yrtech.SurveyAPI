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
    
    public partial class Shop
    {
        public int ShopId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ShopShortName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public Nullable<bool> UseChk { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}
