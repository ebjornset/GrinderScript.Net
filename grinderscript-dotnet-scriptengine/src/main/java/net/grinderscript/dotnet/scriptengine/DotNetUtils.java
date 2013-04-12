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

import grinderscript.net.core.IGrinderContext;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.util.List;
import java.util.Properties;

import net.grinder.util.ClassLoaderUtilities;

public abstract class DotNetUtils {

	static final String J4N_PROPERTIES_RESOURCE_NAME = "META-INF/grinderscript-dotnet.j4n.properties";
	static final String PROPERTY_PREFIX = "grinderscript-dotnet.";
	static final String J4N_LOCATION_PROPERTY_SUFFIX = "j4nLocation";
	static final String SCRIPT_ASSEMBLY_PROPERTY_SUFFIX = "scriptAssembly";
	static final String J4N_ASSEMBLY_PROPERTY_SUFFIX = "j4nAssembly";
	static final String J4N_JAR_PROPERTY_SUFFIX = "j4nJar";
	static final String JNI4NET_ASSEMBLY_PROPERTY_SUFFIX = "jni4NetAssembly";

	static Properties getPropertiesFromResources(String resourceName,
			ClassLoader classLoader) throws IOException {
		Properties properties = new Properties();
		List<String> propertyStrings = ClassLoaderUtilities.allResourceLines(
				classLoader, resourceName);
		for (String propertyString : propertyStrings) {
			String[] propertyContent = propertyString.split("=");
			if (propertyContent.length != 2) {
				throw new IllegalArgumentException(String.format(
						"Not a property spesification: \"%1$s\"", propertyString));
			}
			if (!properties.containsKey(propertyContent[0].trim())) {
				properties.put(propertyContent[0].trim(),
						propertyContent[1].trim());
			}
		}
		return properties;
	}

	static Properties getPropertiesFromResources(String resourceName)
			throws IOException {
		return getPropertiesFromResources(resourceName,
				DotNetUtils.class.getClassLoader());
	}

	static Properties getPropertiesFromResources() throws IOException {
		return getPropertiesFromResources(J4N_PROPERTIES_RESOURCE_NAME);
	}

	static String getJ4nAssemblyLocationFromProperties(Properties properties)
			throws IOException {
		return getCanonicalLocationFilePath(DotNetUtils.getProperty(properties,
				J4N_LOCATION_PROPERTY_SUFFIX), DotNetUtils.getProperty(
				properties, J4N_ASSEMBLY_PROPERTY_SUFFIX));
	}

	static String getScriptAssemblyLocationFromProperties(Properties properties)
			throws IOException {
		return getCanonicalLocationFilePath(DotNetUtils.getProperty(properties,
				J4N_LOCATION_PROPERTY_SUFFIX), DotNetUtils.getProperty(
				properties, SCRIPT_ASSEMBLY_PROPERTY_SUFFIX));
	}

	static String getJni4NetAssemblyLocationFromProperties(Properties properties)
			throws IOException {
		return getCanonicalLocationFilePath(DotNetUtils.getProperty(properties,
				J4N_LOCATION_PROPERTY_SUFFIX), DotNetUtils.getProperty(
				properties, JNI4NET_ASSEMBLY_PROPERTY_SUFFIX));
	}

	static String getCanonicalLocationFilePath(String location,
			String file) throws IOException {
		return new File(location, file).getCanonicalPath();
	}

	static String getDirectoryFromJ4nCodeLocation() throws IOException {
		URL codeLocationUrl = IGrinderContext.class.getProtectionDomain()
				.getCodeSource().getLocation();
		String codeLocation = codeLocationUrl.getFile();
		File file = new File(codeLocation);
		while (!file.isDirectory()) {
			file = file.getParentFile();
		}
		codeLocation = file.getCanonicalPath();
		return codeLocation;
	}

	static String getFileNameFromJ4nCodeLocation(String baseName)
			throws DotNetScriptExecutionException, IOException {
		String codeLocation = getDirectoryFromJ4nCodeLocation();
		File file = new File(codeLocation, baseName);
		if (file.exists()) {
			return file.getCanonicalPath();
		}
		throw new DotNetScriptExecutionException(String.format(
				"Could not find file \"%1$s\" in code loction \"%2$s\"", baseName,
				codeLocation));
	}

	static String getProperty(Properties properties, String suffix) {
		return properties.getProperty(getPropertyName(suffix));
	}

	static boolean containsPropertyKey(Properties properties, String suffix) {
		return properties.containsKey(getPropertyName(suffix));
	}

	static String getPropertyName(String suffix) {
		return PROPERTY_PREFIX + suffix;
	}
}
