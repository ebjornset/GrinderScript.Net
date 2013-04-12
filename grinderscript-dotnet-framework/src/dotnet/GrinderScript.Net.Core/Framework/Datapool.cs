#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Datapool.cs" company="http://GrinderScript.net">
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
using System.Globalization;
using System.Linq;

namespace GrinderScript.Net.Core.Framework
{
    public class Datapool<T> : IDatapool<T> where T : class
    {
        internal IGrinderContext GrinderContext { get; set; }
        public int PhysicalSize { get; private set; }
        public int LogicalSize
        {
            get
            {
                return distributionMode == DatapoolThreadDistributionMode.ThreadShared ? ThreadSharedLogicalSize() : ThreadNonSharedLogicalSize();
            }
        }

        public Datapool(IGrinderContext grinderContext, IDatapoolMetatdata<T> datapoolMetadata)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            if (datapoolMetadata == null)
            {
                throw new ArgumentNullException("datapoolMetadata");
            }

            GrinderContext = grinderContext;
            distributionMode = datapoolMetadata.DistributionMode;
            PhysicalSize = datapoolMetadata.Values.Count;

            int minCapacity;
            int agentCount = int.Parse(GrinderContext.GetProperty(Constants.AgentCountKey, "1"));
            int processCount = int.Parse(GrinderContext.GetProperty(Constants.ProcessCountKey, "1"));
            int agentOffset = GrinderContext.AgentNumber;
            int processOffset = GrinderContext.ProcessNumber - GrinderContext.FirstProcessNumber;
            threadCount = int.Parse(GrinderContext.GetProperty(Constants.ThreadCountKey, "1"));
            if (distributionMode == DatapoolThreadDistributionMode.ThreadUnique)
            {
                if (!(agentCount > GrinderContext.AgentNumber))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot ceate thread unique datapool '{0}', because property '{1}' = '{2}'. Current AgentNumber = '{3}' and indicates that '{1}' must be at least '{4}' for thread uniqueness to work correctly", datapoolMetadata.Name, Constants.AgentCountKey, agentCount, GrinderContext.AgentNumber, GrinderContext.AgentNumber + 1));
                }

                if (processOffset < 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot ceate thread unique datapool '{0}', because thread offset negative ({1}). FirstProcessNumber = {2}, ProcessNumber = {3}", datapoolMetadata.Name, processOffset, GrinderContext.FirstProcessNumber, GrinderContext.ProcessNumber));
                }

                if (!(processCount > processOffset))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot ceate thread unique datapool '{0}', because thread offset = '{1}' is not less than property 'grinder.threads' = '{2}'. FirstProcessNumber = {3}, ProcessNumber = {4}", datapoolMetadata.Name, processOffset, processCount, GrinderContext.FirstProcessNumber, GrinderContext.ProcessNumber));
                }

