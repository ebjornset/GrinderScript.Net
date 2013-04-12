using GrinderScript.Net.Core;

namespace GrinderScript.Net.Verifier.Samples.GrinderScript.Net.Datapool
{
    public class ScriptEngineThatCreateDatapoolFromProperties : DefaultScriptEngine
    {
        protected override void OnInitialize()
        {
			Logger.Info("OnInitialize: Will create data pool from configuration in the grinder.properties. If any properties are missing the default value will be used");
            DatapoolFactory.CreateDatapool<Credentials>();
        }
    }
}