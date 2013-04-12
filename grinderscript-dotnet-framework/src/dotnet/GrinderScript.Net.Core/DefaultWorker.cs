#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultWorker.cs" company="http://GrinderScript.net">
//
//   Copyright Â© 2012 Eirik Bjornset.
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

namespace GrinderScript.Net.Core
{
    public abstract class DefaultWorker : AbstractWorker, IProcessContextAware, IDatapoolManagerAware
    {
        protected override sealed void OnInitialize()
        {
            Logger.Info("OnInitialize: Enter");
            TestList = new TestList(ProcessContext);
            DefaultInitialize();
            Logger.Info("OnInitialize: Exit");
        }

        internal protected ITest AddTest(int testNumber, string testDescription, Action testAction, Action beforeTestAction = null, Action afterTestAction = null, int sleepMillis = 1000)
        {
            return TestList.AddTest(new DefaultTestMetadata(testNumber, testDescription, testAction, sleepMillis, beforeTestAction, afterTestAction));
        }

        internal protected void AddTest(ITest test)
        {
            TestList.AddTest(test);
        }

        protected override sealed void OnRun()
        {
            Logger.Debug("OnRun: Enter");
            DefaultBeforeRun();
            TestList.Run();
            DefaultAfterRun();
            Logger.Debug("OnRun: Exit");
        }

        protected override sealed void OnShutdown()
        {
            SafeLog(() => Logger.Info("OnShutdown: Enter"));
            TestList.Clear();
            DefaultShutdown();
            SafeLog(() => Logger.Info("OnShutdown: Exit"));
        }

        protected abstract void DefaultInitialize();

        protected virtual void DefaultBeforeRun()
        {
        }

        protected virtual void DefaultAfterRun()
        {
        }

        protected virtual void DefaultShutdown()
        {
        }

        public IProcessContext ProcessContext { get; set; }
        public IDatapoolManager DatapoolManager { get; set; }

        internal protected ITestList TestList { get; internal set; }
    }
}