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

import grinderscript.net.core.IGrinderContext;
import grinderscript.net.core.IGrinderLogger;
import grinderscript.net.core.IGrinderTest;

import java.io.IOException;

import net.grinder.common.GrinderException;
import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.script.Grinder.ScriptContext;
import net.grinder.script.NonInstrumentableTypeException;
import net.grinder.script.Test;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import system.Type;

public class DotNetContextBridge implements IGrinderContext {

	private final Logger logger = LoggerFactory
			.getLogger(DotNetContextBridge.class);

	private final ScriptContext scriptContext;
	private final GrinderProperties properties;
	private final ScriptLocation script;

	public DotNetContextBridge(ScriptContext scriptContext,
			GrinderProperties properties, ScriptLocation script) {
		logger.trace(
				"ctor: Enter, scriptContext = {}, properties = {}, script = {}",
				new Object[] { scriptContext, properties, script });
		this.scriptContext = scriptContext;
		this.properties = properties;
		this.script = script;
		logger.trace("ctor: Exit");
	}

	@Override
	public void Sleep(long meanTime) {
		logger.trace("Sleep: Enter, meanTime = {}", meanTime);
		try {
			scriptContext.sleep(meanTime);
		} catch (GrinderException e) {
			throw new DotNetCheckedExceptionMarshall(e);
		}
		logger.trace("Sleep: Exit");
	}

	@Override
	public void Sleep(long meanTime, long sigma) {
		logger.trace("Sleep: Enter, meanTime = {}, sigma = {}", meanTime, sigma);
		try {
			scriptContext.sleep(meanTime, sigma);
		} catch (GrinderException e) {
			throw new DotNetCheckedExceptionMarshall(e);
		}
		logger.trace("Sleep: Exit");
	}

	@Override
	public IGrinderLogger GetLogger(String name) {
		return new DotNetLoggerBridge(name);
	}

	@Override
	public IGrinderLogger GetLogger(Type type) {
		return new DotNetLoggerBridge(type);
	}

	@Override
	public IGrinderTest CreateTest(int number, String description, IGrinderTest testAction) {
		logger.trace(
				"CreateTest: Enter, number = {}, description = {}, testAction = {}",
				new Object[] { number, description, testAction });
		Test test = new Test(number, description);
		DotNetTestBridge testBridge = new DotNetTestBridge(scriptContext, testAction);
		try {
			test.record(testBridge);
		} catch (NonInstrumentableTypeException e) {
			throw new DotNetCheckedExceptionMarshall(e);
		}
		logger.trace("CreateTest: Exit");
		return testBridge;
	}

	@Override
	public boolean ContainsProperty(String key) {
		return properties.containsKey(key);
	}

	@Override
	public String GetProperty(String key) {
		return properties.getProperty(key);
	}

	@Override
	public String GetProperty(String key, String defaultValue) {
		return properties.getProperty(key, defaultValue);
	}

	@Override
	public String getScriptFile() {
		try {
			return script.getFile().getCanonicalPath();
		} catch (IOException e) {
			throw new DotNetCheckedExceptionMarshall(e);
		}
	}

	@Override
	public int getAgentNumber() {
		return scriptContext.getAgentNumber();
	}

	@Override
	public int getProcessNumber() {
		return scriptContext.getProcessNumber();
	}

	@Override
	public String getProcessName() {
		return scriptContext.getProcessName();
	}

	@Override
	public int getFirstProcessNumber() {
		return scriptContext.getFirstProcessNumber();
	}

	@Override
	public int getThreadNumber() {
		return scriptContext.getThreadNumber();
	}

	@Override
	public int getRunNumber() {
		return scriptContext.getRunNumber();
	}
	
	
	ScriptContext getScriptContext() {
		return scriptContext;
	}
	
	GrinderProperties getProperties() {
		return properties;
	}
	
	ScriptLocation getScript() {
		return script;
	}

}
