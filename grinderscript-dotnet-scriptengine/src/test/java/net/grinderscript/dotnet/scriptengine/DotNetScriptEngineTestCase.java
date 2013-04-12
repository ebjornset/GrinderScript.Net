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
import static org.hamcrest.CoreMatchers.instanceOf;
import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.CoreMatchers.nullValue;
import static org.hamcrest.CoreMatchers.sameInstance;
import static org.junit.Assert.assertThat;
import static org.junit.Assert.fail;
import static org.mockito.Mockito.doThrow;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import grinderscript.net.core.IGrinderScriptEngine;
import grinderscript.net.core.IGrinderWorker;
import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.scriptengine.ScriptEngineService.WorkerRunnable;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

public class DotNetScriptEngineTestCase {

	private DotNetScriptEngine scriptEngine;
	private GrinderProperties properties;
	private ScriptLocation script;
	IGrinderScriptEngine underlying;

	@Before
	public void before() throws Exception {
		properties = new GrinderProperties();
		scriptEngine = new DotNetScriptEngine(properties, script);
		underlying = mock(IGrinderScriptEngine.class);
	}

	@After
	public void after() throws Exception {
		scriptEngine.shutdown();
	}

	@Test
	public void ctorShouldSetProperties() {
		assertThat(scriptEngine.getProperties(), is(sameInstance(properties)));
	}

	@Test
	public void descriptionShouldBeDotNetScriptEngine() {
		assertThat(scriptEngine.getDescription(),
				is(equalTo("GrinderScript.Net Script Engine")));
	}

	@Test
	public void initializeShouldInitalizeUnderlying() throws Exception {
		scriptEngine.initialize(underlying);
		verify(underlying).Initialize();
	}

	@Test
	public void initializeShouldSetUnderlying() throws Exception {
		scriptEngine.initialize(underlying);
		assertThat(scriptEngine.getUnderlying(), is(sameInstance(underlying)));
	}

	@Test
	public void createWorkerRunnableShouldCallUnderlying() throws Exception {
		scriptEngine.initialize(underlying);
		scriptEngine.createWorkerRunnable();
		verify(underlying).CreateWorkerRunnable();
	}

	@Test
	public void createWorkerRunnableShouldReturnWorkerFromUnderlying() throws Exception {
		scriptEngine.initialize(underlying);
		IGrinderWorker expected = mock(IGrinderWorker.class);
		when(underlying.CreateWorkerRunnable()).thenReturn(expected);
		WorkerRunnable wr = scriptEngine.createWorkerRunnable();
		assertThat(wr, is(instanceOf(DotNetWorkerRunnable.class)));
		assertThat(((DotNetWorkerRunnable)wr).getUnderlying(), is(sameInstance(expected)));
	}

	@Test
	public void shutdownShouldClearUnderlying() throws Exception {
		scriptEngine.initialize(underlying);
		scriptEngine.shutdown();
		assertThat(scriptEngine.getUnderlying(), is(nullValue()));
	}

	@Test
	public void shutdownShouldClearUnderlyingAndThrowUnderlyingException()
			throws Exception {
		scriptEngine.initialize(underlying);
		Error expected = new Error();
		doThrow(expected).when(underlying).Shutdown();
		try {
			scriptEngine.shutdown();
			fail("Expected exception");
		} catch (Error e) {
			assertThat(e, is(sameInstance(expected)));
		}
		assertThat(scriptEngine.getUnderlying(), is(nullValue()));
	}

	@Test
	public void shutdownShouldBeReCallable() throws Exception {
		scriptEngine.shutdown();
		scriptEngine.shutdown();
	}

}