                minCapacity = agentCount * processCount * threadCount;
            }
            else
            {
                minCapacity = 1;
            }

            if (PhysicalSize < minCapacity)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "To low capacity for datapool '{0}', expected at least '{1}', but was '{2}'", datapoolMetadata.Name, minCapacity, PhysicalSize));
            }

            T[] values = datapoolMetadata.Values.ToArray();

            if (datapoolMetadata.IsRandom && PhysicalSize > 1)
            {
                var random = new Random(datapoolMetadata.Seed);
                for (int i = 0; i < PhysicalSize; i++)
                {
                    int swapWith = random.Next(PhysicalSize);
                    T orgValue = values[i];
                    values[i] = values[swapWith];
                    values[swapWith] = orgValue;
                }
            }

            Tuple<int, int> agentSlice = GetSubtupleInTupleSlicedBy(agentOffset, new Tuple<int, int>(0, PhysicalSize - 1), agentCount);
            Tuple<int, int> processSlice = GetSubtupleInTupleSlicedBy(processOffset, agentSlice, processCount);

            if (distributionMode == DatapoolThreadDistributionMode.ThreadShared)
            {
                nonThreadUniqueValueBucket = new ValueBucket
                {
                    Values = values,
                    Name = datapoolMetadata.Name,
                    StartOffset = 0,
                    EndOffset = PhysicalSize - 1,
                    NextOffset = -1,
                    IsThreadUnique = false,
                    IsCircular = datapoolMetadata.IsCircular,
                    LogicalSize = processSlice.Item2 - processSlice.Item1 + 1
                };
            }
            else
            {
                threadUniqueValueBuckets = new ValueBucket[threadCount];

                for (int i = 0; i < threadCount; i++)
                {
                    Tuple<int, int> threadSlice = distributionMode == DatapoolThreadDistributionMode.ThreadUnique ?
                        GetSubtupleInTupleSlicedBy(i, processSlice, threadCount) :
                        new Tuple<int, int>(0, PhysicalSize - 1);
                    threadUniqueValueBuckets[i] = new ValueBucket
                    {
                        Values = values,
                        Name = datapoolMetadata.Name,
                        StartOffset = threadSlice.Item1,
                        EndOffset = threadSlice.Item2,
                        NextOffset = threadSlice.Item1 - 1,
                        IsThreadUnique = true,
                        IsCircular = datapoolMetadata.IsCircular,
                        LogicalSize = threadSlice.Item2 - threadSlice.Item1 + 1
                    };
                }
            }
        }

        public T NextValue()
        {
            return distributionMode == DatapoolThreadDistributionMode.ThreadShared ? NextThreadSharedValue() : NextThreadNonSharedValue();
        }

        private T NextThreadSharedValue()
        {
            return nonThreadUniqueValueBucket.NextValue(GrinderContext);
        }

        private T NextThreadNonSharedValue()
        {
            var threadNumber = GetValidThreadNumber();
            return threadUniqueValueBuckets[threadNumber].NextValue(GrinderContext);
        }

        private int ThreadSharedLogicalSize()
        {
            return nonThreadUniqueValueBucket.LogicalSize;
        }

        private int ThreadNonSharedLogicalSize()
        {
            var threadNumber = GetValidThreadNumber();
            return threadUniqueValueBuckets[threadNumber].LogicalSize;
        }

        private int GetValidThreadNumber()
        {
            int threadNumber = GrinderContext.ThreadNumber;
            if (threadNumber < 0)
            {
                throw new IllegalStateException(string.Format(CultureInfo.CurrentCulture, "ThreadNumber '{0}' is less than 0", threadNumber));
            }

            if (!(threadNumber < threadCount))
            {
                throw new IllegalStateException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "ThreadNumber '{0}' is not less than property '{1}': '{2}'",
                        threadNumber,
                        Constants.ThreadCountKey,
                        threadCount));
            }
            return threadNumber;
        }

        private class ValueBucket
        {
            private readonly SpinLocked spinLocked = new SpinLocked();

            internal string Name { private get; set; }
            internal T[] Values { private get; set; }
            internal int StartOffset { private get; set; }
            internal int EndOffset { private get; set; }
            internal int NextOffset { private get; set; }
            internal bool IsCircular { private get; set; }
            internal bool IsThreadUnique { private get; set; }
            internal int LogicalSize { get; set; }

            internal T NextValue(IGrinderContext grinderContext)
            {
                T result = null;
                spinLocked.DoLocked(() =>
                {
                    NextOffset++;
                    if (NextOffset > EndOffset)
                    {
                        if (!IsCircular)
                        {
                            throw new IllegalStateException(string.Format(
                                CultureInfo.CurrentCulture,
                                "Non circular {0}thread unique datapool '{1}' is drained: Agent number '{2}', process number '{3}' ({4}), thread number '{5}', run number '{6}'",
                                IsThreadUnique ? string.Empty : "non ",
                                Name,
                                grinderContext.AgentNumber,
                                grinderContext.ProcessNumber,
                                grinderContext.ProcessName,
                                grinderContext.ThreadNumber,
                                grinderContext.RunNumber
                                ));
                        }

                        NextOffset = StartOffset;
                    }

                    result = Values[NextOffset];
                });
                return result;
            }
        }

        private readonly ValueBucket nonThreadUniqueValueBucket;

        private readonly ValueBucket[] threadUniqueValueBuckets;

        private readonly DatapoolThreadDistributionMode distributionMode;

        private readonly int threadCount;

        internal static Tuple<int, int> GetSubtupleInTupleSlicedBy(int offset, Tuple<int, int> tuple, int slices)
        {
            int count = tuple.Item2 - tuple.Item1 + 1;
            int sliceSize = count / slices;
            int reminder = count % slices;
            int first;
            if (reminder > 0)
            {
                int bigSlices = Math.Min(offset, reminder);
                int normalSlices = Math.Max(0, offset - reminder);
                first = ((sliceSize + 1) * bigSlices) + (sliceSize * normalSlices) + tuple.Item1;
            }
            else
            {
                first = (sliceSize * offset) + tuple.Item1;
            }

            if (offset < reminder)
            {
                sliceSize++;
            }

            int last = first + sliceSize - 1;
            return new Tuple<int, int>(first, last);
        }

    }
}
