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

import grinderscript.net.core.IGrinderScriptEngine;
import grinderscript.net.core.IGrinderContext;
import grinderscript.net.core.IGrinderWorker;
import grinderscript.net.core.framework.ScriptEngineBridgeFactory;

import java.io.File;
import java.io.IOException;
import java.util.Properties;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.EngineException;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.script.Grinder;
import net.grinder.script.Grinder.ScriptContext;
import net.grinder.scriptengine.ScriptEngineService.ScriptEngine;
import net.grinder.scriptengine.ScriptEngineService.WorkerRunnable;
import net.sf.jni4net.Bridge;

/**
 * .Net script engine.
 * 
 * @author Eirik Bjornset
 */
public class DotNetScriptEngine implements ScriptEngine {

	private final Logger logger = LoggerFactory
			.getLogger(DotNetScriptEngine.class);

	private final GrinderProperties properties;
	private final IGrinderContext context;
	private IGrinderScriptEngine underlying;

	public DotNetScriptEngine(GrinderProperties properties,
			ScriptLocation script) {
		this(Grinder.grinder, properties, script);
	}

	public DotNetScriptEngine(ScriptContext scriptContext,
			GrinderProperties properties, ScriptLocation script) {
		logger.trace(
				"ctor: Enter, scriptContext = {}, properties =  {}, script = {}",
				new Object[] { scriptContext, properties, script });
		this.properties = properties;
		context = new DotNetContextBridge(scriptContext, properties,
				script);
		logger.trace("ctor: Exit");
	}

	public void initialize() throws DotNetScriptExecutionException, IOException {
		logger.trace("initialize: Enter");
		Bridge.setVerbose(properties.getBoolean(
				DotNetUtils.getPropertyName("bridgeVerbose"), false));
		Bridge.setDebug(properties.getBoolean(
				DotNetUtils.getPropertyName("bridgeDebug"), false));
		Properties jn4Properties = DotNetUtils.getPropertiesFromResources();
		logger.trace("initialize: jn4Properties = {}", jn4Properties);
		String j4nLocation = getJ4nLocationFromPropertiesOrCodeLocation(
				properties, jn4Properties);
		logger.trace("initialize: j4nLocation = {}", j4nLocation);
		String scriptAssemblyName = DotNetUtils.getProperty(jn4Properties,
				DotNetUtils.SCRIPT_ASSEMBLY_PROPERTY_SUFFIX);
		logger.trace("initialize: scriptAssemblyName = {}", scriptAssemblyName);
		String scriptAssemblyCanonicalPath = DotNetUtils
				.getCanonicalLocationFilePath(j4nLocation, scriptAssemblyName);
		logger.trace("initialize: scriptAssemblyCanonicalPath = {}",
				scriptAssemblyCanonicalPath);
		String j4nAssemblyName = DotNetUtils.getProperty(jn4Properties,
				DotNetUtils.J4N_ASSEMBLY_PROPERTY_SUFFIX);
		logger.trace("initialize: j4nAssemblyName = {}", j4nAssemblyName);
		String j4nAssemblyCanonicalPath = DotNetUtils
				.getCanonicalLocationFilePath(j4nLocation, j4nAssemblyName);
		logger.trace("initialize: j4nAssemblyCanonicalPath = {}",
				j4nAssemblyCanonicalPath);
		Bridge.init(new File(j4nLocation));
		Bridge.LoadAndRegisterAssemblyFrom(new File(scriptAssemblyCanonicalPath));
		Bridge.LoadAndRegisterAssemblyFrom(new File(j4nAssemblyCanonicalPath));
		initialize(ScriptEngineBridgeFactory.CreateBridge(context));
		logger.trace("initialize: Exit");
	}

	@Override
	public WorkerRunnable createWorkerRunnable() throws EngineException {
		logger.trace("createWorkerRunnable: Enter");
		WorkerRunnable result = createWorkerRunnable(null);
		logger.trace("createWorkerRunnable: Exit");
		return result;
	}

	@Override
	public WorkerRunnable createWorkerRunnable(Object testRunner)
			throws EngineException {
		logger.trace("createWorkerRunnable: Enter, testRunner = {}", testRunner);
		IGrinderWorker worker = underlying.CreateWorkerRunnable();
		logger.trace("createWorkerRunnable: Exit");
		return new DotNetWorkerRunnable(worker);
	}

	@Override
	public String getDescription() {
		return "GrinderScript.Net Script Engine";
	}

	@Override
	public void shutdown() throws EngineException {
		logger.trace("shutdown: Enter");
		if (underlying != null) {
			try {
				underlying.Shutdown();
			} finally {
				underlying = null;
			}
		}
		logger.trace("shutdown: Exit");
	}
	
	void initialize(IGrinderScriptEngine scriptEngine) {
		logger.trace("initialize: Enter, scriptEngine = {}", scriptEngine);
		scriptEngine.Initialize();
		this.underlying = scriptEngine;
		logger.trace("initialize: Exit");
	}

	String getJ4nLocationFromPropertiesOrCodeLocation(
			GrinderProperties grinderProperties, Properties j4nProperties)
			throws DotNetScriptExecutionException, IOException {
		logger.trace(
				"getJ4nLocationFromPropertiesOrCodeLocation: Enter, filePropertySuffix = {}, grinderProperties = {}, j4nProperties = {}",
				grinderProperties, j4nProperties);
		String j4nLocation;
		String j4nLocationPropertyKey = DotNetUtils
				.getPropertyName(DotNetUtils.J4N_LOCATION_PROPERTY_SUFFIX);
		if (grinderProperties.containsKey(j4nLocationPropertyKey)) {
			j4nLocation = grinderProperties.getProperty(j4nLocationPropertyKey);
			logger.trace(
					"getJ4nLocationFromPropertiesOrCodeLocation: Found j4nLocation property \"{}\" in grinderProperties",
					j4nLocationPropertyKey);
		} else if (j4nProperties.containsKey(j4nLocationPropertyKey)) {
			j4nLocation = j4nProperties.getProperty(j4nLocationPropertyKey);
			logger.trace(
					"getJ4nLocationFromPropertiesOrCodeLocation: Found j4nLocation property \"{}\" in j4nProperties",
					j4nLocationPropertyKey);
		} else {
			j4nLocation = DotNetUtils.getDirectoryFromJ4nCodeLocation();
			logger.trace("getJ4nLocationFromPropertiesOrCodeLocation: Using j4n code base as j4nLocation");
		}
		logger.trace(
				"getJ4nLocationFromPropertiesOrCodeLocation: Exit, j4nLocation = \"{}\"",
				j4nLocation);
		return j4nLocation;
	}
	
	GrinderProperties getProperties() {
		return properties;
	}
	
	IGrinderContext getContext()
	{
		return context;
	}
	
	IGrinderScriptEngine getUnderlying() {
		return underlying;
	}
	
}
