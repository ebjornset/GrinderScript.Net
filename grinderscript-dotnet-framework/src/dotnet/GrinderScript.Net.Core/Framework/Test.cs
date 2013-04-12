#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.Framework
{
    public class Test: ITest
    {
        public Test(ITestMetadata metadata, IGrinderContext grinderContext)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            Metadata = metadata;
            GrinderContext = grinderContext;
            Underlying = grinderContext.CreateTest(metadata.TestNumber, metadata.TestDescription, new TestActionWrapper(metadata.TestAction));
        }

        public void Run()
        {
            if (Metadata.BeforeTestAction != null)
            {
                Metadata.BeforeTestAction();
            }

            Underlying.Run();

            if (Metadata.SleepMillis > 0)
            {
                GrinderContext.Sleep(Metadata.SleepMillis);
            }

            if (Metadata.AfterTestAction != null)
            {
                Metadata.AfterTestAction();
            }
        }

        public ITestMetadata Metadata { get; private set; }
        public IGrinderContext GrinderContext { get; private set; }

        public void Clear()
        {
            Underlying = null;
        }

        internal IGrinderTest Underlying { get; set; }
    }
}
