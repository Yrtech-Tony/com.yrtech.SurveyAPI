using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.yrtech.SurveyAPI.Common
{
    public class CommonHelper
    {
        static JsonSerializerSettings defaultJsonSetting = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };
        public static string Encode(object obj)
        {
            string jsonString = string.Empty;
            if (obj == null)
            {
                return jsonString;
            }
            jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, defaultJsonSetting);
            return jsonString;
        }

        public static string EncodeDto<T>(IEnumerable t)
        {
            string jsonString = string.Empty;
            if (t == null)
            {
                return jsonString;
            }
            jsonString = JsonConvert.SerializeObject(t, Formatting.Indented, defaultJsonSetting);
            return jsonString;
        }
        public static string EncodeDto<T>(T t)
        {
            string jsonString = string.Empty;
            if (t == null)
            {
                return jsonString;
            }
            jsonString = JsonConvert.SerializeObject(t, Formatting.Indented, defaultJsonSetting);
            return jsonString;
        }
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            //序列化对象
            xml.Serialize(Stream, obj);
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream, Encoding.Unicode);
            string str = sr.ReadToEnd();
            sr.Dispose();
            Stream.Dispose();
            str = str.Replace("utf-8", "utf-16");
            return str;
        }
        public static T DecodeString<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                CommonHelper.log("反序列化json错误!" + ex.ToString());
                CommonHelper.log("错误json=" + json);
                return default(T);
            }
        }
        public static string FormatterString(string str, int length, bool sign)
        {
            decimal data;
            string strData = "-";
            if (decimal.TryParse(str, out data))
            {
                if (length == 1)
                {
                    strData = String.Format("{0:N1}", data);
                }
                else if (length == 2)
                {
                    strData = String.Format("{0:N2}", data);
                }
                else
                {
                    strData = Convert.ToInt32(data).ToString("N0");
                }
                if (sign)
                {
                    strData = strData + "%";
                }
            }
            return strData;
        }
        public static void log(string message)
        {
            string appDomainPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = appDomainPath + @"\" + "Log" + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + DateTime.Now.ToString("yyyyMMddHHmmsss") + ".txt";
            //File.Create(fileName);
            if (!Directory.Exists(appDomainPath + @"\" + "Log"))
            {
                Directory.CreateDirectory(appDomainPath + @"\" + "Log");
            }
            if (!Directory.Exists(appDomainPath + @"\" + "Log" + @"\" + DateTime.Now.ToString("yyyyMMdd")))
            {
                Directory.CreateDirectory(appDomainPath + @"\" + "Log" + @"\" + DateTime.Now.ToString("yyyyMMdd"));
            }
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                byte[] by = WriteStringToByte(message, fs);
                fs.Flush();
            }
        }
        public static byte[] WriteStringToByte(string str, FileStream fs)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(str);
            fs.Write(info, 0, info.Length);
            return info;
        }
        // 文件名中非法字符替换
        public static string ReplaceBadCharOfFileName(string fileName)
        {
            string str = fileName;
            str = str.Replace("\\", string.Empty);
            str = str.Replace("/", string.Empty);
            str = str.Replace(":", string.Empty);
            str = str.Replace("*", string.Empty);
            str = str.Replace("?", string.Empty);
            str = str.Replace("\"", string.Empty);
            str = str.Replace("<", string.Empty);
            str = str.Replace(">", string.Empty);
            str = str.Replace("|", string.Empty);
            str = str.Replace(" ", string.Empty);    //前⾯的替换会产⽣空格,最后将其⼀并替换掉
            return str;
        }

    }
}
