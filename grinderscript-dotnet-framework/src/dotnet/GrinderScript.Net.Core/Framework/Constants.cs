#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="http://GrinderScript.net">
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
    internal static class Constants
    {
        internal const string KeyPrefix = "grinderscript-dotnet";
        internal const string WorkerTypeKey = KeyPrefix + ".workerType";
        internal const string ScriptEngineTypeKey = KeyPrefix + ".scriptEngineType";
        internal const string BinFolderKey = KeyPrefix + ".binFolder";
        internal const string LaunchDebuggerKey = KeyPrefix + ".launchDebugger";
        internal const string WorkerGroupKeyPrefix = KeyPrefix + ".scenarioWorker.";
        internal const string VerificationWorkerTestsPrRunKey = KeyPrefix + ".verificationWorker.testsPrRun";
        internal const string DatapoolFactoryKeyPrefix = KeyPrefix + ".datapoolFactory.";
        internal const string DatapoolKeyPrefix = KeyPrefix + ".datapool.";
        internal const string AgentCountKey = KeyPrefix + ".agentCount";
        internal const string LoggerEnabledCacheTtlKey = KeyPrefix + ".loggerEnabledCacheTtl";
        internal const string ProcessCountKey = "grinder.processes";
        internal const string ThreadCountKey = "grinder.threads";
    }
}
