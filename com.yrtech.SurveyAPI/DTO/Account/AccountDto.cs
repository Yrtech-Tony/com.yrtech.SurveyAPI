using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.DTO
{
    [Serializable]
    public class AccountDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int? BrandId { get; set; }
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        public string MemberType { get; set; }
        public string BrandName { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string RoleType { get; set; }
        public string Password { get; set; }
        public bool? UseChk { get; set; }
        public string TelNO { get; set; }
        public string Email { get; set; }
        public string HeadPicUrl { get; set; }
        public List<Tenant> TenantList { get; set; }
        public List<Brand> BrandList { get; set; }
        public List<ShopDto> ShopList { get; set; }
        public List<AreaDto> SmallAreaList { get; set; }
        public List<AreaDto> MiddleAreaList { get; set; }
        public List<AreaDto> BigAreaList { get; set; }
        public List<AreaDto> WideAreaList { get; set; }
        public List<AreaDto> BussinessAreaList { get; set; }
        public List<GroupDto> GroupList { get; set; }
      public List<HiddenColumn> OSSInfo { get; set; }
    }
}