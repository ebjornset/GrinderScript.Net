################################################################################################
### grinder.properties for the CsScript example
################################################################################################
### Load variation stuff
################################################################################################

grinder.duration=30000
grinder.runs=0

grinder.sleepTimeFactor=0
grinder.sleepTimeVariation=0.9

grinder.processes=1
grinder.threads=1

grinder.initialProcesses=1
grinder.processIncrement=1

grinder.processIncrementInterval=5000
grinder.initialSleepTime=5000

################################################################################################
## Use GrinderScript.Net.CsScript as the assembly containing the tests. 
## The location is relative to this 'grinder.properties'
################################################################################################
grinder.script=..\\..\\..\\GrinderScript.Net.CsScript.dll

################################################################################################
## Use GrinderScript.Nets CsScriptWorker as the actual test
################################################################################################
grinderscript-dotnet.workerType=GrinderScript.Net.CsScript.CsScriptWorker, GrinderScript.Net.CsScript

################################################################################################
## The actual .cs script file, location is relative to the assembly specified in 'grinder.script'
################################################################################################
grinderscript-dotnet.csScriptWorker.script=.\\Samples\\GrinderScript.Net\\Core\\WorkerThatLogs.cs

################################################################
## The actual script engine type to use. 
################################################################
#grinderscript-dotnet.scriptEngineType=GrinderScript.Net.Core.DefaultScriptEngine, GrinderScript.Net.Core

################################################################
## Location of the folder where additional assemblies (dlls) can be found
################################################################
#grinderscript-dotnet.binFolder=

################################################################################################
### Misc properties
################################################################################################

################################################################
## Set this to true if you want Jni4Net to be verbose during initialization
################################################################
#grinderscript-dotnet.bridgeVerbose=true

################################################################
## Set this to true if you want to attache a .Net debugger to the process.
################################################################
#grinderscript-dotnet.launchDebugger=true

################################################################################################
### JVM arguments passet to the worker processes
################################################################################################
# Her you can control log levels, memory setting etc.
grinder.jvm.arguments=-Dgrinder.logLevel=info -Xms128m -Xmx512m

################################################################################################
### Log levels can be cached in the CLR, to minimize calls to the JVM to read these.
################################################################################################
#grinderscript-dotnet.loggerEnabledCacheTtl=-1

################################################################################################
### Stuff we just need
################################################################################################
grinder.dcrinstrumentation=true
grinder.logDirectory=logs