#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessContext.cs" company="http://GrinderScript.net">
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
    public class ProcessContext : IProcessContext
    {
        public ProcessContext()
        {
            FrozenStateHelper = new StateHelper("Frozen");
        }

        public string BinFolder { get; internal set; }

        public IDatapoolFactory DatapoolFactory { get; internal set; }

        public IDatapoolManager DatapoolManager { get; internal set; }

        public IGrinderContext GrinderContext { get; internal set; }

        internal IStateHelper FrozenStateHelper { get; set; }

        public void Freeze()
        {
            FrozenStateHelper.SafeTransformToState(() =>
            {
                if (DatapoolFactory == null)
                {
                    throw new ArgumentNullException("DatapoolFactory");
                }

                if (DatapoolManager == null)
                {
                    throw new ArgumentNullException("DatapoolManager");
                }

                if (GrinderContext == null)
                {
                    throw new ArgumentNullException("GrinderContext");
                }
            });
        }

        public object InitializeAwareness(object target)
        {
            FrozenStateHelper.CheckIsInState();
            
            var processContextAware = target as IProcessContextAware;
            if (processContextAware != null)
            {
                processContextAware.ProcessContext = this;
            }

            var grinderContextAware = target as IGrinderContextAware;
            if (grinderContextAware != null)
            {
                grinderContextAware.GrinderContext = GrinderContext;
            }

            var binFolderAware = target as IBinFolderAware;
            if (binFolderAware != null)
            {
                binFolderAware.BinFolder = BinFolder;
            }

            var datapoolFactoryAware = target as IDatapoolFactoryAware;
            if (datapoolFactoryAware != null)
            {
                datapoolFactoryAware.DatapoolFactory = DatapoolFactory;
            }

            var datapoolManagerAware = target as IDatapoolManagerAware;
            if (datapoolManagerAware != null)
            {
                datapoolManagerAware.DatapoolManager = DatapoolManager;
            }

            return target;
        }
    }
}
