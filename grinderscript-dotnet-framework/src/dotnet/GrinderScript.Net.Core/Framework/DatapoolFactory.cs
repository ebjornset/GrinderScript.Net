#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatapoolFactory.cs" company="http://GrinderScript.net">
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
using System.Globalization;
using System.Reflection;

namespace GrinderScript.Net.Core.Framework
{
    public class DatapoolFactory : IDatapoolFactory
    {
        internal IGrinderContext GrinderContext { get; set; }
        internal IDatapoolManager DatapoolManager { get; set; }

        public DatapoolFactory(IGrinderContext grinderContext, IDatapoolManager datapoolManager)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            if (datapoolManager == null)
            {
                throw new ArgumentNullException("datapoolManager");
            }

            GrinderContext = grinderContext;
            DatapoolManager = datapoolManager;
            TypeHelper = new TypeHelper(GrinderContext);
        }

        public void CreateDatapool<T>(IDatapoolValuesFactory<T> datapoolValuesFactory) where T : class
        {
            CreateDatapool(datapoolValuesFactory, typeof(T).Name);
        }

        public void CreateDatapool<T>(IDatapoolValuesFactory<T> datapoolValuesFactory, string name) where T : class
        {
            if (datapoolValuesFactory == null)
            {
                throw new ArgumentNullException("datapoolValuesFactory");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            IList<T> values = datapoolValuesFactory.CreateValues(GrinderContext, name);
            bool isRandom = bool.Parse(GrinderContext.GetProperty(GetPropertyKey(name, "random"), bool.FalseString));
            int seed = int.Parse(GrinderContext.GetProperty(GetPropertyKey(name, "seed"), ((int)DateTime.Now.Ticks).ToString()));
            var distributionMode = (DatapoolThreadDistributionMode)Enum.Parse(typeof(DatapoolThreadDistributionMode), GrinderContext.GetProperty(GetPropertyKey(name, "distributionMode"), DatapoolThreadDistributionMode.ThreadShared.ToString()), false);
            bool isCircular = bool.Parse(GrinderContext.GetProperty(GetPropertyKey(name, "circular"), bool.TrueString));
            var datapoolMetatdata = new DefaultDatapoolMetadata<T>(values, isRandom, seed, distributionMode, isCircular, name);
            CreateDatapool(datapoolMetatdata);
        }

        public void CreateDatapool<T>() where T : class
        {
            CreateDatapool<T>(typeof(T).Name);
        }

        public void CreateDatapool<T>(string name) where T : class
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            TypeHelper.LoadTargetTypeFromProperty<T>(GetPropertyKey(name, "valueType"), null);
            IDatapoolValuesFactory<T> valuesFactory = TypeHelper.CreateTargetTypeFromProperty<IDatapoolValuesFactory<T>>(GetPropertyKey(name, "factoryType"), null);
            CreateDatapool(valuesFactory, name);
        }

        public void CreateDatapool<T>(IDatapoolMetatdata<T> datapoolMetadata) where T : class
        {
            if (datapoolMetadata == null)
            {
                throw new ArgumentNullException("datapoolMetadata");
            }

            DatapoolManager.BuildDatapool(datapoolMetadata);
        }


        public void CreateMissingDatapoolsFromProperties()
        {
            int firstElement = GetValidFirstElement();
            int lastElement = GetValidLastElement(firstElement);
            for (int i = firstElement; i <= lastElement; i++)
            {
                string datapoolName = GrinderContext.GetProperty(GetFactoryPropertyKey(i.ToString()));
                if (!string.IsNullOrWhiteSpace(datapoolName))
                {
                    CreateMissingDatapoolFromProperties(datapoolName);
                }
            }
        }

        internal static string GetPropertyKey<T>(string suffix)
        {
            return GetPropertyKey(typeof(T).Name, suffix);
        }

        internal static string GetPropertyKey(string poolname, string suffix)
        {
            return Constants.DatapoolKeyPrefix + poolname + '.' + suffix;
        }

        internal static string GetFactoryPropertyKey(string suffix)
        {
            return Constants.DatapoolFactoryKeyPrefix + suffix;
        }

        internal protected TypeHelper TypeHelper { get; private set; }

        private void CreateMissingDatapoolFromProperties(string datapoolName)
        {
            if (DatapoolManager.ContainsDatapool(datapoolName))
            {
                return;
            }

            // Must call the generic CreateDatapool by reflection, since we are loading the value type from a string
            Type valueType = TypeHelper.LoadTargetTypeFromProperty(GetPropertyKey(datapoolName, "valueType"), null);
            MethodInfo method = GetType().GetMethod("CreateDatapool", new[] { typeof(string) });
            MethodInfo genericMethod = method.MakeGenericMethod(new[] { valueType });
            genericMethod.Invoke(this, new object[] { datapoolName });
        }

        private int GetValidLastElement(int firstElement)
        {
            int lastElement = GetIntProperty(GetFactoryPropertyKey("lastElement"), 100);
            if (lastElement < firstElement)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property '{0}' can not be < '{1}' ({2}), but was {3}", GetFactoryPropertyKey("lastElement"), GetFactoryPropertyKey("firstElement"), firstElement, lastElement));
            }

            return lastElement;
        }

        private int GetValidFirstElement()
        {
            int firstElement = GetIntProperty(GetFactoryPropertyKey("firstElement"), 1);
            if (firstElement < 1)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property '{0}' should be > 0, but was {1}", GetFactoryPropertyKey("firstElement"), firstElement));
            }

            return firstElement;
        }

        private int GetIntProperty(string key, int defaultValue)
        {
            string valueString = GrinderContext.GetProperty(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return int.Parse(valueString, CultureInfo.CurrentCulture);
        }
    }
}
