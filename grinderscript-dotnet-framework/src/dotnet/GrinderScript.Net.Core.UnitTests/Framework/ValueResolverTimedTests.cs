#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueResolverTimedTests.cs" company="http://GrinderScript.net">
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
using System.Threading;

using GrinderScript.Net.Core.Framework;

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using NUnit.Framework;

    [TestFixture]
    public class ValueResolverTimedTests
    {
        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = TestHelpers.TestUtils.CreateLoggerMock();
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenResolveFuncIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("resolveFunc"), () => new ValueResolverTimed<bool>(null, 1));
        }

        [TestCase]
        public void CtorShouldFaileWhenTtlIsLessThanOne()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws(Is.InstanceOf<ArgumentOutOfRangeException>().With.Message.ContainsSubstring("Can not be less than 1, but was '0'"), () => new ValueResolverTimed<bool>(() => loggerMock.Object.IsDebugEnabled, 0));
            // ReSharper restore ObjectCreationAsStatement
            loggerMock.Verify(l => l.IsDebugEnabled, Times.Never());
        }

        [TestCase]
        public void CtorShouldResolveValue()
        {
            // ReSharper disable ObjectCreationAsStatement
            new ValueResolverTimed<bool>(() => loggerMock.Object.IsDebugEnabled, 1);
            // ReSharper restore ObjectCreationAsStatement
            loggerMock.Verify(l => l.IsDebugEnabled, Times.Exactly(1));
        }

        [TestCase]
        public void GetValueShouldReuseResolvedValueWithinTlt()
        {
            loggerMock.Setup(l => l.IsInfoEnabled).Returns(true);
            var resolver = new ValueResolverTimed<bool>(() => loggerMock.Object.IsInfoEnabled, 10);
            Assert.That(resolver.Value, Is.True);
            loggerMock.Verify(l => l.IsInfoEnabled, Times.Exactly(1));
        }


        [TestCase]
        public void GetValueShouldResolveValueWhenTtlIsExpired()
        {
            loggerMock.Setup(l => l.IsInfoEnabled).Returns(true);
            var resolver = new ValueResolverTimed<bool>(() => loggerMock.Object.IsInfoEnabled, 1);
            Thread.Sleep(2);
            Assert.That(resolver.Value, Is.True);
            loggerMock.Verify(l => l.IsInfoEnabled, Times.Exactly(2));
        }
    }
}