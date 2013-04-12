#region Copyright, license and author information
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueResolverTimed.cs" company="http://GrinderScript.net">
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

namespace GrinderScript.Net.Core.Framework
{
    public class ValueResolverTimed<T> : ValueResolverBase<T>
    {
        private readonly Func<T> resolveFunc;

        private readonly long ttl;

        private long expiresAtTicks;

        private T cachedValue;

        public ValueResolverTimed(Func<T> resolveFunc, long ttl)
            : base(resolveFunc)
        {
            if (ttl < 1)
            {
                throw new ArgumentOutOfRangeException("ttl", string.Format("Can not be less than 1, but was '{0}'", ttl));
            }

            expiresAtTicks = DateTime.Now.AddMilliseconds(ttl).Ticks;
            this.resolveFunc = resolveFunc;
            this.ttl = ttl;
            cachedValue = resolveFunc();
        }

        public override T Value
        {
            get
            {
                var now = DateTime.Now;
                if (now.Ticks > expiresAtTicks)
                {
                    cachedValue = resolveFunc();
                    expiresAtTicks = now.AddMilliseconds(ttl).Ticks;
                }

                return cachedValue;
            }
        }
    }
}