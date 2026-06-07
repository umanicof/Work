using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Diagnostics;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_WEBGL
using UniRx;
#else
using Reactive.Bindings;
#endif

namespace NkfLib
{
    // <summary>
    /// object型の拡張メソッドを管理するクラス
    /// 
    /// 型引数（T）を指定するメソッドと、objectを引数とするメソッドの２通りあるが、これらは挙動が異なるはず。
    /// 型引数を指定した場合は指定された型に対する操作がなされ、objectの場合はGetType()で取得された型に対する操作がされる。
    /// </summary>
    public static partial class ObjectExtensions
    {
        const string kSeparator = ",";       // 区切り記号として使用する文字列

        /// <summary>
        /// すべての公開フィールドの情報を文字列にして返します（デバッグ用）
        /// </summary>
        public static string ToDebugStringFields<T>(this T self)
        {
            return ToDebugStringFields(typeof(T), self);
        }
        public static string ToDebugStringFields(this object self)
        {
            return ToDebugStringFields(self.GetType(), self);
        }
        static string ToDebugStringFields(Type type, object obj)
        {
            if (obj == null)
                return "null";
            return string.Join(kSeparator, type
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Select(c => $"{c.Name}:{c.GetValue(obj)}"));
        }

        /// <summary>
        /// すべての公開プロパティの情報を文字列にして返します（デバッグ用）
        /// </summary>
        public static string ToDebugStringProperties<T>(this T self)
        {
            return ToDebugStringProperties(typeof(T), self);
        }
        public static string ToDebugStringProperties(this object self)
        {
            return ToDebugStringProperties(self.GetType(), self);
        }
        static string ToDebugStringProperties(Type type, object obj)
        {
            if (obj == null)
                return "null";
            try {
                return string.Join(kSeparator, type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(c => c.CanRead)
                    .Select(c => $"{c.Name}:{c.GetValue(obj, null)}"));
            }
            catch {
                return "bad format";
            }
        }

        /// <summary>
        /// すべての公開フィールドと公開プロパティの情報を文字列にして返します（デバッグ用）
        /// </summary>
        public static string ToDebugStringReflection<T>(this T self)
        {
            if (self == null)
                return "null";
            return string.Join(kSeparator,
                self.ToDebugStringFields(),
                self.ToDebugStringProperties());
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_WEBGL
        // IReactivePropertyで型指定が必要だったり、ForceNotifyが無かったり
#else
        /// <summary>
        /// 全てのReactivePropertyに対して強制的に変化通知を発生させる
        /// ・対象はprivate, publicなインスタンスプロパティ
        /// </summary>
        public static void ReactiveProperyForceNotifyAll<T>(this T self)
        {
            ReactiveProperyForceNotifyAll(typeof(T), self);
        }
        public static void ReactiveProperyForceNotifyAll(this object self)
        {
            ReactiveProperyForceNotifyAll(self.GetType(), self);
        }
        static void ReactiveProperyForceNotifyAll(Type type, object obj)
        {            
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);            
            foreach (var info in properties) {
                var rp = info.GetValue(obj) as IReactiveProperty;
                rp?.ForceNotify();
            }
        }

        /// <summary>
        /// ReactivePropertyを考慮したフィールドのコピー
        /// ・対象はprivate, publicなインスタンスフィールド
        /// ・ReactivePropertyであればValueを、それ以外であればそのままコピーする
        /// </summary>
        public static void ReactivePropertyCopyFieldTo<T>(this T self, T to)
        {
            ReactivePropertyCopyFieldTo(typeof(T), self, to);
        }
        public static void ReactivePropertyCopyFieldTo(this object self, object to)
        {
            var type = self.GetType();
            Debug.Assert(type.Equals(to.GetType()));
            ReactivePropertyCopyFieldTo(type, self, to);
        }
        static void ReactivePropertyCopyFieldTo(Type type, object from, object to)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var info in fields)
            {
                var fromRp = info.GetValue(from) as IReactiveProperty;
                var toRp = info.GetValue(to) as IReactiveProperty;
                if (fromRp == null || toRp == null)
                {
                    info.SetValue(to, info.GetValue(from));
                }
                else
                {
                    toRp.Value = fromRp.Value;
                }
            }
        }
#endif

        /// <summary>
        /// フィールドのコピー
        /// ・コピーしたくないフィールドは、NonSerializedAttribute を指定
        /// ・対象はprivate, publicなインスタンスフィールド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="to"></param>
        public static void CopyFieldTo<T>(this T self, T to)
        {
            CopyFieldTo(typeof(T), self, to);
        }
        public static void CopyFieldTo(this object self, object to)
        {
            var type = self.GetType();
            Debug.Assert(type.Equals(to.GetType()));
            CopyFieldTo(type, self, to);
        }
        static void CopyFieldTo(Type type, object from, object to)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             .Where(x => !x.IsDefined(typeof(NonSerializedAttribute), true));
            foreach (var info in fields)
            {
                info.SetValue(to, info.GetValue(from));
            }
        }

        /// <summary>
        /// ディープコピーを作成する
        /// 
        /// ※内部でBinaryFormatterを使用している場合
        /// クローンするクラスには SerializableAttribute 属性、
        /// 不要なフィールドは NonSerializedAttribute 属性をつける。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object CloneDeep(this object target)
        {
            object clone = null;
            using (MemoryStream stream = new MemoryStream()) {

#pragma warning disable SYSLIB0011 // 旧型式（.NET5では代替手段がない？
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, target);
                stream.Position = 0;
                clone = formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }
            return clone;
        }

#if false // @note: .NET5では対応できない？
        /// <summary>
        /// ディープコピーを安全に作成する
        /// ・CloneDeepより安全だが、Json出力すると並びが変わってしまう問題もある
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T CloneDeepSafe<T>(this T target)
        {
            object clone = null;
            using (MemoryStream stream = new MemoryStream()) {

                // Json出力時の並びが変わってしまう
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                serializer.Serialize(stream, target);
                stream.Position = 0;
                clone = serializer.Deserialize(stream);
            }
            return (T)clone;
        }
#endif
    }
}
