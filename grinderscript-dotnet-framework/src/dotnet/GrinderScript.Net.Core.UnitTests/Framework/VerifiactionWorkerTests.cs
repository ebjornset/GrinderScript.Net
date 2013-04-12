#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerifiactiontWorkerTests.cs" company="http://GrinderScript.net">
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
using System.Linq;

using GrinderScript.Net.Core.Framework;
using GrinderScript.Net.Core.UnitTests.TestHelpers;

using Moq;

using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    [TestFixture]
    public class VerifiactiontWorkerTests
    {
        private VerificationWorker worker;

        private Mock<IGrinderContext> grinderContextMock;

        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = TestUtils.CreateContextMock();
            loggerMock = TestUtils.CreateLoggerMock();
            grinderContextMock.Setup(c => c.GetLogger(typeof(VerificationWorker))).Returns(loggerMock.Object);
            grinderContextMock.Setup(c => c.CreateTest(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IGrinderTest>())).Returns((int i, string s, IGrinderTest t) => t);
            var processContext = TestUtils.CreateProcessContext(null, null, null, grinderContextMock.Object);
            worker = new VerificationWorker { ProcessContext = processContext, GrinderContext = grinderContextMock.Object };
        }

        [TestCase]
        public void InitializeShouldAddOneTestToTestListWhenPropertyIsMissing()
        {
            worker.Initialize();
            Assert.That(worker.TestList.Tests.Count(), Is.EqualTo(1));
        }

        [TestCase]
        public void InitializeShouldThrowExceptionWhenPropertyIsLtOne()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.VerificationWorkerTestsPrRunKey, "1")).Returns("0");
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Property 'grinderscript-dotnet.verificationWorker.testsPrRun' should be > 0, but was 0"), () => worker.Initialize());
        }

        [TestCase]
        public void InitializeAddTestToTestListAccordingToProperty()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.VerificationWorkerTestsPrRunKey, "1")).Returns("3");
            worker.Initialize();
            Assert.That(worker.TestList.Tests.Count(), Is.EqualTo(3));
        }
    }
}
