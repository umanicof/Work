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
    /// 辞書形式を介したXMLシリアライザ
    /// ・インスタンス・静的クラスのシリアライズが可能
    /// ・フィールドのみシリアライズ（プロパティはシリアライズしない）
    /// ・シリアライズ対象から除外する場合は、XmlIgnoreAttribute を指定する
    /// ・ReactivePropertyなどはシリアライズできない
    /// </summary>
    public static partial class DictionaryXmlSerializer
    {
        /// <summary>
        /// フィールドリストをインスタンスor静的クラスから取得
        /// ・シリアライズの前処理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">対象のインスタンス（nullなら対象は静的クラス）</param>
        /// <returns></returns>
        public static DictionaryEntry[] GetFields<T>(T instance = null)
            where T : class
        {
            bool isStatic = instance == null;
            var fields = typeof(T).GetFields((isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Where(x => !x.IsDefined(typeof(XmlIgnoreAttribute), true));
            var entries = new DictionaryEntry[fields.Count()];
            int i = 0;
            foreach (FieldInfo info in fields)
            {
                entries[i++] = new DictionaryEntry(info.Name, info.GetValue(instance));
            }
            return entries;
        }

        /// <summary>
        /// フィールドリストをインスタンスor静的クラスに設定
        /// ・デシリアライズの後処理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">対象のインスタンス（nullなら対象は静的クラス）</param>
        /// <returns></returns>
        public static void SetFields<T>(DictionaryEntry[] entries, T instance = null)
            where T : class
        {
            bool isStatic = instance == null;
            var fields = typeof(T).GetFields((isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Where(x => !x.IsDefined(typeof(XmlIgnoreAttribute), true));                                    

            foreach (FieldInfo info in fields)
            {
                var entry = entries.Where(x => x.Key as string == info.Name).FirstOrDefault();
                info.SetValue(instance, entry.Value);
            }            
        }
        
        /// <summary>
        /// インスタンスor静的クラスをXMLにシリアライズしてファイルに保存
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="instance">対象のインスタンス（nullなら対象は静的クラス）</param>
        /// <returns></returns>
        public static bool Save<T>(string filePath, T instance = null)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(DictionaryEntry[]));
                using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false))) { // UTF-8 BOM無し
                    var entries = GetFields<T>(instance);
                    serializer.Serialize(sw, entries);
                }
                return true;
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// ファイルをロードしてXMLからインスタンスor静的クラスをデシリアライズ
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="instance">対象のインスタンス（nullなら対象は静的クラス）</param>
        /// <returns></returns>
        public static bool Load<T>(string filePath, T instance = null)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(DictionaryEntry[]));
                using (StreamReader sr = new StreamReader(filePath, new UTF8Encoding(false))) { 
                    var entries = serializer.Deserialize(sr) as DictionaryEntry[];
                    SetFields<T>(entries, instance);
                }
                return true;
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// インスタンスor静的クラスをXMLにシリアライズして文字列を返却
        /// </summary>
        /// <param name="instance">対象のインスタンス（nullなら対象は静的クラス）</param>
        /// <returns></returns>
        public static string Serialize<T>(T instance = null)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(DictionaryEntry[]));
                using (var sw = new StringWriter()) { 
                    var entries = GetFields<T>(instance);
                    serializer.Serialize(sw, entries);
                    return sw.ToString();
                }
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// XMLの文字列からインスタンスor静的クラスをデシリアライズ
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="instance">対象のインスタンス（nullなら対象は静的クラス）</param>
        /// <returns></returns>
        public static void Deserialize<T>(string xml, T instance = null)
            where T : class
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(DictionaryEntry[]));
                using (var sr = new StringReader(xml)) { 
                    var entries = serializer.Deserialize(sr) as DictionaryEntry[];
                    SetFields<T>(entries, instance);
                }
            }
            catch (Exception e) {
                DebugLog.WriteLine(e);
            }
        }
    }
}
