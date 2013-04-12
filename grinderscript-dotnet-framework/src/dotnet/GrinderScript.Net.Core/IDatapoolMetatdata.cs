#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatapoolMetatdata.cs" company="http://GrinderScript.net">
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

using System.Collections.Generic;

namespace GrinderScript.Net.Core
{
    public interface IDatapoolMetatdata<T> where T: class
    {
        string Name { get; }
        
        bool IsRandom { get; }

        int Seed { get; }

        DatapoolThreadDistributionMode DistributionMode { get; }

        bool IsCircular { get; }

        IList<T> Values { get; }
    }
}