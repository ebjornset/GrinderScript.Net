package net.grinderscript.dotnet.scriptengine;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import system.Exception;
import system.Type;
import grinderscript.net.core.IGrinderLogger;

public class DotNetLoggerBridge implements IGrinderLogger {

	private final Logger logger = LoggerFactory
			.getLogger(DotNetLoggerBridge.class);

	private final Logger underlying;

	public DotNetLoggerBridge(Logger underlyingLogger) {
		logger.trace("ctor: Enter, underlyingLogger = {}", underlyingLogger);
		this.underlying = underlyingLogger;
		logger.trace("ctor: Exit");
	}

	public DotNetLoggerBridge(String name) {
		this(LoggerFactory.getLogger(name));
	}

	public DotNetLoggerBridge(Type nameType) {
		this(nameType.getFullName());
	}

	@Override
	public void Debug(String msg) {
		underlying.debug(msg);
	}

	@Override
	public void Debug(String msg, Exception ex) {
		underlying.debug(msg, ex);
	}

	@Override
	public void Error(String msg) {
		underlying.error(msg);
	}

	@Override
	public void Error(String msg, Exception ex) {
		underlying.error(msg, ex);
	}

	@Override
	public void Info(String msg) {
		underlying.info(msg);
	}

	@Override
	public void Info(String msg, Exception ex) {
		underlying.info(msg, ex);
	}

	@Override
	public boolean isDebugEnabled() {
		return underlying.isDebugEnabled();
	}

	@Override
	public boolean isErrorEnabled() {
		return underlying.isErrorEnabled();
	}

	@Override
	public boolean isInfoEnabled() {
		return underlying.isInfoEnabled();
	}

	@Override
	public boolean isTraceEnabled() {
		return underlying.isTraceEnabled();
	}

	@Override
	public boolean isWarnEnabled() {
		return underlying.isWarnEnabled();
	}

	@Override
	public void Trace(String msg) {
		underlying.trace(msg);
	}

	@Override
	public void Trace(String msg, Exception ex) {
		underlying.trace(msg, ex);
	}

	@Override
	public void Warn(String msg) {
		underlying.warn(msg);
	}

	@Override
	public void Warn(String msg, Exception ex) {
		underlying.warn(msg, ex);
	}

	Logger getUnderlying() {
		return underlying;
	}
}
