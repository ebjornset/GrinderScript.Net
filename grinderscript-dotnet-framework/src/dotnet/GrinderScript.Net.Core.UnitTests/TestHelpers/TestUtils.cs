#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestUtils.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.Framework;

using Moq;

using System;

namespace GrinderScript.Net.Core.UnitTests.TestHelpers
{
    internal static class TestUtils
    {
        internal static Mock<IGrinderLogger> CreateLoggerMock()
        {
            var loggerMock = new Mock<IGrinderLogger>();
            loggerMock.SetupGet(l => l.IsTraceEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsDebugEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsInfoEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsWarnEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsErrorEnabled).Returns(true);
            return loggerMock;
        }

        internal static Mock<IGrinderContext> CreateContextMock()
        {
            var contextMock = new Mock<IGrinderContext>();
            contextMock.Setup(c => c.GetLogger(It.IsAny<Type>())).Returns(new Mock<IGrinderLogger>().Object);
            contextMock.Setup(c => c.GetProperty(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((key, defaultValue) => defaultValue);
            return contextMock;
        }

        internal static ProcessContext CreateProcessContext(
            string binFolder = null, 
            IDatapoolFactory datapoolFactory = null, 
            IDatapoolManager datapoolManager = null, 
            IGrinderContext grinderContext = null)
        {
            var processContext = new ProcessContext
            {
                BinFolder = binFolder,
                DatapoolFactory = datapoolFactory ?? new Mock<IDatapoolFactory>().Object,
                DatapoolManager = datapoolManager ?? new Mock<IDatapoolManager>().Object,
                GrinderContext = grinderContext ?? CreateContextMock().Object
            };

            processContext.Freeze();
            return processContext;
        }

        internal static string CoreAsScriptFile
        {
            get
            {
                return typeof(IGrinderContext).AsScriptFile();
            }
        }

        internal static string TestsAsScriptFile
        {
            get
            {
                return typeof(TestUtils).AsScriptFile();
            }
        }

        internal static string TestsAsAssemblyLocation
        {
            get
            {
                return typeof(TestUtils).AsAssemblyLocation();
            }
        }

        internal static string AsPropertyValue(this Type type)
        {
            return type.AssemblyQualifiedName;
        }

        private static string AsScriptFile(this Type type)
        {
            return type.Assembly.CodeBase.Replace("file:///", string.Empty).Replace('/', '\\');
        }

        private static string AsAssemblyLocation(this Type type)
        {
            string result = type.Assembly.CodeBase.Replace("file:///", string.Empty).Replace('/', '\\');
            return result.Substring(0, result.LastIndexOf('\\'));
        }
    }
}
