#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvDatapoolValuesFactory.cs" company="http://GrinderScript.net">
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CsvHelper;
using CsvHelper.Configuration;

using GrinderScript.Net.Core;
using GrinderScript.Net.Core.Framework;

namespace GrinderScript.Net.Csv
{
    public class CsvDatapoolValuesFactory<T> : IDatapoolValuesFactory<T> where T : class 
    {
        public IList<T> CreateValues(IGrinderContext grinderContext)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            string name = typeof(T).Name;
            return CreateValues(grinderContext, name);

        }

        public IList<T> CreateValues(IGrinderContext grinderContext, string name)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            string fileName = grinderContext.GetProperty(DatapoolFactory.GetPropertyKey(name, "csvFile"), string.Format("{0}.csv", name));
            return CreateValues(fileName);
        }

        public IList<T> CreateValues(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var configuration = new CsvConfiguration { AllowComments = true, Delimiter = ";", IsCaseSensitive = false, HasHeaderRecord = true};
            using (var csv = new CsvReader(new StreamReader(fileName), configuration))
            {
                var result = csv.GetRecords<T>().ToList();
                return result;
            }
        }
    }
}
