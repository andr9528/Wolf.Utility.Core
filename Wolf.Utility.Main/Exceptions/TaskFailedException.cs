using System;
using System.Reflection;
using System.Text;

namespace Wolf.Utility.Main.Exceptions
{
    public class TaskFailedException : BaseException
    {
        public MethodInfo Method { get; }
        public string TaskObjective { get; }

        public TaskFailedException(MethodInfo method, string taskObjective) : base("Task failed to achieve its objective")
        {
            Method = method;
            TaskObjective = taskObjective;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"message => {Message}; ");
            builder.Append($"taskObjective => {TaskObjective};");
            builder.Append($"Method who threw' Name' => {Method.Name};");
            builder.Append($"Method who threw' ReturnType' => {Method.ReturnType};");
            builder.Append($"Stacktrace => {StackTrace};");

            return builder.ToString();
        }
    }
}
