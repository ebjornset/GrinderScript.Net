#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestActionWrapperTests.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class TestActionWrapperTests
    {
        private Action testAction;

        private bool testIsMethodCalled;

        [TestCase]
        public void CtorShouldThrowExceptionWhenMetadataIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("testAction"), () => new TestActionWrapper(null));
        }


        [TestCase]
        public void CtorShouldSetTestAction()
        {
            testAction = TestMethod;
            var wrapper = new TestActionWrapper(testAction);
            Assert.That(wrapper.TestAction, Is.SameAs(testAction));
        }

        [TestCase]
        public void RunShouldCallTestAction()
        {
            testAction = TestMethod;
            var wrapper = new TestActionWrapper(testAction);
            testIsMethodCalled = false;
            wrapper.Run();
            Assert.That(testIsMethodCalled);
        }

        private void TestMethod()
        {
            testIsMethodCalled = true;
        }
    }
}
