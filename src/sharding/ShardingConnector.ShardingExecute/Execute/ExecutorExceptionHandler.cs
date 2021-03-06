using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using ShardingConnector.Exceptions;
using ShardingConnector.Executor.SqlLog;
using ShardingConnector.Logger;

namespace ShardingConnector.ShardingExecute.Execute
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/04/15 16:48:34
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */

    /// <summary>
    /// 
    /// </summary>
    public sealed class ExecutorExceptionHandler
    {
        private static ILogger<ExecutorExceptionHandler> _logger =
            InternalLoggerFactory.CreateLogger<ExecutorExceptionHandler>();
        private static readonly AsyncLocal<bool> IGNORE_EXCEPTION = new AsyncLocal<bool>() {Value = true};

        /**
     * Set throw exception if error occur or not.
     *
     * @param isExceptionThrown throw exception if error occur or not
     */
        public static void SetExceptionThrow(bool trhow)
        {
            ExecutorExceptionHandler.IGNORE_EXCEPTION.Value = trhow;
        }

        /**
     * Get throw exception if error occur or not.
     * 
     * @return throw exception if error occur or not
     */
        public static bool IsThrowException()
        {
            return IGNORE_EXCEPTION.Value;
        }

        /**
     * Handle exception. 
     * 
     * @param exception to be handled exception
     * @throws SQLException SQL exception
     */
        public static void HandleException(Exception exception)
        {
            if (IsThrowException())
            {
                throw new ShardingException("异常", exception);
            }
            _logger.LogError("处理异常",exception);
        }
    }
}