using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Main.Extensions
{
    public static class TaskExtensions
    {
        public static async Task WaitWhile(this Task task, Func<bool> condition, int frequencyOfCheck = 25,
            int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequencyOfCheck);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        public static async Task WaitUntil(this Task task, Func<bool> condition, int frequencyOfCheck = 25,
            int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequencyOfCheck);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }
    }
}
