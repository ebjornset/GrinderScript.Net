#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestScriptEngine.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.UnitTests.TestHelpers
{
    public class TestScriptEngine : AbstractScriptEngine, IBinFolderAware, IDatapoolFactoryAware, IDatapoolManagerAware
    {
        internal bool OnInitializeIsCalled { get; private set; }

        internal bool OnCreateWorkerRunnableIsCalled { get; private set; }

        internal bool OnShutdownIsCalled { get; private set; }

        internal bool SetBinFolderIsCalled
        {
            get
            {
                return BinFolder != null;
            }
        }

        protected override void OnInitialize()
        {
            OnInitializeIsCalled = true;
        }

        protected override IGrinderWorker OnCreateWorkerRunnable()
        {
            OnCreateWorkerRunnableIsCalled = true;
            return CreateGrinderWorkerFromProperty();
        }

        protected override void OnShutdown()
        {
            OnShutdownIsCalled = true;
        }

        public string BinFolder { get; set; }

        public IDatapoolFactory DatapoolFactory { get; set; }

        public IDatapoolManager DatapoolManager { get; set; }
    }
}