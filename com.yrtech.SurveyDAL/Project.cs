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
    
    public partial class Project
    {
        public int ProjectId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectShortName { get; set; }
        public string Year { get; set; }
        public string Quarter { get; set; }
        public Nullable<int> OrderNO { get; set; }
        public Nullable<System.DateTime> ReportDeployDate { get; set; }
        public Nullable<bool> RechckShopShow { get; set; }
        public Nullable<bool> PhotoUploadMode { get; set; }
        public string PhotoSize { get; set; }
        public string ProjectType { get; set; }
        public Nullable<bool> AppealShow { get; set; }
        public string ScoreShowType { get; set; }
        public Nullable<bool> SelfTestChk { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}
