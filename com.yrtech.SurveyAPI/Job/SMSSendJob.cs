using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyAPI.Service;
using com.yrtech.SurveyDAL;
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
            //CommonHelper.log("JobStart");
            string smsTemplate = "SMS_468960176";
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.AddDays(1).Date;
            // 获取6点需要发送短信的数据，插入smsInfo，
            List<ProjectDto> taskList = answerService.GetTaskProject(brandId, "", "", "", startDate, endDate, "自检");
            foreach (ProjectDto project in taskList)
            {
                List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject("", "", project.ShopId.ToString(), "B_Shop");
                if (userInfoObjectList != null && userInfoObjectList.Count > 0)
                {
                    string telNO = userInfoObjectList[0].TelNO == null ? "" : userInfoObjectList[0].TelNO;
                    string shopName = userInfoObjectList[0].ObjectName == null ? "" : userInfoObjectList[0].ObjectName;
                    CommonHelper.log("brandId:" + brandId + " smsTemplate:" + smsTemplate + " telNO:" + telNO + " ShopName:" + shopName);
                    if (!string.IsNullOrEmpty(telNO))
                    {
                        SMSInfo smsInfo = new SMSInfo();
                        List<SMSInfo> smsInfoList = masterService.GetSMSInfo(project.ProjectId.ToString(), userInfoObjectList[0].ObjectId.ToString(), "gtmc0600", telNO);
                        if (smsInfoList != null && smsInfoList.Count > 0)
                        {
                            smsInfo = smsInfoList[0];
                            // 状态未更新状态，或者等待回执，先去查询状态并更新
                            if (smsInfo.SendStatus == null
                                || string.IsNullOrEmpty(smsInfo.SendStatus)
                                || smsInfo.SendStatus == "1")
                            {
                                smsInfo = OSSClientHelper.AlibabaCloudQuerySendDetail(smsInfo, 10, 1);
                                masterService.SaveSMSInfo(smsInfo);
                            }
                            // 发送失败，重新发送，同时把状态置为""
                            else if (smsInfo.SendStatus == "2")
                            {
                                smsInfo = OSSClientHelper.AlibabaCloudSendSms(smsInfo, shopName, smsTemplate);
                                smsInfo.SMSSendDate = DateTime.Now;
                                smsInfo.ErrCode = "";
                                smsInfo.SendStatus = "";
                                masterService.SaveSMSInfo(smsInfo);
                            }
                        }
                        // 数据还未插入，发送短信并插入数据
                        else
                        {
                            smsInfo.ProjectId = project.ProjectId;
                            smsInfo.ShopId = userInfoObjectList[0].ObjectId;
                            smsInfo.SMSBussinessType = "gtmc0600";
                            smsInfo.TelNO = telNO;
                            smsInfo.InUserId = 1;
                            smsInfo.ModifyUserId = 1;
                            smsInfo.SMSSendDate = DateTime.Now;
                            smsInfo = OSSClientHelper.AlibabaCloudSendSms(smsInfo, shopName, smsTemplate);
                            masterService.SaveSMSInfo(smsInfo);
                        }
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
                            SMSInfo smsInfo = new SMSInfo();
                            List<SMSInfo> smsInfoList = masterService.GetSMSInfo(project.ProjectId.ToString(), userInfoObjectList[0].ObjectId.ToString(), "gtmc1030", telNO);
                            if (smsInfoList != null && smsInfoList.Count > 0)
                            {
                                smsInfo = smsInfoList[0];
                                // 状态未更新状态，或者等待回执，先去查询状态并更新
                                if (smsInfo.SendStatus == null
                                    || string.IsNullOrEmpty(smsInfo.SendStatus)
                                    || smsInfo.SendStatus == "1")
                                {
                                    smsInfo = OSSClientHelper.AlibabaCloudQuerySendDetail(smsInfo, 10, 1);
                                    masterService.SaveSMSInfo(smsInfo);
                                }
                                // 发送失败，重新发送，同时把状态置为""
                                else if (smsInfo.SendStatus == "2")
                                {
                                    smsInfo = OSSClientHelper.AlibabaCloudSendSms(smsInfo, shopName, smsTemplate);
                                    smsInfo.SMSSendDate = DateTime.Now;
                                    smsInfo.ErrCode = "";
                                    smsInfo.SendStatus = "";
                                    masterService.SaveSMSInfo(smsInfo);
                                }
                            }
                            // 数据还未插入，发送短信并插入数据
                            else
                            {
                                smsInfo.ProjectId = project.ProjectId;
                                smsInfo.ShopId = userInfoObjectList[0].ObjectId;
                                smsInfo.SMSBussinessType = "gtmc1030";
                                smsInfo.TelNO = telNO;
                                smsInfo.InUserId = 1;
                                smsInfo.ModifyUserId = 1;
                                smsInfo.SMSSendDate = DateTime.Now;
                                smsInfo = OSSClientHelper.AlibabaCloudSendSms(smsInfo, shopName, smsTemplate);
                                masterService.SaveSMSInfo(smsInfo);
                            }
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
                if (recheckStatusList != null && recheckStatusList.Count > 0)
                {
                    List<UserInfoObjectDto> userInfoObjectList = masterService.GetUserInfoObject("", "", project.ShopId.ToString(), "B_Shop");
                    if (userInfoObjectList != null && userInfoObjectList.Count > 0)
                    {
                        string telNO = userInfoObjectList[0].TelNO == null ? "" : userInfoObjectList[0].TelNO;
                        string shopName = userInfoObjectList[0].ObjectName == null ? "" : userInfoObjectList[0].ObjectName;
                        if (!string.IsNullOrEmpty(telNO))
                        {
                            SMSInfo smsInfo = new SMSInfo();
                            List<SMSInfo> smsInfoList = masterService.GetSMSInfo(project.ProjectId.ToString(), userInfoObjectList[0].ObjectId.ToString(), "gtmc1500", telNO);
                            if (smsInfoList != null && smsInfoList.Count > 0)
                            {
                                smsInfo = smsInfoList[0];
                                // 状态未更新状态，或者等待回执，先去查询状态并更新
                                if (smsInfo.SendStatus == null
                                    || string.IsNullOrEmpty(smsInfo.SendStatus)
                                    || smsInfo.SendStatus == "1")
                                {
                                    smsInfo = OSSClientHelper.AlibabaCloudQuerySendDetail(smsInfo, 10, 1);
                                    masterService.SaveSMSInfo(smsInfo);
                                }
                                // 发送失败，重新发送，同时把状态置为""
                                else if (smsInfo.SendStatus == "2")
                                {
                                    smsInfo = OSSClientHelper.AlibabaCloudSendSms(smsInfo, shopName, smsTemplate);
                                    smsInfo.SMSSendDate = DateTime.Now;
                                    smsInfo.ErrCode = "";
                                    smsInfo.SendStatus = "";
                                    masterService.SaveSMSInfo(smsInfo);
                                }
                            }
                            // 数据还未插入，发送短信并插入数据
                            else
                            {
                                smsInfo.ProjectId = project.ProjectId;
                                smsInfo.ShopId = userInfoObjectList[0].ObjectId;
                                smsInfo.SMSBussinessType = "gtmc1500";
                                smsInfo.TelNO = telNO;
                                smsInfo.InUserId = 1;
                                smsInfo.ModifyUserId = 1;
                                smsInfo.SMSSendDate = DateTime.Now;
                                smsInfo = OSSClientHelper.AlibabaCloudSendSms(smsInfo, shopName, smsTemplate);
                                masterService.SaveSMSInfo(smsInfo);
                            }
                        }
                    }
                }

            }
        }
    }
}