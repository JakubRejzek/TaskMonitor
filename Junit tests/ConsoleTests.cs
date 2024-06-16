using System.Diagnostics;
using Xunit;
using TaskMonitor;
namespace Junit_tests
{
	public class ConsoleTests
	{
		[Fact]
		public void ProcessExists_FindsProcess()
		{
			Process process = Process.Start("http://www.google.com/etc/etc/test.txt");
			//Giving system a moment to start the process so it can always be found
			System.Threading.Thread.Sleep(500);
			bool resuls = TaskMonitor.Program.ProcessExists(process.ProcessName);
			process.Kill();
			Assert.True(resuls, "ProcessExists method of program can not verify existance of started process.");
		}
		[Fact]
		public void ProcessExists_FindsAbsentProcess()
		{
			bool resuls = TaskMonitor.Program.ProcessExists("WOQDOIQUWUIOWDQIOUOUIWDQOIWQDOIOIWDQOUIOIQDWIUODOIOQOOOOOOQWDUQWD");
			Assert.False(resuls, "ProcessExists method of program returns false positives");
		}

		[Fact]
		public void ValidateSingleLineMonitoring_CorrectArguments()
		{
			string processName = "System";
			bool results = TaskMonitor.Program.ValidateSingleLineMonitoring(
				new string[] {processName,"1,0","0,5" },
				out Nullable<DateTime> terminationTime,
				out Nullable<double> checktime);
			Assert.True(results, "Validation was calculated as incorrect params");
			Assert.True(terminationTime.HasValue, "Validation did not return termination time");
			Assert.True(checktime.HasValue, "Validation did not return check time");
			Assert.True(checktime.Value > 0, "Allowed invalid timer tick span");
		}
		[Fact]
		public void ValidateSingleLineMonitoring_LessArguments()
		{
			bool results = TaskMonitor.Program.ValidateSingleLineMonitoring(
				new string[] {},
				out Nullable<DateTime> terminationTime,
				out Nullable<double> checktime);
			Assert.False(results, "Validation returned false positive");
			Assert.False(terminationTime.HasValue, "Validation returned value when not supposed to");
			Assert.False(checktime.HasValue, "Validation returned value when not supposed to");
		}
		[Fact]
		public void ValidateSingleLineMonitoring_MoreArguments()
		{
			string processName = "System";
			bool results = TaskMonitor.Program.ValidateSingleLineMonitoring(
				new string[] { processName, "1,0", "0,5", "Invalid" },
				out Nullable<DateTime> terminationTime,
				out Nullable<double> checktime);
			Assert.False(results, "Validation returned false positive");
			Assert.False(terminationTime.HasValue, "Validation returned termination time");
			Assert.False(checktime.HasValue, "Validation returned check time");
		}
		[Fact]
		public void ValidateSingleLineMonitoring_IncorrectValues()
		{
			string processName = "System";
			bool results = TaskMonitor.Program.ValidateSingleLineMonitoring(
				new string[] { processName, "-1,0", "-0,5" },
				out Nullable<DateTime> terminationTime,
				out Nullable<double> checktime);
			Assert.True(results, "Validation was calculated as incorrect params");
			Assert.True(terminationTime.HasValue, "Validation did not return termination time");
			Assert.True(checktime.HasValue, "Validation did not return check time");
			Assert.True(checktime.Value > 0, "Allowed invalid timer tick span");
		}
		[Fact]
		public void StopProcess_ValidStop()
		{
			Process process = Process.Start("http://www.google.com/etc/etc/test.txt");
			//Giving system a moment to start the process so it can always be found
			System.Threading.Thread.Sleep(500);
			TaskMonitor.SimpleMonitorableProcess simpleMonitorableProcess = new TaskMonitor.SimpleMonitorableProcess(DateTime.Now, 1, process.ProcessName);
			simpleMonitorableProcess.StopProcess();
			Assert.True(process.HasExited, "Stop process did not terminate created process");
			process.Kill();
		}
	}
}