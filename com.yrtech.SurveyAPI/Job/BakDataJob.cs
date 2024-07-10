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
            fileService.DBFileBak();
        }
    }
}