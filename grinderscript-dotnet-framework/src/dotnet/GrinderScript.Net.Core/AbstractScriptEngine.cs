#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractScriptEngine.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core
{
    using System;

    using GrinderScript.Net.Core.Framework;

    public abstract class AbstractScriptEngine : AbstractGrinderElement, IGrinderScriptEngine, IProcessContextAware
    {
        protected internal static string WorkerTypeKey { get { return Constants.WorkerTypeKey; } }

        public IGrinderWorker CreateWorkerRunnable()
        {
            Logger.Trace("CreateWorkerRunnable: Enter");
            var result = OnCreateWorkerRunnable();
            Logger.Trace(m => m("CreateWorkerRunnable: Exit, result = {0}", result));
            return result;
        }

        protected IGrinderWorker CreateGrinderWorkerFromProperty(Type defaultWorkerType = null)
        {
            return TypeHelper.CreateTargetTypeFromProperty<IGrinderWorker>(WorkerTypeKey, defaultWorkerType);
        }

        protected abstract IGrinderWorker OnCreateWorkerRunnable();

        public IProcessContext ProcessContext { get; set; }
    }
}
