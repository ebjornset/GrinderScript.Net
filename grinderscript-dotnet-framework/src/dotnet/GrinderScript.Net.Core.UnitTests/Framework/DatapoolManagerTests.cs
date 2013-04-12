#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatapoolManagerTests.cs" company="http://GrinderScript.net">
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

using System.Collections.Generic;

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class DatapoolManagerTests
    {
        private Mock<IGrinderContext> grinderContextMock;

        private DatapoolManager datapoolManager;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = new Mock<IGrinderContext>();
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.GetProperty(Constants.ProcessCountKey, "1")).Returns("3");
            grinderContextMock.Setup(c => c.GetProperty(Constants.ThreadCountKey, "1")).Returns("4");
            datapoolManager = new DatapoolManager(grinderContextMock.Object);
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => new DatapoolManager(null));
        }

        [TestCase]
        public void CtorShouldSetGrinderContext()
        {
            var testList = new DatapoolManager(grinderContextMock.Object);
            Assert.That(testList.GrinderContext, Is.SameAs(grinderContextMock.Object));
        }

        [TestCase]
        public void GetDatapoolShouldThrowExceptionWhenNameIsEmpty()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("name"), () => datapoolManager.GetDatapool<TestValues>(" "));
        }

        [TestCase]
        public void GetDatapoolShouldThrowExceptionWhenDatapoolIsMissing()
        {
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.Contains("Unknown datapool: 'TestValues'"), () => datapoolManager.GetDatapool<TestValues>());
        }

        [TestCase]
        public void GetDatapoolShouldThrowExceptionWhenExistingDatapoolHasWrongType()
        {
            var datapoolMetatdata = CreateDatapoolMetadata();
            datapoolManager.BuildDatapool(datapoolMetatdata);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.Contains("Wrong type for datapool 'TestValues'. Expected 'System.Object', but got 'GrinderScript.Net.Core.UnitTests.Framework.DatapoolManagerTests+TestValues'"), () => datapoolManager.GetDatapool<object>("TestValues"));
        }

        [TestCase]
        public void GetDatapoolShouldWorkWhenDatapoolOfExpectedTypeExists()
        {
            var datapoolMetatdata = CreateDatapoolMetadata();
            datapoolManager.BuildDatapool(datapoolMetatdata);
            Assert.That(datapoolManager.GetDatapool<TestValues>() != null);
        }

        [TestCase]
        public void BuildDatapoolShouldThrowExceptionWhenDatapoolAlreadyExists()
        {
            var datapoolMetatdata = CreateDatapoolMetadata();
            datapoolManager.BuildDatapool(datapoolMetatdata);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.Contains("Duplicate datapool: 'TestValues'"), () => datapoolManager.BuildDatapool(datapoolMetatdata));
        }

        [TestCase]
        public void ContainsDatapoolShouldThrowExceptionWhenNameIsEmpty()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("name"), () => datapoolManager.ContainsDatapool(" "));
        }

        [TestCase]
        public void ContainsDatapoolShouldBeFalseForUnexistingDatapool()
        {
            var actual = datapoolManager.ContainsDatapool(Guid.NewGuid().ToString());
            Assert.That(actual, Is.False);
        }

        [TestCase]
        public void ContainsDatapoolShouldBeTrueForExistingDatapool()
        {
            var datapoolMetatdata = CreateDatapoolMetadata();
            datapoolManager.BuildDatapool(datapoolMetatdata);
            var actual = datapoolManager.ContainsDatapool<TestValues>();
            Assert.That(actual, Is.True);
        }

        private static DefaultDatapoolMetadata<TestValues> CreateDatapoolMetadata()
        {
            var values = new List<TestValues> { new TestValues { IntValue = 1 } };
            var datapoolMetatdata = new DefaultDatapoolMetadata<TestValues>(values, false, 0, DatapoolThreadDistributionMode.ThreadShared, true);
            return datapoolMetatdata;
        }

        public class TestValues
        {
            public int IntValue { get; set; }
        }
    }
}
