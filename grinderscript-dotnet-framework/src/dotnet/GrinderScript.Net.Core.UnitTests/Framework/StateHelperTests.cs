#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateHelperTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.Framework;
using Moq;
using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    [TestFixture]
    public class StateHelperTests
    {
        private StateHelper stateHelper;

        [SetUp]
        public void SetUp()
        {
            stateHelper = new StateHelper("Test");
        }

        [TestCase]
        public void IsInStateShouldBeFalseBeforeSafeTransformToState()
        {
            Assert.That(!stateHelper.IsInState);
        }

        [TestCase]
        public void CheckIsInStateShouldTrowExceptionBeforeSafeTransformToState()
        {
            Assert.Throws(Is.TypeOf<IllegalStateException>().And.Message.EqualTo("Not in state: 'Test'"), () => stateHelper.CheckIsInState());
        }

        [TestCase]
        public void IsNotInStateShouldBeTrueBeforeSafeTransformToState()
        {
            Assert.That(stateHelper.IsNotInState);
        }

        [TestCase]
        public void CheckIsNotInStateShouldNotThrowExceptionBeforeSafeTransformToState()
        {
            Assert.DoesNotThrow(() => stateHelper.CheckIsNotInState());
        }

        [TestCase]
        public void IsInStateShouldBeTrueAfterSafeTransformToState()
        {
            stateHelper.SafeTransformToState(null);
            Assert.That(stateHelper.IsInState);
        }

        [TestCase]
        public void CheckIsInStateShouldNotTrowExceptionAfterSafeTransformToState()
        {
            stateHelper.SafeTransformToState(null);
            Assert.DoesNotThrow(() => stateHelper.CheckIsInState());
        }

        [TestCase]
        public void IsNotInStateShouldBeFalseAfterSafeTransformToState()
        {
            stateHelper.SafeTransformToState(null);
            Assert.That(!stateHelper.IsNotInState);
        }

        [TestCase]
        public void CheckIsNotInStateShouldThrowExceptionAfterSafeTransformToState()
        {
            stateHelper.SafeTransformToState(null);
            Assert.Throws(Is.TypeOf<IllegalStateException>().And.Message.EqualTo("Already in state: 'Test'"), () => stateHelper.CheckIsNotInState());
        }

        [TestCase]
        public void SafeTransformToStateShouldCallTransformationWhenNotInState()
        {
            var transformHelperMock = new Mock<IGrinderTest>();
            stateHelper.SafeTransformToState(() => transformHelperMock.Object.Run());
            transformHelperMock.Verify(t => t.Run());
        }

        [TestCase]
        public void SafeTransformToStateShouldNotCallTransformationWhenInState()
        {
            stateHelper.SafeTransformToState(null);
            var transformHelperMock = new Mock<IGrinderTest>();
            Assert.Throws<IllegalStateException>(() => stateHelper.SafeTransformToState(() => transformHelperMock.Object.Run()));
            transformHelperMock.Verify(t => t.Run(), Times.Never());
        }

        [TestCase]
        public void SafeTransformToStateShouldThrowExceptionWhenInState()
        {
            stateHelper.SafeTransformToState(null);
            Assert.Throws(
                Is.TypeOf<IllegalStateException>().And.Message.EqualTo("Already in state: 'Test'"), () => stateHelper.SafeTransformToState(null));
        }
    }
}
