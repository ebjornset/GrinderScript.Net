using GrinderScript.Net.Core;

namespace $rootnamespace$.Samples.GrinderScript.Net.Datapool
{
    public class WorkerThatFetchDatapoolValueBeforeEachRun : DefaultWorker
    {
        // A member variable for the datapool
        private IDatapool<Credentials> credentialsDatapool;

        // A member variable for the current credentials
        private Credentials credentials;

        protected override void DefaultInitialize()
        {
            // Initialize the datapool
            credentialsDatapool = DatapoolManager.GetDatapool<Credentials>();

            // Initialize the example tests
            AddTest(20001, "WorkerThatFetchDatapoolValueBeforeEachRun-LogOn1", TestLogOn1);
            AddTest(20002, "WorkerThatFetchDatapoolValueBeforeEachRun-LogOn2", TestLogOn2);
        }

        protected override void DefaultBeforeRun()
        {
            // Fetch the next value from the datapool once for each run, it will be reused for all test methods in this run.
            credentials = credentialsDatapool.NextValue();
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