using System;

using GrinderScript.Net.Core;

namespace GrinderScript.Net.Verifier.Samples.GrinderScript.Net.Core
{
    public class WorkerThatThrowsExceptionAtRandom : DefaultWorker
    {
        protected override void DefaultInitialize()
        {
            // A test is just an action, so you can use a lambda expression if you like.
            AddTest(1, "Example test that randomly throws exception",
                () =>
                {
                    bool simulateError = new Random().Next(10) < 2;
                    Logger.Info(m => m("Example test from .Net: Will {0}simulate an error in this run", simulateError ? string.Empty : "not "));
                    if (simulateError)
                    {
                        throw new Exception("Dummy exception from example test");
                    }
                });
        }
    }
}