#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioWorker.cs" company="http://GrinderScript.net">
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
    using System.Collections.Generic;
    using System.Globalization;

    public class ScenarioWorker : AbstractWorker, IProcessContextAware
    {
        protected override void OnInitialize()
        {
            Logger.Trace("OnInitialize: Enter");
            base.OnInitialize();
            int firstElement = GetValidFirstElement();
            int lastElement = GetValidLastElement(firstElement);
            InitializeRandom();
            Logger.Info(m => m("OnInitialize: FirstElement = {0}, LastElement = {1}, IsRandom = {2}, Seed = {3}", firstElement, lastElement, IsRandom, Seed));
            InstantiateWorkers(firstElement, lastElement);
            RandomizeWorkers();
            InitializeAndLogWorkers();
            Logger.Trace("OnInitialize: Exit");
        }

        protected override void OnRun()
        {
            Logger.Trace("OnRun: Enter");
            foreach (var grinderWorker in workers)
            {
                // TODO Random, sequence etc
                grinderWorker.Run();
            }

            Logger.Trace("OnRun: Exit");
        }

        protected override void OnShutdown()
        {
            SafeLog(() => Logger.Trace("OnShutdown: Enter"));
            IList<Exception> exceptions = null;
            foreach (var grinderWorker in workers)
            {
                try
                {
                    grinderWorker.Shutdown();
                }
                catch (Exception e)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(e);
                }
            }

            workers.Clear();
            base.OnShutdown();
            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }

            SafeLog(() => Logger.Trace("OnShutdown: Exit"));
        }

        public int GroupSize { get { return workers.Count; } }

        public IEnumerable<IGrinderWorker> Group { get { return workers; } }

        internal bool IsRandom { get; private set; }

        internal Random Random { get; private set; }

        internal int Seed { get; private set; }

        internal static string GetPropertyKey(string suffix)
        {
            return KeyPrefix + suffix;
        }

        internal static string GetElementPropertyKey(int index, string suffix)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}", KeyPrefix, index, suffix);
        }

        private readonly IList<IGrinderWorker> workers = new List<IGrinderWorker>();

        private int GetValidLastElement(int firstElement)
        {
            int lastElement = GetIntProperty(KeyPrefix + "lastElement", 100);
            if (lastElement < firstElement)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property '{0}' can not be < '{1}' ({2}), but was {3}", GetPropertyKey("lastElement"), GetPropertyKey("firstElement"), firstElement, lastElement));
            }

            return lastElement;
        }

        private int GetValidFirstElement()
        {
            int firstElement = GetIntProperty(GetPropertyKey("firstElement"), 1);
            if (firstElement < 1)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property '{0}' should be > 0, but was {1}", GetPropertyKey("firstElement"), firstElement));
            }

            return firstElement;
        }

        private void InitializeRandom()
        {
            IsRandom = bool.Parse(GrinderContext.GetProperty(GetPropertyKey("random"), bool.TrueString));
            if (!IsRandom)
            {
                return;
            }

            Seed = int.Parse(GrinderContext.GetProperty(GetPropertyKey("seed"), ((int)DateTime.Now.Ticks).ToString()));
            Random = new Random(Seed);
        }

        private void InstantiateWorkers(int firstElement, int lastElement)
        {
            Logger.Trace("InstantiateWorkers: Enter");
            for (int i = firstElement; i <= lastElement; i++)
            {
                AddWorkerToCollectionByLoadFactor(i);
            }

            Logger.Trace("InstantiateWorkers: Exit");
        }

        private void InitializeAndLogWorkers()
        {
            Logger.Trace("InitializeWorkers: Enter");
            int index = 0;
            foreach (var worker in workers)
            {
                index++;
                var logIndex = index;
                var logWorker = worker;
                Logger.Info(m => m("Worker[{0}] = '{1}'", logIndex, logWorker.GetType().FullName));
                ProcessContext.InitializeAwareness(worker);
                worker.Initialize();
            }

            Logger.Trace("InitializeWorkers: Exit");
        }

        private void RandomizeWorkers()
        {
            if (!IsRandom)
            {
                return;
            }

            var orgWorkers = new List<IGrinderWorker>(workers);
            int workerCount = orgWorkers.Count;
            workers.Clear();
            for (int i = 0; i < workerCount; i++)
            {
                int index = Random.Next(0, orgWorkers.Count);
                workers.Add(orgWorkers[index]);
                orgWorkers.RemoveAt(index);
            }
        }

        private void AddWorkerToCollectionByLoadFactor(int index)
        {
            string workerTypeName = GrinderContext.GetProperty(GetElementPropertyKey(index, "workerType"));
            if (!string.IsNullOrWhiteSpace(workerTypeName))
            {
                int loadFactor = GetIntProperty(GetElementPropertyKey(index, "loadFactor"), 1);
                Logger.Info(m => m("AddWorkerToCollectionByLoadFactor: Index = {0}, WorkerType = '{1}', LoadFactor = {2}", index, workerTypeName, loadFactor));
                for (int i = 0; i < loadFactor; i++)
                {
                    workers.Add(TypeHelper.CreateTargetTypeFromName<IGrinderWorker>(workerTypeName));
                }
            }
            else
            {
                Logger.Trace(m => m("AddWorkerToCollectionByLoadFactor: Property '{0}' not set", GetElementPropertyKey(index, "workerType")));
            }
        }

        private int GetIntProperty(string key, int defaultValue)
        {
            string valueString = GrinderContext.GetProperty(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return int.Parse(valueString, CultureInfo.CurrentCulture);
        }

        private const string KeyPrefix = Constants.WorkerGroupKeyPrefix;

        public IProcessContext ProcessContext { get; set; }
    }
}
