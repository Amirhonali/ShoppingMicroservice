using Serilog;

namespace eCommerce.SharedLibrary.Logs
{
	public static class LogException
	{
		public static void LogExceptions(Exception exception)
		{
			LogToFile(exception.Message);
			LogToConsole(exception.Message);
			LogToDebbuger(exception.Message);
		}

		public static void LogToFile(string message) => Log.Information(message);

        public static void LogToConsole(string message) => Log.Warning(message);

        public static void LogToDebbuger(string message) => Log.Debug(message);
    }
}

