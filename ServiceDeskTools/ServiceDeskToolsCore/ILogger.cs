using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore
{
    public interface ILogger
    {
        /// <summary>
        /// Message pour Debug
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Message d'erreur avec son exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Message d'erreur sans Exception !
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Message d'information
        /// </summary>
        /// <param name="messageInfo"></param>
        void Info(string messageInfo);

        /// <summary>
        /// Message d'information success
        /// </summary>
        /// <param name="message"></param>
        void Success(string message);
        
        /// <summary>
        /// Message d'attention
        /// </summary>
        /// <param name="messageWarn"></param>
        void Warn(string messageWarn);
    }
}
