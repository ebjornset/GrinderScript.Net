#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatapoolManager.cs" company="http://GrinderScript.net">
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
    public class DatapoolManager : IDatapoolManager
    {
        internal IGrinderContext GrinderContext { get; set; }

        public DatapoolManager(IGrinderContext grinderContext)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            GrinderContext = grinderContext;
        }

        public IDatapool<T> GetDatapool<T>() where T : class
        {
            return GetDatapool<T>(typeof(T).Name);
        }

        public IDatapool<T> GetDatapool<T>(string name) where T : class
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            object rawValue = null;
            spinLocked.DoLocked(() =>
            {
                if (!datapools.ContainsKey(name))
                {
                    throw new ArgumentException(string.Format("Unknown datapool: '{0}'", name));
                }

                rawValue = datapools[name];
            });

            var result = rawValue as IDatapool<T>;
            if (result != null)
            {
                return result;
            }

            throw new ArgumentException(string.Format("Wrong type for datapool '{0}'. Expected '{1}', but got '{2}'", name, typeof(T).FullName, rawValue.GetType().GetGenericArguments()[0].FullName));
        }

        public void BuildDatapool<T>(IDatapoolMetatdata<T> metadata) where T : class
        {
            var datapool = new Datapool<T>(GrinderContext, metadata);
            spinLocked.DoLocked(
                () =>
                {
                    if (datapools.ContainsKey(metadata.Name))
                    {
                        throw new ArgumentException(string.Format("Duplicate datapool: '{0}'", metadata.Name));
                    }

                    datapools.Add(metadata.Name, datapool);
                });
        }

        public bool ContainsDatapool<T>()
        {
            return ContainsDatapool(typeof(T).Name);
        }

        public bool ContainsDatapool(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            bool result = false;
            spinLocked.DoLocked(
                () =>
                {
                    result = datapools.ContainsKey(name);
                });

            return result;
        }

        private readonly Dictionary<string, object> datapools = new Dictionary<string, object>();
        private readonly SpinLocked spinLocked = new SpinLocked();
    }
}
