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
import static org.hamcrest.CoreMatchers.sameInstance;
import static org.junit.Assert.assertThat;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import grinderscript.net.core.IGrinderTest;
import grinderscript.net.core.IGrinderLogger;

import java.io.File;
import java.io.IOException;

import net.grinder.common.GrinderProperties;
import net.grinder.engine.common.ScriptLocation;
import net.grinder.script.Grinder;
import net.grinder.script.Grinder.ScriptContext;
import net.grinder.script.InternalScriptContext;
import net.grinder.script.TestRegistry;
import net.grinder.script.TestRegistry.RegisteredTest;
import net.sf.jni4net.Bridge;

import org.junit.After;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;

import system.Type;

public class DotNetContextBridgeTestCase {

	private DotNetContextBridge bridge;
	private InternalScriptContext internalScriptContext;
	private ScriptContext scriptContext;
	private TestRegistry testRegistry;
	private RegisteredTest registeredTest;
	private GrinderProperties properties;
	private ScriptLocation script;
	private File scriptFile;

	@BeforeClass
	public static void beforeClass() throws IOException {
		Bridge.init();
	}

	@Before
	public void before() throws Exception {
		registeredTest = mock(RegisteredTest.class);
		testRegistry = mock(TestRegistry.class);
		internalScriptContext = mock(InternalScriptContext.class);
		when(testRegistry.register(any(net.grinder.common.Test.class))).thenReturn(registeredTest);
		when(internalScriptContext.getTestRegistry()).thenReturn(testRegistry);
		Grinder.grinder = internalScriptContext;
		scriptContext = mock(ScriptContext.class);
		properties = mock(GrinderProperties.class);
		scriptFile = File.createTempFile("script", "dll");
		scriptFile.deleteOnExit();
		script = new ScriptLocation(scriptFile);
		bridge = new DotNetContextBridge(scriptContext, properties, script);
	}

	@After
	public void after() {
		Grinder.grinder = null;
	}

	@Test
	public void ctorShouldSetScriptContext() {
		assertThat(bridge.getScriptContext(), is(sameInstance(scriptContext)));
	}

	@Test
	public void ctorShouldSetProperties() {
		assertThat(bridge.getProperties(), is(sameInstance(properties)));
	}

	@Test
	public void ctorShouldSetScript() {
		assertThat(bridge.getScript(), is(sameInstance(script)));
	}

	@Test
	public void shouldCreateTestWithCurrentScriptContext() throws Exception {
		final IGrinderTest testAction = mock(IGrinderTest.class);
		IGrinderTest test = bridge.CreateTest(1, "TestDescription", testAction);
		assertThat(test, is(instanceOf(DotNetTestBridge.class)));
		assertThat(((DotNetTestBridge) test).getScriptContext(),
				is(sameInstance(scriptContext)));
	}

	@Test
	public void shouldCreateTestWithTestAction() throws Exception {
		final IGrinderTest testAction = mock(IGrinderTest.class);
		IGrinderTest test = bridge.CreateTest(1, "TestDescription", testAction);
		assertThat(test, is(instanceOf(DotNetTestBridge.class)));
		assertThat(((DotNetTestBridge) test).getTestAction(),
				is(sameInstance(testAction)));
	}

	@Test
	public void shouldCreateLoggerBridgeFromType() throws Exception {
		IGrinderLogger logger = bridge.GetLogger(Type.typeof());
		assertThat(logger, is(instanceOf(DotNetLoggerBridge.class)));
	}

	@Test
	public void shouldGetCanonicalScriptFilePath() throws Exception {
		String scriptFileName = bridge.getScriptFile();
		assertThat(scriptFileName, is(equalTo(scriptFile.getCanonicalPath())));
	}

	@Test
	public void shouldCreateLoggerBridgeFromString() throws Exception {
		IGrinderLogger logger = bridge.GetLogger("TestName");
		assertThat(logger, is(instanceOf(DotNetLoggerBridge.class)));
	}

	@Test
	public void shouldPassSleepMeanTimeToScriptContext() throws Exception {
		bridge.Sleep(123);
		verify(scriptContext).sleep(123);
	}

	@Test
	public void shouldPassSleepMeanTimeSigmaToScriptContext() throws Exception {
		bridge.Sleep(123, 234);
		verify(scriptContext).sleep(123, 234);
	}

	@Test
	public void shouldPassContainsPropertyToScriptProperties() throws Exception {
		bridge.ContainsProperty("TestKey");
		verify(properties).containsKey("TestKey");
	}

	@Test
	public void shouldPassGetPropertyToScriptProperties() throws Exception {
		bridge.GetProperty("TestKey");
		verify(properties).getProperty("TestKey");
	}

	@Test
	public void shouldPassGetPropertyDefaultToScriptProperties()
			throws Exception {
		bridge.GetProperty("TestKey", "DefaultValue");
		verify(properties).getProperty("TestKey", "DefaultValue");
	}

	@Test
	public void shouldPassGetAgentNumberToScriptContext() throws Exception {
		when(scriptContext.getAgentNumber()).thenReturn(234);
		assertThat(bridge.getAgentNumber(), is(equalTo(234)));
		verify(scriptContext).getAgentNumber();
	}

	@Test
	public void shouldPassGetProcessNumberToScriptContext() throws Exception {
		when(scriptContext.getProcessNumber()).thenReturn(345);
		assertThat(bridge.getProcessNumber(), is(equalTo(345)));
		verify(scriptContext).getProcessNumber();
	}

	@Test
	public void shouldPassGetProcessNameToScriptContext() throws Exception {
		when(scriptContext.getProcessName()).thenReturn("TestName");
		assertThat(bridge.getProcessName(), is(equalTo("TestName")));
		verify(scriptContext).getProcessName();
	}

	@Test
	public void shouldPassGetFirstProcessNumberToScriptContext()
			throws Exception {
		when(scriptContext.getFirstProcessNumber()).thenReturn(456);
		assertThat(bridge.getFirstProcessNumber(), is(equalTo(456)));
		verify(scriptContext).getFirstProcessNumber();
	}

	@Test
	public void shouldPassGetThreadNumberToScriptContext() throws Exception {
		when(scriptContext.getThreadNumber()).thenReturn(567);
		assertThat(bridge.getThreadNumber(), is(equalTo(567)));
		verify(scriptContext).getThreadNumber();
	}

	@Test
	public void shouldPassGetRunNumberToScriptContext() throws Exception {
		when(scriptContext.getRunNumber()).thenReturn(678);
		assertThat(bridge.getRunNumber(), is(equalTo(678)));
		verify(scriptContext).getRunNumber();
	}

}
