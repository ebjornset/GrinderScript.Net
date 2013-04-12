#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerFacade.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.Framework
{
    using System;

    public class LoggerFacade : IGrinderLogger
    {
        private readonly IGrinderLogger underlying;
        internal IValueResolver<bool> IsErrorEnabledResolver { get; set; }
        internal IValueResolver<bool> IsWarnEnabledResolver { get; set; }
        internal IValueResolver<bool> IsInfoEnabledResolver { get; set; }
        internal IValueResolver<bool> IsDebugEnabledResolver { get; set; }
        internal IValueResolver<bool> IsTraceEnabledResolver { get; set; }

        public LoggerFacade(IGrinderLogger underlying, IGrinderContext grinderContext)
        {
            if (underlying == null)
            {
                throw new ArgumentNullException("underlying");
            }

            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            this.underlying = underlying;
            SetupLoggerEnabledValueResolvers(grinderContext);
        }

        public bool IsErrorEnabled { get { return IsErrorEnabledResolver.Value; } }

        public bool IsWarnEnabled { get { return IsWarnEnabledResolver.Value; } }

        public bool IsInfoEnabled { get { return IsInfoEnabledResolver.Value; } }

        public bool IsDebugEnabled { get { return IsDebugEnabledResolver.Value; } }

        public bool IsTraceEnabled { get { return IsTraceEnabledResolver.Value; } }

        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                underlying.Error(message);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                underlying.Error(message, exception);
            }
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                underlying.Warn(message);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                underlying.Warn(message, exception);
            }
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                underlying.Info(message);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                underlying.Info(message, exception);
            }
        }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                underlying.Debug(message);
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                underlying.Debug(message, exception);
            }
        }

        public void Trace(string message)
        {
            if (IsTraceEnabled)
            {
                underlying.Trace(message);
            }
        }

        public void Trace(string message, Exception exception)
        {
            if (IsTraceEnabled)
            {
                underlying.Trace(message, exception);
            }
        }

        private void SetupLoggerEnabledValueResolvers(IGrinderContext grinderContext)
        {
            long loggerEnableCacheTtl = long.Parse(grinderContext.GetProperty(Constants.LoggerEnabledCacheTtlKey, "-1"));
            if (loggerEnableCacheTtl < 0)
            {
                IsErrorEnabledResolver = new ValueResolverOnce<bool>(() => underlying.IsErrorEnabled);
                IsWarnEnabledResolver = new ValueResolverOnce<bool>(() => underlying.IsWarnEnabled);
                IsInfoEnabledResolver = new ValueResolverOnce<bool>(() => underlying.IsInfoEnabled);
                IsDebugEnabledResolver = new ValueResolverOnce<bool>(() => underlying.IsDebugEnabled);
                IsTraceEnabledResolver = new ValueResolverOnce<bool>(() => underlying.IsTraceEnabled);
            }
            else if ((loggerEnableCacheTtl == 0))
            {
                IsErrorEnabledResolver = new ValueResolverAlways<bool>(() => underlying.IsErrorEnabled);
                IsWarnEnabledResolver = new ValueResolverAlways<bool>(() => underlying.IsWarnEnabled);
                IsInfoEnabledResolver = new ValueResolverAlways<bool>(() => underlying.IsInfoEnabled);
                IsDebugEnabledResolver = new ValueResolverAlways<bool>(() => underlying.IsDebugEnabled);
                IsTraceEnabledResolver = new ValueResolverAlways<bool>(() => underlying.IsTraceEnabled);
            }
            else
            {
                IsErrorEnabledResolver = new ValueResolverTimed<bool>(() => underlying.IsErrorEnabled, loggerEnableCacheTtl);
                IsWarnEnabledResolver = new ValueResolverTimed<bool>(() => underlying.IsWarnEnabled, loggerEnableCacheTtl);
                IsInfoEnabledResolver = new ValueResolverTimed<bool>(() => underlying.IsInfoEnabled, loggerEnableCacheTtl);
                IsDebugEnabledResolver = new ValueResolverTimed<bool>(() => underlying.IsDebugEnabled, loggerEnableCacheTtl);
                IsTraceEnabledResolver = new ValueResolverTimed<bool>(() => underlying.IsTraceEnabled, loggerEnableCacheTtl);
            }

            this.Info(x => x("SetupLoggerEnabledValueResolvers: loggerEnableCacheTtl = '{0}'", loggerEnableCacheTtl));
        }
    }
}
