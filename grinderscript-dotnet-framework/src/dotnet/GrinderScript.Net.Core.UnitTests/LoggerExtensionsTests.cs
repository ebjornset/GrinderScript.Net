#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerExtensionsTests.cs" company="http://GrinderScript.net">
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

using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests
{
    [TestFixture]
    public class LoggerExtensionsTests
    {
        private Mock<IGrinderLogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<IGrinderLogger>();
        }

        [TestCase]
        public void TraceWithCallbackShouldNotCallUnderlyingWhenTraceIsNotEnabled()
        {
            loggerMock.SetupGet(l => l.IsTraceEnabled).Returns(false);
            loggerMock.Object.Trace(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Trace(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void TraceWithCallbackShouldCallUnderlyingWhenTraceIsEnabled()
        {
            loggerMock.SetupGet(l => l.IsTraceEnabled).Returns(true);
            loggerMock.Object.Trace(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Trace("msg p1 p2"));
        }

        [TestCase]
        public void DebugWithCallbackShouldNotCallUnderlyingWhenDebugIsNotEnabled()
        {
            loggerMock.SetupGet(l => l.IsDebugEnabled).Returns(false);
            loggerMock.Object.Debug(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Debug(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void DebugWithCallbackShouldCallUnderlyingWhenDebugIsEnabled()
        {
            loggerMock.SetupGet(l => l.IsDebugEnabled).Returns(true);
            loggerMock.Object.Debug(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Debug("msg p1 p2"));
        }

        [TestCase]
        public void InfoWithCallbackShouldNotCallUnderlyingWhenInfoIsNotEnabled()
        {
            loggerMock.SetupGet(l => l.IsInfoEnabled).Returns(false);
            loggerMock.Object.Info(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Info(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void InfoWithCallbackShouldCallUnderlyingWhenInfoIsEnabled()
        {
            loggerMock.SetupGet(l => l.IsInfoEnabled).Returns(true);
            loggerMock.Object.Info(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Info("msg p1 p2"));
        }

        [TestCase]
        public void WarnWithCallbackShouldNotCallUnderlyingWhenWarnIsNotEnabled()
        {
            loggerMock.SetupGet(l => l.IsWarnEnabled).Returns(false);
            loggerMock.Object.Warn(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Warn(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void WarnWithCallbackShouldCallUnderlyingWhenWarnIsEnabled()
        {
            loggerMock.SetupGet(l => l.IsWarnEnabled).Returns(true);
            loggerMock.Object.Warn(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Warn("msg p1 p2"));
        }

        [TestCase]
        public void ErrorWithCallbackShouldNotCallUnderlyingWhenErrorIsNotEnabled()
        {
            loggerMock.SetupGet(l => l.IsErrorEnabled).Returns(false);
            loggerMock.Object.Error(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Error(It.IsAny<string>()), Times.Never());
        }

        [TestCase]
        public void ErrorWithCallbackShouldCallUnderlyingWhenErrorIsEnabled()
        {
            loggerMock.SetupGet(l => l.IsErrorEnabled).Returns(true);
            loggerMock.Object.Error(m => m("msg {0} {1}", "p1", "p2"));
            loggerMock.Verify(l => l.Error("msg p1 p2"));
        }

        //[TestCase]
        //public void TraceWithMsgAndExceptionShouldNotCallUnderlyingTraceWhenTraceIsDisabled()
        //{
        //    loggerMock.Setup(l => l.IsTraceEnabled).Returns(false);
        //    var ex = new Exception();
        //    logger.Trace("msg", ex);
        //    loggerMock.Verify(l => l.Trace(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        //}

        //[TestCase]
        //public void TraceWithMsgAndExceptionShouldCallUnderlyingTraceWhenTraceIsEnabled()
        //{
        //    loggerMock.Setup(l => l.IsTraceEnabled).Returns(true);
        //    var ex = new Exception();
        //    logger.Trace("msg", ex);
        //    loggerMock.Verify(l => l.Trace("msg", ex));
        //}

        //[TestCase]
        //public void DebugWithMsgAndExceptionShouldNotCallUnderlyingDebugWhenDebugIsDiabled()
        //{
        //    loggerMock.Setup(l => l.IsDebugEnabled).Returns(false);
        //    var ex = new Exception();
        //    logger.Debug("msg", ex);
        //    loggerMock.Verify(l => l.Debug(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        //}

        //[TestCase]
        //public void DebugWithMsgAndExceptionShoulCallUnderlyingDebugWhenDebugIsEnabled()
        //{
        //    loggerMock.Setup(l => l.IsDebugEnabled).Returns(true);
        //    var ex = new Exception();
        //    logger.Debug("msg", ex);
        //    loggerMock.Verify(l => l.Debug("msg", ex));
        //}

        //[TestCase]
        //public void InfoWithMsgAndExceptionShouldNotCallUnderlyingInfoWhenInfoIsDisabled()
        //{
        //    loggerMock.Setup(l => l.IsInfoEnabled).Returns(false);
        //    var ex = new Exception();
        //    logger.Info("msg", ex);
        //    loggerMock.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        //}

        //[TestCase]
        //public void InfoWithMsgAndExceptionShouldCallUnderlyingInfoWhenInfoIsEnabled()
        //{
        //    loggerMock.Setup(l => l.IsInfoEnabled).Returns(true);
        //    var ex = new Exception();
        //    logger.Info("msg", ex);
        //    loggerMock.Verify(l => l.Info("msg", ex));
        //}

        //[TestCase]
        //public void WarnWithMsgAndExceptionShouldNotCallUnderlyingWarnWhenWarnIsEnabled()
        //{
        //    loggerMock.Setup(l => l.IsWarnEnabled).Returns(false);
        //    var ex = new Exception();
        //    logger.Warn("msg", ex);
        //    loggerMock.Verify(l => l.Warn(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        //}

        //[TestCase]
        //public void WarnWithMsgAndExceptionShouldCallUnderlyingWarnWhenWarnIsEnabled()
        //{
        //    loggerMock.Setup(l => l.IsWarnEnabled).Returns(true);
        //    var ex = new Exception();
        //    logger.Warn("msg", ex);
        //    loggerMock.Verify(l => l.Warn("msg", ex));
        //}

        //[TestCase]
        //public void ErrorWithMsgAndExceptionShouldNotCallUnderlyingErrorWhenErrorIsDisabled()
        //{
        //    loggerMock.Setup(l => l.IsErrorEnabled).Returns(false);
        //    var ex = new Exception();
        //    logger.Error("msg", ex);
        //    loggerMock.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        //}

        //[TestCase]
        //public void ErrorWithMsgAndExceptionShouldCallUnderlyingErrorWhenErrorIsEnabled()
        //{
        //    loggerMock.Setup(l => l.IsErrorEnabled).Returns(true);
        //    var ex = new Exception();
        //    logger.Error("msg", ex);
        //    loggerMock.Verify(l => l.Error("msg", ex));
        //}
    }
}
