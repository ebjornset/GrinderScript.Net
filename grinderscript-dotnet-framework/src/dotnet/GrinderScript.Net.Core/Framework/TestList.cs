#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestList.cs" company="http://GrinderScript.net">
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
using System.Collections.Generic;

namespace GrinderScript.Net.Core.Framework
{
    public class TestList : ITestList
    {
        internal IProcessContext ProcessContext { get; set; }

        public TestList(IProcessContext processContext)
        {
            if (processContext == null)
            {
                throw new ArgumentNullException("processContext");
            }

            ProcessContext = processContext;
        }

        internal ITest CreateTest(ITestMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            return new Test(metadata, ProcessContext.GrinderContext);
        }

        public ITest AddTest(ITestMetadata metadata)
        {
            ITest test = CreateTest(metadata);
            tests.Add(test);
            return test;
        }

        public void AddTest(ITest test)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }

            tests.Add(test);
        }

        public IEnumerable<ITest> Tests { get { return tests; } }

        public void Clear()
        {
            foreach (var test in tests)
            {
                test.Clear();
            }

            tests.Clear();
        }

        private readonly IList<ITest> tests = new List<ITest>();

        public void Run()
        {
            foreach (var test in tests)
            {
                test.Run();
            }
        }
    }
}
