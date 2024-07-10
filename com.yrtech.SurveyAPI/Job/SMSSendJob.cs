using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyAPI.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.Job
{
    public class SMSSendJob_Start : IJob
    {
        // 广汽丰田自检项目发送短信事情，其他项目暂时不使用
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        string brandId = ConfigurationManager.AppSettings["GTMCBrandId"];
        public void Execute(IJobExecutionContext context)
        {
            CommonHelper.log("JobStart");
            string smsTemplate = "SMS_468960176";
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;
            List<ProjectDto> taskList = answerService.GetTaskProject(brandId, "", "", "",startDate, endDate, "自检");
            foreach (ProjectDto project in taskList)
            {
                List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject("", "", project.ShopId.ToString(), "B_Shop");
                if (userInfoObjectList != null && userInfoObjectList.Count > 0)
                {

                    string telNO = userInfoObjectList[0].TelNO==null?"": userInfoObjectList[0].TelNO;
                    string shopName = userInfoObjectList[0].ObjectName == null ? "" : userInfoObjectList[0].ObjectName;
                    CommonHelper.log("brandId:" + brandId + " smsTemplate:" + smsTemplate + " telNO:" + telNO + " ShopName:" + shopName);
                    if (!string.IsNullOrEmpty(telNO))
                    {

                        OSSClientHelper.AlibabaCloudSendSms(telNO,shopName, smsTemplate);
                    }
                }
                
            }
        }
    }
    public class SMSSendJob_Remind : IJob
    {
        // 广汽丰田自检项目发送短信事情，其他项目暂时不使用
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        RecheckService recheckService = new RecheckService();
        string brandId = ConfigurationManager.AppSettings["GTMCBrandId"];
        public void Execute(IJobExecutionContext context)
        {
            string smsTemplate = "SMS_468960176";
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;
            List<ProjectDto> taskList = answerService.GetTaskProject(brandId, "", "", "", startDate, endDate, "自检");
            foreach (ProjectDto project in taskList)
            {
                List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatusInfo(project.ProjectId.ToString(), project.ShopId.ToString(), "S1");
                if (recheckStatusList == null || recheckStatusList.Count == 0)
                {
                    List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject("", "", project.ShopId.ToString(), "B_Shop");
                    if (userInfoObjectList != null && userInfoObjectList.Count > 0)
                    {
                        string telNO = userInfoObjectList[0].TelNO == null ? "" : userInfoObjectList[0].TelNO;
                        string shopName = userInfoObjectList[0].ObjectName == null ? "" : userInfoObjectList[0].ObjectName;
                        if (!string.IsNullOrEmpty(telNO))
                        {
                            OSSClientHelper.AlibabaCloudSendSms(telNO, shopName, smsTemplate);
                        }
                    }
                }

            }
        }
    }
    public class SMSSendJob_Finish : IJob
    {
        // 广汽丰田自检项目发送短信事情，其他项目暂时不使用
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        RecheckService recheckService = new RecheckService();
        string brandId = ConfigurationManager.AppSettings["GTMCBrandId"];
        public void Execute(IJobExecutionContext context)
        {
            string smsTemplate = "SMS_468960176";
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;
            List<ProjectDto> taskList = answerService.GetTaskProject(brandId, "", "", "", startDate, endDate, "自检");
            foreach (ProjectDto project in taskList)
            {
                List<RecheckStatusDto> recheckStatusList = recheckService.GetShopRecheckStatusInfo(project.ProjectId.ToString(), project.ShopId.ToString(), "S1");
                if (recheckStatusList!= null && recheckStatusList.Count > 0)
                {
                    List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject("", "", project.ShopId.ToString(), "B_Shop");
                    if (userInfoObjectList != null && userInfoObjectList.Count > 0)
                    {
                        string telNO = userInfoObjectList[0].TelNO == null ? "" : userInfoObjectList[0].TelNO;
                        string shopName = userInfoObjectList[0].ObjectName == null ? "" : userInfoObjectList[0].ObjectName;
                        if (!string.IsNullOrEmpty(telNO))
                        {
                            OSSClientHelper.AlibabaCloudSendSms(telNO, shopName, smsTemplate);
                        }
                    }
                }

            }
        }
    }
}