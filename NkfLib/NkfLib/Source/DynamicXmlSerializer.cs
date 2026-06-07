using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Collections;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// 動的クラスのXMLシリアライザ
    /// ・動的クラスのみシリアライズが可能
    /// ・アトリビュートの指定はXmlSerializerのものを使用する
    /// ・ReactivePropertyなどもシリアライズできる
    /// </summary>
    public static class DynamicXmlSerializer
    {
        /// <summary>
        /// 動的クラスをXMLにシリアライズしてファイルに保存
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Save<T>(string filePath, T instance)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false))) { // UTF-8 BOM無し
                    serializer.Serialize(sw, instance);
                }
                return true;
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// ファイルをロードしてXMLから動的クラスをデシリアライズ
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T Load<T>(string filePath)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamReader sr = new StreamReader(filePath, new UTF8Encoding(false))) { 
                     return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// 動的クラスをXMLにシリアライズして文字列を返却
        /// </summary>
        /// <returns></returns>
        public static string Serialize<T>(T instance)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var sw = new StringWriter()) {
                    serializer.Serialize(sw, instance);
                    return sw.ToString();
                }
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// XMLの文字列から動的クラスをデシリアライズ
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var sr = new StringReader(xml)) { 
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return null;
            }
        }
    }
}
