using GrinderScript.Net.Core;

namespace $rootnamespace$.Samples.GrinderScript.Net.Datapool
{
    public class WorkerThatFetchDatapoolValueBeforeEachTest : DefaultWorker
    {
        // A member variable for the datapool
        private IDatapool<Credentials> credentialsDatapool;

        // A member variable for the current credentials
        private Credentials credentials;

        protected override void DefaultInitialize()
        {
            // Initialize the datapool
            credentialsDatapool = DatapoolManager.GetDatapool<Credentials>();

            // Initialize the example tests, passing in the fetch method as a before test action
            AddTest(30001, "WorkerThatFetchDatapoolValueBeforeEachTest-LogOn1", TestLogOn1, FetchNextCredentials);
            AddTest(30002, "WorkerThatFetchDatapoolValueBeforeEachTest-LogOn2", TestLogOn2, FetchNextCredentials);
        }

        private void FetchNextCredentials()
        {
            // Fetch the next credentials from the datapool for each test call
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