#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsScriptWorker.cs" company="http://GrinderScript.net">
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
using System.Globalization;

using CSScriptLibrary;

using GrinderScript.Net.Core;
using GrinderScript.Net.Core.Framework;

namespace GrinderScript.Net.CsScript
{
    public class CsScriptWorker : AbstractWorker, IProcessContextAware, IDatapoolManagerAware
    {
        public IProcessContext ProcessContext { get; set; }

        public IDatapoolManager DatapoolManager { get; set; }

        protected override void OnInitialize()
        {
            Logger.Debug("OnInitialize: Enter");
            if (ProcessContext == null)
            {
                throw new AwarenessException("ProcessContext == null");
            }

            if (DatapoolManager == null)
            {
                throw new AwarenessException("DatapoolManager == null");
            }

            string scriptName = GrinderContext.GetProperty(ScriptFileKey);
            if (string.IsNullOrWhiteSpace(scriptName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Missing property '{0}'", ScriptFileKey));
            }

            Logger.Trace(x => x("OnInitialize: scriptName = {0}", scriptName));
            ScriptWorker = CSScript.Evaluator.LoadFile(scriptName);
            if (!(ScriptWorker is IGrinderWorker))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Type '{0}', from script file '{1}', does not implement '{2}'", ScriptWorker.GetType(), scriptName, typeof(IGrinderWorker).FullName));
            }

            ProcessContext.InitializeAwareness(ScriptWorker);

            ((IGrinderWorker)ScriptWorker).Initialize();

            Logger.Debug(x => x("OnInitialize: Exit, ScriptWorker = {0}", ScriptWorker.GetType().FullName));
        }

        protected override void OnRun()
        {
            ((IGrinderWorker)ScriptWorker).Run();
        }

        protected override void OnShutdown()
        {
            if (ScriptWorker == null)
            {
                return;
            }

            ((IGrinderWorker)ScriptWorker).Shutdown();
            ScriptWorker = null;
        }

        internal dynamic ScriptWorker { get; set; }

        internal string ScriptFileKey { get { return Constants.KeyPrefix + ".csScriptWorker.script"; } }
    }
}
