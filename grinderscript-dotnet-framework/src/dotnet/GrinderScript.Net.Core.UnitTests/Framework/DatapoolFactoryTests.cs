#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatapoolFactoryTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.UnitTests.TestHelpers;

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class DatapoolFactoryTests
    {
        private Mock<IGrinderContext> grinderContextMock;
        private Mock<IDatapoolManager> datapoolManagerMock;

        private DatapoolFactory datapoolFactoy;

        private static IList<TestValue> testValues;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = TestUtils.CreateContextMock();
            grinderContextMock.Setup(c => c.ScriptFile).Returns(TestUtils.TestsAsScriptFile);
            datapoolManagerMock = new Mock<IDatapoolManager>();
            datapoolFactoy = new DatapoolFactory(grinderContextMock.Object, datapoolManagerMock.Object);
            testValues = new List<TestValue> { new TestValue() };
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => new DatapoolFactory(null, datapoolManagerMock.Object));
        }

        [TestCase]
        public void CtorShouldSetGrinderContext()
        {
            var datapoolFactory = new DatapoolFactory(grinderContextMock.Object, datapoolManagerMock.Object);
            Assert.That(datapoolFactory.GrinderContext, Is.SameAs(grinderContextMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenDatapoolManagerIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("datapoolManager"), () => new DatapoolFactory(grinderContextMock.Object, null));
        }

        [TestCase]
        public void CtorShouldSetDatapoolManager()
        {
            var datapoolFactory = new DatapoolFactory(grinderContextMock.Object, datapoolManagerMock.Object);
            Assert.That(datapoolFactory.DatapoolManager, Is.SameAs(datapoolManagerMock.Object));
        }

        [TestCase]
        public void CtorShouldSetTypeHelper()
        {
            var datapoolFactory = new DatapoolFactory(grinderContextMock.Object, datapoolManagerMock.Object);
            Assert.That(datapoolFactory.TypeHelper, Is.Not.Null);
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenNameIsMissing()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("name"), () => datapoolFactoy.CreateDatapool<TestValue>(" "));
        }

        [TestCase]
        public void CreateDatapoolWithValuesFactoryShouldThrowExceptionWhenNameIsMissing()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("name"), () => datapoolFactoy.CreateDatapool(new Mock<IDatapoolValuesFactory<TestValue>>().Object, " "));
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenValuesFactoryIsMissing()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("datapoolValuesFactory"), () => datapoolFactoy.CreateDatapool((IDatapoolValuesFactory<TestValue>)null));
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenMetatdataIsMissing()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("datapoolMetadata"), () => datapoolFactoy.CreateDatapool((IDatapoolMetatdata<TestValue>)null));
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenValueTypeNamePropertyIsMissing()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("valueType"))).Returns<string>(null);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Missing property 'grinderscript-dotnet.datapool.TestValue.valueType'"), () => datapoolFactoy.CreateDatapool<TestValue>());
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenValueTypeNamePropertyIsWrong()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("valueType"))).Returns(typeof(DatapoolFactoryTests).FullName);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Type 'GrinderScript.Net.Core.UnitTests.Framework.DatapoolFactoryTests' is not 'GrinderScript.Net.Core.UnitTests.Framework.DatapoolFactoryTests+TestValue'"), () => datapoolFactoy.CreateDatapool<TestValue>());
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenFactoryTypeNamePropertyIsMissing()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("valueType"))).Returns(typeof(TestValue).FullName);
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("factoryType"))).Returns<string>(null);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Missing property 'grinderscript-dotnet.datapool.TestValue.factoryType'"), () => datapoolFactoy.CreateDatapool<TestValue>());
        }

        [TestCase]
        public void CreateDatapoolShouldThrowExceptionWhenFactoryTypeNamePropertyIsNotImplementingInterface()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("valueType"))).Returns(typeof(TestValue).FullName);
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("factoryType"))).Returns(typeof(DefaultDatapoolMetadata<>).FullName);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.Contains("Type 'GrinderScript.Net.Core.DefaultDatapoolMetadata`1' is not 'GrinderScript.Net.Core.IDatapoolValuesFactory`1[[GrinderScript.Net.Core.UnitTests.Framework.DatapoolFactoryTests+TestValue"), () => datapoolFactoy.CreateDatapool<TestValue>());
        }

        [TestCase]
        public void CreateDatapoolShouldLoadValuesFromFactory()
        {
            SetupValueFactoryMockAndCreateTestDatapool();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.Is<IDatapoolMetatdata<TestValue>>(m => m.Values == testValues)));
        }

        [TestCase]
        public void CreateDatapoolShouldUseRandomPropertyValue()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("random"), bool.FalseString)).Returns(bool.TrueString);
            SetupValueFactoryMockAndCreateTestDatapool();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.Is<IDatapoolMetatdata<TestValue>>(m => m.IsRandom)));
        }

        [TestCase]
        public void CreateDatapoolShouldUseSeedPropertyValue()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("seed"), It.IsAny<string>())).Returns("13");
            SetupValueFactoryMockAndCreateTestDatapool();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.Is<IDatapoolMetatdata<TestValue>>(m => m.Seed == 13)));
        }

        [TestCase]
        public void CreateDatapoolShouldUseDistributionModePropertyValue()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("distributionMode"), DatapoolThreadDistributionMode.ThreadShared.ToString())).Returns(DatapoolThreadDistributionMode.ThreadComplete.ToString());
            SetupValueFactoryMockAndCreateTestDatapool();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.Is<IDatapoolMetatdata<TestValue>>(m => m.DistributionMode == DatapoolThreadDistributionMode.ThreadComplete)));
        }

        [TestCase]
        public void CreateDatapoolShouldUseCircularPropertyValue()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("circular"), bool.TrueString)).Returns(bool.FalseString);
            SetupValueFactoryMockAndCreateTestDatapool();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.Is<IDatapoolMetatdata<TestValue>>(m => !m.IsCircular)));
        }

        [TestCase]
        public void CreateDatapoolShouldAskDatapoolManagerToBuildDatapoolWithCorrectProperties()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("valueType"))).Returns(typeof(TestValue).FullName);
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValue>("factoryType"))).Returns(typeof(TestValueFactory<>).FullName);
            datapoolFactoy.CreateDatapool<TestValue>();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.IsAny<IDatapoolMetatdata<TestValue>>()));
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldUseFirstElementValueWhenPropertyIsProvied()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("firstElement"), "1")).Returns("3");
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            grinderContextMock.Verify(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("1")), Times.Never());
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldUseDefaultFirstElementWhenPropertyIsMissing()
        {
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            grinderContextMock.Verify(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("1")));
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldUseLastElementValueWhenPropertyIsProvided()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("lastElement"), "100")).Returns("3");
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            grinderContextMock.Verify(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("100")), Times.Never());
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldUseDefaultLastElementWhenPropertyIsMissing()
        {
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            grinderContextMock.Verify(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("1")));
        }

        [TestCase]
        public void FirstElementPropertyShouldBeGreaterThanZero()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("firstElement"), "1")).Returns("0");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Property 'grinderscript-dotnet.datapoolFactory.firstElement' should be > 0, but was 0"), () => datapoolFactoy.CreateMissingDatapoolsFromProperties());
        }

        [TestCase]
        public void LastElementPropertyShouldBeNotBeLessThanFirstElement()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("lastElement"), "100")).Returns("0");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Property 'grinderscript-dotnet.datapoolFactory.lastElement' can not be < 'grinderscript-dotnet.datapoolFactory.firstElement' (1), but was 0"), () => datapoolFactoy.CreateMissingDatapoolsFromProperties());
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldCheckNameWithDatapoolManager()
        {
            const string DatapoolName = "TestPoolName1";
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("1"))).Returns(DatapoolName);
            datapoolManagerMock.Setup(dm => dm.ContainsDatapool(DatapoolName)).Returns(true).Verifiable();
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            datapoolManagerMock.Verify();
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldThrowExceptionWhenValueTypenNamePropertyIsMissing()
        {
            const string DatapoolName = "TestPoolName4";
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("4"))).Returns(DatapoolName);
            datapoolManagerMock.Setup(dm => dm.ContainsDatapool(DatapoolName)).Returns(false);
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey(DatapoolName, "valueType"))).Returns<string>(null);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Missing property 'grinderscript-dotnet.datapool.TestPoolName4.valueType'"), () => datapoolFactoy.CreateMissingDatapoolsFromProperties());
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldAddNonExitingDatapoolToDatapoolManager()
        {
            const string DatapoolName = "TestPoolName2";
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("2"))).Returns(DatapoolName);
            datapoolManagerMock.Setup(dm => dm.ContainsDatapool(DatapoolName)).Returns(false);
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey(DatapoolName, "valueType"))).Returns(typeof(TestValue).FullName);
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey(DatapoolName, "factoryType"))).Returns(typeof(TestValueFactory<>).FullName);
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.IsAny<IDatapoolMetatdata<TestValue>>()));
        }

        [TestCase]
        public void CreateMissingDatapoolsFromPropertiesShouldNotAddExistingDatapoolToDatapoolManager()
        {
            const string DatapoolName = "TestPoolName3";
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetFactoryPropertyKey("3"))).Returns(DatapoolName);
            datapoolManagerMock.Setup(dm => dm.ContainsDatapool(DatapoolName)).Returns(true);
            datapoolFactoy.CreateMissingDatapoolsFromProperties();
            datapoolManagerMock.Verify(dm => dm.BuildDatapool(It.IsAny<IDatapoolMetatdata<TestValue>>()), Times.Never());
        }

        private void SetupValueFactoryMockAndCreateTestDatapool()
        {
            var valuesFactoryMock = new Mock<IDatapoolValuesFactory<TestValue>>();
            valuesFactoryMock.Setup(vf => vf.CreateValues(grinderContextMock.Object, It.IsAny<string>())).Returns(testValues);
            datapoolFactoy.CreateDatapool(valuesFactoryMock.Object);
        }

        public class TestValue
        {
        }

        public class TestValueFactory<T> : IDatapoolValuesFactory<T> where T : class
        {
            public IList<T> CreateValues(IGrinderContext context, string name)
            {
                IList<TestValue> result = testValues ?? new List<TestValue> { new TestValue() };
                return (IList<T>)result;
            }
        }
    }
}
