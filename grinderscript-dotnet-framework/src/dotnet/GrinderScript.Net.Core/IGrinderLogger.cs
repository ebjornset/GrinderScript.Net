#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGrinderLogger.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core
{
    using System;

    public interface IGrinderLogger
    {
        bool IsErrorEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsTraceEnabled { get; }

        void Error(string message);

        void Error(string message, Exception exception);

        void Warn(string message);

        void Warn(string message, Exception exception);

        void Info(string message);

        void Info(string message, Exception exception);

        void Debug(string message);

        void Debug(string message, Exception exception);

        void Trace(string message);

        void Trace(string message, Exception exception);
    }
}