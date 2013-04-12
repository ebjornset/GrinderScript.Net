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

import grinderscript.net.core.IGrinderWorker;
import net.grinder.common.UncheckedGrinderException;
import net.grinder.scriptengine.ScriptEngineService.WorkerRunnable;
import net.grinder.scriptengine.ScriptExecutionException;
import net.grinder.util.Sleeper;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class DotNetWorkerRunnable implements WorkerRunnable {

	private final Logger logger = LoggerFactory
			.getLogger(DotNetWorkerRunnable.class);
	
	private IGrinderWorker underlying;

	public DotNetWorkerRunnable(IGrinderWorker underlying)
			throws DotNetScriptExecutionException {
		logger.trace("ctor: Enter, underlying = {}", underlying);
		this.underlying = underlying;
		logger.trace("ctor: Exit");
	}

	@Override
	public void run() throws ScriptExecutionException {
		logger.trace("run: Enter");
		try {
			underlying.Run();
		} catch (UncheckedGrinderException e) {
			if ("Thread has been shut down".equals(e.getMessage())) {
				throw e;
			}
			logger.info("run: Got an UncheckedGrinderException", e);
		} catch (DotNetCheckedExceptionMarshall marshall) {
			if (marshall.getCause() instanceof Sleeper.ShutdownException) {
				throw new DotNetScriptExecutionException(
						"Shutdown during sleep",
						(Sleeper.ShutdownException) marshall.getCause());
			}
		}
		logger.trace("run: Exit");
	}

	@Override
	public void shutdown() throws ScriptExecutionException {
		logger.trace("shutdown: Enter");
		try {
			underlying.Shutdown();
		} finally {
			underlying = null;
		}
		logger.trace("shutdown: Exit");
	}

	IGrinderWorker getUnderlying() {
		return underlying;
	}
}
