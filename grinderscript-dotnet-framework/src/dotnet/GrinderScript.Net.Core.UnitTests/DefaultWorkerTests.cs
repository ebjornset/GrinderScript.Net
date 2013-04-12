#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultWorkerTests.cs" company="http://GrinderScript.net">
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

using System.Linq;

using GrinderScript.Net.Core.Framework;
using GrinderScript.Net.Core.UnitTests.TestHelpers;

using Moq;

using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests
{
    [TestFixture]
    public class DefaultWorkerTests
    {
        private DefaultWorker worker;

        private Mock<IGrinderContext> contextMock;

        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            contextMock = TestUtils.CreateContextMock();
            loggerMock = TestUtils.CreateLoggerMock();
            contextMock.Setup(c => c.GetLogger(typeof(TestDefaultWorker))).Returns(loggerMock.Object);
            contextMock.Setup(c => c.CreateTest(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IGrinderTest>())).Returns((int i, string s, IGrinderTest t) => t);
            var processContext = TestUtils.CreateProcessContext(null, null, null, contextMock.Object);
            worker = new TestDefaultWorker { ProcessContext = processContext, GrinderContext = contextMock.Object };
        }

        [TestCase]
        public void DefaultWorkerShouldBeContextAware()
        {
            var actual = new TestDefaultWorker { GrinderContext = contextMock.Object };
            Assert.That(actual, Is.AssignableTo<IGrinderContextAware>());
            Assert.That(actual.GrinderContext == contextMock.Object);
        }

        [TestCase]
        public void InitializeShouldFailWhenContextIsNull()
        {
            Assert.Throws<AwarenessException>(() => new TestDefaultWorker().Initialize());
        }

        [TestCase]
        public void InitializeShouldSetLogger()
        {
            worker.Initialize();
            Assert.That(worker.Logger, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldLog()
        {
            worker.Initialize();
            loggerMock.Verify(l => l.Info("OnInitialize: Enter"));
            loggerMock.Verify(l => l.Info("OnInitialize: Exit"));
        }

        [TestCase]
        public void RunShouldLog()
        {
            worker.Initialize();
            worker.Run();
            loggerMock.Verify(l => l.Debug("OnRun: Enter"));
            loggerMock.Verify(l => l.Debug("OnRun: Exit"));
        }

        [TestCase]
        public void AddTestFromParamsShouldAddItToTestList()
        {
            worker.Initialize();
            var actual = AddTestToWorker(new Mock<IGrinderScriptEngine>(), 12, "AddTest", 99);
            Assert.That(worker.TestList.Tests.Count(), Is.EqualTo(1));
            Assert.That(actual, Is.SameAs(worker.TestList.Tests.First()));
            Assert.That(actual.Metadata.TestNumber, Is.EqualTo(12));
            Assert.That(actual.Metadata.TestDescription, Is.EqualTo("AddTest"));
            Assert.That(actual.Metadata.SleepMillis, Is.EqualTo(99));
        }

        [TestCase]
        public void AddExistingTestShouldAddItToTestList()
        {
            worker.Initialize();
            var testMock = new Mock<ITest>();
            worker.AddTest(testMock.Object);
            Assert.That(worker.TestList.Tests.Count(), Is.EqualTo(1));
            Assert.That(worker.TestList.Tests.First(), Is.SameAs(testMock.Object));
        }

        [TestCase]
        public void RunShouldRunTestInTestList()
        {
            worker.Initialize();
            var testActionsMock = new Mock<IGrinderScriptEngine>();
            AddTestToWorker(testActionsMock);
            worker.Run();
            testActionsMock.Verify(ta => ta.Initialize()); // Setup as BeforeTestAction
            testActionsMock.Verify(ta => ta.CreateWorkerRunnable()); // Setup as TestAction
            testActionsMock.Verify(ta => ta.Shutdown()); // Setup as AfterTestAction
        }

        [TestCase]
        public void ShutdownShouldLog()
        {
            worker.Initialize();
            worker.Shutdown();
            loggerMock.Verify(l => l.Info("OnShutdown: Enter"));
            loggerMock.Verify(l => l.Info("OnShutdown: Exit"));
        }

        [TestCase]
        public void ShutdownShouldBeReCallable()
        {
            worker.Initialize();
            worker.Shutdown();
            Assert.DoesNotThrow(() => worker.Shutdown());
        }

        private ITest AddTestToWorker(Mock<IGrinderScriptEngine> testActionsMock, int testNumber = 12, string testDescripton = "TestDescription", int sleepMillis = 22)
        {
            var actual = worker.AddTest(
                testNumber,
                testDescripton,
                () => testActionsMock.Object.CreateWorkerRunnable(),
                () => testActionsMock.Object.Initialize(),
                () => testActionsMock.Object.Shutdown(),
                sleepMillis);
            return actual;
        }

        private class TestDefaultWorker : DefaultWorker
        {
            protected override void DefaultInitialize()
            {
            }
        }
    }
}
