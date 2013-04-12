#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTestMetadata.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core
{
    public class DefaultTestMetadata : ITestMetadata
    {
        public DefaultTestMetadata(int testNumber, string testDescription, Action testAction, long sleepMillis = 0, Action beforeTestAction = null, Action afterTestAction = null)
        {
            if (string.IsNullOrWhiteSpace(testDescription))
            {
                throw new ArgumentNullException("testDescription");
            }

            if (testAction == null)
            {
                throw new ArgumentNullException("testAction");
            }

            TestNumber = testNumber;
            TestDescription = testDescription;
            TestAction = testAction;
            SleepMillis = sleepMillis;
            BeforeTestAction = beforeTestAction;
            AfterTestAction = afterTestAction;
        }

        public int TestNumber { get; private set; }

        public string TestDescription { get; private set; }

        public long SleepMillis { get; private set; }

        public Action TestAction { get; private set; }

        public Action BeforeTestAction { get; private set; }

        public Action AfterTestAction { get; private set; }
    }
}
