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

import grinderscript.net.core.IGrinderTest;
import net.grinder.script.Grinder.ScriptContext;
import net.grinder.script.InvalidContextException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class DotNetTestBridge implements IGrinderTest {

	private final Logger logger = LoggerFactory
			.getLogger(DotNetTestBridge.class);

	private final IGrinderTest testAction;
	private final ScriptContext scriptContext;

	public DotNetTestBridge(ScriptContext scriptContext, IGrinderTest testAction) {
		logger.trace("ctor: Enter, scriptContext = {}, testAction = {}", scriptContext, testAction);
		this.scriptContext = scriptContext;
		this.testAction = testAction;
		logger.trace("ctor: Exit");
	}

	@Override
	public void Run() {
		logger.trace("run: Enter");
		try {
			testAction.Run();
		} catch (system.Exception clrEx) {
			try {
				scriptContext.getStatistics().getForCurrentTest()
						.setSuccess(false);
			} catch (InvalidContextException e) {
				throw new DotNetCheckedExceptionMarshall(e);
			}
		}
		logger.trace("run: Exit");
	}

	IGrinderTest getTestAction() {
		return testAction;
	}
	
	ScriptContext getScriptContext() {
		return scriptContext;
	}
}
