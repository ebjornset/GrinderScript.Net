using GrinderScript.Net.Core;

namespace $rootnamespace$.Samples.GrinderScript.Net.Datapool
{
    public class WorkerThatFetchDatapoolValueOnce : DefaultWorker
    {
        // A member variable for the current credentials
        private Credentials credentials;

        protected override void DefaultInitialize()
        {
            // Initialize the datapool
            var credentialsDatapool = DatapoolManager.GetDatapool<Credentials>();

            // Fetch the next value from the datapool once, it will be reused for all testmethods in all runs.
            credentials = credentialsDatapool.NextValue();

            // Initialize the example tests
            AddTest(10001, "WorkerThatFetchDatapoolValueOnce-LogOn1", TestLogOn1);
            AddTest(10002, "WorkerThatFetchDatapoolValueOnce-LogOn2", TestLogOn2);
        }

        private void TestLogOn1()
        {
            // Use the credentials for the first logon here. (We are only logging the user properties in this example.)
            LogMethodAndCredentialsInfo("TestLogOn1", credentials);
        }

        private void TestLogOn2()
        {
            // Use the credentials for the second logon test here. (We are only logging the user properties in this example.)
            LogMethodAndCredentialsInfo("TestLogOn2", credentials);
        }

        private void LogMethodAndCredentialsInfo(string methodName, Credentials logCredentials)
        {
            Logger.Info(m => m("{0}: Credentials.Username = '{1}', Credentials.Password = '{2}'", methodName, logCredentials.Username, logCredentials.Password));
        }
    }
}