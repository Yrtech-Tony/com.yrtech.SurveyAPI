using com.yrtech.SurveyAPI.Common;
using Survey.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace com.yrtech.SurveyAPI.Service
{
    public class DataDownService
    {
        Entities db = new Entities();

        public async Task<APIResult> GetAllTenant()
        {
            List<Tenant> tenant = await db.Tenant.ToListAsync();
            return new APIResult() { Status = true, Body = CommonHelper.EncodeDto<Tenant>(tenant) };
        }
    }
}