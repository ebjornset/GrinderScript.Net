#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerFacadeTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.UnitTests.TestHelpers;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class LoggerFacadeTests
    {
        private Mock<IGrinderLogger> underlyingLoggerMock;
        private Mock<IGrinderContext> grinderContextMock;

        private Mock<IValueResolver<bool>> isErrorEnabledResolverMock;
        private Mock<IValueResolver<bool>> isWarnEnabledResolverMock;
        private Mock<IValueResolver<bool>> isInfoEnabledResolverMock;
        private Mock<IValueResolver<bool>> isDebugEnabledResolverMock;
        private Mock<IValueResolver<bool>> isTraceEnabledResolverMock;

        private LoggerFacade logger;

        [SetUp]
        public void SetUp()
        {
            underlyingLoggerMock = new Mock<IGrinderLogger>();
            grinderContextMock = TestUtils.CreateContextMock();
            logger = new LoggerFacade(underlyingLoggerMock.Object, grinderContextMock.Object);
            isErrorEnabledResolverMock = new Mock<IValueResolver<bool>>();
            isWarnEnabledResolverMock = new Mock<IValueResolver<bool>>();
            isInfoEnabledResolverMock = new Mock<IValueResolver<bool>>();
            isDebugEnabledResolverMock = new Mock<IValueResolver<bool>>();
            isTraceEnabledResolverMock = new Mock<IValueResolver<bool>>();
            logger.IsErrorEnabledResolver = isErrorEnabledResolverMock.Object;
            logger.IsWarnEnabledResolver = isWarnEnabledResolverMock.Object;
            logger.IsInfoEnabledResolver = isInfoEnabledResolverMock.Object;
            logger.IsDebugEnabledResolver = isDebugEnabledResolverMock.Object;
            logger.IsTraceEnabledResolver = isTraceEnabledResolverMock.Object;
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenUnderlyingIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("underlying"), () => new LoggerFacade(null, grinderContextMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => new LoggerFacade(underlyingLoggerMock.Object, null));
        }

        [TestCase]
        public void CtorShouldSetupValuResolverOnceWhenLoggerEnabledCacheTtlIsLessThanZero()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.LoggerEnabledCacheTtlKey, "-1")).Returns("-1");
            var localLogger = new LoggerFacade(underlyingLoggerMock.Object, grinderContextMock.Object);
            AssertThatValueResolversAreOfType(localLogger, typeof(ValueResolverOnce<bool>));
        }

        [TestCase]
        public void CtorShouldSetupValuResolverAlwaysWhenLoggerEnabledCacheTtlIsEqualToZero()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.LoggerEnabledCacheTtlKey, "-1")).Returns("0");
            var localLogger = new LoggerFacade(underlyingLoggerMock.Object, grinderContextMock.Object);
            AssertThatValueResolversAreOfType(localLogger, typeof(ValueResolverAlways<bool>));
        }

        [TestCase]
        public void CtorShouldSetupValuResolverTimedWhenLoggerEnabledCacheTtlIsGreaterThenZero()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.LoggerEnabledCacheTtlKey, "-1")).Returns("1");
            var localLogger = new LoggerFacade(underlyingLoggerMock.Object, grinderContextMock.Object);
            AssertThatValueResolversAreOfType(localLogger, typeof(ValueResolverTimed<bool>));
        }

        [TestCase]
        public void IsTraceEnableShouldCallUnderlyingIsTraceEnabled()
        {
            underlyingLoggerMock.SetupGet(l => l.IsTraceEnabled).Returns(true);
            isTraceEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsTraceEnabled);
            Assert.That(logger.IsTraceEnabled);
            underlyingLoggerMock.VerifyGet(l => l.IsTraceEnabled);
        }

        [TestCase]
        public void IsDebugEnableShouldCallUnderlyingIsDebugEnabled()
        {
            underlyingLoggerMock.SetupGet(l => l.IsDebugEnabled).Returns(true);
            isDebugEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsDebugEnabled);
            Assert.That(logger.IsDebugEnabled);
            underlyingLoggerMock.VerifyGet(l => l.IsDebugEnabled);
        }

        [TestCase]
        public void IsInfoEnableShouldCallUnderlyingIsInfoEnabled()
        {
            underlyingLoggerMock.SetupGet(l => l.IsInfoEnabled).Returns(true);
            isInfoEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsInfoEnabled);
            Assert.That(logger.IsInfoEnabled);
            underlyingLoggerMock.VerifyGet(l => l.IsInfoEnabled);
        }

        [TestCase]
        public void IsWarnEnableShouldCallUnderlyingIsWarnEnabled()
        {
            underlyingLoggerMock.SetupGet(l => l.IsWarnEnabled).Returns(true);
            isWarnEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsWarnEnabled);
            Assert.That(logger.IsWarnEnabled);
            underlyingLoggerMock.VerifyGet(l => l.IsWarnEnabled);
        }

        [TestCase]
        public void IsErrorEnableShouldCallUnderlyingIsErrorEnabled()
        {
            underlyingLoggerMock.SetupGet(l => l.IsErrorEnabled).Returns(true);
            isErrorEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsErrorEnabled);
            Assert.That(logger.IsErrorEnabled);
            underlyingLoggerMock.VerifyGet(l => l.IsErrorEnabled);
        }

        [TestCase]
        public void TraceWithMsgShouldNotCallUnderlyingTraceWhenTraceIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsTraceEnabled).Returns(false);
            isTraceEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsTraceEnabled);
            logger.Trace("msg");
            underlyingLoggerMock.Verify(l => l.Trace(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void TraceWithMsgShouldCallUnderlyingTraceWhenTraceIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsTraceEnabled).Returns(true);
            isTraceEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsTraceEnabled);
            logger.Trace("msg");
            underlyingLoggerMock.Verify(l => l.Trace("msg"));
        }

        [TestCase]
        public void TraceWithMsgAndExceptionShouldNotCallUnderlyingTraceWhenTraceIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsTraceEnabled).Returns(false);
            isTraceEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsTraceEnabled);
            var ex = new Exception();
            logger.Trace("msg", ex);
            underlyingLoggerMock.Verify(l => l.Trace(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestCase]
        public void TraceWithMsgAndExceptionShouldCallUnderlyingTraceWhenTraceIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsTraceEnabled).Returns(true);
            isTraceEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsTraceEnabled);
            var ex = new Exception();
            logger.Trace("msg", ex);
            underlyingLoggerMock.Verify(l => l.Trace("msg", ex));
        }

        [TestCase]
        public void DebugWithMsgShouldNotCallUnderlyingDebugWhenDebugIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsDebugEnabled).Returns(false);
            isDebugEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsDebugEnabled);
            logger.Debug("msg");
            underlyingLoggerMock.Verify(l => l.Debug(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void DebugWithMsgShouldCallUnderlyingDebugWhenDebugIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsDebugEnabled).Returns(true);
            isDebugEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsDebugEnabled);
            logger.Debug("msg");
            underlyingLoggerMock.Verify(l => l.Debug("msg"));
        }

        [TestCase]
        public void DebugWithMsgAndExceptionShouldNotCallUnderlyingDebugWhenDebugIsDiabled()
        {
            underlyingLoggerMock.Setup(l => l.IsDebugEnabled).Returns(false);
            isDebugEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsDebugEnabled);
            var ex = new Exception();
            logger.Debug("msg", ex);
            underlyingLoggerMock.Verify(l => l.Debug(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestCase]
        public void DebugWithMsgAndExceptionShoulCallUnderlyingDebugWhenDebugIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsDebugEnabled).Returns(true);
            isDebugEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsDebugEnabled);
            var ex = new Exception();
            logger.Debug("msg", ex);
            underlyingLoggerMock.Verify(l => l.Debug("msg", ex));
        }

        [TestCase]
        public void InfoWithMsgShouldNotCallUnderlyingInfoWhenInfoIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsInfoEnabled).Returns(false);
            isInfoEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsInfoEnabled);
            logger.Info("msg");
            underlyingLoggerMock.Verify(l => l.Info(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void InfoWithMsgShouldCallUnderlyingInfoWhenInfoIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsInfoEnabled).Returns(true);
            isInfoEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsInfoEnabled);
            logger.Info("msg");
            underlyingLoggerMock.Verify(l => l.Info("msg"));
        }

        [TestCase]
        public void InfoWithMsgAndExceptionShouldNotCallUnderlyingInfoWhenInfoIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsInfoEnabled).Returns(false);
            isInfoEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsInfoEnabled);
            var ex = new Exception();
            logger.Info("msg", ex);
            underlyingLoggerMock.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestCase]
        public void InfoWithMsgAndExceptionShouldCallUnderlyingInfoWhenInfoIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsInfoEnabled).Returns(true);
            isInfoEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsInfoEnabled);
            var ex = new Exception();
            logger.Info("msg", ex);
            underlyingLoggerMock.Verify(l => l.Info("msg", ex));
        }

        [TestCase]
        public void WarnWithMsgShouldNotCallUnderlyingWarnWhenWarnIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsWarnEnabled).Returns(false);
            isWarnEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsWarnEnabled);
            logger.Warn("msg");
            underlyingLoggerMock.Verify(l => l.Warn(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void WarnWithMsgShouldCallUnderlyingWarnWhenWarnIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsWarnEnabled).Returns(true);
            isWarnEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsWarnEnabled);
            logger.Warn("msg");
            underlyingLoggerMock.Verify(l => l.Warn("msg"));
        }

        [TestCase]
        public void WarnWithMsgAndExceptionShouldNotCallUnderlyingWarnWhenWarnIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsWarnEnabled).Returns(false);
            isWarnEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsWarnEnabled);
            var ex = new Exception();
            logger.Warn("msg", ex);
            underlyingLoggerMock.Verify(l => l.Warn(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestCase]
        public void WarnWithMsgAndExceptionShouldCallUnderlyingWarnWhenWarnIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsWarnEnabled).Returns(true);
            isWarnEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsWarnEnabled);
            var ex = new Exception();
            logger.Warn("msg", ex);
            underlyingLoggerMock.Verify(l => l.Warn("msg", ex));
        }

        [TestCase]
        public void ErrorWithMsgShouldNotCallUnderlyingErrorWhenErrorIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsErrorEnabled).Returns(false);
            isErrorEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsErrorEnabled);
            logger.Error("msg");
            underlyingLoggerMock.Verify(l => l.Error(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void ErrorWithMsgShouldCallUnderlyingErrorWhenErrorIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsErrorEnabled).Returns(true);
            isErrorEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsErrorEnabled);
            logger.Error("msg");
            underlyingLoggerMock.Verify(l => l.Error("msg"));
        }

        [TestCase]
        public void ErrorWithMsgAndExceptionShouldNotCallUnderlyingErrorWhenErrorIsDisabled()
        {
            underlyingLoggerMock.Setup(l => l.IsErrorEnabled).Returns(false);
            isErrorEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsErrorEnabled);
            var ex = new Exception();
            logger.Error("msg", ex);
            underlyingLoggerMock.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [TestCase]
        public void ErrorWithMsgAndExceptionShouldCallUnderlyingErrorWhenErrorIsEnabled()
        {
            underlyingLoggerMock.Setup(l => l.IsErrorEnabled).Returns(true);
            isErrorEnabledResolverMock.Setup(r => r.Value).Returns(underlyingLoggerMock.Object.IsErrorEnabled);
            var ex = new Exception();
            logger.Error("msg", ex);
            underlyingLoggerMock.Verify(l => l.Error("msg", ex));
        }

        private static void AssertThatValueResolversAreOfType(LoggerFacade logger, Type valueResolverType)
        {
            Assert.That(logger.IsErrorEnabledResolver, Is.InstanceOf(valueResolverType));
            Assert.That(logger.IsWarnEnabledResolver, Is.InstanceOf(valueResolverType));
            Assert.That(logger.IsInfoEnabledResolver, Is.InstanceOf(valueResolverType));
            Assert.That(logger.IsDebugEnabledResolver, Is.InstanceOf(valueResolverType));
            Assert.That(logger.IsTraceEnabledResolver, Is.InstanceOf(valueResolverType));
        }
    }
}
