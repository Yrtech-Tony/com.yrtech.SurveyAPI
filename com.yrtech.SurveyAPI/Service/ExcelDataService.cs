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
                // 失分说明编码
                string lossResultStrCode = "";

                if (!string.IsNullOrEmpty(item.LossResult))
                {
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
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
                if (!string.IsNullOrEmpty(item.LossResult))
                {
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                    foreach (LossResultDto lossResult in lossResultList)
                    {
                        if (!string.IsNullOrEmpty(lossResult.LossDesc))
                        {
                            lossResultStr += lossResult.LossDesc + ";";
                        }
                        if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                        {
                            lossResultStr += lossResult.LossDesc2 + ";";
                        }
                    }

                }
                // 去掉最后一个分号
                if (!string.IsNullOrEmpty(lossResultStr))
                {
                    lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
                }
                sheet.GetCell("H" + (rowIndex + 2)).Value = lossResultStrCode;
                sheet.GetCell("I" + (rowIndex + 2)).Value = lossResultStr;
                // 通过复审
                sheet.GetCell("J" + (rowIndex + 2)).Value = item.PassRecheckName;
                //复审得分
                sheet.GetCell("K" + (rowIndex + 2)).Value = item.RecheckScore;
                //复审得分
                sheet.GetCell("L" + (rowIndex + 2)).Value = item.RecheckContent;

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
        // 得分导出-横向
        public string ShopAnsewrScoreInfoExport_L(string projectId, string shopId)
        {
            List<RecheckDto> recheckList = recheckService.GetShopRecheckScoreInfo(projectId, shopId, "", "");
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
                sheet.GetCell(head[i * 3] + 1).Value = subjectList[i].SubjectCode;
                sheet.GetCell(head[(i * 3 + 1)] + 1).Value = subjectList[i].SubjectCode + "-描述";
                //sheet.GetCell(head[(i * 3 + 2)] + 1).Value = "得分";
                sheet.GetCell(head[(i * 3 + 2)] + 1).Value = "备注";
            }
            // 绑定数据
            foreach (RecheckDto item in recheckList)
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
                string lossResultStrCode = "";

                if (!string.IsNullOrEmpty(item.LossResult))
                {
                    List<LossResultDto> lossResultList = CommonHelper.DecodeString<List<LossResultDto>>(item.LossResult);
                    foreach (LossResultDto lossResult in lossResultList)
                    {
                        lossResultStrCode += masterService.GetSubjectLossCodeByAnswerLossName(projectId.ToString(), item.SubjectId.ToString(), lossResult.LossDesc) + ";";
                        if (!string.IsNullOrEmpty(lossResult.LossDesc))
                        {
                            lossResultStr += lossResult.LossDesc + ";";
                        }
                        if (!string.IsNullOrEmpty(lossResult.LossDesc2))
                        {
                            lossResultStr += lossResult.LossDesc2 + ";";
                        }
                    }
                }
                // 去掉最后一个分号
                if (!string.IsNullOrEmpty(lossResultStr))
                {
                    lossResultStr = lossResultStr.Substring(0, lossResultStr.Length - 1);
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
                            sheet.GetCell(head[i] + (rowIndex + 2)).Value = lossResultStrCode;
                            sheet.GetCell(head[i + 1] + (rowIndex + 2)).Value = lossResultStr;
                            //sheet.GetCell(head[i + 2] + (rowIndex + 2)).Value = item.PhotoScore;
                            sheet.GetCell(head[i + 2] + (rowIndex + 2)).Value = item.Remark;
                        }
                    }
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
                //审核进行中
                if (!string.IsNullOrEmpty(item.Status_S2))
                    sheet.GetCell("E" + (rowIndex + 2)).Value = "√";
                //审核完毕
                if (!string.IsNullOrEmpty(item.Status_S3))
                    sheet.GetCell("F" + (rowIndex + 2)).Value = "√";
                //审核修改
                if (!string.IsNullOrEmpty(item.Status_S4))
                    sheet.GetCell("G" + (rowIndex + 2)).Value = "√";
                // 仲裁
                if (!string.IsNullOrEmpty(item.Status_S5))
                    sheet.GetCell("H" + (rowIndex + 2)).Value = "√";
                // 督导抽查
                if (!string.IsNullOrEmpty(item.Status_S6))
                    sheet.GetCell("I" + (rowIndex + 2)).Value = "√";
                //PM 抽查
                if (!string.IsNullOrEmpty(item.Status_S7))
                    sheet.GetCell("J" + (rowIndex + 2)).Value = "√";

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

            return filePath.Replace(basePath, ""); ;
        }
        #endregion
    }
}