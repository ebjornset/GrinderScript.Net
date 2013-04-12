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
import static org.hamcrest.CoreMatchers.nullValue;
import static org.hamcrest.CoreMatchers.sameInstance;
import static org.junit.Assert.assertThat;
import static org.mockito.Mockito.doThrow;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import grinderscript.net.core.IGrinderWorker;

import org.junit.Before;
import org.junit.Test;

public class DotNetWorkerRunnableTestCase {

	private DotNetWorkerRunnable runnable;
	private IGrinderWorker underlying;

	@Before
	public void setUp() throws Exception {
		underlying = mock(IGrinderWorker.class);
		runnable = new DotNetWorkerRunnable(underlying);
	}

	@Test
	public void ctorShouldSetWorker() {
		assertThat(runnable.getUnderlying(), is(sameInstance(underlying)));
	}

	@Test
	public void runShouldRunUnderlying() throws Exception {
		runnable.run();
		verify(underlying).Run();
	}

	@Test
	public void shutdownShouldShuthdownUnderlying() throws Exception {
		runnable.shutdown();
		verify(underlying).Shutdown();
	}

	@Test
	public void shutdownShouldClearWorkerEvenWhenAnExceptionIsThrown()
			throws Exception {
		Error expected = new Error();
		doThrow(expected).when(underlying).Shutdown();
		try {
			runnable.shutdown();
		} catch (Error e) {
			assertThat(e, is(sameInstance(expected)));
		}
		assertThat(runnable.getUnderlying(), is(nullValue()));
	}

	@Test(expected = Error.class)
	public void shutdownShouldThrowUnderlyingException() throws Exception {
		doThrow(new Error()).when(underlying).Shutdown();
		runnable.shutdown();
	}
}
