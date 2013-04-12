using GrinderScript.Net.Core;

namespace GrinderScript.Net.Verifier.Samples.GrinderScript.Net.Core
{
    public class ScriptEngineThatCreateWorkerByCtor : AbstractScriptEngine
    {
        protected override IGrinderWorker OnCreateWorkerRunnable()
        {
            Logger.Info("OnCreateWorkerRunnable: Will return new WorkerThatLogs()");
            return new WorkerThatLogs();
        }
    }
}
