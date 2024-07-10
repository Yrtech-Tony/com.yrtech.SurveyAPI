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
            CommonHelper.log("Application_Start");
            GlobalConfiguration.Configure(WebApiConfig.Register);

            #region 删除服务器临时文件
            //1.创建作业调度池(Scheduler)
            IScheduler scheduler_bakDelete = StdSchedulerFactory.GetDefaultScheduler();
            //2.创建一个具体的作业即job (具体的job需要单独在一个文件中执行)
            var job_bakDelete = JobBuilder.Create<BakDataJob>().Build();
            //3.创建并配置一个触发器即trigger   1s执行一次
            var cron_bakDelete = ConfigurationManager.AppSettings["bak_corn"];
            var trigger_bakDelete = TriggerBuilder.Create().WithCronSchedule(cron_bakDelete).Build();
            //4.将job和trigger加入到作业调度池中
            scheduler_bakDelete.ScheduleJob(job_bakDelete, trigger_bakDelete);
            //5.开启调度
            scheduler_bakDelete.Start();
            #endregion
            #region GTMC 点检开始短信通知
            CommonHelper.log("点检开始短信通知");
            //1.创建作业调度池(Scheduler)
            IScheduler scheduler_smsStart = StdSchedulerFactory.GetDefaultScheduler();
            //2.创建一个具体的作业即job (具体的job需要单独在一个文件中执行)
            var job_smsStart = JobBuilder.Create<SMSSendJob_Start>().Build();
            //3.创建并配置一个触发器即trigger   1s执行一次
            var cron_smsStart = ConfigurationManager.AppSettings["smsStart_corn"];
            var trigger_smsStart = TriggerBuilder.Create().WithCronSchedule(cron_smsStart).Build();
            //4.将job和trigger加入到作业调度池中
            scheduler_smsStart.ScheduleJob(job_smsStart, trigger_smsStart);
            //5.开启调度
            scheduler_smsStart.Start();
            #endregion
            #region GTMC 点检未提交提醒
            //1.创建作业调度池(Scheduler)
            IScheduler scheduler_smsRemind = StdSchedulerFactory.GetDefaultScheduler();
            //2.创建一个具体的作业即job (具体的job需要单独在一个文件中执行)
            var job_smsRemind = JobBuilder.Create<SMSSendJob_Remind>().Build();
            //3.创建并配置一个触发器即trigger   1s执行一次
            var cron_smsRemind = ConfigurationManager.AppSettings["smsRemind_corn"];
            var trigger_smsRemind = TriggerBuilder.Create().WithCronSchedule(cron_smsRemind).Build();
            //4.将job和trigger加入到作业调度池中
            scheduler_smsRemind.ScheduleJob(job_smsRemind, trigger_smsRemind);
            //5.开启调度
            scheduler_smsRemind.Start();
            #endregion
            #region GTMC 点检提交完成通知
            //1.创建作业调度池(Scheduler)
            IScheduler scheduler_smsFinish = StdSchedulerFactory.GetDefaultScheduler();
            //2.创建一个具体的作业即job (具体的job需要单独在一个文件中执行)
            var job_smsFinish = JobBuilder.Create<SMSSendJob_Finish>().Build();
            //3.创建并配置一个触发器即trigger   1s执行一次
            var cron_smsFinish = ConfigurationManager.AppSettings["smsFeedback_corn"];
            var trigger_smsFinish = TriggerBuilder.Create().WithCronSchedule(cron_smsFinish).Build();
            //4.将job和trigger加入到作业调度池中
            scheduler_smsFinish.ScheduleJob(job_smsFinish, trigger_smsFinish);
            //5.开启调度
            scheduler_smsFinish.Start();
            #endregion
        }
        protected void Application_End(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(5000);
            
        }
    }
}
