#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptEngineBridgeFactoryTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.UnitTests.TestHelpers;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using GrinderScript.Net.Core.Framework;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ScriptEngineBridgeFactoryTests
    {
        [TestCase]
        public void CreateBridgeShouldCreateBrdgeWithContext()
        {
            Mock<IGrinderContext> contextMock = TestUtils.CreateContextMock();
            Mock<IGrinderLogger> loggerMock = TestUtils.CreateLoggerMock();
            contextMock.Setup(c => c.GetLogger(typeof(ScriptEngineBridge))).Returns(loggerMock.Object);
            var scriptEngine = ScriptEngineBridgeFactory.CreateBridge(contextMock.Object);
            Assert.That(scriptEngine, Is.AssignableTo<ScriptEngineBridge>());
            Assert.That(((ScriptEngineBridge)scriptEngine).GrinderContext == contextMock.Object);
        }
    }
}
