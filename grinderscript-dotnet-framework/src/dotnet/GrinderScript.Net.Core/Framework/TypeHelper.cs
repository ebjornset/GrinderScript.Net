#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="http://GrinderScript.net">
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
using System.Reflection;

namespace GrinderScript.Net.Core.Framework
{
    public class TypeHelper
    {
        internal IGrinderLogger Logger { get; private set; }
        internal IGrinderContext GrinderContext { get; set; }

        public TypeHelper(IGrinderContext grinderContext)
        {
            if (grinderContext == null)
            {
                throw new ArgumentNullException("grinderContext");
            }

            Logger = new LoggerFacade(grinderContext.GetLogger(typeof(TypeHelper)), grinderContext);
            GrinderContext = grinderContext;
        }

        public T CreateTargetTypeFromProperty<T>(string typeKey, Type defaultType) where T : class
        {
            Logger.Trace(m => m("CreateTargetTypeFromProperty: Enter, typeKey = {0}, defaultType = {1}", typeKey, defaultType));
            Type resultType = LoadTargetTypeFromProperty<T>(typeKey, defaultType);
            T result = InstantiateTagetType<T>(resultType);
            Logger.Trace(m => m("CreateTargetTypeFromProperty: Exit, Result = {0}", result));
            return result;
        }

        public T CreateTargetTypeFromName<T>(string typeName) where T : class
        {
            Logger.Trace(m => m("CreateTargetTypeFromName: Enter, typeName = {0}", typeName));
            Type resultType = LoadTargetTypeFromName<T>(typeName);
            T result = InstantiateTagetType<T>(resultType);
            Logger.Trace(m => m("CreateTargetTypeFromName: Exit, Result = {0}", result));
            return result;
        }

        private T InstantiateTagetType<T>(Type targetType)
        {
            Logger.Trace(m => m("InstantiateTaget: Enter, targetType = {0}, T = {1}", targetType.FullName, typeof(T).FullName));
            targetType = ValidateType<T>(targetType);
            var result = Activator.CreateInstance(targetType);
            Logger.Trace(m => m("InstantiateTaget: Exit, Result = {0}", result));
            return (T)result;
        }

        public Type LoadTargetTypeFromProperty<T>(string typeKey, Type defaultType) where T : class
        {
            Logger.Trace(m => m("LoadTargetTypeFromProperty: Enter, typeKey = {0}, defaultType = {1}", typeKey, defaultType));
            string typeName = GrinderContext.GetProperty(typeKey);
            if (string.IsNullOrWhiteSpace(typeName))
            {
                if (defaultType == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Missing property '{0}'", typeKey));
                }

                defaultType = ValidateType<T>(defaultType);
                return defaultType;
            }

            Type result = LoadTargetTypeFromName<T>(typeName);
            Logger.Trace(m => m("LoadTargetTypeFromProperty: Exit, Result = {0}", result));
            return result;
        }

        public Type LoadTargetTypeFromName<T>(string typeName) where T : class
        {
            Logger.Trace(m => m("LoadTargetTypeFromName: Enter, typeName = {0}", typeName));
            Type targetType = LoadTargetTypeFromName(typeName);
            targetType = ValidateType<T>(targetType);
            Logger.Trace(m => m("LoadTargetTypeFromName: Exit, Result = {0}", targetType.FullName));
            return targetType;
        }

        public Type LoadTargetTypeFromProperty(string typeKey, Type defaultType)
        {
            Logger.Trace(m => m("LoadTargetTypeFromProperty: Enter, typeKey = {0}, defaultType = {1}", typeKey, defaultType));
            string typeName = GrinderContext.GetProperty(typeKey);
            if (string.IsNullOrWhiteSpace(typeName))
            {
                if (defaultType == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Missing property '{0}'", typeKey));
                }

                return defaultType;
            }

            Type result = LoadTargetTypeFromName(typeName);
            Logger.Trace(m => m("LoadTargetTypeFromProperty: Exit, Result = {0}", result));
            return result;
        }

        public Type LoadTargetTypeFromName(string typeName)
        {
            Logger.Trace(m => m("LoadTargetTypeFromName: Enter, typeName = {0}", typeName));
            Assembly assembly = Assembly.LoadFrom(GrinderContext.ScriptFile);
            Type targetType = assembly.GetType(typeName) ?? Type.GetType(typeName);
            if (targetType == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Unknown type: '{0}'", typeName));
            }

            Logger.Trace(m => m("LoadTargetTypeFromName: Exit, Result = {0}", targetType.FullName));
            return targetType;
        }

        private Type ValidateType<T>(Type type)
        {
            Logger.Trace(m => m("ValidateType: Enter, type = {0}, T = {1}", type.FullName, typeof(T).FullName));
            Type resultType = type;
            var expectedType = typeof(T);
            if (!expectedType.IsAssignableFrom(type))
            {
                if (type.IsGenericTypeDefinition && expectedType.IsGenericType)
                {
                    var genericType = type.MakeGenericType(expectedType.GetGenericArguments());
                    if (!expectedType.IsAssignableFrom(genericType))
                    {
                        throw CreateWrongTypeArgumentException<T>(type);
                    }

                    resultType = genericType;
                }
                else
                {
                    throw CreateWrongTypeArgumentException<T>(type);
                }
            }

            Logger.Trace(m => m("ValidateType: Exit, resultType = {0}", resultType.FullName));
            return resultType;
        }

        private static ArgumentException CreateWrongTypeArgumentException<T>(Type type)
        {
            var exeption = new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Type '{0}' is not '{1}'", type.FullName, typeof(T).FullName));
            return exeption;
        }
    }
}
