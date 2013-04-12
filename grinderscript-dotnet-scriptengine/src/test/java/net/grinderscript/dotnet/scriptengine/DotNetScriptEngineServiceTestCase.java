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

import static org.hamcrest.CoreMatchers.equalTo;
import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.CoreMatchers.notNullValue;
import static org.hamcrest.CoreMatchers.nullValue;
import static org.hamcrest.CoreMatchers.sameInstance;
import static org.junit.Assert.assertThat;
import static org.junit.Assert.fail;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.doThrow;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;
import static org.mockito.Mockito.verify;

import java.io.File;
import java.io.IOException;
import java.util.List;

import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.EngineException;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.scriptengine.Instrumenter;
import net.grinder.scriptengine.ScriptEngineService.ScriptEngine;

import org.junit.Before;
import org.junit.Test;

public class DotNetScriptEngineServiceTestCase {

	private DotNetScriptEngineService service;
	private GrinderProperties properties;
	private DotNetScriptEngineFactory factory;
	private DotNetScriptEngine engine;

	@Before
	public void before() throws Exception {
		properties = mock(GrinderProperties.class);
		factory = mock(DotNetScriptEngineFactory.class);
		engine = mock(DotNetScriptEngine.class);
		when(factory.createScriptEngine(any(GrinderProperties.class), any(ScriptLocation.class))).thenReturn(engine);
		service = new DotNetScriptEngineService(properties, factory);
	}

	@Test
	public void packageCtorShouldSetProperties() {
		DotNetScriptEngineService service = new DotNetScriptEngineService(properties);
		assertThat(service.getProperties(), is(sameInstance(properties)));
	}
	
	@Test
	public void packageCtorShouldSetFactory() {
		assertThat(service.getFactory(), is(sameInstance(factory)));
	}

	@Test
	public void publicCtorShouldSetProperties() {
		assertThat(service.getProperties(), is(sameInstance(properties)));
	}
	
	@Test
	public void publicCtorShouldSetFactory() {
		DotNetScriptEngineService service = new DotNetScriptEngineService(properties);
		assertThat(service.getFactory(), is(notNullValue()));
	}
	
	@Test
	public void createInstrumentersShouldReturnEmptyList() throws Exception {
		List<? extends Instrumenter> instrumenters = service.createInstrumenters();	
		assertThat(instrumenters, is(notNullValue()));
		assertThat(instrumenters.size(), is(equalTo(0)));
	}
	
	@Test
	public void createScriptEngineWithDllScriptShouldReturnADotNetScriptEngine() throws Exception {
		File scriptFile = createTestScriptFile(".dLL");
		ScriptEngine scriptEngine = service.createScriptEngine(new ScriptLocation(scriptFile));
		assertThat(scriptEngine, is(sameInstance((ScriptEngine)engine)));
	}

	@Test
	public void createScriptEngineShouldInitializeEngine() throws Exception {
		File scriptFile = createTestScriptFile(".dLL");
		service.createScriptEngine(new ScriptLocation(scriptFile));
		verify(engine).initialize();
	}
	
	@Test
	public void createScriptEngineShouldThrowEngineExceptionWhenInitializeFails() throws Exception {
		IOException expected = new IOException();
		doThrow(expected).when(engine).initialize();
		File scriptFile = createTestScriptFile(".dLL");
		try {
			service.createScriptEngine(new ScriptLocation(scriptFile));
			fail("Expected exception");
		} catch (EngineException e) {
			assertThat(e.getMessage(), is(equalTo("Could not initalize the CLR bridge")));
			assertThat(e.getCause(), is(sameInstance((Throwable)expected)));
		}
	}
	
	@Test
	public void createScriptEngineWithNonDllScriptShouldReturnNull() throws Exception {
		File scriptFile = createTestScriptFile(".dlx");
		ScriptEngine scriptEngine = service.createScriptEngine(new ScriptLocation(scriptFile));
		assertThat(scriptEngine, is(nullValue()));
	}

	private File createTestScriptFile(String suffix) throws IOException {
		File scriptFile = File.createTempFile("dummy", suffix);
		scriptFile.deleteOnExit();
		return scriptFile;
	}
	
}
