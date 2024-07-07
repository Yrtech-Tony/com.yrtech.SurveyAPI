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
    public class FileService
    {
        public void DBFileBak()
        {
            // 删除数据库备份文件
            CommonHelper.log("进入方法");
            string filePath = @"D:\Survey\DBLog";
            try
            {
                DateTime now = DateTime.Now.AddDays(-4);
                string file = now.ToString("yyyy-MM-dd");
                DirectoryInfo dr = new DirectoryInfo(filePath);
                FileSystemInfo[] fsInfo = dr.GetFileSystemInfos();//返回文件夹所有的文件和子目录
                //// 备份文件到OSS
                //CommonHelper.log("开始上传");
                //foreach (FileSystemInfo fs in fsInfo)
                //{
                //    if (fs is DirectoryInfo)
                //    {
                //    }
                //    else
                //    {
                //        if (fs.LastWriteTime < now)
                //        {
                //            OSSClientHelper.PutObjectMultipart("DBLog" + @"/" +file + @"/" + fs.Name, fs.FullName);
                //        }
                //    }
                //}
                //CommonHelper.log("上传完毕");
                CommonHelper.log("开始删除");
                foreach (FileSystemInfo fs in fsInfo)
                {
                    if (fs.CreationTime < now)
                    {
                        if (fs is DirectoryInfo)
                        {
                            DirectoryInfo subdr = new DirectoryInfo(fs.FullName);
                            subdr.Delete(true); // 删除子目录和文件
                        }
                        else
                        {
                            File.Delete(fs.FullName);
                        }
                    }
                }
                CommonHelper.log("删除完毕");
            }
            catch (Exception ex)
            {
                CommonHelper.log(ex.InnerException + "_" + ex.Message);
            }
        }

    }
}