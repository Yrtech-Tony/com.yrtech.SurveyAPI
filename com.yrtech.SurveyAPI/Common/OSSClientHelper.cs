using Aliyun.OSS;
using Aliyun.OSS.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace com.yrtech.SurveyAPI.Common
{
    public class OSSClientHelper
    {
        protected const string accessid = "LTAI4FknXd6u5KvkU9EGgoxP";
        protected const string accessKey = "RtWE4s9G0dNFCPDcaNvs5k4arOMHCo";
        protected const string endpoin = "http://oss-cn-beijing-internal.aliyuncs.com";
       
       // protected const string endpoin = "http://oss-cn-beijing.aliyuncs.com";
        //protected const string bucket = WebConfigurationManager.AppSettings["OSSBucket"];

        public static bool UploadOSSFile(string key, Stream fileStream,long length)
        {
            try
            {
                string md5 = OssUtils.ComputeContentMd5(fileStream, length);
                //创建上传Object的Metadata 
                ObjectMetadata objectMetadata = new ObjectMetadata() {
                    ContentMd5 = md5
                };

                OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
                var result = ossClient.PutObject(WebConfigurationManager.AppSettings["OSSBucket"], key, fileStream, objectMetadata);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
        public static void PutObjectMultipart(string bucketName, string key, string fileToUpload)
        {
            OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
            var partSize = 1000 * 1000;
            var initRequest = new InitiateMultipartUploadRequest(bucketName, key);
            var uploadId = ossClient.InitiateMultipartUpload(initRequest);
            var fi = new FileInfo(fileToUpload);
            var fileSize = fi.Length;
            var parCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                parCount++;
            }

            var partEtags = new List<PartETag>();
            for (var i = 0; i < parCount; i++)
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var skipBytes = (long)partSize * i;
                    fs.Seek(skipBytes, 0);
                    var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                    var uploadPartRequest = new UploadPartRequest(bucketName, key, uploadId.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = i + 1
                    };
                    var upLoadPartRequestResult = ossClient.UploadPart(uploadPartRequest);
                    partEtags.Add(upLoadPartRequestResult.PartETag);

                }
            }
            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(bucketName, key, uploadId.UploadId);
            foreach (var partETag in partEtags)
            {
                completeMultipartUploadRequest.PartETags.Add(partETag);
            }
            var completeResult = ossClient.CompleteMultipartUpload(completeMultipartUploadRequest);
        }
        public static void GetObject(string key, string fileToDownload)
        {
            OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
            var o = ossClient.GetObject(WebConfigurationManager.AppSettings["OSSBucket"], key);
            using (var requestStream = o.Content)
            {
                byte[] buf = new byte[1024];
                var fs = File.Open(fileToDownload, FileMode.OpenOrCreate);
                var len = 0;
                while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                {
                    fs.Write(buf, 0, len);
                }
                fs.Close();
            }
        }

        public static void DeleteObject(string key)
        {
            OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
            ossClient.DeleteObject(WebConfigurationManager.AppSettings["OSSBucket"], key);
        }
    }
}