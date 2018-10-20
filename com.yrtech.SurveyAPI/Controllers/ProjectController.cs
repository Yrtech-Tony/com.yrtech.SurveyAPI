using com.yrtech.SurveyAPI.Common;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace com.yrtech.SurveyAPI.Controllers
{
    public class ProjectController : ApiController
    {
        Entities db = new Entities();
        public APIResult Get(string tenantId, string brandId)
        {
            try
            {
                var proQuery = db.Project.AsQueryable();
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    int tId = int.Parse(tenantId);
                    proQuery = proQuery.Where(x => x.TenantId == tId);
                }
                if (!string.IsNullOrWhiteSpace(brandId))
                {
                    int bId = int.Parse(brandId);
                    proQuery = proQuery.Where(x => x.BrandId == bId);
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(proQuery.ToList()) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
