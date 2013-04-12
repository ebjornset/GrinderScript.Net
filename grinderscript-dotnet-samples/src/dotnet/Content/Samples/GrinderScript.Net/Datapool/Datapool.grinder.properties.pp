################################################################################################
### grinder.properties for the data pool examples
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

grinder.processIncrementInterval=1000
grinder.initialSleepTime=1000

################################################################################################
### Actual test
################################################################################################
#grinderscript-dotnet.workerType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.WorkerThatFetchDatapoolValueOnce, $rootnamespace$
#grinderscript-dotnet.workerType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.WorkerThatFetchDatapoolValueBeforeEachRun, $rootnamespace$
grinderscript-dotnet.workerType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.WorkerThatFetchDatapoolValueBeforeEachTest, $rootnamespace$

################################################################################################
### Test configuration stuff
################################################################################################

################################################################
## The assembly containing the tests. 
################################################################
grinder.script=..\\..\\..\\$rootnamespace$.dll

################################################################
## The actual script engine type to use. 
################################################################
#grinderscript-dotnet.scriptEngineType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.ScriptEngineThatCreateDatapoolFromMetadata, $rootnamespace$
grinderscript-dotnet.scriptEngineType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.ScriptEngineThatCreateDatapoolFromProperties, $rootnamespace$
#grinderscript-dotnet.scriptEngineType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.ScriptEnginetThatCreateDatapoolFromCsvValuesFactory, $rootnamespace$

################################################################
## Datapool configuration. 
################################################################
grinderscript-dotnet.agentCount=1

grinderscript-dotnet.datapoolFactory.1=Credentials
grinderscript-dotnet.datapool.Credentials.valueType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.Credentials, $rootnamespace$
grinderscript-dotnet.datapool.Credentials.factoryType=GrinderScript.Net.Csv.CsvDatapoolValuesFactory`1, GrinderScript.Net.Csv
grinderscript-dotnet.datapool.Credentials.random=true
grinderscript-dotnet.datapool.Credentials.seed=428653
grinderscript-dotnet.datapool.Credentials.distributionMode=ThreadShared
#grinderscript-dotnet.datapool.Credentials.distributionMode=ThreadUnique
#grinderscript-dotnet.datapool.Credentials.distributionMode=ThreadComplete
grinderscript-dotnet.datapool.Credentials.circular=true
grinderscript-dotnet.datapool.Credentials.csvFile=Samples\\GrinderScript.Net\\Datapool\\Credentials.csv

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
grinder.jvm.arguments=-Dgrinder.logLevel=info -Xms64m -Xmx512m

################################################################################################
### Log levels can be cached in the CLR, to minimize calls to the JVM to read these.
################################################################################################
#grinderscript-dotnet.loggerEnabledCacheTtl=-1

################################################################################################
### Stuff we just need
################################################################################################
grinder.dcrinstrumentation=true
grinder.logDirectory=logs