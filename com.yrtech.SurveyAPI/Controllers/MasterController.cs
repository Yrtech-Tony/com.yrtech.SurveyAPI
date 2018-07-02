using Survey.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;

namespace com.yrtech.SurveyAPI.Controllers
{
    public class MasterController : ApiController
    {
        MasterService service = new MasterService();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Tenant")]
        public Task<APIResult> GetTenant(string tenantId)
        {
            return service.GetTenant(tenantId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Brand")]
        public Task<APIResult> GetBrand(string tenantId, string userId)
        {
            return service.GetBrand(tenantId, userId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="brandId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Project")]
        public Task<APIResult> GetProject(string tenantId,string brandId,string projectId)
        {
            return service.GetProject(tenantId, brandId,projectId);
        }
        [HttpGet]
        [ActionName("Shop")]
        public Task<APIResult> GetShop(string tenantId, string brandId, string shopId)
        {
            return service.GetShop(tenantId, brandId, shopId);
        }

    }
}
