﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Survey : DbContext
    {
        public Survey()
            : base("name=Survey")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AnswerShopInfo> AnswerShopInfo { get; set; }
        public virtual DbSet<AreaShop> AreaShop { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<ReCheckStatus> ReCheckStatus { get; set; }
        public virtual DbSet<RecheckStatusDtl> RecheckStatusDtl { get; set; }
        public virtual DbSet<Shop> Shop { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Tenant> Tenant { get; set; }
        public virtual DbSet<TenantMemberTypeCharge> TenantMemberTypeCharge { get; set; }
        public virtual DbSet<TenantSimple> TenantSimple { get; set; }
        public virtual DbSet<UserInfoBrand> UserInfoBrand { get; set; }
        public virtual DbSet<UserInfoObject> UserInfoObject { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<RoleType> RoleType { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<HiddenColumn> HiddenColumn { get; set; }
        public virtual DbSet<ReportFileActionLog> ReportFileActionLog { get; set; }
        public virtual DbSet<ReportFile> ReportFile { get; set; }
        public virtual DbSet<AppealFile> AppealFile { get; set; }
        public virtual DbSet<Appeal> Appeal { get; set; }
        public virtual DbSet<SubjectFile> SubjectFile { get; set; }
        public virtual DbSet<SubjectInspectionStandard> SubjectInspectionStandard { get; set; }
        public virtual DbSet<SubjectLossResult> SubjectLossResult { get; set; }
        public virtual DbSet<LabelObject> LabelObject { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }
        public virtual DbSet<Label> Label { get; set; }
    }
}
