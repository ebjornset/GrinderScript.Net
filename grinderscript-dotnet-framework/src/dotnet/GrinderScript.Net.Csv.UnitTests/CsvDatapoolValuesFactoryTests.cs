#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvDatapoolValuesFactoryTests.cs" company="http://GrinderScript.net">
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

using CsvHelper.Configuration;

using GrinderScript.Net.Core;
using GrinderScript.Net.Core.Framework;

using Moq;

using System;

using NUnit.Framework;

namespace GrinderScript.Net.Csv.UnitTests
{
    [TestFixture]
    public class CsvDatapoolValuesFactoryTests
    {
        private Mock<IGrinderContext> grinderContextMock;
        private CsvDatapoolValuesFactory<TestValues> factory;
 
        [SetUp]
        public void SetUp()
        {
            factory = new CsvDatapoolValuesFactory<TestValues>();
            grinderContextMock = new Mock<IGrinderContext>();
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValues>("csvFile"), "TestValues.csv")).Returns("TestValues.csv");
        }

        [TestCase]
        public void CreateValuesShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => factory.CreateValues((IGrinderContext)null));
        }

        [TestCase]
        public void CreateValuesWithNameShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => factory.CreateValues(null, "TestValues"));
        }

        [TestCase]
        public void CreateValuesWithFileNameShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("fileName"), () => factory.CreateValues(" "));
        }

        [TestCase]
        public void CreateDatapoolWithNameShouldThrowExceptionWhenNameIsMissing()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("name"), () => factory.CreateValues(grinderContextMock.Object, " "));
        }

        [TestCase]
        public void CreateValuesShouldCreateValuesFromLinesInTestValuesCsvFile()
        {
            var actual = factory.CreateValues(grinderContextMock.Object);
            Assert.That(actual.Count == 2);
            var value1 = actual[0];
            Assert.That(value1.StringProp1 == "StringProp1;");
            Assert.That(value1.StringProp2 == "stringprop2");
            Assert.That(value1.IntProp == 23);
            Assert.That(value1.LongProp == 234);
            Assert.That(value1.BoolProp);
            var value2 = actual[1];
            Assert.That(value2.StringProp1 == ";StringProp1");
            Assert.That(value2.StringProp2 == "stringprop2 ");
            Assert.That(value2.IntProp == 34);
            Assert.That(value2.LongProp == 345);
            Assert.That(!value2.BoolProp);
        }

        [TestCase]
        public void CreateValuesShouldUseCsvFileFromProperty()
        {
            grinderContextMock.Setup(c => c.GetProperty(DatapoolFactory.GetPropertyKey<TestValues>("csvFile"), "TestValues.csv")).Returns("TestValuesCsvFileWithADifferentNameThanTheType.csv");
            var actual = factory.CreateValues(grinderContextMock.Object);
            Assert.That(actual.Count == 1);
            var value1 = actual[0];
            Assert.That(value1.StringProp1 == "StringProp1;");
            Assert.That(value1.StringProp2 == "stringprop2");
            Assert.That(value1.IntProp == 23);
            Assert.That(value1.LongProp == 234);
            Assert.That(value1.BoolProp);
        }

        [TestCase]
        public void CreateValuesShouldUseCsvFileFromParameter()
        {
            var actual = factory.CreateValues("TestValuesCsvFileWithADifferentNameThanTheType.csv");
            Assert.That(actual.Count == 1);
            var value1 = actual[0];
            Assert.That(value1.StringProp1 == "StringProp1;");
            Assert.That(value1.StringProp2 == "stringprop2");
            Assert.That(value1.IntProp == 23);
            Assert.That(value1.LongProp == 234);
            Assert.That(value1.BoolProp);
        }

        public class TestValues
        {
            public string StringProp1 { get; set; }
            public int IntProp { get; set; }
            public long LongProp { get; set; }
            public bool BoolProp { get; set; }
            public string StringProp2 { get; set; }
            [CsvField(Ignore = true)]
            public string IgnoredProp { get; set; }
        }
    }
}
