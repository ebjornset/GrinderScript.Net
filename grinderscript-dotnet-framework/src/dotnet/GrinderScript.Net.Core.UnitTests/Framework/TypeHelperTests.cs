#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelperTests.cs" company="http://GrinderScript.net">
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
    public class TypeHelperTests
    {
        private Mock<IGrinderContext> grinderContextMock;
        private Mock<IGrinderLogger> loggerMock;
        private TypeHelper typeHelper;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = TestHelpers.TestUtils.CreateContextMock();
            loggerMock = TestHelpers.TestUtils.CreateLoggerMock();
            grinderContextMock.Setup(c => c.ScriptFile).Returns(TestHelpers.TestUtils.TestsAsScriptFile);
            grinderContextMock.Setup(c => c.GetLogger(typeof(AbstractGrinderElementTests.TestElement))).Returns(loggerMock.Object);
            typeHelper = new TypeHelper(grinderContextMock.Object);
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => new TypeHelper(null));
        }

        [TestCase]
        public void CtorShouldSetLogger()
        {
            Assert.That(typeHelper.Logger, Is.Not.Null);
        }

        [TestCase]
        public void CreateTargetTypeFromNameWithValidTypeShouldReturnInstance()
        {
            var instance = typeHelper.CreateTargetTypeFromName<AbstractGrinderElement>(typeof(AbstractGrinderElementTests.TestElement).FullName);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase]
        public void CreateTargetTypeFromNameWithUnknownTypeShouldThrowException()
        {
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Unknown type: 'NotAValidTypeIPresume'"), () => typeHelper.CreateTargetTypeFromName<AbstractGrinderElement>("NotAValidTypeIPresume"));
        }

        [TestCase]
        public void CreateTargetTypeFromNameWithUnassignableTypeShouldThrowException()
        {
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Type 'GrinderScript.Net.Core.UnitTests.Framework.AbstractGrinderElementTests+TestElement' is not 'GrinderScript.Net.Core.IGrinderTest'"), () => typeHelper.CreateTargetTypeFromName<IGrinderTest>(typeof(AbstractGrinderElementTests.TestElement).FullName));
        }

        [TestCase]
        public void CreateTargetTypeFromPropertyWithValidTypeShouldReturnInstance()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns(typeof(AbstractGrinderElementTests.TestElement).FullName);
            var instance = typeHelper.CreateTargetTypeFromProperty<AbstractGrinderElement>("TestTypeKey", null);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase]
        public void CreateTargetTypeFromPropertyWithUnknownTypeShouldThrowException()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns("NotAValidTypeIPresume");
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Unknown type: 'NotAValidTypeIPresume'"), () => typeHelper.CreateTargetTypeFromProperty<AbstractGrinderElement>("TestTypeKey", null));
        }

        [TestCase]
        public void CreateTargetTypeFromPropertyWithUnassignableTypeShouldThrowException()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns(typeof(AbstractGrinderElementTests.TestElement).FullName);
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Type 'GrinderScript.Net.Core.UnitTests.Framework.AbstractGrinderElementTests+TestElement' is not 'GrinderScript.Net.Core.IGrinderTest'"), () => typeHelper.CreateTargetTypeFromProperty<IGrinderTest>("TestTypeKey", null));
        }

        [TestCase]
        public void CreateTargetTypeFromMissingPropertyWithDefaultTypeShouldReturnDefaultTypeInstance()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns<string>(null);
            var instance = typeHelper.CreateTargetTypeFromProperty<AbstractGrinderElement>("TestTypeKey", typeof(AbstractGrinderElementTests.TestElement));
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.GetType(), Is.SameAs(typeof(AbstractGrinderElementTests.TestElement)));
        }

        [TestCase]
        public void CreateTargetTypeFromMissingPropertyAndNoDefaultTypeShouldThrowException()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns<string>(null);
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Missing property 'TestTypeKey'"), () => typeHelper.CreateTargetTypeFromProperty<AbstractGrinderElement>("TestTypeKey", null));
        }

        [TestCase]
        public void CreateTargetTypeFromMissingPropertyAndUnassignableDefaultTypeShouldThrowException()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns<string>(null);
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Type 'GrinderScript.Net.Core.UnitTests.Framework.AbstractGrinderElementTests+TestElement' is not 'GrinderScript.Net.Core.IGrinderTest'"), () => typeHelper.CreateTargetTypeFromProperty<IGrinderTest>("TestTypeKey", typeof(AbstractGrinderElementTests.TestElement)));
        }

        [TestCase]
        public void LoadTargetTypeFromPropertyWithValidTypeShouldReturnType()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns(typeof(AbstractGrinderElementTests.TestElement).FullName);
            var type = typeHelper.LoadTargetTypeFromProperty("TestTypeKey", null);
            Assert.That(type, Is.Not.Null);
            Assert.That(type, Is.SameAs(typeof(AbstractGrinderElementTests.TestElement)));
        }

        [TestCase]
        public void LoadTargetTypeFromMissingPropertyWithDefaultTypeShouldReturnDefaultTypeInstance()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns<string>(null);
            var type = typeHelper.LoadTargetTypeFromProperty("TestTypeKey", typeof(AbstractGrinderElementTests.TestElement));
            Assert.That(type, Is.Not.Null);
            Assert.That(type, Is.SameAs(typeof(AbstractGrinderElementTests.TestElement)));
        }

        [TestCase]
        public void LoadTargetTypeFromMissingPropertyAndNoDefaultTypeShouldThrowException()
        {
            grinderContextMock.Setup(c => c.GetProperty("TestTypeKey")).Returns<string>(null);
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Missing property 'TestTypeKey'"), () => typeHelper.LoadTargetTypeFromProperty("TestTypeKey", null));
        }
    }
}
