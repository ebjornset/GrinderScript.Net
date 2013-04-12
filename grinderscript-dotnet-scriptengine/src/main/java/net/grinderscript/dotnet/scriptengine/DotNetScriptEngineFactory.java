/*
 * Copyright © 2012 Eirik Bjornset.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package net.grinderscript.dotnet.scriptengine;

import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.ScriptLocation;

public interface DotNetScriptEngineFactory {
	DotNetScriptEngine createScriptEngine(GrinderProperties properties,
			ScriptLocation script);

	class Default implements DotNetScriptEngineFactory {

		@Override
		public DotNetScriptEngine createScriptEngine(
				GrinderProperties properties, ScriptLocation script) {
			return new DotNetScriptEngine(properties, script);
		}

	}
}
