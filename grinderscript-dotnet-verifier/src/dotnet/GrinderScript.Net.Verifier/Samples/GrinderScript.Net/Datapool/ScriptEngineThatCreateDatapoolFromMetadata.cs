using System;
using GrinderScript.Net.Core;
using GrinderScript.Net.Csv;

namespace GrinderScript.Net.Verifier.Samples.GrinderScript.Net.Datapool
{
    public class ScriptEngineThatCreateDatapoolFromMetadata : DefaultScriptEngine
    {
        protected override void OnInitialize()
        {
			Logger.Info("OnInitialize: Will create data pool from meta data. NB! Configuration will NOT be read from the grinder.properties file!");
            const bool IsRandom = true;
            var seed = (int)DateTime.Now.Ticks;
            const DatapoolThreadDistributionMode DistributionMode = DatapoolThreadDistributionMode.ThreadShared;
            const bool IsCircular = true;
            var values = new CsvDatapoolValuesFactory<Credentials>().CreateValues(GrinderContext);
            var metaData = new DefaultDatapoolMetadata<Credentials>(values, IsRandom, seed, DistributionMode, IsCircular);
            DatapoolFactory.CreateDatapool(metaData);
        }
    }
}