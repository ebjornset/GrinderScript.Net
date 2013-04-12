#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessContext.cs" company="http://GrinderScript.net">
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
    public interface IProcessContext
    {
        string BinFolder { get; }

        IDatapoolFactory DatapoolFactory { get; }
        
        IDatapoolManager DatapoolManager { get; }

        IGrinderContext GrinderContext { get; }

        object InitializeAwareness(object target);
    }
}