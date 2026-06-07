using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    // <summary>
    /// Exception型の拡張メソッドを管理するクラス
    /// </summary>
    public static partial class ExceptionExtensions
    {
        /// <summary>
        /// 指定型のExceptionが含まれるかどうか
        /// </summary>
        public static bool Contains<T>(this Exception self) where T : Exception
        {
            if (self is T)
                return true;
            return self.InnerException?.Contains<T>() ?? false;
        }

        /// <summary>
        /// 最初に発生したExceptionを取得
        /// ・ネストの最深のException
        /// </summary>
        public static Exception GetFirstException(this Exception self)
        {
            Exception ret = self;
            while (ret.InnerException != null) {
                ret = ret.InnerException;
            }
            return ret;
        }
    }
}
