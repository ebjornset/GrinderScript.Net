#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTestMetadataTests.cs" company="http://GrinderScript.net">
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

using Moq;

using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests
{
    [TestFixture]
    public class DefaultTestMetadataTests
    {
        private Mock<IGrinderTest> testActionsMock;

        [SetUp]
        public void SetUp()
        {
            testActionsMock = new Mock<IGrinderTest>();
        }

        [TestCase]
        public void CtorShouldRequireDescription()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("testDescription"), () => new DefaultTestMetadata(23, null, testActionsMock.Object.Run));
        }

        [TestCase]
        public void CtorShouldRequireTestAction()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("testAction"), () => new DefaultTestMetadata(23, "TestDescription", null));
        }

        [TestCase]
        public void CtorShouldSetTestNumber()
        {
            var metaData = new DefaultTestMetadata(23, "TestDescription", testActionsMock.Object.Run);
            Assert.That(metaData.TestNumber == 23);
        }

        [TestCase]
        public void CtorShouldSetTestDescription()
        {
            var metaData = new DefaultTestMetadata(23, "TestDescription", testActionsMock.Object.Run);
            Assert.That(metaData.TestDescription == "TestDescription");
        }

        [TestCase]
        public void CtorShouldSetTestAction()
        {
            var metaData = new DefaultTestMetadata(23, "TestDescription", testActionsMock.Object.Run);
            Assert.That(metaData.TestAction == testActionsMock.Object.Run);
        }

        [TestCase]
        public void CtorShouldSetSleepMillis()
        {
            var metaData = new DefaultTestMetadata(23, "TestDescription", testActionsMock.Object.Run, 34);
            Assert.That(metaData.SleepMillis == 34);
        }

        [TestCase]
        public void CtorShouldSetBeforeTest()
        {
            var beforeTestActionsMock = new Mock<IGrinderTest>();
            var metaData = new DefaultTestMetadata(23, "TestDescription", testActionsMock.Object.Run, 34, beforeTestActionsMock.Object.Run);
            Assert.That(metaData.BeforeTestAction == beforeTestActionsMock.Object.Run);
        }

        [TestCase]
        public void CtorShouldSetAfterTest()
        {
            var afterTestActionsMock = new Mock<IGrinderTest>();
            var metaData = new DefaultTestMetadata(23, "TestDescription", testActionsMock.Object.Run, 34, null, afterTestActionsMock.Object.Run);
            Assert.That(metaData.AfterTestAction == afterTestActionsMock.Object.Run);
        }
    }
}
