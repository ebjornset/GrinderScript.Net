#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerExtensions.cs" company="http://GrinderScript.net">
//
//   Copyright © 2012 Eirik Bjornset.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
//
// <author>Eirik Bjornset</author>
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Globalization;

namespace GrinderScript.Net.Core
{
    public static class LoggerExtensions
    {
        public static void Error(this IGrinderLogger logger, Action<LogMessageHandler> messageCallback)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(FormatMessage(messageCallback));
            }
        }

        public static void Warn(this IGrinderLogger logger, Action<LogMessageHandler> messageCallback)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(FormatMessage(messageCallback));
            }
        }

        public static void Info(this IGrinderLogger logger, Action<LogMessageHandler> messageCallback)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(FormatMessage(messageCallback));
            }
        }

        public static void Debug(this IGrinderLogger logger, Action<LogMessageHandler> messageCallback)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(FormatMessage(messageCallback));
            }
        }

        public static void Trace(this IGrinderLogger logger, Action<LogMessageHandler> messageCallback)
        {
            if (logger.IsTraceEnabled)
            {
                logger.Trace(FormatMessage(messageCallback));
            }
        }

        private static string FormatMessage(Action<LogMessageHandler> messageCallback)
        {
            string message = null;
            messageCallback((format, args) => message = string.Format(CultureInfo.CurrentCulture, format, args));
            return message;
        }
        
    }
}
