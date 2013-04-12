#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatapoolTests.cs" company="http://GrinderScript.net">
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

using Moq;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    using System;

    using GrinderScript.Net.Core.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class DatapoolTests
    {
        private Mock<IGrinderContext> grinderContextMock;
        private Mock<IDatapoolMetatdata<TestValue>> metadataMock;

        private IList<TestValue> values;

        [SetUp]
        public void SetUp()
        {
            grinderContextMock = new Mock<IGrinderContext>();
            grinderContextMock.Setup(c => c.GetProperty(Constants.ThreadCountKey, "1")).Returns("1");
            metadataMock = new Mock<IDatapoolMetatdata<TestValue>>();
            values = new List<TestValue>();
            AddTestValues(2);
            metadataMock.Setup(md => md.Values).Returns(values);
            metadataMock.Setup((md => md.Name)).Returns("TestValue");
            SetupThreadSharedContextFor2Agents3Processes4Threads();
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenGrinderContextIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("grinderContext"), () => new Datapool<TestValue>(null, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenMetadataIsNull()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("datapoolMetadata"), () => new Datapool<TestValue>(grinderContextMock.Object, null));
        }

        [TestCase]
        public void CtorShouldSetGrinderContext()
        {
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            Assert.That(datapool.GrinderContext, Is.SameAs(grinderContextMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenNonThreadUniqueValuesIsLessThanOne()
        {
            AddTestValues(0);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("To low capacity for datapool 'TestValue', expected at least '1', but was '0'"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenThreadUniqueValuesIsLessThanNeeded()
        {
            AddTestValues(23);
            SetupThreadUniqueContextFor2Agents3Processes4Threads();
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("To low capacity for datapool 'TestValue', expected at least '24', but was '23'"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenAgentNumberIsEqualToAgentCount()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.AgentNumber).Returns(2);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadUnique);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot ceate thread unique datapool 'TestValue', because property 'grinderscript-dotnet.agentCount' = '2'. Current AgentNumber = '2' and indicates that 'grinderscript-dotnet.agentCount' must be at least '3' for thread uniqueness to work correctly"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenAgentNumberIsAboveAgentCount()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.AgentNumber).Returns(4);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadUnique);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot ceate thread unique datapool 'TestValue', because property 'grinderscript-dotnet.agentCount' = '2'. Current AgentNumber = '4' and indicates that 'grinderscript-dotnet.agentCount' must be at least '5' for thread uniqueness to work correctly"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenThreadOffsetIsNegative()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.AgentNumber).Returns(0);
            grinderContextMock.Setup(c => c.GetProperty(Constants.ThreadCountKey, "1")).Returns("3");
            grinderContextMock.Setup(c => c.FirstProcessNumber).Returns(2);
            grinderContextMock.Setup(c => c.ProcessNumber).Returns(1);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadUnique);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot ceate thread unique datapool 'TestValue', because thread offset negative (-1). FirstProcessNumber = 2, ProcessNumber = 1"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenThreadOffsetIsEqualToThreadCount()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.AgentNumber).Returns(1);
            grinderContextMock.Setup(c => c.GetProperty(Constants.ProcessCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.FirstProcessNumber).Returns(1);
            grinderContextMock.Setup(c => c.ProcessNumber).Returns(3);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadUnique);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot ceate thread unique datapool 'TestValue', because thread offset = '2' is not less than property 'grinder.threads' = '2'. FirstProcessNumber = 1, ProcessNumber = 3"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldThrowExceptionWhenThreadOffsetIsAboveThreadCount()
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.AgentNumber).Returns(0);
            grinderContextMock.Setup(c => c.GetProperty(Constants.ProcessCountKey, "1")).Returns("3");
            grinderContextMock.Setup(c => c.FirstProcessNumber).Returns(0);
            grinderContextMock.Setup(c => c.ProcessNumber).Returns(4);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadUnique);
            Assert.Throws(Is.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot ceate thread unique datapool 'TestValue', because thread offset = '4' is not less than property 'grinder.threads' = '3'. FirstProcessNumber = 0, ProcessNumber = 4"), () => new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object));
        }

        [TestCase]
        public void CtorShouldRandomizeWithoutException()
        {
            metadataMock.Setup(md => md.IsRandom).Returns(true);
            // ReSharper disable ObjectCreationAsStatement
            new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestCase]
        public void CtorShouldNotRandomizeOneValue()
        {
            metadataMock.Setup(md => md.Values).Returns(new List<TestValue> { new TestValue { IntValue = 23 } });
            metadataMock.Setup(md => md.IsRandom).Returns(true);
            // ReSharper disable ObjectCreationAsStatement
            new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestCase]
        public void NextValueShouldUseAllValuesInCircleWhenDatapoolIsThreadSharedAndCircular()
        {
            metadataMock.Setup(md => md.IsRandom).Returns(false);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadShared);
            metadataMock.Setup(md => md.IsCircular).Returns(true);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[1]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
        }

        [TestCase]
        public void NextValueShouldThrowExceptionWhenDatapoolIsThreadSharedAndNonCircularAndAllValuesAreUsed()
        {
            SetupContextForAgentNumber1ProcessNumber2ThreadNumber3RunNumber4();
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadShared);
            metadataMock.Setup(md => md.IsCircular).Returns(false);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            datapool.NextValue();
            datapool.NextValue();
            Assert.Throws(Is.InstanceOf<IllegalStateException>().With.Message.EqualTo("Non circular non thread unique datapool 'TestValue' is drained: Agent number '1', process number '2' (TestProcessYYY), thread number '3', run number '4'"), () => datapool.NextValue());
        }

        [TestCase]
        public void NextValueShouldThrowExceptionWhenDatapoolIsThreadUniqueAndThreadNumberIsNegative()
        {
            SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues();
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            grinderContextMock.Setup(gc => gc.ThreadNumber).Returns(-1);
            Assert.Throws(Is.InstanceOf<IllegalStateException>().With.Message.EqualTo("ThreadNumber '-1' is less than 0"), () => datapool.NextValue());
        }

        [TestCase]
        public void NextValueShouldThrowExceptionWhenDatapoolIsThreadUniqueAndThreadNumberIsNotLessThanThreadCount()
        {
            SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues();
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            grinderContextMock.Setup(gc => gc.ThreadNumber).Returns(4);
            Assert.Throws(Is.InstanceOf<IllegalStateException>().With.Message.EqualTo("ThreadNumber '4' is not less than property 'grinder.threads': '4'"), () => datapool.NextValue());
        }

        [TestCase]
        public void NextValueShouldCircleFirstBucketWhenDatapoolIsThreadUniqueAndCircularForFirstAgentProcessThread()
        {
            SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues();
            SetupContextForAgentNumber0ProcessNumber0ThreadNumber0RunNumber5();
            metadataMock.Setup(md => md.IsCircular).Returns(true);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[1]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[2]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
        }

        [TestCase]
        public void NextValueShouldThrowExceptionWhenDatapoolIsThreadUniqueAndNonCircularAndAllValuesInFirstBucketAreUsedForFirstAgentProcessThread()
        {
            SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues();
            SetupContextForAgentNumber0ProcessNumber0ThreadNumber0RunNumber5();
            metadataMock.Setup(md => md.IsCircular).Returns(false);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            datapool.NextValue();
            datapool.NextValue();
            datapool.NextValue();
            Assert.Throws(Is.InstanceOf<IllegalStateException>().With.Message.EqualTo("Non circular thread unique datapool 'TestValue' is drained: Agent number '0', process number '0' (TestProcessXXX), thread number '0', run number '5'"), () => datapool.NextValue());
        }

        [TestCase]
        public void NextValueShouldCircleLastBucketWhenDatapoolIsThreadUniqueAndCircularForLastAgentProcessThread()
        {
            SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues();
            SetupContextForAgentNumber1ProcessNumber2ThreadNumber3RunNumber4();
            metadataMock.Setup(md => md.IsCircular).Returns(true);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            Assert.That(datapool.NextValue(), Is.SameAs(values[47]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[48]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[47]));
        }

        [TestCase]
        public void NextValueShouldThrowExceptionWhenDatapoolIsThreadUniqueAndNonCircularAndAllValuesInLastBucketAreUsedForLastAgentProcessThread()
        {
            SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues();
            SetupContextForAgentNumber1ProcessNumber2ThreadNumber3RunNumber4();
            metadataMock.Setup(md => md.IsCircular).Returns(false);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            datapool.NextValue();
            datapool.NextValue();
            Assert.Throws(Is.InstanceOf<IllegalStateException>().With.Message.EqualTo("Non circular thread unique datapool 'TestValue' is drained: Agent number '1', process number '2' (TestProcessYYY), thread number '3', run number '4'"), () => datapool.NextValue());
        }

        [TestCase]
        public void NextValueShouldUseAllValuesInCircleForAllThreadsWhenDatapoolThreadCompleteAndCircular()
        {
            metadataMock.Setup(md => md.IsRandom).Returns(false);
            metadataMock.Setup(md => md.DistributionMode).Returns(DatapoolThreadDistributionMode.ThreadComplete);
            metadataMock.Setup(md => md.IsCircular).Returns(true);
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("1");
            grinderContextMock.Setup(c => c.GetProperty(Constants.ProcessCountKey, "1")).Returns("1");
            grinderContextMock.Setup(c => c.GetProperty(Constants.ThreadCountKey, "1")).Returns("2");
            grinderContextMock.Setup(gc => gc.AgentNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.FirstProcessNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.ProcessNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.ProcessName).Returns("TestProcessYYY");
            grinderContextMock.Setup(gc => gc.RunNumber).Returns(1);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);

            grinderContextMock.Setup(gc => gc.ThreadNumber).Returns(0);
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[1]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));

            grinderContextMock.Setup(gc => gc.ThreadNumber).Returns(1);
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[1]));
            Assert.That(datapool.NextValue(), Is.SameAs(values[0]));
        }

        [TestCase]
        public void PhysicalSizeShouldBeSameAsMetatdataValueSize()
        {
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            Assert.That(datapool.PhysicalSize, Is.EqualTo(values.Count));
        }

        [TestCase]
        public void LogicalSizeShouldBeSameAsPhysicalSizeWhenDistibutionModeIsThreadComplete()
        {
            SetupThreadCompleteContextFor2Agents3Processes4Threads();
            SetupContextForAgentNumber0ProcessNumber0ThreadNumber0RunNumber5();
            AddTestValues(49);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            Assert.That(datapool.LogicalSize, Is.EqualTo(datapool.PhysicalSize));
        }

        [TestCase]
        public void LogicalSizeShouldBeSameAsProcessBucketSizeWhenDistibutionModeIsThreadShared()
        {
            SetupThreadSharedContextFor2Agents3Processes4Threads();
            SetupContextForAgentNumber0ProcessNumber0ThreadNumber0RunNumber5();
            AddTestValues(49);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            // First process of first agent gets 49 / 2 (agentCount) (+ 1 leftover from division) = 25 / 3 (processCount) (+ 1 leftover from division) = 9
            Assert.That(datapool.LogicalSize, Is.EqualTo(9)); 
        }

        [TestCase]
        public void LogicalSizeShouldBeSameAsThreadBucketSizeWhenDistibutionModeIsThreadUnique()
        {
            SetupThreadUniqueContextFor2Agents3Processes4Threads();
            SetupContextForAgentNumber0ProcessNumber0ThreadNumber0RunNumber5();
            AddTestValues(49);
            var datapool = new Datapool<TestValue>(grinderContextMock.Object, metadataMock.Object);
            // First process of first agent gets 49 / 2 (agentCount) (+ 1 leftover from dividion) = 25 / 3 (processCount) (+ 1 leftover from division) = 9 / 4 (threadcount) (+ 1 leftover from division) = 3
            Assert.That(datapool.LogicalSize, Is.EqualTo(3));
        }

        [TestCase]
        public void GetSubtuple0InTuple0To4SlicedBy3ShouldBe0To1()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(0, new Tuple<int, int>(0, 4), 3), Is.EqualTo(new Tuple<int, int>(0, 1)));
        }

        [TestCase]
        public void GetSubtuple1InTuple0To4SliceddBy3ShouldBe2To3()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(1, new Tuple<int, int>(0, 4), 3), Is.EqualTo(new Tuple<int, int>(2, 3)));
        }

        [TestCase]
        public void GetSubtuple2InTuple0To4SlicedBy3ShouldBe4To4()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(2, new Tuple<int, int>(0, 4), 3), Is.EqualTo(new Tuple<int, int>(4, 4)));
        }

        [TestCase]
        public void GetSubtuple0InTuple0To3SlicedBy2ShouldBe0To1()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(0, new Tuple<int, int>(0, 3), 2), Is.EqualTo(new Tuple<int, int>(0, 1)));
        }

        [TestCase]
        public void GetSubtuple1InTuple0To3SliceddBy2ShouldBe2To3()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(1, new Tuple<int, int>(0, 3), 2), Is.EqualTo(new Tuple<int, int>(2, 3)));
        }

        [TestCase]
        public void GetSubtuple2InTuple25To48SliceddBy3ShouldBe41To48()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(2, new Tuple<int, int>(25, 48), 3), Is.EqualTo(new Tuple<int, int>(41, 48)));
        }

        [TestCase]
        public void GetSubtuple3InTuple41To48SliceddBy4ShouldBe47To48()
        {
            Assert.That(Datapool<TestValue>.GetSubtupleInTupleSlicedBy(3, new Tuple<int, int>(41, 48), 4), Is.EqualTo(new Tuple<int, int>(47, 48)));
        }

        private void SetupContextForAgentNumber0ProcessNumber0ThreadNumber0RunNumber5()
        {
            grinderContextMock.Setup(gc => gc.AgentNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.FirstProcessNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.ProcessNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.ProcessName).Returns("TestProcessXXX");
            grinderContextMock.Setup(gc => gc.ThreadNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.RunNumber).Returns(5);
        }

        private void SetupContextForAgentNumber1ProcessNumber2ThreadNumber3RunNumber4()
        {
            grinderContextMock.Setup(gc => gc.AgentNumber).Returns(1);
            grinderContextMock.Setup(gc => gc.FirstProcessNumber).Returns(0);
            grinderContextMock.Setup(gc => gc.ProcessNumber).Returns(2);
            grinderContextMock.Setup(gc => gc.ProcessName).Returns("TestProcessYYY");
            grinderContextMock.Setup(gc => gc.ThreadNumber).Returns(3);
            grinderContextMock.Setup(gc => gc.RunNumber).Returns(4);
        }

        private void SetupThreadUniqueContextFor2Agents3Processes4ThreadsAnd49TestValues()
        {
            SetupThreadUniqueContextFor2Agents3Processes4Threads();
            AddTestValues(49);
        }

        private void SetupThreadUniqueContextFor2Agents3Processes4Threads()
        {
            SetupContextFor2Agents3Processes4ThreadsAndDistributionMode(DatapoolThreadDistributionMode.ThreadUnique);
        }

        private void SetupThreadCompleteContextFor2Agents3Processes4Threads()
        {
            SetupContextFor2Agents3Processes4ThreadsAndDistributionMode(DatapoolThreadDistributionMode.ThreadComplete);
        }

        private void SetupThreadSharedContextFor2Agents3Processes4Threads()
        {
            SetupContextFor2Agents3Processes4ThreadsAndDistributionMode(DatapoolThreadDistributionMode.ThreadShared);
        }

        private void SetupContextFor2Agents3Processes4ThreadsAndDistributionMode(DatapoolThreadDistributionMode distributionMode)
        {
            grinderContextMock.Setup(c => c.GetProperty(Constants.AgentCountKey, "1")).Returns("2");
            grinderContextMock.Setup(c => c.GetProperty(Constants.ProcessCountKey, "1")).Returns("3");
            grinderContextMock.Setup(c => c.GetProperty(Constants.ThreadCountKey, "1")).Returns("4");
            metadataMock.Setup(md => md.DistributionMode).Returns(distributionMode);
        }

        private void AddTestValues(int count)
        {
            values.Clear();
            for (int i = 0; i < count; i++)
            {
                values.Add(new TestValue { IntValue = i });
            }
        }

        public class TestValue
        {
            public int IntValue { get; set; }
        }
    }
}
