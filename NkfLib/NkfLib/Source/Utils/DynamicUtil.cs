using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text;

namespace NkfLib
{
    /// <summary>
    /// dynamic型のユーティリティ
    /// ・dynamic型の拡張メソッドは作れない
    /// </summary>
    public static partial class DynamicUtil
    {
        /// <summary>
        /// フィールド・プロパティ有無チェック
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValueExist(dynamic instance, string name)
        {
            if (instance is IDictionary<string, object> dic) // ExpandoObjectなど
                return dic.ContainsKey(name);

            var obj = (object)instance;
            var type = obj.GetType();
            return type.GetProperty(name) != null || type.GetField(name) != null;
        }

        /// <summary>
        /// フィールド、プロパティの取得
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryGetValue(dynamic instance, string name, out object value)
        {
            if (instance is IDictionary<string, object> dic) // ExpandoObjectなど
                return dic.TryGetValue(name, out value);

            var obj = (object)instance;
            var type = obj.GetType();
            var fieldInfo = type.GetField(name);
            if (fieldInfo != null)
            {
                value = fieldInfo.GetValue(obj);
                return true;
            }

            var propertyInfo = type.GetProperty(name);
            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(obj);
                return true;
            }

            value = default;
            return false;
        }
    }
}
