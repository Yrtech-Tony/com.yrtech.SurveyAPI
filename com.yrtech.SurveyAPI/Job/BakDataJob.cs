using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.Job
{
    public class BakDataJob :IJob
    {
        FileService fileService = new FileService();
        public void Execute(IJobExecutionContext context)
        {
            CommonHelper.log("进入定时任务");
            fileService.DBFileBak();
        }
    }
}