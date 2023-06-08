using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.Job;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace com.yrtech.SurveyAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //1.创建作业调度池(Scheduler)
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //2.创建一个具体的作业即job (具体的job需要单独在一个文件中执行)
            var job = JobBuilder.Create<BakDataJob>().Build();
            //3.创建并配置一个触发器即trigger   1s执行一次
            var cron = ConfigurationManager.AppSettings["bak_corn"];
            var trigger = TriggerBuilder.Create().WithCronSchedule(cron).Build();
            //4.将job和trigger加入到作业调度池中
            scheduler.ScheduleJob(job, trigger);
            //5.开启调度
            scheduler.Start();
        }
    }
}
