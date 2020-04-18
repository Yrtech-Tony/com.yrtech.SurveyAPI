using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class ShopController : ApiController
    {
        ShopService shopService = new ShopService();
        [HttpGet]
        [Route("Shop/GetShopByProjectId")]
        public APIResult GetShopByProjectId(string projectId)
        {
            try
            {
                var lst = shopService.GetShopByProjectId(projectId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lst) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Shop/GetShopSubjectTypeExam")]
        public APIResult GetShopSubjectTypeExam(string projectId, string shopId)
        {
            try
            {
                var lst = shopService.GetShopSubjectTypeExam(projectId, shopId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(lst) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
