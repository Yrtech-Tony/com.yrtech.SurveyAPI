using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyAPI.DTO;
using Purchase.DAL;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class RecheckControl : ApiController
    {
        RecheckService recheckService = new RecheckService();
        MasterService masterService = new MasterService();

        [HttpPost]
        [Route("Recheck/SaveRecheckStatusDtl")]
        public APIResult SaveRecheckStatusDtl(string projectId,string shopId,string recheckTypeId,string userId)
        {
            try
            {
                RecheckStatusDtl dtl = new RecheckStatusDtl();
                dtl.ProjectId = Convert.ToInt32(projectId);
                dtl.ShopId = Convert.ToInt32(shopId);
                dtl.RecheckTypeId = Convert.ToInt32(recheckTypeId);
                dtl.InUserId = Convert.ToInt32(userId);
                recheckService.SaveRecheckStatusDtl(dtl);
                // 复审类型的个数
                int recheckTypeCount = masterService.GetSubjectRecheckType(projectId, "").Count;
                // 已经复审的类型的个数
                int comRecheckTypeCount = recheckService.GetShopRecheckStautsDtl(projectId, shopId).Count;
                if (recheckTypeCount!=0&&recheckTypeCount == comRecheckTypeCount)
                {
                    ReCheckStatus status = new ReCheckStatus();
                    status.ProjectId = Convert.ToInt32(projectId);
                    status.InUserId = Convert.ToInt32(userId);
                    status.ShopId = Convert.ToInt32(shopId);
                    status.StatusCode = "S3";
                    recheckService.SaveRecheckStatus(status);
                }
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Recheck/SaveRecheckStatus")]
        public APIResult SaveRecheckStatus(string projectId, string shopId, string recheckTypeId, string userId)
        {
            try
            {
                    ReCheckStatus status = new ReCheckStatus();
                    status.ProjectId = Convert.ToInt32(projectId);
                    status.InUserId = Convert.ToInt32(userId);
                    status.ShopId = Convert.ToInt32(shopId);
                    status.StatusCode = "S3";
                    recheckService.SaveRecheckStatus(status);
                return new APIResult() { Status = true, Body = "保存成功" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/GetRecheckStatus")]
        public APIResult GetRecheckStatus(string projectId, string shopId,string statusCode)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStauts(projectId,shopId,statusCode)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Recheck/GetRecheckStatusDtl")]
        public APIResult GetRecheckStatusDtl(string projectId, string shopId)
        {
            try
            {
                return new APIResult() { Status = true, Body = CommonHelper.Encode(recheckService.GetShopRecheckStautsDtl(projectId, shopId)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
