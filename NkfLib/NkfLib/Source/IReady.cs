using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    /// <summary>
    /// 準備完了I/F
    /// </summary>
    public interface IReady
    {
        /// <summary>
        /// 準備完了
        /// </summary>
        public bool IsReady { get; }
    }
}
