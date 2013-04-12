using System;

using GrinderScript.Net.Core;

namespace GrinderScript.Net.Verifier.Samples.GrinderScript.Net.Core
{
    public class WorkerThatLogs : DefaultWorker
    {
        protected override void DefaultInitialize()
        {
            AddTest(1001, "Logger.TraceMessage", TraceMessage);
            AddTest(1002, "Logger.TraceMessageException", TraceMessageException);
            AddTest(1003, "Logger.TraceMessageHandler", TraceMessageHandler);

            AddTest(2001, "Logger.DebugMessage", DebugMessage);
            AddTest(2002, "Logger.DebugMessageException", DebugMessageException);
            AddTest(2003, "Logger.DebugMessageHandler", DebugMessageHandler);

            AddTest(3001, "Logger.InfoMessage", InfoMessage);
            AddTest(3002, "Logger.InfoMessageException", InfoMessageException);
            AddTest(3003, "Logger.InfoMessageHandler", InfoMessageHandler);

            AddTest(4001, "Logger.WarnMessage", WarnMessage);
            AddTest(4002, "Logger.WarnMessageException", WarnMessageException);
            AddTest(4003, "Logger.WarnMessageHandler", WarnMessageHandler);

            AddTest(5001, "Logger.ErrorMessage", ErrorMessage);
            AddTest(5002, "Logger.ErrorMessageException", ErrorMessageException);
            AddTest(5003, "Logger.ErrorMessageHandler", ErrorMessageHandler);
        }

        private void TraceMessage()
        {
            Logger.Trace("Example trace message");
        }

        private void TraceMessageException()
        {
            Logger.Trace("Example trace message and exception", new Exception("Example trace exception"));
        }

        private void TraceMessageHandler()
        {
            Logger.Trace(m => m("Example trace message, param 1 = '{0}', param 2 = '{1}'", "value 1", "value 2"));
        }

        private void DebugMessage()
        {
            Logger.Debug("Example debug message");
        }

        private void DebugMessageException()
        {
            Logger.Debug("Example debug message and exception", new Exception("Example debug  exception"));
        }

        private void DebugMessageHandler()
        {
            Logger.Debug(m => m("Example debug message, param 1 = '{0}', param 2 = '{1}'", "value 1", "value 2"));
        }

        private void InfoMessage()
        {
            Logger.Info("Example info message");
        }

        private void InfoMessageException()
        {
            Logger.Info("Example info message and exception", new Exception("Example info exception"));
        }

        private void InfoMessageHandler()
        {
            Logger.Info(m => m("Example info message, param 1 = '{0}', param 2 = '{1}'", "value 1", "value 2"));
        }

        private void WarnMessage()
        {
            Logger.Warn("Example warn message");
        }

        private void WarnMessageException()
        {
            Logger.Warn("Example warn message and exception", new Exception("Example warn exception"));
        }

        private void WarnMessageHandler()
        {
            Logger.Warn(m => m("Example warn message, param 1 = '{0}', param 2 = '{1}'", "value 1", "value 2"));
        }

        private void ErrorMessage()
        {
            Logger.Error("Example error message");
        }

        private void ErrorMessageException()
        {
            Logger.Error("Example error message and exception", new Exception("Example error exception"));
        }

        private void ErrorMessageHandler()
        {
            Logger.Error(m => m("Example error message, param 1 = '{0}', param 2 = '{1}'", "value 1", "value 2"));
        }
    }
}