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

        public void Execute(IJobExecutionContext context)
        {

            Trace.WriteLine("Execute BakDataJob " + DateTime.Now.ToString());
        }
    }
}