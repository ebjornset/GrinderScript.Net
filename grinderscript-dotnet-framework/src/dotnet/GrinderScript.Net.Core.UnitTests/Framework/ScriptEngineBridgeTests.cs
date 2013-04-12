#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptEngineBridgeTests.cs" company="http://GrinderScript.net">
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
    using System.Reflection;

    using GrinderScript.Net.Core.Framework;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ScriptEngineBridgeTests
    {
        private ScriptEngineBridge bridge;

        private Mock<IGrinderContext> contextMock;

        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            contextMock = TestUtils.CreateContextMock();
            loggerMock = TestUtils.CreateLoggerMock();
            contextMock.Setup(c => c.GetLogger(typeof(ScriptEngineBridge))).Returns(loggerMock.Object);
            bridge = new ScriptEngineBridge(contextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if (bridge != null)
            {
                bridge.Shutdown();
            }

            bridge = null;
        }

        [TestCase]
        public void CtorShouldFailWhenContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ScriptEngineBridge(null));
        }

        [TestCase]
        public void CtorShouldSetContext()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            var localBridge = new ScriptEngineBridge(contextMock.Object);
            Assert.That(localBridge.GrinderContext, Is.SameAs(contextMock.Object));
        }

        [TestCase]
        public void ScriptEngineBridgeShouldBeProcessContextAware()
        {
            var actual = new ScriptEngineBridge(contextMock.Object);
            Assert.That(actual, Is.AssignableTo<IProcessContextAware>());
        }

        [TestCase]
        public void InitializeShouldSetLogger()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            bridge.Initialize();
            Assert.That(bridge.Logger, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldLog()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            bridge.Initialize();
            loggerMock.Verify(l => l.Trace("OnInitialize: Enter"));
            loggerMock.Verify(l => l.Trace("OnInitialize: Exit"));
        }

        [TestCase]
        public void InitializeShouldInstantiateUnderlyingScriptEngineFromProperty()
        {
            InitializeBridgeWithTestScriptEngine();
            Assert.IsAssignableFrom<TestScriptEngine>(bridge.ScriptEngine);
        }

        [TestCase]
        public void InitializeShouldInstantiateDefaultScriptEngineWhenPropertyIsNull()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            bridge.Initialize();
            Assert.IsAssignableFrom<DefaultScriptEngine>(bridge.ScriptEngine);
        }

        [TestCase]
        public void InitializeShouldInitializeUnderlyingScriptEngine()
        {
            var testScriptEngine = InitializeBridgeWithTestScriptEngine();
            Assert.That(testScriptEngine.OnInitializeIsCalled);
        }

        [TestCase]
        public void InitializeShouldSetBinFolderWhenUnderlyingScriptEngineIsBinFolderAware()
        {
            var testScriptEngine = InitializeBridgeWithTestScriptEngine();
            Assert.That(testScriptEngine.SetBinFolderIsCalled);
        }

        [TestCase]
        public void InitializeShouldShouldSetAwarenessOnTheUnderlyingScriptEngine()
        {
            var testScriptEngine = InitializeBridgeWithTestScriptEngine();
            Assert.That(testScriptEngine.GrinderContext, Is.Not.Null);
            Assert.That(testScriptEngine.DatapoolFactory, Is.Not.Null);
            Assert.That(testScriptEngine.DatapoolManager, Is.Not.Null);
            Assert.That(testScriptEngine.ProcessContext, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldCreateMissingDatapoolsFromProperties()
        {
            const string DatapoolName = "TestPoolName2";
            contextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("2"))).Returns(DatapoolName);
            contextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey(DatapoolName, "valueType"))).Returns(typeof(DatapoolFactoryTests.TestValue).FullName);
            contextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey(DatapoolName, "factoryType"))).Returns(typeof(DatapoolFactoryTests.TestValueFactory<>).FullName);
            InitializeBridgeWithTestScriptEngine();
            Assert.That(bridge.ProcessContext.DatapoolManager.ContainsDatapool("TestPoolName2"));
        }

        [TestCase]
        public void ShutdownShouldShutdownUnderlyingScriptEngine()
        {
            var testScriptEngine = InitializeBridgeWithTestScriptEngine();
            bridge.Shutdown();
            Assert.That(testScriptEngine.OnShutdownIsCalled, Is.True);
        }

        [TestCase]
        public void ShutdownShouldSetUnderlyingScriptEngineToNull()
        {
            InitializeBridgeWithTestScriptEngine();
            bridge.Shutdown();
            Assert.That(bridge.ScriptEngine, Is.Null);
        }

        [TestCase]
        public void ShutdownShouldBeReCallable()
        {
            InitializeBridgeWithTestScriptEngine();
            bridge.Shutdown();
            Assert.DoesNotThrow(() => bridge.Shutdown());
        }

        [TestCase]
        public void ShutdownShouldLog()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            bridge.Initialize();
            bridge.Shutdown();
            loggerMock.Verify(l => l.Trace("OnShutdown: Enter"));
            loggerMock.Verify(l => l.Trace("OnShutdown: Exit"));
        }

        [TestCase]
        public void CreateWorkerRunnableShouldUseUnderlyingScriptEngineToCreateTheActualWorker()
        {
            var testScriptEngine = InitializeBridgeWithTestScriptEngine();
            bridge.CreateWorkerRunnable();
            Assert.That(testScriptEngine.OnCreateWorkerRunnableIsCalled);
        }

        [TestCase]
        public void CreateWorkerRunnableShouldInitializeTheActualWorker()
        {
            InitializeBridgeWithTestScriptEngine();
            var worker = bridge.CreateWorkerRunnable();
            Assert.IsAssignableFrom<TestWorker>(worker);
            Assert.That(((TestWorker)worker).OnInitializeIsCalled);
        }

        [TestCase]
        public void CreateWorkerRunnableShouldSetAwarenessOnTheActualWorker()
        {
            InitializeBridgeWithTestScriptEngine();
            var worker = bridge.CreateWorkerRunnable();
            Assert.IsAssignableFrom<TestWorker>(worker);
            var actual = (TestWorker)worker;
            Assert.That(actual.GrinderContext, Is.Not.Null);
            Assert.That(actual.DatapoolFactory, Is.Not.Null);
            Assert.That(actual.DatapoolManager, Is.Not.Null);
            Assert.That(actual.ProcessContext, Is.Not.Null);
        }

        [TestCase]
        public void CreateWorkerRunnableShouldLog()
        {
            InitializeBridgeWithTestScriptEngine();
            bridge.CreateWorkerRunnable();
            loggerMock.Verify(l => l.Trace("OnCreateWorkerRunnable: Enter"));
        }

        [TestCase]
        public void OnAssemblyResolveShouldLoadExistingAssemblyDirectlyFromArgsName()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            bridge.Initialize();
            var args = new ResolveEventArgs(TestUtils.TestsAsScriptFile, GetType().Assembly);
            Assembly actual = bridge.OnAssemblyResolve(null, args);
            Assert.That(actual, Is.Not.Null);
        }

        [TestCase]
        public void OnAssemblyResolveShouldLoadAssemblyWithSpesificeNamePart()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            contextMock.Setup(c => c.GetProperty(Constants.BinFolderKey)).Returns(TestUtils.TestsAsAssemblyLocation);
            bridge.Initialize();
            var args = new ResolveEventArgs(GetType().Assembly.FullName, null);
            Assembly actual = bridge.OnAssemblyResolve(null, args);
            Assert.That(actual, Is.Not.Null);
        }

        [TestCase]
        public void OnAssemblyResolveShouldLoadAssemblyWithBasicNamePart()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            contextMock.Setup(c => c.GetProperty(Constants.BinFolderKey)).Returns(TestUtils.TestsAsAssemblyLocation);
            bridge.Initialize();
            var args = new ResolveEventArgs(GetType().Assembly.GetName().Name, null);
            Assembly actual = bridge.OnAssemblyResolve(null, args);
            Assert.That(actual, Is.Not.Null);
        }

        [TestCase]
        public void OnAssemblyResolveShouldLogWarningAboutNonExistingAssembly()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            contextMock.Setup(c => c.GetProperty(Constants.BinFolderKey)).Returns(TestUtils.TestsAsAssemblyLocation);
            bridge.Initialize();
            var args = new ResolveEventArgs("NotAValidAssemblyIPresume", null);
            bridge.OnAssemblyResolve(null, args);
            loggerMock.Verify(l => l.Warn("Assembly not found: 'NotAValidAssemblyIPresume'"));
        }

        [TestCase]
        public void OnAssemblyResolveShouldThrowExceptionForInvalidAssembly()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.CoreAsScriptFile);
            contextMock.Setup(c => c.GetProperty(Constants.BinFolderKey)).Returns(TestUtils.TestsAsAssemblyLocation);
            bridge.Initialize();
            var args = new ResolveEventArgs("TestHelpers\\InvalidAssembly", null);
            Assert.Throws<BadImageFormatException>(() => bridge.OnAssemblyResolve(null, args));
        }

        private TestScriptEngine InitializeBridgeWithTestScriptEngine()
        {
            contextMock.Setup(c => c.ScriptFile).Returns(TestUtils.TestsAsScriptFile);
            contextMock.Setup(c => c.GetProperty(Constants.ScriptEngineTypeKey)).Returns(typeof(TestScriptEngine).AsPropertyValue());
            contextMock.Setup(c => c.GetProperty(Constants.WorkerTypeKey)).Returns(typeof(TestWorker).AsPropertyValue());
            bridge.Initialize();
            return bridge.ScriptEngine as TestScriptEngine;
        }
    }
}
