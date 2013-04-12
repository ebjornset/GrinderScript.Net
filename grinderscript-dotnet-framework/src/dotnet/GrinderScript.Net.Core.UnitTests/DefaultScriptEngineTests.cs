#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScriptEngineTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.Framework;
using GrinderScript.Net.Core.UnitTests.TestHelpers;

using Moq;

using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests
{
    [TestFixture]
    public class DefaultScriptEngineTests
    {
        private DefaultScriptEngine scriptEngine;

        private Mock<IGrinderContext> contextMock;

        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            contextMock = TestUtils.CreateContextMock();
            loggerMock = TestUtils.CreateLoggerMock();
            contextMock.Setup(c => c.GetLogger(typeof(DefaultScriptEngine))).Returns(loggerMock.Object);
            scriptEngine = new DefaultScriptEngine { GrinderContext = contextMock.Object };
        }

        [TestCase]
        public void DefaultScriptEngineShouldBeContextAware()
        {
            var actual = new DefaultScriptEngine { GrinderContext = contextMock.Object };
            Assert.That(actual, Is.AssignableTo<IGrinderContextAware>());
            Assert.That(actual.GrinderContext == contextMock.Object);
        }

        [TestCase]
        public void InitializeShouldSetContext()
        {
            scriptEngine.Initialize();
            Assert.That(scriptEngine.GrinderContext, Is.SameAs(contextMock.Object));
        }

        [TestCase]
        public void InitializeShouldSetLogger()
        {
            scriptEngine.Initialize();
            Assert.That(scriptEngine.Logger, Is.Not.Null);
            loggerMock.Verify(l => l.Trace("Initialize: Exit"));
        }

        [TestCase]
        public void CreateWorkerRunnableShouldThrowExceptionWhenPropertyIsNull()
        {
            scriptEngine.Initialize();
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Missing property 'grinderscript-dotnet.workerType'"), () => scriptEngine.CreateWorkerRunnable());
        }

        [TestCase]
        public void CreateWorkerRunnableShouldInstantiateWorkerFromProperty()
        {
            InitialiseScriptEngineWithTestWorker();
            Assert.IsAssignableFrom<TestWorker>(scriptEngine.CreateWorkerRunnable());
        }

        [TestCase]
        public void CreateWorkerRunnableShouldNotInitializeWorker()
        {
            var testWorker = InitialiseScriptEngineWithTestWorkerAndCreateWorkerRunnable();
            Assert.That(testWorker.OnInitializeIsCalled, Is.False);
        }

        [TestCase]
        public void CreateWorkerRunnableShouldLog()
        {
            InitialiseScriptEngineWithTestWorkerAndCreateWorkerRunnable();
            loggerMock.Verify(l => l.Trace("OnCreateWorkerRunnable: Enter"));
        }

        [TestCase]
        public void ShutdownShouldBeReCallable()
        {
            scriptEngine.Initialize();
            scriptEngine.Shutdown();
            Assert.DoesNotThrow(() => scriptEngine.Shutdown());
        }

        private TestWorker InitialiseScriptEngineWithTestWorkerAndCreateWorkerRunnable()
        {
            InitialiseScriptEngineWithTestWorker();
            return (TestWorker)scriptEngine.CreateWorkerRunnable();
        }

        private void InitialiseScriptEngineWithTestWorker()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.TestsAsScriptFile);
            contextMock.Setup(c => c.GetProperty(AbstractScriptEngine.WorkerTypeKey)).Returns(typeof(TestWorker).AsPropertyValue());
            scriptEngine.Initialize();
        }
    }
}
