#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerificationWorker.cs" company="http://GrinderScript.net">
//
//   Copyright Â© 2012 Eirik Bjornset.
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

namespace GrinderScript.Net.Core.Framework
{
    public class VerificationWorker : DefaultWorker
    {
        protected override void DefaultInitialize()
        {
            Logger.Info("DefaultInitialize: Enter");
            int testsPrRun = GetTestPrRunFromProperty();
            ITest test = AddTest(1, "Dummytest used to verify that GrinderScript.Net works in The Grinder", () => { });
            for (int i = 1; i < testsPrRun; i++)
            {
                AddTest(test);
            }
            Logger.Info("DefaultInitialize: Exit");
        }

        private int GetTestPrRunFromProperty()
        {
            string valueString = GrinderContext.GetProperty(Constants.VerificationWorkerTestsPrRunKey, "1");
            int result = int.Parse(valueString, CultureInfo.CurrentCulture);
            if (result < 1)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property '{0}' should be > 0, but was {1}", Constants.VerificationWorkerTestsPrRunKey, result));
            }

            return result;
        }
    }
}