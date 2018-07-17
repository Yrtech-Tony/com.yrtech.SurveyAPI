using Aliyun.OSS;
using Aliyun.OSS.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace com.yrtech.SurveyAPI.Common
{
    public class OSSClientHelper
    {
        protected const string accessid = "LTAIVT0LDGaF40dk";
        protected const string accessKey = "2Oqit9KkBDNHFGBZZEjMlknxslIcuM";
        protected const string endpoin = "http://oss-cn-beijing.aliyuncs.com";
        protected const string bucket = "yrsurvey";

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
                var result = ossClient.PutObject(bucket, key, fileStream, objectMetadata);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}