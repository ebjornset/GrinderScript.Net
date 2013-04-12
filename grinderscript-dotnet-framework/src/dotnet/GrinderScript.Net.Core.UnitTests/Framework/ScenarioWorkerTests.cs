#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioWorkerTests.cs" company="http://GrinderScript.net">
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
    using System;

    using GrinderScript.Net.Core.Framework;

    using Moq;

    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class ScenarioWorkerTests
    {
        private ScenarioWorker scenaroio;

        private Mock<IGrinderContext> contextMock;

        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            contextMock = new Mock<IGrinderContext>();
            contextMock.Setup(c => c.GetLogger(It.IsAny<Type>())).Returns(new Mock<IGrinderLogger>().Object);
            contextMock.Setup(c => c.ScriptFile).Returns(value: TestUtils.TestsAsScriptFile);
            loggerMock = new Mock<IGrinderLogger>();
            loggerMock.SetupGet(l => l.IsTraceEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsDebugEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsInfoEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsWarnEnabled).Returns(true);
            loggerMock.SetupGet(l => l.IsErrorEnabled).Returns(true);
            contextMock.Setup(c => c.GetLogger(typeof(ScenarioWorker))).Returns(loggerMock.Object);
            contextMock.Setup(c => c.GetProperty(It.IsAny<string>(), It.IsAny<string>())).Returns((string k, string d) => d);
            scenaroio = new ScenarioWorker { GrinderContext = contextMock.Object, ProcessContext = TestUtils.CreateProcessContext(null, null, null, contextMock.Object)};
        }

        [TestCase]
        public void WorkerGroupShouldBeContextAware()
        {
            var actual = new ScenarioWorker { GrinderContext = contextMock.Object };
            Assert.That(actual, Is.AssignableTo<IGrinderContextAware>());
            Assert.That(actual.GrinderContext == contextMock.Object);
        }

        [TestCase]
        public void InitializeShouldFailWhenContextIsNull()
        {
            Assert.Throws<AwarenessException>(() => new ScenarioWorker().Initialize());
        }

        [TestCase]
        public void InitializeShouldSetLogger()
        {
            scenaroio.Initialize();
            Assert.That(scenaroio.Logger, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldLog()
        {
            scenaroio.Initialize();
            loggerMock.Verify(l => l.Trace("OnInitialize: Enter"));
            loggerMock.Verify(l => l.Trace("OnInitialize: Exit"));
        }

        [TestCase]
        public void FirstElementPropertyShouldBeGreaterThanZero()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("firstElement"), "1")).Returns("0");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Property 'grinderscript-dotnet.scenarioWorker.firstElement' should be > 0, but was 0"), () => scenaroio.Initialize());
        }

        [TestCase]
        public void InitializeShouldLogFirstAndLastElementAndRandom()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.TrueString);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("seed"), It.IsAny<string>())).Returns("345");
            scenaroio.Initialize();
            loggerMock.Verify(l => l.Info("OnInitialize: FirstElement = 1, LastElement = 100, IsRandom = True, Seed = 345"));
        }

        [TestCase]
        public void InitializeShouldUseFirstElementValueWhenPropertyIsProvied()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("firstElement"), "1")).Returns("3");
            scenaroio.Initialize();
            contextMock.Verify(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "workerType")), Times.Never());
        }

        [TestCase]
        public void InitializeShouldUseDefaultFirstElementWhenPropertyIsMissing()
        {
            scenaroio.Initialize();
            contextMock.Verify(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "workerType")));
        }

        [TestCase]
        public void InitializeShouldUseLastElementValueWhenPropertyIsProvided()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("lastElement"), "100")).Returns("3");
            scenaroio.Initialize();
            contextMock.Verify(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(100, "workerType")), Times.Never());
        }

        [TestCase]
        public void InitializeShouldUseDefaultLastElementWhenPropertyIsMissing()
        {
            scenaroio.Initialize();
            contextMock.Verify(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(100, "workerType")));
        }

        [TestCase]
        public void LastElementPropertyShouldBeNotBeLessThanFirstElement()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("lastElement"), "100")).Returns("0");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Property 'grinderscript-dotnet.scenarioWorker.lastElement' can not be < 'grinderscript-dotnet.scenarioWorker.firstElement' (1), but was 0"), () => scenaroio.Initialize());
        }

        [TestCase]
        public void InitializeShouldUseRandomValueWhenRandomPropertyIsProvided()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.FalseString);
            scenaroio.Initialize();
            Assert.That(scenaroio.IsRandom, Is.False);
        }

        [TestCase]
        public void InitializeShouldNotInitializeRandomWhenRandomPropertyIsFalse()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.FalseString);
            scenaroio.Initialize();
            Assert.That(scenaroio.Random, Is.Null);
        }

        [TestCase]
        public void InitializeShouldNotInitializeSeedWhenRandomPropertyIsFalse()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.FalseString);
            scenaroio.Initialize();
            Assert.That(scenaroio.Seed, Is.EqualTo(0));
        }

        [TestCase]
        public void InitializeShouldInitializeRandomWhenRandomPropertyIsNotFalse()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.TrueString);
            scenaroio.Initialize();
            Assert.That(scenaroio.Random, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldInitializeSeedWhenRandomPropertyIsNotFalse()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.TrueString);
            scenaroio.Initialize();
            Assert.That(scenaroio.Seed, Is.Not.EqualTo(0));
        }

        [TestCase]
        public void InitializeShouldInitializeSeedFromPropertyWhenProvided()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.TrueString);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("seed"), It.IsAny<string>())).Returns("543");
            scenaroio.Initialize();
            Assert.That(scenaroio.Seed, Is.EqualTo(543));
        }

        [TestCase]
        public void InitializeShouldIgnoreMissingWorkerType()
        {
            scenaroio.Initialize();
            contextMock.Verify(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "loadFactor")), Times.Never());
        }

        [TestCase]
        public void InitializeShouldLogMissingWorkerType()
        {
            scenaroio.Initialize();
            loggerMock.Verify(l => l.Trace("AddWorkerToCollectionByLoadFactor: Property 'grinderscript-dotnet.scenarioWorker.1.workerType' not set"));
        }

        [TestCase]
        public void InitializeShouldUseLoadFactorWhenPropertyIsProvided()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "workerType"))).Returns(typeof(TestWorker1).FullName);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "loadFactor"), "1")).Returns("3");
            scenaroio.Initialize();
            Assert.That(scenaroio.GroupSize, Is.EqualTo(3));
        }

        [TestCase]
        public void InitializeShouldLogWorkerPropertiesForProvidedWorkerType()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(4, "workerType"))).Returns(typeof(TestWorker1).FullName);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(4, "loadFactor"), "1")).Returns("3");
            scenaroio.Initialize();
            loggerMock.Verify(l => l.Info("AddWorkerToCollectionByLoadFactor: Index = 4, WorkerType = 'GrinderScript.Net.Core.UnitTests.Framework.ScenarioWorkerTests+TestWorker1', LoadFactor = 3"));
        }

        [TestCase]
        public void InitializeShouldUseDefaultLoadFactorWhenPropertyIsMissing()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "workerType"))).Returns(typeof(TestWorker1).FullName);
            scenaroio.Initialize();
            Assert.That(scenaroio.GroupSize, Is.EqualTo(1));
        }

        [TestCase]
        public void InitializeShouldFailForUnknownWorkerType()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "workerType"))).Returns("NotAValidTypeIPresume");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Unknown type: 'NotAValidTypeIPresume'"), () => scenaroio.Initialize());
        }

        [TestCase]
        public void InitializeShouldInitalizeUnderlyingWorkers()
        {
            SetupTestWorkers();
            scenaroio.Initialize();
            foreach (var underlying in scenaroio.Group)
            {
                Assert.IsInstanceOf<TestWorker>(underlying);
                Assert.That(((TestWorker)underlying).OnInitializeIsCalled);
            }
        }

        [TestCase]
        public void InitializeShouldLogWorkerInfoForAllUnderlyingWorkers()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.FalseString);
            SetupTestWorkers();
            scenaroio.Initialize();
            loggerMock.Verify(l => l.Info("Worker[1] = 'GrinderScript.Net.Core.UnitTests.Framework.ScenarioWorkerTests+TestWorker1'"));
            loggerMock.Verify(l => l.Info("Worker[2] = 'GrinderScript.Net.Core.UnitTests.Framework.ScenarioWorkerTests+TestWorker2'"));
            loggerMock.Verify(l => l.Info("Worker[3] = 'GrinderScript.Net.Core.UnitTests.Framework.ScenarioWorkerTests+TestWorker100'"));
        }

        [TestCase]
        public void RunShouldLog()
        {
            scenaroio.Initialize();
            scenaroio.Run();
            loggerMock.Verify(l => l.Trace("OnRun: Enter"));
            loggerMock.Verify(l => l.Trace("OnRun: Exit"));
        }

        [TestCase]
        public void RunShouldRunAllWorkersWhenRunningSequential()
        {
            SetupTestWorkers();
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.FalseString);
            scenaroio.Initialize();
            scenaroio.Run();
            foreach (var underlying in scenaroio.Group)
            {
                Assert.IsInstanceOf<TestWorker>(underlying);
                Assert.That(((TestWorker)underlying).OnRunIsCalled);
                Assert.That(((TestWorker)underlying).OnRunCallCount, Is.EqualTo(1));
            }
        }

        [TestCase]
        public void RunShouldRunAllWorkersWhenRunningRandomly()
        {
            SetupTestWorkers();
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.TrueString);
            scenaroio.Initialize();
            scenaroio.Run();
            foreach (var underlying in scenaroio.Group)
            {
                Assert.IsInstanceOf<TestWorker>(underlying);
                Assert.That(((TestWorker)underlying).OnRunIsCalled);
                Assert.That(((TestWorker)underlying).OnRunCallCount, Is.EqualTo(1));
            }
        }

        [TestCase]
        public void ShutdownShouldLog()
        {
            scenaroio.Initialize();
            scenaroio.Shutdown();
            loggerMock.Verify(l => l.Trace("OnShutdown: Enter"));
            loggerMock.Verify(l => l.Trace("OnShutdown: Exit"));
        }

        [TestCase]
        public void ShutdownShouldBeReCallable()
        {
            scenaroio.Initialize();
            scenaroio.Shutdown();
            Assert.DoesNotThrow(() => scenaroio.Shutdown());
        }

        [TestCase]
        public void ShutdownShouldShutdownUnderlyingWorkers()
        {
            SetupTestWorkers();
            scenaroio.Initialize();
            var group = new List<IGrinderWorker>(scenaroio.Group);
            scenaroio.Shutdown();
            foreach (var underlying in group)
            {
                Assert.IsInstanceOf<TestWorker>(underlying);
                Assert.That(((TestWorker)underlying).OnShutdowIsCalled);
            }
        }

        [TestCase]
        public void ShutdownShouldClearUnderlyingWorkers()
        {
            SetupTestWorkers();
            scenaroio.Initialize();
            Assert.That(scenaroio.GroupSize, Is.EqualTo(3));
            scenaroio.Shutdown();
            Assert.That(scenaroio.GroupSize, Is.EqualTo(0));
        }

        [TestCase]
        public void ShutdownShouldRetrowUnderlyingExceptions()
        {
            SetupTestWorkers();
            SetupTestWorkersWithShutdownExceptions();
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetPropertyKey("random"), It.IsAny<string>())).Returns(bool.FalseString);
            scenaroio.Initialize();
            var exception = Assert.Throws<AggregateException>(() => scenaroio.Shutdown());
            Assert.That(exception.InnerExceptions.Count, Is.EqualTo(2));
            Assert.That(exception.InnerExceptions[0], Is.InstanceOf<InvalidOperationException>());
            Assert.That(exception.InnerExceptions[1], Is.InstanceOf<ApplicationException>());
        }

        private void SetupTestWorkers()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(1, "workerType"))).Returns(typeof(TestWorker1).FullName);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(2, "workerType"))).Returns(typeof(TestWorker2).FullName);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(100, "workerType"))).Returns(typeof(TestWorker100).FullName);
        }

        private void SetupTestWorkersWithShutdownExceptions()
        {
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(3, "workerType"))).Returns(typeof(TestWorker3ShutdownException).FullName);
            contextMock.Setup(c => c.GetProperty(ScenarioWorker.GetElementPropertyKey(4, "workerType"))).Returns(typeof(TestWorker4ShutdownException).FullName);
        }

        internal class TestWorker1 : TestWorker
        {
        }

        internal class TestWorker2 : TestWorker
        {
        }

        internal class TestWorker3ShutdownException : TestWorker
        {
            protected override void OnShutdown()
            {
                base.OnShutdown();
                throw new InvalidOperationException();
            }
        }

        internal class TestWorker4ShutdownException : TestWorker
        {
            protected override void OnShutdown()
            {
                base.OnShutdown();
                throw new ApplicationException();
            }
        }

        internal class TestWorker100 : TestWorker
        {
        }
    }
}
