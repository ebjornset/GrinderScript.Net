#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestListTests.cs" company="http://GrinderScript.net">
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

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class TestListTests
    {
        private Mock<IProcessContext> processContextMock;
        private Mock<IGrinderContext> grinderContextMock;
        private Mock<IGrinderScriptEngine> testActionsMock;

        private DefaultTestMetadata metadata;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = new Mock<IGrinderContext>();
            processContextMock = new Mock<IProcessContext>();
            processContextMock.Setup(pc => pc.GrinderContext).Returns(grinderContextMock.Object);
            testActionsMock  = new Mock<IGrinderScriptEngine>();
            metadata = new DefaultTestMetadata(99, "TestDescription", testActionsMock.Object.Initialize);
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenProcessComtextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("processContext"), () => new TestList(null));
        }

        [TestCase]
        public void CtorShouldSetProcessContext()
        {
            var testList = new TestList(processContextMock.Object);
            Assert.That(testList.ProcessContext, Is.SameAs(processContextMock.Object));
        }

        [TestCase]
        public void AddTestFromMetadataShouldThrowExceptionWhenMetadataIsNull()
        {
            var testList = new TestList(processContextMock.Object);
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("metadata"), () => testList.AddTest((ITestMetadata)null));
        }

        [TestCase]
        public void AddTestFromMetadataShouldCreateTestWithMetadata()
        {
            var testList = new TestList(processContextMock.Object);
            var test = testList.AddTest(metadata);
            Assert.That(test.Metadata == metadata);
        }

        [TestCase]
        public void AddTestFromMetadataShouldAddTestToList()
        {
            var testList = new TestList(processContextMock.Object);
            testList.AddTest(metadata);
            Assert.That(testList.Tests.Count() == 1);
        }

        [TestCase]
        public void AddTestFromTestShouldThrowExceptionWheTestIsNull()
        {
            var testList = new TestList(processContextMock.Object);
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("test"), () => testList.AddTest((ITest)null));
        }

        [TestCase]
        public void AddExisitingTestShouldAddTestToList()
        {
            var testMock = new Mock<ITest>();
            var testList = new TestList(processContextMock.Object);
            testList.AddTest(testMock.Object);
            Assert.That(testList.Tests.Count() == 1);
        }

        [TestCase]
        public void ClearShouldClearList()
        {
            var testMock = new Mock<ITest>();
            var testList = new TestList(processContextMock.Object);
            testList.AddTest(testMock.Object);
            Assert.That(testList.Tests.Count() == 1);
            testList.Clear();
            Assert.That(!testList.Tests.Any());
        }

        [TestCase]
        public void ClearShouldClearTestsInList()
        {
            var testMock = new Mock<ITest>();
            var testList = new TestList(processContextMock.Object);
            testList.AddTest(testMock.Object);
            testList.Clear();
            testMock.Verify(t => t.Clear());
        }

        [TestCase]
        public void RunShouldRunTestsInList()
        {
            var testMock = new Mock<ITest>();
            var testList = new TestList(processContextMock.Object);
            testList.AddTest(testMock.Object);
            testList.AddTest(testMock.Object);
            testList.AddTest(testMock.Object);
            testList.Run();
            testMock.Verify(t => t.Run(), Times.Exactly(3));
        }
    }
}
