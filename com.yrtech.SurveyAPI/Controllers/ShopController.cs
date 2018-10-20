using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using Purchase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace com.yrtech.SurveyAPI.Controllers
{
    public class ShopController : ApiController
    {
        Entities db = new Entities();
        public APIResult Get(string projectId, string tenantId)
        {
            try
            {
                ShopService shopService = new ShopService();
                var lst = shopService.GetShopByProjectId(projectId, tenantId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lst) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }

        public APIResult GetExamType(string projectId, string shopId)
        {
            try
            {
                ShopService shopService = new ShopService();
                var lst = shopService.GetExamType(projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lst) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
