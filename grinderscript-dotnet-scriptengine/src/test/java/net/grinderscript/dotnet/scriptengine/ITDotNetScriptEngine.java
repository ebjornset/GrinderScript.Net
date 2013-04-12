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

import java.io.File;

import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.testutility.AbstractJUnit4FileTestCase;
import net.grinder.util.Directory;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

public class ITDotNetScriptEngine extends AbstractJUnit4FileTestCase {

	private DotNetScriptEngine scriptEngine;
	private GrinderProperties properties;
	private ScriptLocation script;

	@Before
	public void before() throws Exception {
		properties = new GrinderProperties();
	}
	
	@After
	public void after() throws Exception {
		super.tearDown();
		if (scriptEngine != null) {
			scriptEngine.shutdown();
			scriptEngine = null;
		}
	}

	@Test
	public void shouldInitializeSuccessfullyWithValidConfiguration() throws Exception {
		script = new ScriptLocation(new Directory(
				getDirectory()), new File("my.dll"));
		DotNetScriptEngineService scriptEngineService = new DotNetScriptEngineService(properties);
		scriptEngine = (DotNetScriptEngine) scriptEngineService.createScriptEngine(script);
		scriptEngine.initialize();
	}

}
