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

import static java.util.Collections.emptyList;

import java.io.IOException;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.EngineException;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.scriptengine.Instrumenter;
import net.grinder.scriptengine.ScriptEngineService;
import net.grinder.util.FileExtensionMatcher;

/**
 * .Net script engine.
 * 
 * @author Eirik Bjornset
 */
public class DotNetScriptEngineService implements ScriptEngineService {

	private final Logger logger = LoggerFactory
			.getLogger(DotNetScriptEngineService.class);
	
	private final FileExtensionMatcher dllFileMatcher = new FileExtensionMatcher(".dll");

	private final GrinderProperties properties;
	private final DotNetScriptEngineFactory factory;

	DotNetScriptEngineService(GrinderProperties properties, DotNetScriptEngineFactory factory) {
		logger.trace("ctor: Enter, properties = {}, factory = {}", properties, factory);
		this.properties = properties;
		this.factory = factory;
		logger.trace("ctor: Exit");
	}
	
	public DotNetScriptEngineService(GrinderProperties properties) {
		this(properties, new DotNetScriptEngineFactory.Default());
	}

	/**
	 * {@inheritDoc}
	 */
	@Override public List<? extends Instrumenter> createInstrumenters()
			throws EngineException {
		return emptyList();
	}

	/**
	 * {@inheritDoc}
	 */
	@Override public ScriptEngine createScriptEngine(ScriptLocation script)
			throws EngineException {
		logger.trace("createScriptEngine: Enter, script = {}", script);
		ScriptEngine result = null;
		if (dllFileMatcher.accept(script.getFile())) {
			DotNetScriptEngine engine = factory.createScriptEngine(properties, script);
			try {
				engine.initialize();
			} catch (IOException e) {
				throw new EngineException("Could not initalize the CLR bridge",
						e);
			}
			result = engine;
		}
		logger.trace("createScriptEngine: Exit, result = {}", result);
		return result;
	}

	GrinderProperties getProperties()
	{
		return properties;
	}
	
	DotNetScriptEngineFactory getFactory() {
		return factory;
	}
}
