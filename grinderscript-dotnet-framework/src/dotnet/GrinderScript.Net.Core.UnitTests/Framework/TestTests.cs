#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTests.cs" company="http://GrinderScript.net">
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

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class TestTests
    {
        private Mock<IGrinderContext> grinderContextMock;
        private Mock<ITestMetadata> metatdataMock;
        private Mock<IGrinderTest> grinderTestMock;
 
        [SetUp]
        public void SetUp()
        {
            grinderTestMock = new Mock<IGrinderTest>();
            metatdataMock = new Mock<ITestMetadata>();
            metatdataMock.SetupGet(md => md.TestAction).Returns(() => grinderTestMock.Object.Run);
            grinderContextMock = new Mock<IGrinderContext>();
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenMetadataIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("metadata"), () => new Test(null, grinderContextMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => new Test(metatdataMock.Object, null));
        }

        [TestCase]
        public void CtorShouldSetMetadata()
        {
            var test = new Test(metatdataMock.Object, grinderContextMock.Object);
            Assert.That(test.Metadata, Is.SameAs(metatdataMock.Object));
        }

        [TestCase]
        public void CtorShouldCreateWrappedTestFromMetaData()
        {
            metatdataMock.Setup(md => md.TestNumber).Returns(98);
            metatdataMock.Setup(md => md.TestDescription).Returns("TestDescription");
            grinderContextMock.Setup(gc => gc.CreateTest(98, "TestDescription", It.Is<TestActionWrapper>(t => t.TestAction == grinderTestMock.Object.Run))).Returns(grinderTestMock.Object).Verifiable();
            var test = new Test(metatdataMock.Object, grinderContextMock.Object);
            Assert.That(test.Underlying == grinderTestMock.Object);
            grinderTestMock.Verify();
        }

        [TestCase]
        public void ClearShouldClearUnderlying()
        {
            var test = new Test(metatdataMock.Object, grinderContextMock.Object) { Underlying = new Mock<IGrinderTest>().Object };
            Assert.That(test.Underlying != null);
            test.Clear();
            Assert.That(test.Underlying == null);
        }

        [TestCase]
        public void RunShouldRunUnderlying()
        {
            var test = new Test(metatdataMock.Object, grinderContextMock.Object) { Underlying = grinderTestMock.Object };
            test.Run();
            grinderTestMock.Verify(gt => gt.Run());
        }

        [TestCase]
        public void RunShouldRunBeforeTestAction()
        {
            var beforeTestActionMock = new Mock<IGrinderTest>();
            metatdataMock.SetupGet(md => md.BeforeTestAction).Returns(() => beforeTestActionMock.Object.Run);
            var test = new Test(metatdataMock.Object, grinderContextMock.Object) { Underlying = new Mock<IGrinderTest>().Object };
            test.Run();
            beforeTestActionMock.Verify(a => a.Run());
        }

        [TestCase]
        public void RunShouldRunAfterTestAction()
        {
            var afterTestActionMock = new Mock<IGrinderTest>();
            metatdataMock.SetupGet(md => md.AfterTestAction).Returns(() => afterTestActionMock.Object.Run);
            var test = new Test(metatdataMock.Object, grinderContextMock.Object) { Underlying = new Mock<IGrinderTest>().Object };
            test.Run();
            afterTestActionMock.Verify(a => a.Run());
        }

        [TestCase]
        public void RunShouldSleepWhenSleepMillisIsGtZero()
        {
            metatdataMock.Setup(md => md.SleepMillis).Returns(1);
            var test = new Test(metatdataMock.Object, grinderContextMock.Object) { Underlying = grinderTestMock.Object };
            test.Run();
            grinderContextMock.Verify(gc => gc.Sleep(1));
        }

        [TestCase]
        public void RunShouldNotSleepWhenSleepMillisIsNotGtZero()
        {
            metatdataMock.Setup(md => md.SleepMillis).Returns(0);
            var test = new Test(metatdataMock.Object, grinderContextMock.Object) { Underlying = grinderTestMock.Object };
            test.Run();
            grinderContextMock.Verify(gc => gc.Sleep(It.IsAny<int>()), Times.Never());
        }
    }
}
