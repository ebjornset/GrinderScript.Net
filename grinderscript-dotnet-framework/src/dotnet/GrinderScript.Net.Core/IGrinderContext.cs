#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGrinderContext.cs" company="http://GrinderScript.net">
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

    public interface IGrinderContext
    {
        void Sleep(long meanTime);

        void Sleep(long meanTime, long sigma);

        IGrinderLogger GetLogger(string name);

        IGrinderLogger GetLogger(Type nameType);

        IGrinderTest CreateTest(int number, string description, IGrinderTest test);

        bool ContainsProperty(string key);

        string GetProperty(string key);

        string GetProperty(string key, string defaultValue);

        string ScriptFile { get; }

        int AgentNumber { get; }

        int ProcessNumber { get; }

        string ProcessName { get; }

        int FirstProcessNumber { get; }

        int ThreadNumber { get; }

        int RunNumber { get; }
    }
}