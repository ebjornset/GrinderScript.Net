#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractGrinderElement.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.Framework
{
    using System;

    public abstract class AbstractGrinderElement : IGrinderContextAware
    {
        public IGrinderContext GrinderContext { get; set; }

        public IGrinderLogger Logger { get; private set; }

        public void Initialize()
        {
            if (GrinderContext == null)
            {
                throw new AwarenessException("GrinderContext == null");
            }

            Logger = new LoggerFacade(GrinderContext.GetLogger(GetType()), GrinderContext);
            Logger.Trace("Initialize: Enter");
            TypeHelper = new TypeHelper(GrinderContext);
            OnInitialize();
            Logger.Trace("Initialize: Exit");
        }

        public void Shutdown()
        {
            SafeLog(() => Logger.Trace("Shutdown: Enter"));

            try
            {
                OnShutdown();
            }
            finally
            {
                SafeLog(() => Logger.Trace("Shutdown: Exit"));

                GrinderContext = null;
                Logger = null;
            }
        }

        internal protected TypeHelper TypeHelper { get; private set; }

        protected void SafeLog(Action logAction)
        {
            if (Logger != null)
            {
                logAction();
            }
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnShutdown()
        {
        }
    }
}
