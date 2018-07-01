using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.yrtech.SurveyAPI.Common
{
    public class CommonHelper
    {
        public static string EncodeDto<T>(IEnumerable t)
        {
            string jsonString = string.Empty;

            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };
                jsonString = JsonConvert.SerializeObject(t);
            }
            catch (Exception)
            {
                return "";
            }
            return jsonString;
        }
        public static string EncodeDto<T>(T t)
        {
            string jsonString = string.Empty;

            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };
                jsonString = JsonConvert.SerializeObject(t);
            }
            catch (Exception)
            {
            }
            return jsonString;
        }
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream, Encoding.Unicode);
            string str = sr.ReadToEnd();
            sr.Dispose();
            Stream.Dispose();
            str = str.Replace("utf-8", "utf-16");
            return str;
        }

        public static string FormatterString(string str, int length,bool sign)
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
                    strData =Convert.ToInt32(data).ToString("N0");
                }
                if (sign)
                {
                    strData= strData + "%";
                }
            }
            return strData;
        }
    }
}
