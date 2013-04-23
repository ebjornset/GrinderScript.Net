################################################################################################
### grinder.properties for the scenario example
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
### Uses the scenario worker as the actual test
################################################################################################
grinderscript-dotnet.workerType=GrinderScript.Net.Core.Framework.ScenarioWorker, GrinderScript.Net.Core

################################################################################################
### Test configuration stuff
################################################################################################

################################################################
## The assembly containing the tests. 
################################################################
grinder.script=..\\..\\..\\$rootnamespace$.dll

################################################################
## Uses the default script engine.
################################################################

################################################################
## Scenario configuration. 
################################################################
grinderscript-dotnet.scenarioWorker.1.workerType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.WorkerThatFetchDatapoolValueOnce, $rootnamespace$
grinderscript-dotnet.scenarioWorker.1.loadFactor=1

grinderscript-dotnet.scenarioWorker.2.workerType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.WorkerThatFetchDatapoolValueBeforeEachRun, $rootnamespace$
grinderscript-dotnet.scenarioWorker.2.loadFactor=3

grinderscript-dotnet.scenarioWorker.3.workerType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.WorkerThatFetchDatapoolValueBeforeEachTest, $rootnamespace$
grinderscript-dotnet.scenarioWorker.3.loadFactor=5

grinderscript-dotnet.scenarioWorker.random=true
grinderscript-dotnet.scenarioWorker.seed=654321

################################################################
## Datapool configuration. 
################################################################
grinderscript-dotnet.agentCount=1

grinderscript-dotnet.datapoolFactory.1=Credentials
grinderscript-dotnet.datapool.Credentials.valueType=$rootnamespace$.Samples.GrinderScript.Net.Datapool.Credentials, $rootnamespace$
grinderscript-dotnet.datapool.Credentials.factoryType=GrinderScript.Net.Csv.CsvDatapoolValuesFactory`1, GrinderScript.Net.Csv
grinderscript-dotnet.datapool.Credentials.random=true
grinderscript-dotnet.datapool.Credentials.seed=428653
#grinderscript-dotnet.datapool.Credentials.distributionMode=ThreadShared
#grinderscript-dotnet.datapool.Credentials.distributionMode=ThreadUnique
grinderscript-dotnet.datapool.Credentials.distributionMode=ThreadComplete
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