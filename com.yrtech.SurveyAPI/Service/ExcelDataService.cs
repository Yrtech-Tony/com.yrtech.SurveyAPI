using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using Infragistics.Documents.Excel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using System.Web.Hosting;

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
                shop.GroupCode = sheet.GetCell("F" + (i + 3)).Value == null ? "" : sheet.GetCell("F" + (i + 3)).Value.ToString().Trim();
                string useChk = sheet.GetCell("G" + (i + 3)).Value == null ? "" : sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
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
                subject.CheckPoint = sheet.GetCell("I" + (i + 3)).Value == null ? "" : sheet.GetCell("I" + (i + 3)).Value.ToString().Trim();
                subject.InspectionDesc = sheet.GetCell("J" + (i + 3)).Value == null ? "" : sheet.GetCell("J" + (i + 3)).Value.ToString().Trim();
                subject.Remark = sheet.GetCell("K" + (i + 3)).Value == null ? "" : sheet.GetCell("K" + (i + 3)).Value.ToString().Trim();
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
                list.Add(file);
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

                loss.LossDesc = sheet.GetCell("B" + (i + 3)).Value == null ? "" : sheet.GetCell("B" + (i + 3)).Value.ToString().Trim();
                list.Add(loss);
            }
            return list;
        }
        #endregion
        #region 导出
        // 导出账号
        public string UserInfoExport(string tenantId, string brandId)
        {
            List<UserInfo> list = masterService.GetUserInfo(tenantId, brandId, "", "", "", "", "", "");
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
            List<RecheckDto> recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, "", "");
            Workbook book = Workbook.Load(basePath + @"\Excel\" + "ShopAnswerInfo.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 0;
            foreach (RecheckDto item in recheckList)
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
                //失分说明
                string lossResultStr = "";
                if (!string.IsNullOrEmpty(item.LossResult))
                {
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                    foreach (LossResultDto lossResult in lossResultList)
                    {
                        lossResultStr += lossResult.LossDesc + ";";
                    }
                }
                sheet.GetCell("H" + (rowIndex + 2)).Value = lossResultStr;
                // 通过复审
                sheet.GetCell("I" + (rowIndex + 2)).Value = item.PassRecheckName;
                //复审得分
                sheet.GetCell("J" + (rowIndex + 2)).Value = item.RecheckScore;
                //复审得分
                sheet.GetCell("K" + (rowIndex + 2)).Value = item.RecheckContent;

                rowIndex++;
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

            return filePath.Replace(basePath, ""); ;
        }
        #endregion
    }
}