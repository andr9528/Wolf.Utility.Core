using System;
using System.Threading.Tasks;
using Wolf.Utility.Main.Logging.Enum;

namespace Wolf.Utility.Main.Extensions.Methods
{
    public static class TaskExtensions
    {
        public delegate string LogMessageDelegate();

        public static async Task WaitWhile(this Task task, Func<bool> condition, int frequencyOfCheck = 25,
            int timeout = -1, LogMessageDelegate logMessage = null, bool shouldLogInLoop = false)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition())
                {
                    if (logMessage != null && shouldLogInLoop)
                        Logging.Logging.Log(LogType.Await, logMessage.Invoke());
                    await Task.Delay(frequencyOfCheck);
                }
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        public static async Task WaitUntil(this Task task, Func<bool> condition, int frequencyOfCheck = 25,
            int timeout = -1, LogMessageDelegate logMessage = null, bool shouldLogInLoop = false)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition())
                {
                    if (logMessage != null && shouldLogInLoop)
                        Logging.Logging.Log(LogType.Await, logMessage.Invoke());
                    await Task.Delay(frequencyOfCheck);
                }
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }
    }
}
