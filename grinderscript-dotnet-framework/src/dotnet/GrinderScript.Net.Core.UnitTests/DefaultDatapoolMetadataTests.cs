#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaulDatapoolMetadataTests.cs" company="http://GrinderScript.net">
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

using NUnit.Framework;
using System.Collections.Generic;

namespace GrinderScript.Net.Core.UnitTests
{
    [TestFixture]
    public class DefaulDatapoolMetadataTests
    {
        [TestCase]
        public void CtorShouldRequireValues()
        {
            Assert.Throws(Is.InstanceOf<ArgumentNullException>().With.Message.Contains("values"), () => new DefaultDatapoolMetadata<object>(null, false, 0, DatapoolThreadDistributionMode.ThreadShared, false));
        }

        [TestCase]
        public void CtorShouldSetValues()
        {
            var values = new List<object>();
            var metaData = new DefaultDatapoolMetadata<object>(values, true, 0, DatapoolThreadDistributionMode.ThreadShared, false);
            Assert.That(metaData.Values == values);
        }

        [TestCase]
        public void CtorShouldUsePassedInName()
        {
            var metaData = new DefaultDatapoolMetadata<object>(new List<object>(), true, 0, DatapoolThreadDistributionMode.ThreadShared, false, "TestName");
            Assert.That(metaData.Name == "TestName");
        }

        [TestCase]
        public void CtorShouldUseTypeNameWhenNoNameIsPassedIn()
        {
            var metaData = new DefaultDatapoolMetadata<object>(new List<object>(), true, 0, DatapoolThreadDistributionMode.ThreadShared, false, " ");
            Assert.That(metaData.Name == "Object");
        }

        [TestCase]
        public void CtorShouldSetISRandom()
        {
            var metaData = new DefaultDatapoolMetadata<object>(new List<object>(), true, 0, DatapoolThreadDistributionMode.ThreadShared, false);
            Assert.That(metaData.IsRandom);
        }

        [TestCase]
        public void CtorShouldSetSeed()
        {
            var metaData = new DefaultDatapoolMetadata<object>(new List<object>(), false, 12, DatapoolThreadDistributionMode.ThreadShared, false);
            Assert.That(metaData.Seed == 12);
        }

        [TestCase]
        public void CtorShouldSetIsThreadUnique()
        {
            var metaData = new DefaultDatapoolMetadata<object>(new List<object>(), false, 0, DatapoolThreadDistributionMode.ThreadUnique, false);
            Assert.That(metaData.DistributionMode == DatapoolThreadDistributionMode.ThreadUnique);
        }

        [TestCase]
        public void CtorShouldSetIsSircular()
        {
            var metaData = new DefaultDatapoolMetadata<object>(new List<object>(), false, 0, DatapoolThreadDistributionMode.ThreadShared, true);
            Assert.That(metaData.IsCircular);
        }
    }
}
