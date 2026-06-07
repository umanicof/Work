using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace NkfLib
{
    // <summary>
    /// Enum型の拡張メソッド（またはユーティリティ）を管理するクラス
    /// </summary>
    public static partial class EnumExtention
    {
        /// <summary>
        /// Description属性のリストを取得（ユーティリティ）
        /// </summary>
        /// <typeparam name="T">Enumの型</typeparam>
        /// <param name="self"></param>
        public static IEnumerable<string> GetDescriptions<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            var descriptions = new List<string>();
            foreach (var e in typeof(T).GetFields()) {
                var attr = Attribute.GetCustomAttribute(e, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attr != null) {
                    descriptions.Add(attr.Description);
                }
            }
            return descriptions;
        }

        /// <summary>
        /// 属性を取得
        /// This extension method is broken out so you can use a similar pattern with 
        /// other MetaData elements in the future. This is your base method for each.
        /// </summary>
        /// <param name="self"></param>
        public static T GetAttribute<T>(this Enum self) where T : Attribute
        {
            var fieldInfo = self.GetType().GetField(self.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(T), false);
            return attributes.FirstOrDefault() as T;
        }

        /// <summary>
        /// Description属性を取得
        /// This method creates a specific call to the above method, requesting the
        /// Description MetaData attribute.
        /// </summary>
        /// <param name="self"></param>
        public static string GetDescription(this Enum self)
        {
            var attr = self.GetAttribute<DescriptionAttribute>();
            return attr?.Description;
        }
    };

}
