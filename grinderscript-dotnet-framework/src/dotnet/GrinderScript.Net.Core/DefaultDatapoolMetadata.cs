#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatapoolMetadata.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core
{
    public class DefaultDatapoolMetadata<T> : IDatapoolMetatdata<T> where T: class
    {
        public DefaultDatapoolMetadata(IList<T> values, bool isRandom, int seed, DatapoolThreadDistributionMode distributionMode, bool isCircular, string name = null)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            Name = string.IsNullOrWhiteSpace(name) ? typeof(T).Name : name;
            Values = values;
            IsRandom = isRandom;
            Seed = seed;
            DistributionMode = distributionMode;
            IsCircular = isCircular;
        }

        public string Name { get; internal set; }

        public bool IsRandom { get; internal set; }

        public int Seed { get; internal set; }

        public DatapoolThreadDistributionMode DistributionMode { get; internal set; }

        public bool IsCircular { get; internal set; }

        public IList<T> Values { get; internal set; }
    }
}
