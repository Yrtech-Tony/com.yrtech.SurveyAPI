using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using Infragistics.Documents.Excel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using System.Web.Hosting;
using System.Collections;

namespace com.yrtech.SurveyAPI.Service
{
    public class ExcelDataService
    {
        string basePath = HostingEnvironment.MapPath(@"~/");
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();
        AnswerService answerService = new AnswerService();
        ShopService shopService = new ShopService();
        RecheckService recheckService = new RecheckService();
        ReportFileService reportService = new ReportFileService();
        AppealService appealService = new AppealService();
        #region 导入
        // 导入经销商
        public List<ShopDto> ShopImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ShopDto> list = new List<ShopDto>();
            for (int i = 0; i < 10000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                ShopDto shop = new ShopDto();
                shop.ShopCode = shopCode;
                shop.ShopName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                shop.ShopShortName = sheet.GetCell("C" + (i + 3)).Value == null ? "" : sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                shop.Province = sheet.GetCell("D" + (i + 3)).Value == null ? "" : sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                shop.City = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                shop.Address = sheet.GetCell("F" + (i + 3)).Value == null ? "" : sheet.GetCell("F" + (i + 3)).Value.ToString().Trim();
                shop.GroupCode = sheet.GetCell("G" + (i + 3)).Value == null ? "" : sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
                string useChk = sheet.GetCell("H" + (i + 3)).Value == null ? "" : sheet.GetCell("H" + (i + 3)).Value.ToString().Trim();
                if (useChk == "1")
                {
                    shop.UseChk = true;
                }
                else { shop.UseChk = false; }
                list.Add(shop);
            }
            return list;
        }
        // 导入区域
        public List<AreaDto> AreaImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<AreaDto> list = new List<AreaDto>();
            for (int i = 0; i < 10000; i++)
            {
                string areaCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(areaCode)) break;
                AreaDto area = new AreaDto();
                area.AreaCode = areaCode;
                area.AreaName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                area.AreaType = sheet.GetCell("C" + (i + 3)).Value == null ? "" : sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                area.ParentCode = sheet.GetCell("D" + (i + 3)).Value == null ? "" : sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                string useChk = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                if (useChk == "1")
                {
                    area.UseChk = true;
                }
                else { area.UseChk = false; }
                list.Add(area);
            }
            return list;

        }
        //导入小区下经销商
        public List<ShopDto> AreaShopImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ShopDto> list = new List<ShopDto>();
            for (int i = 0; i < 100000; i++)
            {
                string areaCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                string shopCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(areaCode) || string.IsNullOrEmpty(shopCode)) break;
                ShopDto areaShop = new ShopDto();
                areaShop.AreaCode = areaCode;
                areaShop.ShopCode = shopCode;
                list.Add(areaShop);
            }
            return list;

        }
        // 导入账号信息
        public List<UserInfoDto> UserInfoImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<UserInfoDto> list = new List<UserInfoDto>();
            for (int i = 0; i < 10000; i++)
            {
                string accountId = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(accountId)) break;
                UserInfoDto userInfo = new UserInfoDto();
                userInfo.AccountId = accountId; ;
                userInfo.AccountName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                userInfo.Password = sheet.GetCell("C" + (i + 3)).Value == null ? "" : sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                userInfo.RoleTypeName = sheet.GetCell("D" + (i + 3)).Value == null ? "" : sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                userInfo.Email = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                userInfo.TelNO = sheet.GetCell("F" + (i + 3)).Value == null ? "" : sheet.GetCell("F" + (i + 3)).Value.ToString().Trim();
                string useChk = sheet.GetCell("G" + (i + 3)).Value == null ? "" : sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
                if (useChk == "1")
                {
                    userInfo.UseChk = true;
                }
                else { userInfo.UseChk = false; }
                list.Add(userInfo);
            }
            return list;

        }
        // 导入所属信息
        public List<UserInfoObjectDto> UserInfoObjectImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<UserInfoObjectDto> list = new List<UserInfoObjectDto>();
            for (int i = 0; i < 10000; i++)
            {
                string accountId = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(accountId)) break;
                UserInfoObjectDto userInfoObject = new UserInfoObjectDto();
                userInfoObject.AccountId = accountId;
                userInfoObject.ObjectCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(userInfoObject);
            }
            return list;

        }
        // 导入经销商卷别类别
        public List<ProjectShopExamTypeDto> ProjectShopExamTypeImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ProjectShopExamTypeDto> list = new List<ProjectShopExamTypeDto>();
            for (int i = 0; i < 10000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                ProjectShopExamTypeDto projectShopExamTypeDto = new ProjectShopExamTypeDto();
                projectShopExamTypeDto.ShopCode = shopCode;
                projectShopExamTypeDto.ExamTypeCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(projectShopExamTypeDto);
            }
            return list;

        }
        // 导入执行人员关联的经销商
        public List<UserInfoObjectDto> UserInfoObjectImport_ExcuteShop(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<UserInfoObjectDto> list = new List<UserInfoObjectDto>();
            for (int i = 0; i < 10000; i++)
            {
                string accountId = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(accountId)) break;
                UserInfoObjectDto userInfoObject = new UserInfoObjectDto();
                userInfoObject.AccountId = accountId;
                userInfoObject.ObjectCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(userInfoObject);
            }
            return list;

        }
        // 导入期号
        public List<ProjectDto> TaskProjectImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[1];
            List<ProjectDto> list = new List<ProjectDto>();
            for (int i = 0; i < 10000; i++)
            {
                string projectCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(projectCode)) break;
                ProjectDto project = new ProjectDto();
                project.ProjectCode = projectCode;
                project.ProjectName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                project.ProjectShortName = sheet.GetCell("C" + (i + 3)).Value == null ? "" : sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                project.Year = sheet.GetCell("D" + (i + 3)).Value == null ? "" : sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                project.Quarter = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                string orderNO = sheet.GetCell("F" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(orderNO))
                {
                    project.OrderNO = null;
                }
                else {
                    project.OrderNO = Convert.ToInt32(orderNO);
                }
                string startDate = sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
                //CommonHelper.log("startDate:"+startDate);
                if (string.IsNullOrEmpty(startDate))
                {
                    project.StartDate = null;
                }
                else {
                    project.StartDate = Convert.ToDateTime(sheet.GetCell("G" + (i + 3)).Value.ToString().Trim());
                }
                string endDate = sheet.GetCell("H" + (i + 3)).Value.ToString().Trim();
                //CommonHelper.log("endDate:" + endDate);
                if (string.IsNullOrEmpty(endDate))
                {
                    project.EndDate = null;
                }
                else
                {
                    project.EndDate = Convert.ToDateTime(sheet.GetCell("H" + (i + 3)).Value.ToString().Trim());
                }
                project.ShopCode = sheet.GetCell("I" + (i + 3)).Value == null ? "" : sheet.GetCell("I" + (i + 3)).Value.ToString().Trim();
                project.ProjectGroup = sheet.GetCell("J" + (i + 3)).Value == null ? "" : sheet.GetCell("J" + (i + 3)).Value.ToString().Trim();
                project.ProjectType = sheet.GetCell("K" + (i + 3)).Value == null ? "" : sheet.GetCell("K" + (i + 3)).Value.ToString().Trim();
                list.Add(project);
            }
            return list;
        }
        // 导入题目
        public List<SubjectDto> SubjectImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<SubjectDto> list = new List<SubjectDto>();
            for (int i = 0; i < 10000; i++)
            {
                string subjectCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(subjectCode)) break;
                SubjectDto subject = new SubjectDto();
                subject.SubjectCode = subjectCode;
                string orderNO = sheet.GetCell("B" + (i + 3)).Value.ToString();
                if (string.IsNullOrEmpty(orderNO))
                {
                    subject.OrderNO = null;
                }
                else
                {
                    subject.OrderNO = Convert.ToInt32(sheet.GetCell("B" + (i + 3)).Value.ToString().Trim());
                }
                string fullScore = sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(fullScore))
                {
                    subject.FullScore = null;
                }
                else
                {
                    subject.FullScore = Convert.ToDecimal(sheet.GetCell("C" + (i + 3)).Value.ToString().Trim());
                }
                string lowScore = sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(lowScore))
                {
                    subject.LowScore = null;
                }
                else
                {
                    subject.LowScore = Convert.ToDecimal(sheet.GetCell("D" + (i + 3)).Value.ToString().Trim());
                }
                subject.Implementation = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                subject.ExamTypeCode = sheet.GetCell("F" + (i + 3)).Value == null ? "" : sheet.GetCell("F" + (i + 3)).Value.ToString().Trim();
                subject.RecheckTypeCode = sheet.GetCell("G" + (i + 3)).Value == null ? "" : sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
                subject.HiddenCode_SubjectTypeName = sheet.GetCell("H" + (i + 3)).Value == null ? "" : sheet.GetCell("H" + (i + 3)).Value.ToString().Trim();
                string mustScore =  sheet.GetCell("I" + (i + 3)).Value == null ? "" : sheet.GetCell("I" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(mustScore))
                {
                    subject.MustScore = null;
                }
                else if (mustScore == "1")
                {
                    subject.MustScore = true;
                }
                else {
                    subject.MustScore = false;
                }
                subject.CheckPoint = sheet.GetCell("J" + (i + 3)).Value == null ? "" : sheet.GetCell("J" + (i + 3)).Value.ToString().Trim();
                subject.InspectionDesc = sheet.GetCell("K" + (i + 3)).Value == null ? "" : sheet.GetCell("K" + (i + 3)).Value.ToString().Trim();
                subject.Remark = sheet.GetCell("L" + (i + 3)).Value == null ? "" : sheet.GetCell("L" + (i + 3)).Value.ToString().Trim();
                subject.ImproveAdvice = sheet.GetCell("M" + (i + 3)).Value == null ? "" : sheet.GetCell("M" + (i + 3)).Value.ToString().Trim();
                //subject.SubjectPatternCode = sheet.GetCell("N" + (i + 3)).Value == null ? "" : sheet.GetCell("N" + (i + 3)).Value.ToString().Trim();
                list.Add(subject);
            }
            return list;
        }
        // 标准照片
        public List<FileResultDto> SubjectFileImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[1];
            List<FileResultDto> list = new List<FileResultDto>();
            for (int i = 0; i < 10000; i++)
            {
                string subjectCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(subjectCode)) break;
                FileResultDto file = new FileResultDto();
                file.SubjectCode = subjectCode;
                file.FileName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                file.FileDemo = sheet.GetCell("C" + (i + 3)).Value == null ? "" : sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                file.FileDemoDesc = sheet.GetCell("D" + (i + 3)).Value == null ? "" : sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                file.FileRemark = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                list.Add(file);
            }
            return list;
        }
        // 题目和标准照片
        public List<FileResultDto> SubjectAndFileImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<FileResultDto> list = new List<FileResultDto>();
            for (int i = 0; i < 10000; i++)
            {
                string subjectCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(subjectCode)) break;
                FileResultDto subject = new FileResultDto();
                subject.SubjectCode = subjectCode;
                string orderNO = sheet.GetCell("B" + (i + 3)).Value.ToString();
                if (string.IsNullOrEmpty(orderNO))
                {
                    subject.OrderNO = null;
                }
                else
                {
                    subject.OrderNO = Convert.ToInt32(sheet.GetCell("B" + (i + 3)).Value.ToString().Trim());
                }
                string fullScore = sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(fullScore))
                {
                    subject.FullScore = null;
                }
                else
                {
                    subject.FullScore = Convert.ToDecimal(sheet.GetCell("C" + (i + 3)).Value.ToString().Trim());
                }
                string lowScore = sheet.GetCell("D" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(lowScore))
                {
                    subject.LowScore = null;
                }
                else
                {
                    subject.LowScore = Convert.ToDecimal(sheet.GetCell("D" + (i + 3)).Value.ToString().Trim());
                }
                subject.Implementation = sheet.GetCell("E" + (i + 3)).Value == null ? "" : sheet.GetCell("E" + (i + 3)).Value.ToString().Trim();
                subject.ExamTypeCode = sheet.GetCell("F" + (i + 3)).Value == null ? "" : sheet.GetCell("F" + (i + 3)).Value.ToString().Trim();
                subject.RecheckTypeCode = sheet.GetCell("G" + (i + 3)).Value == null ? "" : sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
                subject.HiddenCode_SubjectTypeName = sheet.GetCell("H" + (i + 3)).Value == null ? "" : sheet.GetCell("H" + (i + 3)).Value.ToString().Trim();
                string mustScore = sheet.GetCell("I" + (i + 3)).Value == null ? "" : sheet.GetCell("I" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(mustScore))
                {
                    subject.MustScore = null;
                }
                else if (mustScore == "1")
                {
                    subject.MustScore = true;
                }
                else
                {
                    subject.MustScore = false;
                }
                subject.CheckPoint = sheet.GetCell("J" + (i + 3)).Value == null ? "" : sheet.GetCell("J" + (i + 3)).Value.ToString().Trim();
                subject.InspectionDesc = sheet.GetCell("K" + (i + 3)).Value == null ? "" : sheet.GetCell("K" + (i + 3)).Value.ToString().Trim();
                subject.Remark = sheet.GetCell("L" + (i + 3)).Value == null ? "" : sheet.GetCell("L" + (i + 3)).Value.ToString().Trim();
                subject.ImproveAdvice = sheet.GetCell("M" + (i + 3)).Value == null ? "" : sheet.GetCell("M" + (i + 3)).Value.ToString().Trim();
               // subject.SubjectPatternCode = sheet.GetCell("N" + (i + 3)).Value == null ? "" : sheet.GetCell("N" + (i + 3)).Value.ToString().Trim();
                subject.FileName = sheet.GetCell("N" + (i + 3)).Value == null ? "" : sheet.GetCell("N" + (i + 3)).Value.ToString().Trim(); ;

                subject.FileDemo = sheet.GetCell("O" + (i + 3)).Value == null ? "" : sheet.GetCell("O" + (i + 3)).Value.ToString().Trim();
                subject.FileDemoDesc = sheet.GetCell("P" + (i + 3)).Value == null ? "" : sheet.GetCell("P" + (i + 3)).Value.ToString().Trim();
                subject.FileRemark = sheet.GetCell("Q" + (i + 3)).Value == null ? "" : sheet.GetCell("Q" + (i + 3)).Value.ToString().Trim();
                list.Add(subject);
            }
            return list;
        }
        // 标准检查标准
        public List<InspectionStandardResultDto> SubjectInspectionStandardImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[2];
            List<InspectionStandardResultDto> list = new List<InspectionStandardResultDto>();
            for (int i = 0; i < 10000; i++)
            {
                string subjectCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(subjectCode)) break;
                InspectionStandardResultDto ins = new InspectionStandardResultDto();
                ins.SubjectCode = subjectCode;
                ins.InspectionStandardName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(ins);
            }
            return list;
        }
        // 失分说明
        public List<LossResultDto> SubjectLossImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[3];
            List<LossResultDto> list = new List<LossResultDto>();
            for (int i = 0; i < 10000; i++)
            {
                string subjectCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(subjectCode)) break;
                LossResultDto loss = new LossResultDto();
                loss.SubjectCode = subjectCode;
                loss.LossCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();

                loss.LossDesc = sheet.GetCell("C" + (i + 3)).Value == null ? "" : sheet.GetCell("C" + (i + 3)).Value.ToString().Trim();
                list.Add(loss);
            }
            return list;
        }
        // 导入章节下体系信息
        public List<ChapterSubjectDto> ChapterSubjectImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ChapterSubjectDto> list = new List<ChapterSubjectDto>();
            for (int i = 0; i < 10000; i++)
            {
                string chapterCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(chapterCode)) break;
                ChapterSubjectDto chapterSubject = new ChapterSubjectDto();
                chapterSubject.ChapterCode = chapterCode;
                chapterSubject.SubjectCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(chapterSubject);
            }
            return list;

        }
        // 导入报告类型下经销商
        public List<ReportTypeShopDto> ReportTypeShopImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ReportTypeShopDto> list = new List<ReportTypeShopDto>();
            for (int i = 0; i < 10000; i++)
            {
                string reportTypeCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(reportTypeCode)) break;
                ReportTypeShopDto reportTypeShop = new ReportTypeShopDto();
                reportTypeShop.ReportTypeCode = reportTypeCode;
                reportTypeShop.ShopCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(reportTypeShop);
            }
            return list;

        }
        // 导入章节和报告类型关系
        public List<ChapterReportTypeDto> ChapterReportTypeImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ChapterReportTypeDto> list = new List<ChapterReportTypeDto>();
            for (int i = 0; i < 10000; i++)
            {
                string reportTypeCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(reportTypeCode)) break;
                ChapterReportTypeDto chapterReportTypeDto = new ChapterReportTypeDto();
                chapterReportTypeDto.ReportTypeCode = reportTypeCode;
                chapterReportTypeDto.ChapterCode = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(chapterReportTypeDto);
            }
            return list;

        }
        // 申诉设置导入
        public List<AppealSetDto> AppealShopSetImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<AppealSetDto> list = new List<AppealSetDto>();
            for (int i = 0; i < 10000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                AppealSetDto appealSet = new AppealSetDto();
                appealSet.ShopCode = shopCode;
                //if (string.IsNullOrEmpty(sheet.GetCell("B" + (i + 3)).Value.ToString()))
                //{
                //    appealSet.AppealEndDate = null;
                //}
                //else {
                //    appealSet.AppealStartDate = Convert.ToDateTime(sheet.GetCell("B" + (i + 3)).Value.ToString());
                //}
                if (string.IsNullOrEmpty(sheet.GetCell("B" + (i + 3)).Value.ToString()))
                {
                    appealSet.AppealEndDate = null;
                }
                else
                {
                    appealSet.AppealEndDate = Convert.ToDateTime(sheet.GetCell("B" + (i + 3)).Value.ToString());
                }
                list.Add(appealSet);
            }
            return list;
        }
        // 申诉导入
        public List<AppealDto> AppealImport(string ossPath)
        { // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<AppealDto> list = new List<AppealDto>();
            for (int i = 0; i < 20000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 2)).Value == null ? "" : sheet.GetCell("A" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                AppealDto appeal = new AppealDto();
                appeal.ShopCode = shopCode;
                appeal.SubjectCode = sheet.GetCell("B" + (i + 2)).Value == null ? "" : sheet.GetCell("B" + (i + 2)).Value.ToString().Trim();
                appeal.LossResultImport = sheet.GetCell("C" + (i + 2)).Value == null ? "" : sheet.GetCell("C" + (i + 2)).Value.ToString().Trim();
                list.Add(appeal);
            }
            return list;

        }
        // 申诉反馈导入
        public List<AppealDto> AppealFeedBackImport(string ossPath)
        { // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<AppealDto> list = new List<AppealDto>();
            for (int i = 0; i < 20000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 2)).Value == null ? "" : sheet.GetCell("A" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                AppealDto appeal = new AppealDto();
                appeal.ShopCode = shopCode;
                appeal.SubjectCode = sheet.GetCell("B" + (i + 2)).Value == null ? "" : sheet.GetCell("B" + (i + 2)).Value.ToString().Trim();
                string appealFeedBackStatusStr = sheet.GetCell("C" + (i + 2)).Value == null ? "" : sheet.GetCell("C" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(appealFeedBackStatusStr))
                {
                    appeal.FeedBackStatus = null;
                }
                else if (appealFeedBackStatusStr == "1")
                {
                    appeal.FeedBackStatus = true;
                }
                else
                {
                    appeal.FeedBackStatus = false;
                }
                appeal.FeedBackReason = sheet.GetCell("D" + (i + 2)).Value == null ? "" : sheet.GetCell("D" + (i + 2)).Value.ToString().Trim();
                list.Add(appeal);
            }
            return list;

        }
        // 导入区域岗位
        public List<ReportJobRateDto> ReportJobRateImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ReportJobRateDto> list = new List<ReportJobRateDto>();
            for (int i = 0; i < 10000; i++)
            {
                string areaCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(areaCode)) break;
                ReportJobRateDto reportJobRate = new ReportJobRateDto();
                reportJobRate.AreaCode = areaCode;
                reportJobRate.JobName = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                if (sheet.GetCell("C" + (i + 3)).Value == null)
                {
                    reportJobRate.JobFullCount = 0;
                }
                else
                {
                    reportJobRate.JobFullCount = Convert.ToInt32(sheet.GetCell("C" + (i + 3)).Value.ToString().Trim());
                }
                if (sheet.GetCell("D" + (i + 3)).Value == null)
                {
                    reportJobRate.JobActualCount = 0;
                }
                else
                {
                    reportJobRate.JobActualCount = Convert.ToInt32(sheet.GetCell("D" + (i + 3)).Value.ToString().Trim());
                }
                list.Add(reportJobRate);
            }
            return list;

        }
        // 申诉改善
        public List<ImproveDto> ImproveImport(string ossPath)
        { // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<ImproveDto> list = new List<ImproveDto>();
            for (int i = 0; i < 20000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 2)).Value == null ? "" : sheet.GetCell("A" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                ImproveDto improve = new ImproveDto();
                improve.ShopCode = shopCode;
                improve.SubjectCode = sheet.GetCell("B" + (i + 2)).Value == null ? "" : sheet.GetCell("B" + (i + 2)).Value.ToString().Trim();
                improve.ImproveContent = sheet.GetCell("C" + (i + 2)).Value == null ? "" : sheet.GetCell("C" + (i + 2)).Value.ToString().Trim();
                improve.ImproveCycle = sheet.GetCell("D" + (i + 2)).Value == null ? "" : sheet.GetCell("D" + (i + 2)).Value.ToString().Trim();
                list.Add(improve);
            }
            return list;

        }
        // 通知对象-RoleType
        public List<NoticeRoleTypeDto> NoticeRoleTypeImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<NoticeRoleTypeDto> list = new List<NoticeRoleTypeDto>();
            for (int i = 0; i < 10000; i++)
            {
                string roleTypeCode = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(roleTypeCode)) break;
                NoticeRoleTypeDto noticeRoleType = new NoticeRoleTypeDto();
                noticeRoleType.RoleTypeCode = roleTypeCode;
                list.Add(noticeRoleType);
            }
            return list;

        }
        // 通知对象-UserId
        public List<NoticeUserDto> NoticeUserImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<NoticeUserDto> list = new List<NoticeUserDto>();
            for (int i = 0; i < 10000; i++)
            {
                string accountId = sheet.GetCell("A" + (i + 3)).Value == null ? "" : sheet.GetCell("A" + (i + 3)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(accountId)) break;
                NoticeUserDto noticeUser = new NoticeUserDto();
                noticeUser.AccountId = accountId;
                list.Add(noticeUser);
            }
            return list;

        }
        #endregion
        #region 导出
        // 导出账号-厂商
        public string UserInfoExport(string tenantId, string brandId)
        {
            List<UserInfo> list = masterService.GetUserInfo(tenantId, brandId, "", "", "", "", "", "", true,"");
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "UserInfo.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;

            foreach (UserInfo item in list)
            {
                //账号
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.AccountId;
                //姓名
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.AccountName;
                //密码
                sheet.GetCell("C" + (rowIndex + 2)).Value = item.Password;
                //权限
                string roleTypeName = "";
                if (item.RoleType == "B_Brand") roleTypeName = "厂商";
                else if (item.RoleType == "B_Bussiness") roleTypeName = "业务";
                else if (item.RoleType == "B_WideArea") roleTypeName = "广域区域";
                else if (item.RoleType == "B_BigArea") roleTypeName = "大区";
                else if (item.RoleType == "B_MiddleArea") roleTypeName = "中区";
                else if (item.RoleType == "B_SmallArea") roleTypeName = "小区";
                else if (item.RoleType == "B_Shop") roleTypeName = "经销商";
                else if (item.RoleType == "B_Group") roleTypeName = "集团";
                sheet.GetCell("D" + (rowIndex + 2)).Value = roleTypeName;
                //Email
                sheet.GetCell("E" + (rowIndex + 2)).Value = item.Email;
                //Tel
                sheet.GetCell("F" + (rowIndex + 2)).Value = item.TelNO;
                // useChk
                if (item.UseChk == true)
                {
                    sheet.GetCell("G" + (rowIndex + 2)).Value = "Y";
                }
                else { sheet.GetCell("G" + (rowIndex + 2)).Value = "N"; }

                rowIndex++;
            }

            //保存excel文件
            string fileName = "账号" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 得分导出
        public string ShopAnsewrScoreInfoExport(string projectId, string shopId)
        {
            if (shopId == null) shopId = "";
            List<RecheckDto> recheckList = new List<RecheckDto>();
            if (string.IsNullOrEmpty(shopId)) // 经销商为空即查询所有经销商时，返回数据量太大时，EF容易报错，所以循环查询后添加到List
            {
                List<ProjectShopExamTypeDto> shopList = shopService.GetProjectShopExamType("", projectId, "");
                foreach (ProjectShopExamTypeDto shop in shopList)
                {
                    recheckList.AddRange(recheckService.GetShopRecheckScoreInfo(projectId, shop.ShopId.ToString(), "", ""));
                }
            }
            else {
                recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, "", "");
            }
            List<AppealDto> appealList = appealService.GetFeedBackInfoByAll(projectId, "", "", "", "");
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "ShopAnswerInfo.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;
            foreach (RecheckDto item in recheckList)
            {
                try
                {
                    //经销商代码
                    sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                    //经销商名称
                    sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                    //题目代码
                    sheet.GetCell("C" + (rowIndex + 2)).Value = item.SubjectCode;
                    //题目代码
                    sheet.GetCell("D" + (rowIndex + 2)).Value = item.OrderNO;
                    //检查点
                    sheet.GetCell("E" + (rowIndex + 2)).Value = item.CheckPoint;
                    //得分
                    sheet.GetCell("F" + (rowIndex + 2)).Value = item.PhotoScore;
                    //得分备注
                    sheet.GetCell("G" + (rowIndex + 2)).Value = item.Remark;
                    // 失分说明编码
                    string lossResultStrCode = "";

                    if (!string.IsNullOrEmpty(item.LossResult))
                    {
                        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                        lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            lossResultStrCode += masterService.GetSubjectLossCodeByAnswerLossName(projectId.ToString(), item.SubjectId.ToString(), lossResult.LossDesc) + ";";
                        }
                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStrCode))
                    {
                        lossResultStrCode = lossResultStrCode.Substring(0, lossResultStrCode.Length - 1);
                    }

                    //失分说明
                    string lossResultStr = "";
                    string lossResultStr2 = "";
                    if (!string.IsNullOrEmpty(item.LossResult))
                    {
                        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                        // 去掉重复项，有可能2条失分说明中勾选了重复的失分说明
                        lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            if (!string.IsNullOrEmpty(lossResult.LossDesc))
                            {
                                lossResultStr += lossResult.LossDesc + ";";
                            }
                            if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                            {
                                lossResultStr2 += lossResult.LossDesc2 + ";";
                            }
                        }

                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStr))
                    {
                        lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(lossResultStr2))
                    {
                        lossResultStr2 = lossResultStr2.Substring(0, lossResultStr2.Length - 1);
                    }
                    sheet.GetCell("H" + (rowIndex + 2)).Value = lossResultStrCode;
                    sheet.GetCell("I" + (rowIndex + 2)).Value = lossResultStr;
                    sheet.GetCell("J" + (rowIndex + 2)).Value = lossResultStr2;
                    // 通过复审
                    sheet.GetCell("K" + (rowIndex + 2)).Value = item.PassRecheckName;
                    //复审得分
                    sheet.GetCell("L" + (rowIndex + 2)).Value = item.RecheckScore;
                    //复审意见
                    sheet.GetCell("M" + (rowIndex + 2)).Value = item.RecheckContent;
                    int projectInt = Convert.ToInt32(projectId);
                    List<AppealDto> appeal = appealList.Where(x => x.ProjectId == projectInt && x.ShopId == item.ShopId && x.SubjectId == item.SubjectId).ToList();
                    string appealStatus = "";
                    string appealReason = "";
                    string feedBackStatus = "";
                    string feedBackContent = "";
                    if (appeal != null && appeal.Count > 0)
                    {
                        appealStatus = appeal[0].AppealStatus == true ? "申诉" : "不申诉";
                        appealReason = appeal[0].AppealReason;
                        feedBackStatus = appeal[0].FeedBackStatusStr;
                        feedBackContent = appeal[0].FeedBackReason;
                    }
                    sheet.GetCell("N" + (rowIndex + 2)).Value = appealStatus;
                    sheet.GetCell("O" + (rowIndex + 2)).Value = appealReason;
                    sheet.GetCell("P" + (rowIndex + 2)).Value = feedBackStatus;
                    sheet.GetCell("Q" + (rowIndex + 2)).Value = feedBackContent;
                    rowIndex++;
                }
                catch (Exception ex)
                {
                    CommonHelper.log(ex.Message);
                    continue;
                }
            }
            //保存excel文件
            string fileName = "经销商得分" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            // CommonHelper.log("保存开始");
            string filePath = dirPath + fileName;
            book.Save(filePath);
            // CommonHelper.log("保存结束");

            return filePath.Replace(basePath, ""); ;
        }
        // 得分导出-横向
        public string ShopAnsewrScoreInfoExport_L(string projectId, string shopId, string columnList)
        {
            List<ProjectDto> projectList = masterService.GetProject("", "", projectId, "", "", "","");
            if (projectList == null || projectList.Count == 0)
            {
                return "";
            }
            if (projectList[0].ProjectType == "明检")
            {
                return ShopAnsewrScoreInfoExport_L_Mingjian(projectId, shopId, columnList);
            }
            else
            {
                return ShopAnsewrScoreInfoExport_L_Micai(projectId, shopId, columnList);
            }

        }
        // 明检报告导出
        public string ShopAnsewrScoreInfoExport_L_Mingjian(string projectId, string shopId, string columnList)
        {
            if (shopId == null) shopId = "";
            List<RecheckDto> recheckList = new List<RecheckDto>();
            if (string.IsNullOrEmpty(shopId)) // 经销商为空即查询所有经销商时，返回数据量太大时，EF容易报错，所以循环查询后添加到List
            {
                List<ProjectShopExamTypeDto> shopList = shopService.GetProjectShopExamType("", projectId, "");
                foreach (ProjectShopExamTypeDto shop in shopList)
                {
                    recheckList.AddRange(recheckService.GetShopRecheckScoreInfo(projectId, shop.ShopId.ToString(), "", ""));
                }
            }
            else {
                recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, "", "");
            }
            

            List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", "", "").OrderBy(x => x.SubjectCode).ToList();
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "ShopAnswerInfo_L.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;
            int shopId_Temp = 0;
            // 填充表头
            string[] head = { "C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","AA","AB","AC","AD","AE"
                    ,"AF","AG","AH","AI","AJ","AK","AL","AM","AN","AO","AP","AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ","BA","BB","BC","BD","BE","BF"
                    ,"BG","BH","BI","BJ","BK","BL","BM","BN","BO","BP","BQ","BR","BS","BT","BU","BV","BW","BX","BY","BZ","CA","CB","CC","CD","CE","CF","CG"
                    ,"CH","CI","CJ","CK","CL","CM","CN","CO","CP","CQ","CR","CS","CT","CU","CV","CW","CX","CY","CZ","DA","DB","DC","DD","DE","DF","DG","DH"
                    ,"DI","DJ","DK","DL","DM","DN","DO","DP","DQ","DR","DS","DT","DU","DV","DW","DX","DY","DZ","EA","EB","EC","ED","EE","EF","EG","EH","EI"
                    ,"EJ","EK","EL","EM","EN","EO","EP","EQ","ER","ES","ET","EU","EV","EW","EX","EY","EZ","FA","FB","FC","FD","FE","FF","FG","FH","FI","FJ"
                    ,"FK","FL","FM","FN","FO","FP","FQ","FR","FS","FT","FU","FV","FW","FX","FY","FZ","GA","GB","GC","GD","GE","GF","GG","GH","GI","GJ","GK"
                    ,"GL","GM","GN","GO","GP","GQ","GR","GS","GT","GU","GV","GW","GX","GY","GZ","HA","HB","HC","HD","HE","HF","HG","HH","HI","HJ","HK","HL"
                    ,"HM","HN","HO","HP","HQ","HR","HS","HT","HU","HV","HW","HX","HY","HZ","IA","IB","IC","ID","IE","IF","IG","IH","II","IJ","IK","IL","IM"
                    ,"IN","IO","IP","IQ","IR","IS","IT","IU","IV","IW","IX","IY","IZ","JA","JB","JC","JD","JE","JF","JG","JH","JI","JJ","JK","JL","JM","JN"
                    ,"JO","JP","JQ","JR","JS","JT","JU","JV","JW","JX","JY","JZ","KA","KB","KC","KD","KE","KF","KG","KH","KI","KJ","KK","KL","KM","KN","KO"
                    ,"KP","KQ","KR","KS","KT","KU","KV","KW","KX","KY","KZ","LA","LB","LC","LD","LE","LF","LG","LH","LI","LJ","LK","LL","LM","LN","LO","LP"
                    ,"LQ","LR","LS","LT","LU","LV","LW","LX","LY","LZ","MA","MB","MC","MD","ME","MF","MG","MH","MI","MJ","MK","ML","MM","MN","MO","MP","MQ"
                    ,"MR","MS","MT","MU","MV","MW","MX","MY","MZ","NA","NB","NC","ND","NE","NF","NG","NH","NI","NJ","NK","NL","NM","NN","NO","NP","NQ","NR"
                    ,"NS","NT","NU","NV","NW","NX","NY","NZ","OA","OB","OC","OD","OE","OF","OG","OH","OI","OJ","OK","OL","OM","ON","OO","OP","OQ","OR","OS"
                    ,"OT","OU","OV","OW","OX","OY","OZ","PA","PB","PC","PD","PE","PF","PG","PH","PI","PJ","PK","PL","PM","PN","PO","PP","PQ","PR","PS","PT"
                    ,"PU","PV","PW","PX","PY","PZ","QA","QB","QC","QD","QE","QF","QG","QH","QI","QJ","QK","QL","QM","QN","QO","QP","QQ","QR","QS","QT","QU"
                    ,"QV","QW","QX","QY","QZ","RA","RB","RC","RD","RE","RF","RG","RH","RI","RJ","RK","RL","RM","RN","RO","RP","RQ","RR","RS","RT","RU","RV"
                    ,"RW","RX","RY","RZ","SA","SB","SC","SD","SE","SF","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SP","SQ","SR","SS","ST","SU","SV","SW"
                    ,"SX","SY","SZ","TA","TB","TC","TD","TE","TF","TG","TH","TI","TJ","TK","TL","TM","TN","TO","TP","TQ","TR","TS","TT","TU","TV","TW","TX"
                    ,"TY","TZ","UA","UB","UC","UD","UE","UF","UG","UH","UI","UJ","UK","UL","UM","UN","UO","UP","UQ","UR","US","UT","UU","UV","UW","UX","UY"
                    ,"UZ","VA","VB","VC","VD","VE","VF","VG","VH","VI","VJ","VK","VL","VM","VN","VO","VP","VQ","VR","VS","VT","VU","VV","VW","VX","VY","VZ"
                    ,"WA","WB","WC","WD","WE","WF","WG","WH","WI","WJ","WK","WL","WM","WN","WO","WP","WQ","WR","WS","WT","WU","WV","WW","WX","WY","WZ","XA"
                    ,"XB","XC","XD","XE","XF","XG","XH","XI","XJ","XK","XL","XM","XN","XO","XP","XQ","XR","XS","XT","XU","XV","XW","XX","XY","XZ","YA","YB"
                    ,"YC","YD","YE","YF","YG","YH","YI","YJ","YK","YL","YM","YN","YO","YP","YQ","YR","YS","YT","YU","YV","YW","YX","YY","YZ","ZA","ZB","ZC"
                    ,"ZD","ZE","ZF","ZG","ZH","ZI","ZJ","ZK","ZL","ZM","ZN","ZO","ZP","ZQ","ZR","ZS","ZT","ZU","ZV","ZW","ZX","ZY","ZZ"};
            for (int i = 0; i < subjectList.Count; i++)
            {
                //int j = 0;
                string[] column = columnList.Split(';');
                sheet.GetCell(head[i * column.Length] + 1).Value = subjectList[i].SubjectCode;

                for (int j = 1; j < column.Length; j++)
                {
                    if (column[j].Contains("LossCode"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = subjectList[i].SubjectCode + "-失分代码";
                    }
                    if (column[j].Contains("LossName"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = subjectList[i].SubjectCode + "-描述";
                    }
                    if (column[j].Contains("LossName2"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = subjectList[i].SubjectCode + "-补充描述";
                    }
                    if (column[j].Contains("PhotoScore"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = "得分";
                    }
                    if (column[j].Contains("Remark"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = "得分备注";
                    }
                }
                //sheet.GetCell(head[(i * 4 + 2)] + 1).Value = "得分";
                //sheet.GetCell(head[(i * 4 + 3)] + 1).Value = "备注";
            }
            // 绑定数据
            foreach (RecheckDto item in recheckList)
            {
                try
                {
                    // 判断经销商是否一致，如果不一致的话，开始往下一行写数据，如果一致还在当前行写入数据
                    if (shopId_Temp == 0)
                    {
                        shopId_Temp = item.ShopId;
                    }
                    else if (shopId_Temp != item.ShopId)
                    {
                        rowIndex++;
                        shopId_Temp = item.ShopId;
                    }
                    //失分说明
                    string lossResultStr = "";
                    string lossResultStr2 = "";
                    string lossResultStrCode = "";

                    if (!string.IsNullOrEmpty(item.LossResult))
                    {
                        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                        lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            lossResultStrCode += masterService.GetSubjectLossCodeByAnswerLossName(projectId.ToString(), item.SubjectId.ToString(), lossResult.LossDesc) + ";";
                            if (!string.IsNullOrEmpty(lossResult.LossDesc))
                            {
                                lossResultStr += lossResult.LossDesc + ";";
                            }
                            if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                            {
                                lossResultStr2 += lossResult.LossDesc2 + ";";
                            }
                        }
                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStr))
                    {
                        lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(lossResultStr2))
                    {
                        lossResultStr2 = lossResultStr2.Substring(0, lossResultStr2.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(lossResultStrCode))
                    {
                        lossResultStrCode = lossResultStrCode.Substring(0, lossResultStrCode.Length - 1);
                    }
                    //经销商代码
                    sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                    //经销商名称
                    sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                    for (int i = 0; i < head.Length; i++)
                    {
                        string cellValue = sheet.GetCell(head[i] + 1).Value == null ? "" : sheet.GetCell(head[i] + 1).Value.ToString();
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            if (cellValue == item.SubjectCode)
                            {
                                int j = 0;
                                if (columnList.Contains("LossCode"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = lossResultStrCode;
                                    j++;
                                }
                                if (columnList.Contains("LossName"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = lossResultStr;
                                    j++;
                                }
                                if (columnList.Contains("LossName2"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = lossResultStr2;
                                    j++;
                                }
                                if (columnList.Contains("PhotoScore"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = item.PhotoScore;
                                    j++;
                                }
                                if (columnList.Contains("Remark"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = item.Remark;
                                }
                            }
                        }
                    }
                }
                catch (Exception eex)
                {

                    CommonHelper.log(eex.Message);
                    continue;
                }
            }
            //保存excel文件
            string fileName = "经销商得分" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 密采报告导出
        public string ShopAnsewrScoreInfoExport_L_Micai(string projectId, string shopId, string columnList)
        {
            if (shopId == null) shopId = "";
            List<RecheckDto> recheckList = new List<RecheckDto>();
            if (string.IsNullOrEmpty(shopId)) // 经销商为空即查询所有经销商时，返回数据量太大时，EF容易报错，所以循环查询后添加到List
            {
                List<ProjectShopExamTypeDto> shopList = shopService.GetProjectShopExamType("", projectId, "");
                foreach (ProjectShopExamTypeDto shop in shopList)
                {
                    recheckList.AddRange(recheckService.GetShopRecheckScoreInfo(projectId, shop.ShopId.ToString(), "", ""));
                }
            }
            else {
                recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, "", "");
            }
           
            List<SubjectDto> subjectList = masterService.GetSubject(projectId, "", "", "").OrderBy(x => x.SubjectCode).ToList();
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "ShopAnswerInfo_L_M.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;
            int shopId_Temp = 0;
            // 填充表头
            string[] head = { "AH","AI","AJ","AK","AL","AM","AN","AO","AP","AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ","BA","BB","BC","BD","BE","BF"
                    ,"BG","BH","BI","BJ","BK","BL","BM","BN","BO","BP","BQ","BR","BS","BT","BU","BV","BW","BX","BY","BZ","CA","CB","CC","CD","CE","CF","CG"
                    ,"CH","CI","CJ","CK","CL","CM","CN","CO","CP","CQ","CR","CS","CT","CU","CV","CW","CX","CY","CZ","DA","DB","DC","DD","DE","DF","DG","DH"
                    ,"DI","DJ","DK","DL","DM","DN","DO","DP","DQ","DR","DS","DT","DU","DV","DW","DX","DY","DZ","EA","EB","EC","ED","EE","EF","EG","EH","EI"
                    ,"EJ","EK","EL","EM","EN","EO","EP","EQ","ER","ES","ET","EU","EV","EW","EX","EY","EZ","FA","FB","FC","FD","FE","FF","FG","FH","FI","FJ"
                    ,"FK","FL","FM","FN","FO","FP","FQ","FR","FS","FT","FU","FV","FW","FX","FY","FZ","GA","GB","GC","GD","GE","GF","GG","GH","GI","GJ","GK"
                    ,"GL","GM","GN","GO","GP","GQ","GR","GS","GT","GU","GV","GW","GX","GY","GZ","HA","HB","HC","HD","HE","HF","HG","HH","HI","HJ","HK","HL"
                    ,"HM","HN","HO","HP","HQ","HR","HS","HT","HU","HV","HW","HX","HY","HZ","IA","IB","IC","ID","IE","IF","IG","IH","II","IJ","IK","IL","IM"
                    ,"IN","IO","IP","IQ","IR","IS","IT","IU","IV","IW","IX","IY","IZ","JA","JB","JC","JD","JE","JF","JG","JH","JI","JJ","JK","JL","JM","JN"
                    ,"JO","JP","JQ","JR","JS","JT","JU","JV","JW","JX","JY","JZ","KA","KB","KC","KD","KE","KF","KG","KH","KI","KJ","KK","KL","KM","KN","KO"
                    ,"KP","KQ","KR","KS","KT","KU","KV","KW","KX","KY","KZ","LA","LB","LC","LD","LE","LF","LG","LH","LI","LJ","LK","LL","LM","LN","LO","LP"
                    ,"LQ","LR","LS","LT","LU","LV","LW","LX","LY","LZ","MA","MB","MC","MD","ME","MF","MG","MH","MI","MJ","MK","ML","MM","MN","MO","MP","MQ"
                    ,"MR","MS","MT","MU","MV","MW","MX","MY","MZ","NA","NB","NC","ND","NE","NF","NG","NH","NI","NJ","NK","NL","NM","NN","NO","NP","NQ","NR"
                    ,"NS","NT","NU","NV","NW","NX","NY","NZ","OA","OB","OC","OD","OE","OF","OG","OH","OI","OJ","OK","OL","OM","ON","OO","OP","OQ","OR","OS"
                    ,"OT","OU","OV","OW","OX","OY","OZ","PA","PB","PC","PD","PE","PF","PG","PH","PI","PJ","PK","PL","PM","PN","PO","PP","PQ","PR","PS","PT"
                    ,"PU","PV","PW","PX","PY","PZ","QA","QB","QC","QD","QE","QF","QG","QH","QI","QJ","QK","QL","QM","QN","QO","QP","QQ","QR","QS","QT","QU"
                    ,"QV","QW","QX","QY","QZ","RA","RB","RC","RD","RE","RF","RG","RH","RI","RJ","RK","RL","RM","RN","RO","RP","RQ","RR","RS","RT","RU","RV"
                    ,"RW","RX","RY","RZ","SA","SB","SC","SD","SE","SF","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SP","SQ","SR","SS","ST","SU","SV","SW"
                    ,"SX","SY","SZ","TA","TB","TC","TD","TE","TF","TG","TH","TI","TJ","TK","TL","TM","TN","TO","TP","TQ","TR","TS","TT","TU","TV","TW","TX"
                    ,"TY","TZ","UA","UB","UC","UD","UE","UF","UG","UH","UI","UJ","UK","UL","UM","UN","UO","UP","UQ","UR","US","UT","UU","UV","UW","UX","UY"
                    ,"UZ","VA","VB","VC","VD","VE","VF","VG","VH","VI","VJ","VK","VL","VM","VN","VO","VP","VQ","VR","VS","VT","VU","VV","VW","VX","VY","VZ"
                    ,"WA","WB","WC","WD","WE","WF","WG","WH","WI","WJ","WK","WL","WM","WN","WO","WP","WQ","WR","WS","WT","WU","WV","WW","WX","WY","WZ","XA"
                    ,"XB","XC","XD","XE","XF","XG","XH","XI","XJ","XK","XL","XM","XN","XO","XP","XQ","XR","XS","XT","XU","XV","XW","XX","XY","XZ","YA","YB"
                    ,"YC","YD","YE","YF","YG","YH","YI","YJ","YK","YL","YM","YN","YO","YP","YQ","YR","YS","YT","YU","YV","YW","YX","YY","YZ","ZA","ZB","ZC"
                    ,"ZD","ZE","ZF","ZG","ZH","ZI","ZJ","ZK","ZL","ZM","ZN","ZO","ZP","ZQ","ZR","ZS","ZT","ZU","ZV","ZW","ZX","ZY","ZZ"};
            for (int i = 0; i < subjectList.Count; i++)
            {
                //int j = 0;
                string[] column = columnList.Split(';');
                sheet.GetCell(head[i * column.Length] + 1).Value = subjectList[i].SubjectCode;

                for (int j = 1; j < column.Length; j++)
                {
                    if (column[j].Contains("LossCode"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = subjectList[i].SubjectCode + "-失分代码";
                    }
                    if (column[j].Contains("LossName"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = subjectList[i].SubjectCode + "-描述";
                    }
                    if (column[j].Contains("LossName2"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = subjectList[i].SubjectCode + "-补充描述";
                    }
                    if (column[j].Contains("PhotoScore"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = "得分";
                    }
                    if (column[j].Contains("Remark"))
                    {
                        sheet.GetCell(head[(i * column.Length + j)] + 1).Value = "得分备注";
                    }
                }
                //sheet.GetCell(head[(i * 4 + 2)] + 1).Value = "得分";
                //sheet.GetCell(head[(i * 4 + 3)] + 1).Value = "备注";
            }
            // 绑定数据
            foreach (RecheckDto item in recheckList)
            {
                try
                {


                    // 判断经销商是否一致，如果不一致的话，开始往下一行写数据，如果一致还在当前行写入数据
                    if (shopId_Temp == 0)
                    {
                        shopId_Temp = item.ShopId;
                    }
                    else if (shopId_Temp != item.ShopId)
                    {
                        rowIndex++;
                        shopId_Temp = item.ShopId;
                    }
                    //失分说明
                    string lossResultStr = "";
                    string lossResultStr2 = "";
                    string lossResultStrCode = "";

                    if (!string.IsNullOrEmpty(item.LossResult))
                    {
                        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                        lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                        foreach (LossResultDto lossResult in lossResultList)
                        {
                            lossResultStrCode += masterService.GetSubjectLossCodeByAnswerLossName(projectId.ToString(), item.SubjectId.ToString(), lossResult.LossDesc) + ";";
                            if (!string.IsNullOrEmpty(lossResult.LossDesc))
                            {
                                lossResultStr += lossResult.LossDesc + ";";
                            }
                            if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                            {
                                lossResultStr2 += lossResult.LossDesc2 + ";";
                            }
                        }
                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStr))
                    {
                        lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(lossResultStr2))
                    {
                        lossResultStr2 = lossResultStr2.Substring(0, lossResultStr2.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(lossResultStrCode))
                    {
                        lossResultStrCode = lossResultStrCode.Substring(0, lossResultStrCode.Length - 1);
                    }
                    //经销商代码
                    sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                    //经销商名称
                    sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                    // 绑定经销商进店信息
                    List<AnswerShopInfoDto> answerShopInfoList = answerService.GetAnswerShopInfo(projectId, item.ShopId.ToString(), "");
                    if (answerShopInfoList != null && answerShopInfoList.Count > 0)
                    {
                        //省份
                        sheet.GetCell("C" + (rowIndex + 2)).Value = answerShopInfoList[0].Province;
                        //城市
                        sheet.GetCell("D" + (rowIndex + 2)).Value = answerShopInfoList[0].City;
                        //进店方式
                        sheet.GetCell("E" + (rowIndex + 2)).Value = answerShopInfoList[0].InShopMode;
                        //进店地址 
                        sheet.GetCell("F" + (rowIndex + 2)).Value = answerShopInfoList[0].InShopAddress;
                        //地址是否一致
                        //if (answerShopInfoList[0].AddressCheck == true)
                        //{
                        //    sheet.GetCell("G" + (rowIndex + 2)).Value = "是";
                        //}
                        //else
                        //{ 
                        sheet.GetCell("G" + (rowIndex + 2)).Value = answerShopInfoList[0].AddressCheck;
                        //销售顾问姓名
                        sheet.GetCell("H" + (rowIndex + 2)).Value = answerShopInfoList[0].SalesName;
                        //销售顾问姓名确认方式
                        sheet.GetCell("I" + (rowIndex + 2)).Value = answerShopInfoList[0].SalesNameCheckMode;
                        //无法确认原因
                        sheet.GetCell("J" + (rowIndex + 2)).Value = answerShopInfoList[0].SakesNameCheckReason;
                        //评估员姓名
                        sheet.GetCell("K" + (rowIndex + 2)).Value = answerShopInfoList[0].ExecuteName;
                        //籍贯
                        sheet.GetCell("L" + (rowIndex + 2)).Value = answerShopInfoList[0].ExcuteAddress;
                        //居住城市
                        sheet.GetCell("M" + (rowIndex + 2)).Value = answerShopInfoList[0].ExcuteCity;
                        //虚拟职业
                        sheet.GetCell("N" + (rowIndex + 2)).Value = answerShopInfoList[0].ExcuteJob;
                        //购车用途
                        sheet.GetCell("O" + (rowIndex + 2)).Value = answerShopInfoList[0].CarBuyPurpose;
                        //购车预算
                        if (answerShopInfoList[0].CarBuyBudget == null)
                        {
                            sheet.GetCell("P" + (rowIndex + 2)).Value = "0";
                        }
                        else
                        {
                            sheet.GetCell("P" + (rowIndex + 2)).Value = answerShopInfoList[0].CarBuyBudget.ToString();
                        }
                        //目标购买车型
                        sheet.GetCell("Q" + (rowIndex + 2)).Value = answerShopInfoList[0].CarBuyType;
                        // 对比竞品车型
                        sheet.GetCell("R" + (rowIndex + 2)).Value = answerShopInfoList[0].CarCompetitor;
                        // 虚拟家庭住址
                        sheet.GetCell("S" + (rowIndex + 2)).Value = answerShopInfoList[0].ExcuteHomeAddress;
                        // 评估员留店电话
                        sheet.GetCell("T" + (rowIndex + 2)).Value = answerShopInfoList[0].ExcutePhone;
                        // 调查日期
                        if (answerShopInfoList[0].InShopStartDate == null)
                        {
                            sheet.GetCell("U" + (rowIndex + 2)).Value = "";
                        }
                        else
                        {
                            sheet.GetCell("U" + (rowIndex + 2)).Value = Convert.ToDateTime(answerShopInfoList[0].InShopStartDate).ToString("yyyy-MM-dd");
                        }
                        // 进店时间
                        if (answerShopInfoList[0].InShopStartDate == null)
                        {
                            sheet.GetCell("V" + (rowIndex + 2)).Value = "";
                        }
                        else
                        {
                            sheet.GetCell("V" + (rowIndex + 2)).Value = Convert.ToDateTime(answerShopInfoList[0].InShopStartDate).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        // 出店时间
                        if (answerShopInfoList[0].InShopEndDate == null)
                        {
                            sheet.GetCell("W" + (rowIndex + 2)).Value = "";
                        }
                        else
                        {
                            sheet.GetCell("W" + (rowIndex + 2)).Value = Convert.ToDateTime(answerShopInfoList[0].InShopEndDate).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        // 进店时长
                        if (answerShopInfoList[0].InShopStartDate != null && answerShopInfoList[0].InShopEndDate != null)
                        {
                            sheet.GetCell("X" + (rowIndex + 2)).Value = CommonHelper.DiffMinutes(Convert.ToDateTime(answerShopInfoList[0].InShopEndDate), Convert.ToDateTime(answerShopInfoList[0].InShopStartDate)).ToString();
                        }
                        else
                        {
                            sheet.GetCell("X" + (rowIndex + 2)).Value = "0";
                        }
                        // 是否试乘试驾
                        //if (answerShopInfoList[0].TestDriverCheck == true)
                        //{
                        //    sheet.GetCell("Y" + (rowIndex + 2)).Value = "是";
                        //}
                        //else {
                        sheet.GetCell("Y" + (rowIndex + 2)).Value = answerShopInfoList[0].TestDriverCheck;
                        //}
                        // 试乘试驾开始时间
                        if (answerShopInfoList[0].TestDriverStartDate == null)
                        {
                            sheet.GetCell("Z" + (rowIndex + 2)).Value = "";
                        }
                        else
                        {
                            sheet.GetCell("Z" + (rowIndex + 2)).Value = Convert.ToDateTime(answerShopInfoList[0].TestDriverStartDate).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        // 试乘试驾结束时间
                        if (answerShopInfoList[0].TestDriverEndDate == null)
                        {
                            sheet.GetCell("AA" + (rowIndex + 2)).Value = "";
                        }
                        else
                        {
                            sheet.GetCell("AA" + (rowIndex + 2)).Value = Convert.ToDateTime(answerShopInfoList[0].TestDriverEndDate).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        // 试乘试驾时长
                        if (answerShopInfoList[0].TestDriverStartDate != null && answerShopInfoList[0].TestDriverEndDate != null)
                        {
                            sheet.GetCell("AB" + (rowIndex + 2)).Value = CommonHelper.DiffMinutes(Convert.ToDateTime(answerShopInfoList[0].TestDriverEndDate), Convert.ToDateTime(answerShopInfoList[0].TestDriverStartDate)).ToString();
                        }
                        else { sheet.GetCell("AB" + (rowIndex + 2)).Value = "0"; }
                        // 天气异常情况
                        sheet.GetCell("AC" + (rowIndex + 2)).Value = answerShopInfoList[0].WeatherCondition;
                        // 店外异常情况
                        sheet.GetCell("AD" + (rowIndex + 2)).Value = answerShopInfoList[0].OutShopCondition;
                        // 店内异常情况
                        sheet.GetCell("AE" + (rowIndex + 2)).Value = answerShopInfoList[0].InShopCondition;
                        // 录音录像是否完整
                        sheet.GetCell("AF" + (rowIndex + 2)).Value = answerShopInfoList[0].VideoComplete;
                        // 执行是否被识别
                        sheet.GetCell("AG" + (rowIndex + 2)).Value = answerShopInfoList[0].ExecuteRecogniz;
                    }

                    for (int i = 0; i < head.Length; i++)
                    {
                        string cellValue = sheet.GetCell(head[i] + 1).Value == null ? "" : sheet.GetCell(head[i] + 1).Value.ToString();
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            if (cellValue == item.SubjectCode)
                            {
                                int j = 0;
                                if (columnList.Contains("LossCode"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = lossResultStrCode;
                                    j++;
                                }
                                if (columnList.Contains("LossName"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = lossResultStr;
                                    j++;
                                }
                                if (columnList.Contains("LossName2"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = lossResultStr2;
                                    j++;
                                }
                                if (columnList.Contains("PhotoScore"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = item.PhotoScore;
                                    j++;
                                }
                                if (columnList.Contains("Remark"))
                                {
                                    sheet.GetCell(head[i + j] + (rowIndex + 2)).Value = item.Remark;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonHelper.log(ex.Message);
                    continue;
                }
            }
            //保存excel文件
            string fileName = "经销商得分" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 审核状态导出
        public string RecheckStatusExport(string projectId, string shopId, string shopCode)
        {
            List<RecheckStatusDto> recheckList = recheckService.GetShopRecheckStatus(projectId, shopId, shopCode);
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "RecheckStatus.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;
            foreach (RecheckStatusDto item in recheckList)
            {
                //经销商代码
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                //进店
                if (!string.IsNullOrEmpty(item.Status_S0))
                    sheet.GetCell("C" + (rowIndex + 2)).Value = "√";
                //提交复审
                if (!string.IsNullOrEmpty(item.Status_S1))
                    sheet.GetCell("D" + (rowIndex + 2)).Value = "√";
                //提交复审时间
                if (item.Status_S1_Date != null)
                    sheet.GetCell("E" + (rowIndex + 2)).Value = Convert.ToDateTime(item.Status_S1_Date).ToString("yyyy-MM-dd HH:mm:ss");
                //审核进行中
                if (!string.IsNullOrEmpty(item.Status_S2))
                    sheet.GetCell("F" + (rowIndex + 2)).Value = "√";
                //审核完毕
                if (!string.IsNullOrEmpty(item.Status_S3))
                    sheet.GetCell("G" + (rowIndex + 2)).Value = "√";
                //审核修改
                if (!string.IsNullOrEmpty(item.Status_S4))
                    sheet.GetCell("H" + (rowIndex + 2)).Value = "√";
                //一审确认
                if (!string.IsNullOrEmpty(item.Status_S8))
                    sheet.GetCell("I" + (rowIndex + 2)).Value = "√";
                //二审完毕
                if (!string.IsNullOrEmpty(item.Status_S9))
                    sheet.GetCell("J" + (rowIndex + 2)).Value = "√";
                // 仲裁
                if (!string.IsNullOrEmpty(item.Status_S5))
                    sheet.GetCell("K" + (rowIndex + 2)).Value = "√";
                // 督导抽查
                if (!string.IsNullOrEmpty(item.Status_S6))
                    sheet.GetCell("L" + (rowIndex + 2)).Value = "√";
                //PM 抽查
                if (!string.IsNullOrEmpty(item.Status_S7))
                    sheet.GetCell("M" + (rowIndex + 2)).Value = "√";

                rowIndex++;
            }

            //保存excel文件
            string fileName = "复审进度" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 进店信息导出
        public string AnswerShopInfoExport(string projectId, string shopId)
        {
            List<AnswerShopInfoDto> list = answerService.GetAnswerShopInfo(projectId, shopId, "");
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "AnswerShopInfo.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;
            foreach (AnswerShopInfoDto item in list)
            {
                //经销商代码
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                // 执行组长
                sheet.GetCell("C" + (rowIndex + 2)).Value = item.TeamLeader;
                // 开始时间
                sheet.GetCell("D" + (rowIndex + 2)).Value = item.StartDate;
                // 添加时间
                sheet.GetCell("E" + (rowIndex + 2)).Value = item.InDateTime;
                // 修改时间
                sheet.GetCell("F" + (rowIndex + 2)).Value = item.ModifyDateTime;
                rowIndex++;
            }

            //保存excel文件
            string fileName = "进店信息" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 报告下载日志导出
        public string ReportLogExport(string project, string reportFileName, string startDate, string endDate)
        {
            List<ReportFileActionLogDto> list = reportService.ReportFileActionLogSearch("", "", "", project, reportFileName, startDate, endDate);
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "ReportLog.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 1;

            foreach (ReportFileActionLogDto item in list)
            {
                //操作时间
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.InDateTime;
                //下载账号
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.AccountId;
                //下载姓名
                sheet.GetCell("C" + (rowIndex + 2)).Value = item.AccountName;
                // 期号代码
                sheet.GetCell("D" + (rowIndex + 2)).Value = item.ProjectCode;
                //期号名称
                sheet.GetCell("E" + (rowIndex + 2)).Value = item.ProjectName;
                //文件
                sheet.GetCell("F" + (rowIndex + 2)).Value = item.ReportFileName;

                rowIndex++;
            }

            //保存excel文件
            string fileName = "报告下载日志" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, "");
        }
        // 申诉导出
        public string AppealExport(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopIdStr, string keyword, int pageNum, int pageCoun)
        {
            List<AppealDto> list = appealService.GetShopAppealInfoByPage(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopIdStr, keyword, pageNum, pageCoun);
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "Appeal.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 1;

            foreach (AppealDto item in list)
            {
                //经销商代码
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                //体系号
                sheet.GetCell("C" + (rowIndex + 2)).Value = item.SubjectCode;
                // 检查点
                sheet.GetCell("D" + (rowIndex + 2)).Value = item.CheckPoint;
                //申诉理由
                sheet.GetCell("E" + (rowIndex + 2)).Value = item.AppealReason;
                //反馈状态
                sheet.GetCell("F" + (rowIndex + 2)).Value = item.FeedBackStatusStr;
                //反馈意见
                sheet.GetCell("G" + (rowIndex + 2)).Value = item.FeedBackReason;
                //反馈人
                sheet.GetCell("H" + (rowIndex + 2)).Value = item.FeedBackUserName;
                //反馈时间
                sheet.GetCell("I" + (rowIndex + 2)).Value = item.FeedBackDateTime;

                rowIndex++;
            }

            //保存excel文件
            string fileName = "申诉" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 申诉反馈导出
        public string AppealFeedbackExport(string projectId, string keyword, string userId, string appealStatus, int pageNum, int pageCoun)
        {
            List<AppealDto> list = appealService.GetFeedBackInfoByPage(projectId, keyword, userId, appealStatus, "", pageNum, pageCoun);
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "Appeal.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 1;

            foreach (AppealDto item in list)
            {
                //经销商代码
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                //体系号
                sheet.GetCell("C" + (rowIndex + 2)).Value = item.SubjectCode;
                // 检查点
                sheet.GetCell("D" + (rowIndex + 2)).Value = item.CheckPoint;
                // 未达标情况说明
                if (!string.IsNullOrEmpty(item.LossResultImport)) // 导入的失分说明
                {
                    sheet.GetCell("E" + (rowIndex + 2)).Value = item.LossResultImport;
                }// 系统生成的失分说明
                else
                {
                    //失分说明由json转成字符串
                    string lossResultStr = "";
                    if (!string.IsNullOrEmpty(item.LossResult))
                    {
                        List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                        lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                        foreach (LossResultDto lossResult in lossResultList)
                        {

                            if (!string.IsNullOrEmpty(lossResult.LossDesc))
                            {
                                lossResultStr += lossResult.LossDesc + ";";
                            }
                        }
                    }
                    // 去掉最后一个分号
                    if (!string.IsNullOrEmpty(lossResultStr))
                    {
                        lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                    }
                    sheet.GetCell("E" + (rowIndex + 2)).Value = lossResultStr;
                }
                //申诉理由
                sheet.GetCell("F" + (rowIndex + 2)).Value = item.AppealReason;
                //反馈状态
                sheet.GetCell("G" + (rowIndex + 2)).Value = item.FeedBackStatusStr;
                //反馈意见
                sheet.GetCell("H" + (rowIndex + 2)).Value = item.FeedBackReason;
                //反馈人
                sheet.GetCell("I" + (rowIndex + 2)).Value = item.FeedBackUserName;
                //反馈时间
                sheet.GetCell("J" + (rowIndex + 2)).Value = item.FeedBackDateTime;

                rowIndex++;
            }
            //保存excel文件
            string fileName = "申诉反馈" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        // 报告平台导出扣分细项
        public string ReportShopLossResultExport(string projectId, string bussinessType, string wideArea, string bigArea, string middleArea, string smallArea, string shopId, string keyword)
        {
            List<AnswerDto> list = reportService.ReportShopLossResult(projectId, bussinessType, wideArea, bigArea, middleArea, smallArea, shopId, keyword);
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "ReportLossResult.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 1;

            foreach (AnswerDto answer in list)
            {
                //失分说明
                string lossResultAddStr = "";
                if (!string.IsNullOrEmpty(answer.LossResult))
                {
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                    // 去掉重复项，有可能2条失分说明中勾选了重复的失分说明
                    lossResultList = lossResultList.Where((x, i) => lossResultList.FindIndex(z => z.LossDesc == x.LossDesc && z.LossDesc2 == x.LossDesc2) == i).ToList();
                    foreach (LossResultDto lossResult in lossResultList)
                    {
                        if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                        {
                            lossResultAddStr += lossResult.LossDesc2 + ";";
                        }
                    }
                }
                // 去掉最后一个分号
                if (!string.IsNullOrEmpty(lossResultAddStr))
                {
                    lossResultAddStr = lossResultAddStr.Substring(0, lossResultAddStr.Length - 1);
                }
                answer.LossResultAdd = lossResultAddStr;
            }
            foreach (AnswerDto item in list)
            {
                //经销商代码
                sheet.GetCell("A" + (rowIndex + 2)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 2)).Value = item.ShopName;
                //一级指标
                sheet.GetCell("C" + (rowIndex + 2)).Value = item.ChapterName;
                // 检查点
                sheet.GetCell("D" + (rowIndex + 2)).Value = item.CheckPoint;
                //检查点是否达标
                sheet.GetCell("E" + (rowIndex + 2)).Value = item.LossResultAdd;
                rowIndex++;
            }

            //保存excel文件
            string fileName = "检核细节" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            return filePath.Replace(basePath, ""); ;
        }
        #endregion
    }
}