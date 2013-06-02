#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsScriptWorkerTests.cs" company="http://GrinderScript.net">
//
//   Copyright Â© 2012 Eirik Bjornset.
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

using CSScriptLibrary;

using GrinderScript.Net.Core;
using GrinderScript.Net.Core.Framework;
using GrinderScript.Net.Core.UnitTests.TestHelpers;

using Mono.CSharp;

using Moq;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace GrinderScript.Net.CsScript.UnitTests
{
    [TestFixture]
    public class CsScriptWorkerTests
    {
        private Mock<IGrinderContext> grinderContextMock;
        private Mock<IDatapoolManager> datapoolManagerMock;
        private IProcessContext processContext;
        private Mock<IGrinderLogger> loggerMock;
        private CsScriptWorker worker;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = TestUtils.CreateContextMock();
            datapoolManagerMock = new Mock<IDatapoolManager>();
            processContext = TestUtils.CreateProcessContext(null, null, datapoolManagerMock.Object, grinderContextMock.Object);
            loggerMock = TestUtils.CreateLoggerMock();
            grinderContextMock.Setup(c => c.GetLogger(It.IsAny<Type>())).Returns(loggerMock.Object);
            worker = new CsScriptWorker { GrinderContext = grinderContextMock.Object, ProcessContext = processContext, DatapoolManager = datapoolManagerMock.Object };
            CSScript.Evaluator.Reset();
        }

        [TestCase]
        public void InitializeShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws<AwarenessException>(() => new CsScriptWorker { ProcessContext = processContext, DatapoolManager = datapoolManagerMock.Object }.Initialize());
        }

        [TestCase]
        public void InitializeShouldThrowExceptionWhenProcessContextIsNull()
        {
            Assert.Throws<AwarenessException>(() => new CsScriptWorker { GrinderContext = grinderContextMock.Object, DatapoolManager = datapoolManagerMock.Object }.Initialize());
        }

        [TestCase]
        public void InitializeShouldThrowExceptionWhenDatapoolManagerIsNull()
        {
            Assert.Throws<AwarenessException>(() => new CsScriptWorker { GrinderContext = grinderContextMock.Object, ProcessContext = processContext }.Initialize());
        }

        [TestCase]
        public void InitializeShouldThrowExceptionOnMissingScriptProperty()
        {
            grinderContextMock.Setup(c => c.GetProperty(worker.ScriptFileKey)).Returns((string)null);
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Missing property 'grinderscript-dotnet.csScriptWorker.script'"), () => worker.Initialize());
        }

        [TestCase]
        public void InitializeShouldThrowExceptionOnInvalidScript()
        {
            grinderContextMock.Setup(c => c.GetProperty(worker.ScriptFileKey)).Returns("CsScriptWorkerThatIsInvalid.txt");
            Assert.Throws<InternalErrorException>(() => worker.Initialize());
        }

        [TestCase]
        public void InitializeShouldLoadValidSubTypeScript()
        {
            InitializeWorkerWithValidSubTypeScript();
            Assert.That(worker.ScriptWorker, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldLoadValidDuckTypeScript()
        {
            grinderContextMock.Setup(c => c.GetProperty(worker.ScriptFileKey)).Returns("CsScriptWorkerThatIsValidDuckType.txt");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Type 'CsScriptWorkerThatIsValidDuckType', from script file 'CsScriptWorkerThatIsValidDuckType.txt', does not implement 'GrinderScript.Net.Core.IGrinderWorker'"), () => worker.Initialize());
            Assert.That(worker.ScriptWorker, Is.Not.Null);
        }

        [TestCase]
        public void InitializeShouldSetupGrinderContextAwareScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            Assert.That(worker.ScriptWorker, Is.InstanceOf<IGrinderContextAware>());
            Assert.That(((IGrinderContextAware)worker.ScriptWorker).GrinderContext, Is.SameAs(grinderContextMock.Object));
        }

        [TestCase]
        public void InitializeShouldSetupProcessContextAwareScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            Assert.That(worker.ScriptWorker, Is.InstanceOf<IProcessContextAware>());
            Assert.That(((IProcessContextAware)worker.ScriptWorker).ProcessContext, Is.SameAs(processContext));
        }

        [TestCase]
        public void InitializeShouldSetupDatapoolManagerAwareScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            Assert.That(worker.ScriptWorker, Is.InstanceOf<IDatapoolManagerAware>());
            Assert.That(((IDatapoolManagerAware)worker.ScriptWorker).DatapoolManager, Is.SameAs(datapoolManagerMock.Object));
        }

        [TestCase]
        public void InitializeShouldInitializeUndelyingScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            loggerMock.Verify(l => l.Info("CsScriptWorkerThatIsValidSubType-OnInitialize"));
        }

        [TestCase]
        public void RunShouldRunUndelyingScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            worker.Run();
            loggerMock.Verify(l => l.Info("CsScriptWorkerThatIsValidSubType-OnRun"));
        }

        [TestCase]
        public void ShutdownShouldShutdownUndelyingScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            worker.Shutdown();
            loggerMock.Verify(l => l.Info("CsScriptWorkerThatIsValidSubType-OnShutdown"));
        }

        [TestCase]
        public void ShutdownShouldClearScriptWorker()
        {
            InitializeWorkerWithValidSubTypeScript();
            Assert.That(worker.ScriptWorker, Is.Not.Null);
            worker.Shutdown();
            Assert.That((object)worker.ScriptWorker, Is.Null);
        }

        [TestCase]
        public void ShutdownShouldBeReentrant()
        {
            InitializeWorkerWithValidSubTypeScript();
            worker.Shutdown();
            worker.Shutdown();
        }

        private void InitializeWorkerWithValidSubTypeScript()
        {
            grinderContextMock.Setup(c => c.GetProperty(worker.ScriptFileKey)).Returns("CsScriptWorkerThatIsValidSubType.txt");
            worker.Initialize();
        }
    }
}
