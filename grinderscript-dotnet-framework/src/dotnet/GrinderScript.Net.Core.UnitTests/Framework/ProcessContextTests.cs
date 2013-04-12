#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessContextTests.cs" company="http://GrinderScript.net">
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

using GrinderScript.Net.Core.Framework;

using Moq;

using NUnit.Framework;

namespace GrinderScript.Net.Core.UnitTests.Framework
{
    [TestFixture]
    public class ProcessContextTests
    {
        private string binFolder;
        private Mock<IDatapoolFactory> datapoolFactoryMock;
        private Mock<IDatapoolManager> datapoolManagerMock;
        private Mock<IGrinderContext> grinderContextMock;

        [SetUp]
        public void SetUp()
        {
            binFolder = Guid.NewGuid().ToString();
            datapoolFactoryMock = new Mock<IDatapoolFactory>();
            datapoolManagerMock = new Mock<IDatapoolManager>();
            grinderContextMock = new Mock<IGrinderContext>();
            CreateFrozenProcessContext();
        }

        [TestCase]
        public void FreezeShouldUseSafeTransformState()
        {
            Mock<IStateHelper> stateHelperMock = SetupStateHelperMock();
            var processContext = CreateEditableProcessContext();
            processContext.FrozenStateHelper = stateHelperMock.Object;
            processContext.Freeze();
            stateHelperMock.Verify();
        }

        [TestCase]
        public void FreezeShouldThorwExceptionWhenDatapoolFactoryIsMissing()
        {
            var processContext = CreateEditableProcessContext();
            processContext.DatapoolFactory = null;
            Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.Contains("DatapoolFactory"), processContext.Freeze);
        }

        [TestCase]
        public void FreezeShouldThorwExceptionWhenDatapoolManagerIsMissing()
        {
            var processContext = CreateEditableProcessContext();
            processContext.DatapoolManager = null;
            Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.Contains("DatapoolManager"), processContext.Freeze);
        }

        [TestCase]
        public void FreezeShouldThorwExceptionWhenGrinderContextIsMissing()
        {
            var processContext = CreateEditableProcessContext();
            processContext.GrinderContext = null;
            Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.Contains("GrinderContext"), processContext.Freeze);
        }

        [TestCase]
        public void InitializeAwarenessShouldCheckIsFrozen()
        {
            Mock<IStateHelper> stateHelperMock = SetupStateHelperMock();
            var processContext = CreateEditableProcessContext();
            processContext.FrozenStateHelper = stateHelperMock.Object;
            processContext.InitializeAwareness(new object());
            stateHelperMock.Verify(sh => sh.CheckIsInState());
        }

        [TestCase]
        public void FreezeShouldWorkWhenAllMandatoryPropertiesAreSet()
        {
            CreateEditableProcessContext().Freeze();
        }

        [TestCase]
        public void InitializeAwarenessWhenTargetIsBinFolderAwareShouldSetTargetBinFolder()
        {
            var target = new BinFolderAwareGrinderElement();
            var processContext = CreateFrozenProcessContext();
            processContext.InitializeAwareness(target);
            Assert.That(target.BinFolder, Is.EqualTo(processContext.BinFolder));
        }

        [TestCase]
        public void InitializeAwarenessWhenTargetIsDatapoolFactoryAwareShouldSetTargetDatapoolFactory()
        {
            var target = new DatapoolFactoryAwareGrinderElement();
            var processContext = CreateFrozenProcessContext();
            processContext.InitializeAwareness(target);
            Assert.That(target.DatapoolFactory, Is.SameAs(processContext.DatapoolFactory));
        }

        [TestCase]
        public void InitializeAwarenessWhenTargetIsDatapoolManagerAwareShouldSetTargetDatapoolManager()
        {
            var target = new DatapoolManagerAwareGrinderElement();
            var processContext = CreateFrozenProcessContext();
            processContext.InitializeAwareness(target);
            Assert.That(target.DatapoolManager, Is.SameAs(processContext.DatapoolManager));
        }

        [TestCase]
        public void InitializeAwarenessWhenTargetIsGrinderContextAwareShouldSetTargetGrinderContext()
        {
            var target = new GrinderContextAwareGrinderElement();
            var processContext = CreateFrozenProcessContext();
            processContext.InitializeAwareness(target);
            Assert.That(target.GrinderContext, Is.SameAs(processContext.GrinderContext));
        }

        [TestCase]
        public void InitializeAwarenessWhenTargetIsProcessContextAwareShouldSetTargetProcessContext()
        {
            var target = new ProcessContextAwareGrinderElement();
            var processContext = CreateFrozenProcessContext();
            processContext.InitializeAwareness(target);
            Assert.That(target.ProcessContext, Is.SameAs(processContext));
        }

        private ProcessContext CreateEditableProcessContext()
        {
            var processContext = new ProcessContext
            {
                BinFolder = binFolder,
                DatapoolFactory = datapoolFactoryMock.Object,
                DatapoolManager = datapoolManagerMock.Object,
                GrinderContext = grinderContextMock.Object,
            };

            return processContext;
        }

        private ProcessContext CreateFrozenProcessContext()
        {
            var processContext = CreateEditableProcessContext();
            processContext.Freeze();
            return processContext;
        }

        private static Mock<IStateHelper> SetupStateHelperMock()
        {
            var stateHelperMock = new Mock<IStateHelper>();
            stateHelperMock.Setup(sh => sh.SafeTransformToState(It.IsAny<Action>())).Callback<Action>(a => a()).Verifiable();
            return stateHelperMock;
        }

        private class BinFolderAwareGrinderElement : AbstractGrinderElement, IBinFolderAware
        {
            public string BinFolder { get; set; }
        }

        private class DatapoolFactoryAwareGrinderElement : AbstractGrinderElement, IDatapoolFactoryAware
        {
            public IDatapoolFactory DatapoolFactory { get; set; }
        }

        private class DatapoolManagerAwareGrinderElement : AbstractGrinderElement, IDatapoolManagerAware
        {
            public IDatapoolManager DatapoolManager { get; set; }
        }

        private class GrinderContextAwareGrinderElement : AbstractGrinderElement, IGrinderContextAware
        {
            public new IGrinderContext GrinderContext { get; set; }
        }

        private class ProcessContextAwareGrinderElement : AbstractGrinderElement, IProcessContextAware
        {
            public IProcessContext ProcessContext { get; set; }
        }
    }
}
