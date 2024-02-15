using com.yrtech.SurveyAPI.Common;
using com.yrtech.SurveyAPI.DTO;
using com.yrtech.SurveyDAL;
using Infragistics.Documents.Excel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using System.Web.Hosting;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Data.SqlClient;

namespace com.yrtech.SurveyAPI.Service
{
    public class PhotoService
    {
        Survey db = new Survey();
        string basePath = HostingEnvironment.MapPath(@"~/");
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();
        AnswerService answerService = new AnswerService();
        ShopService shopService = new ShopService();
        RecheckService recheckService = new RecheckService();
        AppealService appealService = new AppealService();
        #region 下载标准照片
        public string FileResultDownLoad(string projectId, string shopId)
        {
            List<AnswerDto> list = answerService.GetShopAnswerScoreInfo(projectId, shopId, "", "");
            if (list == null || list.Count == 0) return "";
            basePath = basePath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            #region 从OSS把文件下载到服务器
            foreach (AnswerDto answer in list)
            {
                try
                {
                    // 下载标准照片
                    List<FileResultDto> fileResultList = new List<FileResultDto>();
                    if (!string.IsNullOrEmpty(answer.FileResult))
                    {

                        List<FileResultDto> fileResultList_temp = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                        foreach (FileResultDto fileresult in fileResultList_temp)
                        {
                            if (!string.IsNullOrEmpty(fileresult.Url))
                            {
                                fileResultList.Add(fileresult);
                            }
                        }
                    }
                    foreach (FileResultDto fileResult in fileResultList)
                    {
                        // 获取文件夹信息
                        string folder1 = GetFolderName(answer.ProjectId.ToString(), "1", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                        string folder2 = GetFolderName(answer.ProjectId.ToString(), "2", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                        string folder3 = GetFolderName(answer.ProjectId.ToString(), "3", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                        string folder4 = GetFolderName(answer.ProjectId.ToString(), "4", answer.ShopCode, answer.ShopName, answer.SubjectCode, fileResult.FileName, fileResult.SeqNO.ToString(), fileResult.ModifyDateTime.ToString("yyyyMMddHHmmssfff"), "1", answer.OrderNO.ToString(), "");
                        if (!string.IsNullOrEmpty(folder4))
                        {
                            // 创建1级目录
                            if (!string.IsNullOrEmpty(folder1))
                            {
                                if (!Directory.Exists(folder + @"\" + folder1))
                                {
                                    Directory.CreateDirectory(folder + @"\" + folder1);
                                }
                            }  // 创建2级目录
                            if (!string.IsNullOrEmpty(folder2))
                            {
                                if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2))
                                {
                                    Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2);
                                }
                            }
                            // 创建3级目录
                            if (!string.IsNullOrEmpty(folder3))
                            {
                                if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3))
                                {
                                    Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3);
                                }
                            }
                            try
                            {
                                // 下载文件的名称为标准照片名称
                                if (!string.IsNullOrEmpty(fileResult.Url))
                                {
                                    string[] url = fileResult.Url.Split(';');
                                    for (int i = 0; i < url.Length; i++)
                                    {

                                        string urlstr = url[i].ToString();
                                        if (url.Length == 1)
                                        {
                                            // 已有的文件先删除
                                            string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4 + ".jpg").Replace("\\", @"\");
                                            if (File.Exists(filePath))
                                            {
                                                File.Delete(filePath);
                                            }
                                            OSSClientHelper.GetObject(urlstr, filePath);
                                        }
                                        else
                                        {
                                            // 已有的文件先删除
                                            string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4).Replace("\\", @"\");
                                            if (File.Exists(filePath + "_" + (i + 1).ToString() + ".jpg"))
                                            {
                                                File.Delete(filePath + "_" + (i + 1).ToString() + ".jpg");
                                            }
                                            OSSClientHelper.GetObject(urlstr, filePath + "_" + (i + 1).ToString() + ".jpg");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
                catch (Exception eex)
                {

                    continue;
                }
            }
            #endregion
            // 压缩文件，如果失败返回空
            if (!ZipInForFileResutl(list, downLoadfolder, basePath, downLoadPath, 9))
            {
                return "";
            }
            else
            { // 压缩成功上传到OSS
                return OSSClientHelper.PutObjectMultipart("DownTempFile" + @"/" + downLoadfolder + ".zip", downLoadPath);
            }
            //return downLoadPath.Replace(HostingEnvironment.MapPath(@"~/"), "");
        }
        /// <summary>
        /// 压缩标准照片
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="foler"></param>
        /// <param name="folderToZip"></param>
        /// <param name="zipedFile"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ZipInForFileResutl(List<AnswerDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (AnswerDto answer in fileNames)
                    {
                        try
                        {

                        List<FileResultDto> fileResultList = new List<FileResultDto>();
                        if (!string.IsNullOrEmpty(answer.FileResult))
                        {
                            List<FileResultDto> fileResultList_temp = CommonHelper.DecodeString<List<FileResultDto>>(answer.FileResult);
                            foreach (FileResultDto fileresult in fileResultList_temp)
                            {
                                if (!string.IsNullOrEmpty(fileresult.Url))
                                {
                                    fileResultList.Add(fileresult);
                                }
                            }
                        }
                        foreach (FileResultDto photo in fileResultList)
                        {
                            try
                            {
                                // 获取文件夹信息
                                string folder1 = GetFolderName(answer.ProjectId.ToString(), "1", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                                string folder2 = GetFolderName(answer.ProjectId.ToString(), "2", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                                string folder3 = GetFolderName(answer.ProjectId.ToString(), "3", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                                string folder4 = GetFolderName(answer.ProjectId.ToString(), "4", answer.ShopCode, answer.ShopName, answer.SubjectCode, photo.FileName, photo.SeqNO.ToString(), photo.ModifyDateTime.ToString("yyyyMMddHHmmssfff"), "1", answer.OrderNO.ToString(), "");
                                string photoName = "";
                                // 下载文件的名称为标准照片名称
                                if (!string.IsNullOrEmpty(photo.Url))
                                {
                                    string[] url = photo.Url.Split(';');
                                    for (int i = 0; i < url.Length; i++)
                                    {
                                        if (url.Length == 1)
                                        {
                                            photoName = (folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4 + ".jpg").Replace("\\", @"\");
                                        }
                                        else
                                        {
                                            photoName = (folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4).Replace("\\", @"\") + "_" + (i + 1).ToString() + ".jpg";
                                        }
                                        // }
                                        string file = Path.Combine(folderToZip, foler, photoName);
                                        string extension = string.Empty;
                                        if (!System.IO.File.Exists(file))
                                        {
                                            comment += foler + "，文件：" + photoName + "不存在。\r\n";
                                            continue;
                                        }
                                        using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, photoName)))
                                        {
                                            byte[] buffer = new byte[fs.Length];
                                            fs.Read(buffer, 0, buffer.Length);
                                            entry = new ZipEntry(foler + "/" + photoName);
                                            entry.DateTime = DateTime.Now;
                                            entry.Size = fs.Length;
                                            fs.Close();
                                            crc.Reset();
                                            crc.Update(buffer);
                                            entry.Crc = crc.Value;
                                            zipOutStream.PutNextEntry(entry);
                                            zipOutStream.Write(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                        catch (Exception eex)
            {

                continue; ;
            }
        }
                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }
            }
            catch (Exception ex)
            {
               // CommonHelper.log("11111" + ex.Message.ToString() + "  " + ex.InnerException);
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion
        #region 下载失分照片
        public string LossResultDownLoad(string projectId, string shopId)
        {
            List<AnswerDto> list = answerService.GetShopAnswerScoreInfo(projectId, shopId, "", "");
            if (list == null || list.Count == 0) return "";
            basePath = basePath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            #region 从OSS把文件下载到服务器
           // string answerId = "";
            foreach (AnswerDto answer in list)
            {
                try
                {
                   // answerId = answer.AnswerId.ToString();
                    List<LossResultDto> lossResult = new List<LossResultDto>();
                    if (!string.IsNullOrEmpty(answer.LossResult))
                    {
                        List<LossResultDto> lossResult_Temp = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                        foreach (LossResultDto lossresult in lossResult_Temp)
                        {
                            if (!string.IsNullOrEmpty(lossresult.LossFileNameUrl))
                            {
                                lossResult.Add(lossresult);
                            }
                        }
                    }
                    foreach (LossResultDto loss in lossResult)
                    {
                        string lossResultStrCode = "";
                        if (!string.IsNullOrEmpty(loss.LossDesc))
                        {
                            lossResultStrCode = masterService.GetSubjectLossCodeByAnswerLossName(projectId.ToString(), answer.SubjectId.ToString(), loss.LossDesc);
                        }
                        // 获取文件夹信息
                        string folder1 = GetFolderName(answer.ProjectId.ToString(), "1", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "2", "", "");
                        string folder2 = GetFolderName(answer.ProjectId.ToString(), "2", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "2", "", "");
                        string folder3 = GetFolderName(answer.ProjectId.ToString(), "3", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "2", "", "");
                        string folder4 = GetFolderName(answer.ProjectId.ToString(), "4", answer.ShopCode, answer.ShopName, answer.SubjectCode, loss.LossDesc, loss.LossId.ToString(), loss.LastTime.ToString("yyyyMMddHHmmssfff"), "2", answer.OrderNO.ToString(), lossResultStrCode);
                        if (!string.IsNullOrEmpty(folder4))
                        {
                            // 创建1级目录
                            if (!string.IsNullOrEmpty(folder1))
                            {
                                if (!Directory.Exists(folder + @"\" + folder1))
                                {
                                    Directory.CreateDirectory(folder + @"\" + folder1);
                                }
                            }  // 创建2级目录
                            if (!string.IsNullOrEmpty(folder2))
                            {
                                if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2))
                                {
                                    Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2);
                                }
                            }
                            // 创建3级目录
                            if (!string.IsNullOrEmpty(folder3))
                            {
                                if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3))
                                {
                                    Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3);
                                }
                            }
                            try
                            {
                                // 下载文件的名称为失分照片名称
                                if (!string.IsNullOrEmpty(loss.LossFileNameUrl))
                                {
                                    string[] url = loss.LossFileNameUrl.Split(';');
                                    for (int i = 0; i < url.Length; i++)
                                    {
                                        string urlstr = url[i].ToString();
                                        if (url.Length == 1)
                                        {
                                            // 已有的文件先删除
                                            string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4 + ".jpg").Replace("\\", @"\");
                                            if (File.Exists(filePath))
                                            {
                                                File.Delete(filePath);
                                            }
                                            OSSClientHelper.GetObject(urlstr, filePath);
                                        }
                                        else
                                        {
                                            // 已有的文件先删除
                                            string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4).Replace("\\", @"\");
                                            if (File.Exists(filePath + "_" + (i + 1).ToString() + ".jpg"))
                                            {
                                                File.Delete(filePath + "_" + (i + 1).ToString() + ".jpg");
                                            }
                                            OSSClientHelper.GetObject(urlstr, filePath + "_" + (i + 1).ToString() + ".jpg");
                                        }
                                    }
                                }
                            }
                            catch (Exception) { }
                        }

                    }
                }
                catch (Exception eex)
                {
                    //string msg = eex.Message + "_" + answerId;
                    continue;
                }
            }
            #endregion
            // 压缩文件，压缩失败返回空
            if (!ZipInForLossResutl(list, downLoadfolder, basePath, downLoadPath, 9))
            {
                return "";
            }
            else // 压缩成功后上传到OSS
            {
                return OSSClientHelper.PutObjectMultipart("DownTempFile" + @"/" + downLoadfolder + ".zip", downLoadPath);
            }
            // return downLoadPath.Replace(HostingEnvironment.MapPath(@"~/"), "");
        }
        /// <summary>
        /// 压缩失分照片
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="foler"></param>
        /// <param name="folderToZip"></param>
        /// <param name="zipedFile"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ZipInForLossResutl(List<AnswerDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (AnswerDto answer in fileNames)
                    {
                        try
                        {
                            List<LossResultDto> lossResultList = new List<LossResultDto>();
                            if (!string.IsNullOrEmpty(answer.LossResult))
                            {
                                List<LossResultDto> lossResult_Temp = CommonHelper.DecodeString<List<LossResultDto>>(answer.LossResult);
                                foreach (LossResultDto lossresult in lossResult_Temp)
                                {
                                    if (!string.IsNullOrEmpty(lossresult.LossFileNameUrl))
                                    {
                                        lossResultList.Add(lossresult);
                                    }
                                }
                            }
                            foreach (LossResultDto photo in lossResultList)
                            {
                                string lossResultStrCode = "";
                                if (!string.IsNullOrEmpty(photo.LossDesc))
                                {
                                    lossResultStrCode = masterService.GetSubjectLossCodeByAnswerLossName(answer.ProjectId.ToString(), answer.SubjectId.ToString(), photo.LossDesc);
                                }
                                // 获取文件夹信息
                                string folder1 = GetFolderName(answer.ProjectId.ToString(), "1", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "2", "", "");
                                string folder2 = GetFolderName(answer.ProjectId.ToString(), "2", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "2", "", "");
                                string folder3 = GetFolderName(answer.ProjectId.ToString(), "3", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "2", "", "");
                                string folder4 = GetFolderName(answer.ProjectId.ToString(), "4", answer.ShopCode, answer.ShopName, answer.SubjectCode, photo.LossDesc, photo.LossId.ToString(), photo.LastTime.ToString("yyyyMMddHHmmssfff"), "2", answer.OrderNO.ToString(), lossResultStrCode);
                                string photoName = "";
                                // 下载文件的名称为失分照片名称
                                if (!string.IsNullOrEmpty(photo.LossFileNameUrl))
                                {
                                    string[] url = photo.LossFileNameUrl.Split(';');
                                    for (int i = 0; i < url.Length; i++)
                                    {
                                        if (url.Length == 1)
                                        {
                                            photoName = (folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4 + ".jpg").Replace("\\", @"\");
                                        }
                                        else
                                        {
                                            photoName = (folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4).Replace("\\", @"\") + "_" + (i + 1).ToString() + ".jpg";
                                        }
                                        // }
                                        string file = Path.Combine(folderToZip, foler, photoName);
                                        string extension = string.Empty;
                                        if (!System.IO.File.Exists(file))
                                        {
                                            comment += foler + "，文件：" + photoName + "不存在。\r\n";
                                            continue;
                                        }
                                        using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, photoName)))
                                        {
                                            byte[] buffer = new byte[fs.Length];
                                            fs.Read(buffer, 0, buffer.Length);
                                            entry = new ZipEntry(foler + "/" + photoName);
                                            entry.DateTime = DateTime.Now;
                                            entry.Size = fs.Length;
                                            fs.Close();
                                            crc.Reset();
                                            crc.Update(buffer);
                                            entry.Crc = crc.Value;
                                            zipOutStream.PutNextEntry(entry);
                                            zipOutStream.Write(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception eex)
                        {

                            continue; ;
                        }
                    }
                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }

            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion
        #region 照片下载重命名
        public string GetFolderName(string projectId, string fileTypeCode, string shopCode, string shopName, string subectCode, string photoName, string photoOrder, string photoTime, string photoType, string subjectOrder, string lossResultCode)
        {
            string folderName = "";
            List<FileRenameDto> fileRenameList_Folder = masterService.GetFileRename(projectId, fileTypeCode, photoType);
            // 设置了命名规则
            if (fileRenameList_Folder != null && fileRenameList_Folder.Count > 0)
            {
                for (int i = 0; i < fileRenameList_Folder.Count; i++)
                {
                    if (fileRenameList_Folder[i].SeqNO == i + 1)
                    {
                        if (fileRenameList_Folder[i].OptionCode == "ProjectCode")
                        {
                            folderName += fileRenameList_Folder[i].ProjectCode + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "ProjectName")
                        {
                            folderName += fileRenameList_Folder[i].ProjectName + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "ShopCode")
                        {
                            folderName += shopCode + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "ShopName")
                        {
                            folderName += shopName + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "SubjectCode")
                        {
                            folderName += subectCode + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "SubjectOrder")
                        {
                            string order = "";
                            if (subjectOrder.Length == 1)
                            {
                                order = "00" + subjectOrder;
                            }
                            else if (subjectOrder.Length == 2)
                            {
                                order = "0" + subjectOrder;
                            }
                            else
                            {
                                order = subjectOrder;
                            }
                            folderName += order + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "PhotoName")
                        {
                            folderName += photoName + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "PhotoOrder")
                        {
                            folderName += photoOrder + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "PhotoTime")
                        {
                            folderName += photoTime + fileRenameList_Folder[i].ConnectStr;
                        }
                        if (fileRenameList_Folder[i].OptionCode == "LossResultCode")
                        {
                            folderName += lossResultCode + fileRenameList_Folder[i].ConnectStr;
                        }
                        //if (fileRenameList_Folder[i].OptionCode == "CheckPoint")
                        //{
                        //    folderName += checkPoint + fileRenameList_Folder[i].ConnectStr;
                        //}
                        if (fileRenameList_Folder[i].OptionCode == "Other")
                        {
                            folderName += fileRenameList_Folder[i].OtherName + fileRenameList_Folder[i].ConnectStr;
                        }
                    }
                }
            }
            else // 未设置命名规则
            {
                if (fileTypeCode == "1")
                {
                    return folderName = projectId;
                }
                if (fileTypeCode == "2" || fileTypeCode == "3")
                {
                    folderName = shopCode;
                }
                if (fileTypeCode == "4")
                {
                    folderName = shopCode + "_" + shopName + "_" + subectCode + "_" + photoName;
                }
            }
            return CommonHelper.ReplaceBadCharOfFileName(folderName);
        }
        #endregion
        #region 照片上传日志
        public void SaveAnswerPhotoLog(AnswerPhotoLog answerPhotoLog)
        {
            // CommonHelper.log("ProjectId:" + answerPhotoLog.ProjectId.ToString() + "ShopId:" + answerPhotoLog.ShopId.ToString() + "URL" + answerPhotoLog.FileUrl);
            answerPhotoLog.InDateTime = DateTime.Now;
            db.AnswerPhotoLog.Add(answerPhotoLog);
            db.SaveChanges();
        }
        public List<AnswerPhotoLog> GetAnswerPhoto(string projectId, string shopId)
        {
            if (projectId == null) projectId = "";
            if (shopId == null) shopId = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@ShopId", shopId)};
            Type t = typeof(AnswerPhotoLog);
            string sql = @"SELECT A.*
                         FROM AnswerPhotoLog A
                        WHERE A.ProjectId = @ProjectId ";
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerPhotoLog>().ToList();
        }
        #endregion
        #region 删除缓存的文件
        public void DeleteCacheFile(string path, int day)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos(); //返回目录中所有文件和子目录
            foreach (FileSystemInfo file in fileInfo)
            {
                if (file.LastAccessTime.Date == DateTime.Now.AddDays(day).Date)
                {
                    if (file is DirectoryInfo)
                    {
                        DirectoryInfo subdir = new DirectoryInfo(file.FullName);
                        subdir.Delete(true); //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(file.FullName);
                    }
                }
            }
        }
        #endregion
        #region 下载申诉附件
        public string AppealFileDownLoad(string projectId, string shopId)
        {
            List<AppealDto> list = appealService.GetFeedBackInfoByAll(projectId, "", "", "", "");
            if (list == null || list.Count == 0) return "";
            basePath = basePath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (AppealDto answer in list)
            {
                // 下载申诉照片
                List<AppealFileDto> fileResultList = new List<AppealFileDto>();

                fileResultList = appealService.AppealFileSearch(answer.AppealId.ToString(), "");
                foreach (AppealFileDto fileResult in fileResultList)
                {
                    // 获取文件夹信息
                    string folder1 = answer.ShopName;
                    string folder2 = answer.SubjectCode;

                    if (!string.IsNullOrEmpty(fileResult.FileName))
                    {
                        // 创建1级目录
                        if (!string.IsNullOrEmpty(folder1))
                        {
                            if (!Directory.Exists(folder + @"\" + folder1))
                            {
                                Directory.CreateDirectory(folder + @"\" + folder1);
                            }
                        }  // 创建2级目录
                        if (!string.IsNullOrEmpty(folder2))
                        {
                            if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2))
                            {
                                Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2);
                            }
                        }
                        try
                        {
                            string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + fileResult.FileName).Replace("\\", @"\");
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            OSSClientHelper.GetObject(fileResult.ServerFileName, filePath);
                        }
                        catch (Exception ex) { }
                    }

                }
            }
            // 压缩文件，如果失败返回空
            if (!ZipInAppealFile(list, downLoadfolder, basePath, downLoadPath, 9))
            {
                return "";
            }
            else
            { // 压缩成功上传到OSS
                return OSSClientHelper.PutObjectMultipart("DownTempFile" + @"/" + downLoadfolder + ".zip", downLoadPath);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="foler"></param>
        /// <param name="folderToZip"></param>
        /// <param name="zipedFile"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ZipInAppealFile(List<AppealDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (AppealDto answer in fileNames)
                    {
                        List<AppealFileDto> fileResultList = new List<AppealFileDto>();
                        fileResultList = appealService.AppealFileSearch(answer.AppealId.ToString(), "");
                        foreach (AppealFileDto photo in fileResultList)
                        {
                            try
                            {
                                // 获取文件夹信息
                                string folder1 = answer.ShopName;
                                string folder2 = answer.SubjectCode;
                                // string folder3 = GetFolderName(answer.ProjectId.ToString(), "3", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                                //string folder4 = GetFolderName(answer.ProjectId.ToString(), "4", answer.ShopCode, answer.ShopName, answer.SubjectCode, photo.FileName, photo.SeqNO.ToString(), photo.ModifyDateTime.ToString("yyyyMMddHHmmssfff"), "1", answer.OrderNO.ToString(), "");
                                string photoName = "";
                                // 下载文件的名称为标准照片名称
                                if (!string.IsNullOrEmpty(photo.FileName))
                                {
                                    photoName = (folder1 + @"\" + folder2 + @"\" + photo.FileName).Replace("\\", @"\");
                                    string file = Path.Combine(folderToZip, foler, photoName);
                                    string extension = string.Empty;
                                    if (!System.IO.File.Exists(file))
                                    {
                                        comment += foler + "，文件：" + photoName + "不存在。\r\n";
                                        continue;
                                    }
                                    using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, photoName)))
                                    {
                                        byte[] buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, buffer.Length);
                                        entry = new ZipEntry(foler + "/" + photoName);
                                        entry.DateTime = DateTime.Now;
                                        entry.Size = fs.Length;
                                        fs.Close();
                                        crc.Reset();
                                        crc.Update(buffer);
                                        entry.Crc = crc.Value;
                                        zipOutStream.PutNextEntry(entry);
                                        zipOutStream.Write(buffer, 0, buffer.Length);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }
            }
            catch (Exception ex)
            {
                CommonHelper.log("11111" + ex.Message.ToString() + "  " + ex.InnerException);
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion
        #region 下载进店信息附件
        public string AnswerShopInfoFileDownLoad(string projectId, string shopId)
        {
            List<AnswerShopInfoDto> list = answerService.GetAnswerShopInfo(projectId, shopId, "");
            if (list == null || list.Count == 0) return "";
            basePath = basePath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (AnswerShopInfoDto answer in list)
            {
                // 下载进店信息附件
                List<AnswerShopInfoFileDto> fileResultList = new List<AnswerShopInfoFileDto>();

                fileResultList = answerService.AnswerShopInfoFileSearch(answer.Id.ToString(), "");
                foreach (AnswerShopInfoFileDto fileResult in fileResultList)
                {
                    // 获取文件夹信息
                    string folder1 = answer.ProjectName;
                    string folder2 = answer.ShopName;

                    if (!string.IsNullOrEmpty(fileResult.FileName))
                    {
                        // 创建1级目录
                        if (!string.IsNullOrEmpty(folder1))
                        {
                            if (!Directory.Exists(folder + @"\" + folder1))
                            {
                                Directory.CreateDirectory(folder + @"\" + folder1);
                            }
                        }  // 创建2级目录
                        if (!string.IsNullOrEmpty(folder2))
                        {
                            if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2))
                            {
                                Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2);
                            }
                        }
                        try
                        {
                            string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + fileResult.FileName).Replace("\\", @"\");
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            OSSClientHelper.GetObject(fileResult.ServerFileName, filePath);
                        }
                        catch (Exception ex) { }
                    }

                }
            }
            // 压缩文件，如果失败返回空
            if (!ZipInShopInfoFile(list, downLoadfolder, basePath, downLoadPath, 9))
            {
                return "";
            }
            else
            { // 压缩成功上传到OSS
                return OSSClientHelper.PutObjectMultipart("DownTempFile" + @"/" + downLoadfolder + ".zip", downLoadPath);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="foler"></param>
        /// <param name="folderToZip"></param>
        /// <param name="zipedFile"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ZipInShopInfoFile(List<AnswerShopInfoDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (AnswerShopInfoDto answer in fileNames)
                    {
                        List<AnswerShopInfoFileDto> fileResultList = new List<AnswerShopInfoFileDto>();
                        fileResultList = answerService.AnswerShopInfoFileSearch(answer.Id.ToString(), "");
                        foreach (AnswerShopInfoFileDto photo in fileResultList)
                        {
                            try
                            {
                                // 获取文件夹信息
                                string folder1 = answer.ProjectName;
                                string folder2 = answer.ShopName;
                                // string folder3 = GetFolderName(answer.ProjectId.ToString(), "3", answer.ShopCode, answer.ShopName, answer.SubjectCode, "", "", "", "1", "", "");
                                //string folder4 = GetFolderName(answer.ProjectId.ToString(), "4", answer.ShopCode, answer.ShopName, answer.SubjectCode, photo.FileName, photo.SeqNO.ToString(), photo.ModifyDateTime.ToString("yyyyMMddHHmmssfff"), "1", answer.OrderNO.ToString(), "");
                                string photoName = "";
                                // 下载文件的名称为标准照片名称
                                if (!string.IsNullOrEmpty(photo.FileName))
                                {
                                    photoName = (folder1 + @"\" + folder2 + @"\" + photo.FileName).Replace("\\", @"\");
                                    string file = Path.Combine(folderToZip, foler, photoName);
                                    string extension = string.Empty;
                                    if (!System.IO.File.Exists(file))
                                    {
                                        comment += foler + "，文件：" + photoName + "不存在。\r\n";
                                        continue;
                                    }
                                    using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, photoName)))
                                    {
                                        byte[] buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, buffer.Length);
                                        entry = new ZipEntry(foler + "/" + photoName);
                                        entry.DateTime = DateTime.Now;
                                        entry.Size = fs.Length;
                                        fs.Close();
                                        crc.Reset();
                                        crc.Update(buffer);
                                        entry.Crc = crc.Value;
                                        zipOutStream.PutNextEntry(entry);
                                        zipOutStream.Write(buffer, 0, buffer.Length);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }
            }
            catch (Exception ex)
            {
                CommonHelper.log("11111" + ex.Message.ToString() + "  " + ex.InnerException);
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion
    }
}