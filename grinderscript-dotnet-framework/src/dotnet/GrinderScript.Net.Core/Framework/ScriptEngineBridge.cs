#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptEngineBridge.cs" company="http://GrinderScript.net">
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

using System.Linq;

namespace GrinderScript.Net.Core.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public sealed class ScriptEngineBridge : AbstractScriptEngine
    {
        public IGrinderScriptEngine ScriptEngine { get; internal set; }
        
        public IList<string> BinPath { get; private set; }

        public ScriptEngineBridge(IGrinderContext grinderContext)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            GrinderContext = grinderContext;
        }

        protected override void OnInitialize()
        {
            Logger.Trace("OnInitialize: Enter");
            InitializeDebugger();
            InitializeBinPath();
            InitializeProcessContext();
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            ScriptEngine = TypeHelper.CreateTargetTypeFromProperty<IGrinderScriptEngine>(Constants.ScriptEngineTypeKey, typeof(DefaultScriptEngine));
            ProcessContext.InitializeAwareness(ScriptEngine);
            ScriptEngine.Initialize();
            ProcessContext.DatapoolFactory.CreateMissingDatapoolsFromProperties();
            Logger.Trace("OnInitialize: Exit");
        }

        protected override IGrinderWorker OnCreateWorkerRunnable()
        {
            Logger.Trace("OnCreateWorkerRunnable: Enter");
            var result = ScriptEngine.CreateWorkerRunnable();
            ProcessContext.InitializeAwareness(result);
            result.Initialize();
            Logger.Trace(m => m("OnCreateWorkerRunnable: Exit, Result = {0}", result));
            return result;
        }

        protected override void OnShutdown()
        {
            SafeLog(() => Logger.Trace("OnShutdown: Enter"));
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                if (ScriptEngine != null)
                {
                    ScriptEngine.Shutdown();
                }
            }
            finally
            {
                ScriptEngine = null;
            }

            SafeLog(() => Logger.Trace("OnShutdown: Exit"));
        }

        private void InitializeDebugger()
        {
            Logger.Trace("InitializeDebugger: Enter");

            if (bool.Parse(GrinderContext.GetProperty(Constants.LaunchDebuggerKey, bool.FalseString)))
            {
                Logger.Trace("InitializeDebugger: Launching debugger...");
                System.Diagnostics.Debugger.Launch();
            }
            else
            {
                Logger.Trace("InitializeDebugger: Not launching debugger...");
            }

            Logger.Trace("InitializeDebugger: Exit");
        }

        private void InitializeBinPath()
        {
            Logger.Trace("InitializeBinPath: Enter");
            string binFolder = GrinderContext.GetProperty(Constants.BinFolderKey);
            BinPath = string.IsNullOrWhiteSpace(binFolder) ? new[] { ScriptFolder } : new[] { FixDirectorySuffix(binFolder), ScriptFolder };
            Logger.Trace("InitializeBinPath: Exit");
        }

        private void InitializeProcessContext()
        {
            Logger.Trace("InitializeProcessContext: Enter");
            var processContext = new ProcessContext
            {
                BinFolder = BinPath[0],
                GrinderContext = GrinderContext,
                DatapoolManager = new DatapoolManager(GrinderContext)
            };
            processContext.DatapoolFactory = new DatapoolFactory(GrinderContext, processContext.DatapoolManager);
            processContext.Freeze();
            ProcessContext = processContext;
            Logger.Trace("InitializeProcessContext: Exit");
        }

        private string FixDirectorySuffix(string directory)
        {
            string orgDirectory = directory;
            Logger.Trace(m => m("FixDirectorySuffix: Enter, directory = {0}", orgDirectory));
            var dirSep = Path.DirectorySeparatorChar.ToString();
            directory = directory.Trim();
            if (!directory.EndsWith(dirSep, StringComparison.OrdinalIgnoreCase))
            {
                directory = directory + dirSep;
            }

            Logger.Trace(m => m("FixDirectorySuffix: Exit, directory = {0}", directory));
            return directory;
        }

        private string ScriptFolder
        {
            get
            {
                string scriptFile = GrinderContext.ScriptFile;
                string scriptFolder = FixDirectorySuffix(new FileInfo(scriptFile).Directory.FullName);
                return scriptFolder;
            }
        }

        internal Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Logger.Trace(m => m("OnAssemblyResolve, args.Name = '{0}', args.RequestingAssembly = '{1}'", args.Name, args.RequestingAssembly != null ? args.RequestingAssembly.FullName : string.Empty));
            if (File.Exists(args.Name))
            {
                Logger.Trace(m => m("Requested name '{0}' is an existing file, will try to load it", args.Name));
                return Assembly.LoadFrom(args.Name);
            }

            string fileName = GetFileName(args.Name);
            foreach (Assembly assembly in BinPath.Select(binFolder => LoadAssemblyFromFolder(args.Name, fileName, binFolder)).Where(assembly => assembly != null))
            {
                return assembly;
            }

            Logger.Warn(m => m("Assembly not found: '{0}'", args.Name));
            return null;
        }

        private Assembly LoadAssemblyFromFolder(string orignalName, string fileName, string folder)
        {
            var binFolderFileName = Path.Combine(folder, fileName);
            if (File.Exists(binFolderFileName))
            {
                Logger.Trace(m => m("Found file, will try to load it, FileName = {0}", binFolderFileName));
                Assembly assembly = Assembly.LoadFrom(binFolderFileName);
                Logger.Trace(m => m("Assembly '{0}' loaded from '{1}', Result = '{2}", orignalName, binFolderFileName, assembly));
                return assembly;
            }

            Logger.Trace(m => m("Assembly '{0}' not found as '{1}'", orignalName, binFolderFileName));
            return null;
        }

        private static string GetFileName(string originalName)
        {
            string fileName = (originalName.IndexOf(',') > 0 ? originalName.Substring(0, originalName.IndexOf(',')) : originalName).Trim();
            if (!fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName + ".dll";
            }

            return fileName;
        }
    }
}
