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
                string useChk=sheet.GetCell("G" + (i + 3)).Value == null ? "" : sheet.GetCell("G" + (i + 3)).Value.ToString().Trim();
                if (useChk == "1")
                {
                    shop.UseChk = true;
                }
                else { shop.UseChk = false; }
                list.Add(shop);
            }
            list = (from shop in list orderby shop.ImportChk select shop).ToList();
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
            return list = (from area in list orderby area.ImportChk select area).ToList(); ;

        }
        //设置小区下经销商
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
                if (string.IsNullOrEmpty(areaCode)|| string.IsNullOrEmpty(shopCode)) break;
                ShopDto areaShop = new ShopDto();
                areaShop.AreaCode = areaCode;
                areaShop.ShopCode = shopCode;
                list.Add(areaShop);
            }
            return list = (from shop in list orderby shop.ImportChk select shop).ToList(); ;

        }

    }
}