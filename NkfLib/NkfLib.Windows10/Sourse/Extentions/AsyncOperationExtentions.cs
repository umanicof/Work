using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace NkfLib
{
    /// <summary>
    /// AsyncOperation 拡張メソッド
    /// ・古いAPIの非同期の仕組みをTaskとして使用するために用意
    /// 　出典：https://blog.xin9le.net/entry/2012/11/12/123231
    /// </summary>
    public static partial class AsyncOperationExtentions
    { 
        public static Task<T> AsTask<T>(this IAsyncOperation<T> operation)
        {
            var tcs = new TaskCompletionSource<T>();
            operation.Completed =
                delegate
                {
                    switch (operation.Status)
                    {
                        case AsyncStatus.Completed:
                            tcs.SetResult(operation.GetResults());
                            break;

                        case AsyncStatus.Error:
                            tcs.SetException(operation.ErrorCode);
                            break;

                        case AsyncStatus.Canceled:
                            tcs.SetCanceled();
                            break;

                        default:
                            throw new Exception();
                    }
                };
            
            return tcs.Task;
        }

        public static TaskAwaiter<T> GetAwaiter<T>(this IAsyncOperation<T> operation)
        {
            return operation.AsTask().GetAwaiter();
        }
    }
}
