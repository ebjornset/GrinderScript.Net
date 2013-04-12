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

import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.CoreMatchers.notNullValue;
import static org.junit.Assert.assertThat;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import java.io.IOException;

import net.sf.jni4net.Bridge;

import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.slf4j.Logger;

import system.Type;

public class DotNetLoggerBridgeTestCase {

	private DotNetLoggerBridge bridge;
	private Logger logger;

	@BeforeClass
	public static void beforeClass() throws IOException {
		Bridge.init();
	}

	@Before
	public void before() throws Exception {
		logger = mock(Logger.class);
		bridge = new DotNetLoggerBridge(logger);
	}
	
	@Test
	public void shouldPassIsTraceEnabledToUnderlyingLogger() {
		when(logger.isTraceEnabled()).thenReturn(true);
		assertThat(bridge.isTraceEnabled(), is(true));
		verify(logger).isTraceEnabled();
	}

	@Test
	public void shouldPassIsDebugEnabledToUnderlyingLogger() {
		when(logger.isDebugEnabled()).thenReturn(true);
		assertThat(bridge.isDebugEnabled(), is(true));
		verify(logger).isDebugEnabled();
	}

	@Test
	public void shouldPassIsInfoEnabledToUnderlyingLogger() {
		when(logger.isInfoEnabled()).thenReturn(true);
		assertThat(bridge.isInfoEnabled(), is(true));
		verify(logger).isInfoEnabled();
	}

	@Test
	public void shouldPassIsWarnEnabledToUnderlyingLogger() {
		when(logger.isWarnEnabled()).thenReturn(true);
		assertThat(bridge.isWarnEnabled(), is(true));
		verify(logger).isWarnEnabled();
	}

	@Test
	public void shouldPassIsErrorEnabledToUnderlyingLogger() {
		when(logger.isErrorEnabled()).thenReturn(true);
		assertThat(bridge.isErrorEnabled(), is(true));
		verify(logger).isErrorEnabled();
	}

	@Test
	public void shouldPassTraceMsgToUnderlyingLogger() {
		bridge.Trace("msg");
		verify(logger).trace("msg");
	}

	@Test
	public void shouldPassTraceMsgAndExceptionToUnderlyingLogger() {
		system.Exception ex = new system.Exception();
		bridge.Trace("msg", ex);
		verify(logger).trace("msg", ex);
	}

	@Test
	public void shouldPassDebugMsgToUnderlyingLogger() {
		bridge.Debug("msg");
		verify(logger).debug("msg");
	}

	@Test
	public void shouldPassDebugMsgAndExceptionToUnderlyingLogger() {
		system.Exception ex = new system.Exception();
		bridge.Debug("msg", ex);
		verify(logger).debug("msg", ex);
	}

	@Test
	public void shouldPassInfoMsgToUnderlyingLogger() {
		bridge.Info("msg");
		verify(logger).info("msg");
	}

	@Test
	public void shouldPassInfoMsgAndExceptionToUnderlyingLogger() {
		system.Exception ex = new system.Exception();
		bridge.Info("msg", ex);
		verify(logger).info("msg", ex);
	}

	@Test
	public void shouldPassWarnMsgToUnderlyingLogger() {
		bridge.Warn("msg");
		verify(logger).warn("msg");
	}

	@Test
	public void shouldPassWarnMsgAndExceptionToUnderlyingLogger() {
		system.Exception ex = new system.Exception();
		bridge.Warn("msg", ex);
		verify(logger).warn("msg", ex);
	}

	@Test
	public void shouldPassErrorMsgToUnderlyingLogger() {
		bridge.Error("msg");
		verify(logger).error("msg");
	}

	@Test
	public void shouldPassErrorMsgAndExceptionToUnderlyingLogger() {
		system.Exception ex = new system.Exception();
		bridge.Error("msg", ex);
		verify(logger).error("msg", ex);
	}

	@Test
	public void shouldContructLoggerFromType() {
		DotNetLoggerBridge bridge = new DotNetLoggerBridge(Type.typeof());
		assertThat(bridge.getUnderlying(), is(notNullValue()));
	}
	
	@Test
	public void shouldContructLoggerFromString() {
		DotNetLoggerBridge bridge = new DotNetLoggerBridge("TestName"); 
		assertThat(bridge.getUnderlying(), is(notNullValue()));
	}
}
