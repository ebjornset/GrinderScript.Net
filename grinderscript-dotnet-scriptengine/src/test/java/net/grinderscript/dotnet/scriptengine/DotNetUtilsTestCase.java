package net.grinderscript.dotnet.scriptengine;

import static org.junit.Assert.*;
import static org.hamcrest.CoreMatchers.*;

import java.io.File;
import java.util.Properties;

import org.junit.Test;

public class DotNetUtilsTestCase {

	@Test
	public void shouldPrefixPropertyWithGrinderscriptHypenDotnetPeriod() {
		assertThat(DotNetUtils.getPropertyName("test"),
				is(equalTo("grinderscript-dotnet.test")));
	}

	@Test
	public void shouldFindPropertiesFromResources() throws Exception {
		assertThat(DotNetUtils.getPropertiesFromResources(), is(notNullValue()));
	}

	@Test
	public void shouldFindJ4nLocationInPropertiesFromResources()
			throws Exception {
		assertThat(DotNetUtils.getProperty(
				DotNetUtils.getPropertiesFromResources(),
				DotNetUtils.J4N_LOCATION_PROPERTY_SUFFIX), is(notNullValue()));
	}

	@Test
	public void shouldFindJ4nAssemblyInPropertiesFromResources()
			throws Exception {
		assertThat(DotNetUtils.getProperty(
				DotNetUtils.getPropertiesFromResources(),
				DotNetUtils.J4N_ASSEMBLY_PROPERTY_SUFFIX), is(notNullValue()));
	}

	@Test
	public void shouldFindScriptAssemblyInPropertiesFromResources()
			throws Exception {
		assertThat(DotNetUtils.containsPropertyKey(
				DotNetUtils.getPropertiesFromResources(),
				DotNetUtils.SCRIPT_ASSEMBLY_PROPERTY_SUFFIX), is(true));
	}

	@Test
	public void shouldFindJni4NetAssemblyInPropertiesFromResources()
			throws Exception {
		assertThat(DotNetUtils.getProperty(
				DotNetUtils.getPropertiesFromResources(),
				DotNetUtils.JNI4NET_ASSEMBLY_PROPERTY_SUFFIX), is(notNullValue()));
	}

	@Test
	public void j4nLocationInPropertiesFromResourcesShouldBeADirectory()
			throws Exception {
		final Properties properties = DotNetUtils.getPropertiesFromResources();
		assertThat(
				String.format("Not a directory: \"%1$s\"", DotNetUtils
						.getProperty(properties,
								DotNetUtils.J4N_LOCATION_PROPERTY_SUFFIX)),
				new File(DotNetUtils.getProperty(properties,
						DotNetUtils.J4N_LOCATION_PROPERTY_SUFFIX))
						.isDirectory(), is(true));
	}

	@Test
	public void j4nAssemblyShouldExistInJ4nLocationInPropertiesFromResources()
			throws Exception {
		final Properties properties = DotNetUtils.getPropertiesFromResources();
		assertThat(
				String.format("Not a file: \"%1$s\"", DotNetUtils
						.getJ4nAssemblyLocationFromProperties(properties)),
				new File(DotNetUtils
						.getJ4nAssemblyLocationFromProperties(properties))
						.isFile(), is(true));
	}

	@Test
	public void scriptAssemblyShouldExistInJ4nLocationInPropertiesFromResources()
			throws Exception {
		final Properties properties = DotNetUtils.getPropertiesFromResources();
		assertThat(
				String.format("Not a file: \"%1$s\"", DotNetUtils
						.getScriptAssemblyLocationFromProperties(properties)),
				new File(DotNetUtils
						.getScriptAssemblyLocationFromProperties(properties))
						.isFile(), is(true));
	}

	@Test
	public void jni4NetAssemblyShouldExistInJ4nLocationInPropertiesFromResources()
			throws Exception {
		final Properties properties = DotNetUtils.getPropertiesFromResources();
		assertThat(
				String.format("Not a file: \"%1$s\"", DotNetUtils
						.getJni4NetAssemblyLocationFromProperties(properties)),
				new File(DotNetUtils
						.getJni4NetAssemblyLocationFromProperties(properties))
						.isFile(), is(true));
	}

	@Test
	public void directoryFromJ4nCodeLocationShouldBeADirectory()
			throws Exception {
		final String directoryFromCodeLocation = DotNetUtils
				.getDirectoryFromJ4nCodeLocation();
		assertThat(String.format("Not a directory: \"%1$s\"",
				directoryFromCodeLocation),
				new File(directoryFromCodeLocation).isDirectory(), is(true));
	}

	@Test
	public void j4nJarShouldExistInDirectoryFromJ4nCodeLocation()
			throws Exception {
		String j4nAssemblyName = DotNetUtils.getProperty(
				DotNetUtils.getPropertiesFromResources(),
				DotNetUtils.J4N_JAR_PROPERTY_SUFFIX);
		assertThat(
				String.format("Not a file in j4nCodeLocation: \"%1$s\"",
						j4nAssemblyName),
				new File(DotNetUtils
						.getFileNameFromJ4nCodeLocation(j4nAssemblyName))
						.isFile(), is(true));
	}

}
