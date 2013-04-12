#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractGrinderElementTests.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using GrinderScript.Net.Core.Framework;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class AbstractGrinderElementTests
    {
        private AbstractGrinderElement element;

        private Mock<IGrinderContext> contextMock;

        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            contextMock = TestHelpers.TestUtils.CreateContextMock();
            loggerMock = TestHelpers.TestUtils.CreateLoggerMock();
            contextMock.Setup(c => c.ScriptFile).Returns(TestHelpers.TestUtils.TestsAsScriptFile);
            contextMock.Setup(c => c.GetLogger(typeof(TestElement))).Returns(loggerMock.Object);
            element = new TestElement { GrinderContext = contextMock.Object };
        }

        [TestCase]
        public void ElementShouldBeContextAware()
        {
            var actual = new TestElement { GrinderContext = contextMock.Object };
            Assert.That(actual, Is.AssignableTo<IGrinderContextAware>());
            Assert.That(actual.GrinderContext == contextMock.Object);
        }

        [TestCase]
        public void InitializeShouldSetLogger()
        {
            element.Initialize();
            Assert.That(element.Logger, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldLog()
        {
            element.Initialize();
            loggerMock.Verify(l => l.Trace("Initialize: Exit"));
        }

        [TestCase]
        public void InitializeShouldSetTypeHelper()
        {
            element.Initialize();
            Assert.That(element.TypeHelper, Is.Not.Null);
        }

        [TestCase]
        public void ShutdownShouldLog()
        {
            element.Initialize();
            element.Shutdown();
            loggerMock.Verify(l => l.Trace("Shutdown: Enter"));
            loggerMock.Verify(l => l.Trace("Shutdown: Exit"));
        }

        [TestCase]
        public void ShutdownShouldBeReCallable()
        {
            element.Initialize();
            element.Shutdown();
            Assert.DoesNotThrow(() => element.Shutdown());
        }

        internal class TestElement : AbstractGrinderElement
        {
        }
    }
}
