﻿#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueResolverAlwaysTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.Framework;

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using NUnit.Framework;

    [TestFixture]
    public class ValueResolverAlwaysTests
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
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("resolveFunc"), () => new ValueResolverAlways<bool>(null));
        }

        [TestCase]
        public void CtorShouldNotResolveValue()
        {
            // ReSharper disable ObjectCreationAsStatement
            new ValueResolverAlways<bool>(() => loggerMock.Object.IsDebugEnabled);
            // ReSharper restore ObjectCreationAsStatement
            loggerMock.Verify(l => l.IsDebugEnabled, Times.Never());
        }

        [TestCase]
        public void GetValueShouldReuseResolvedValue()
        {
            loggerMock.Setup(l => l.IsInfoEnabled).Returns(true);
            var resolver = new ValueResolverAlways<bool>(() => loggerMock.Object.IsInfoEnabled);
            Assert.That(resolver.Value, Is.True);
            Assert.That(resolver.Value, Is.True);
            Assert.That(resolver.Value, Is.True);
            loggerMock.Verify(l => l.IsInfoEnabled, Times.Exactly(3));
        }
    }
}