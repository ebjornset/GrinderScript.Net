using GrinderScript.Net.Core;
using GrinderScript.Net.Csv;

namespace GrinderScript.Net.Verifier.Samples.GrinderScript.Net.Datapool
{
    public class ScriptEnginetThatCreateDatapoolFromCsvValuesFactory : DefaultScriptEngine
    {
        protected override void OnInitialize()
        {
            Logger.Info("OnInitialize: Will create datapool from cvs values factory. Configuration will be read from the grinder.properties");
            DatapoolFactory.CreateDatapool(new CsvDatapoolValuesFactory<Credentials>());
        }
    }
}