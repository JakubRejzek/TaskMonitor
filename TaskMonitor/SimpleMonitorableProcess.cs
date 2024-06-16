using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TaskMonitor
{
	public class SimpleMonitorableProcess
	{
		DateTime terminationDate { get; set; }
		double checktime { get; set; }
		string processName { get; set; }
		private Timer timer { get; set; }

		public SimpleMonitorableProcess(DateTime terminationDate, double checktime, string processName)
		{
			this.terminationDate = terminationDate;
			this.checktime = checktime;
			this.processName = processName;
			this.timer = new Timer(checktime*60*1000);
			timer.Elapsed += CheckProcess;
			timer.Enabled = true;
		}
		public void CheckProcess(object sender, ElapsedEventArgs e)
		{
			System.Timers.Timer timer = (System.Timers.Timer)sender;
			if (DateTime.Now > terminationDate)
			{

				timer.Enabled = false;
				StopProcess();
			}
			else
			{
				if (Process.GetProcessesByName(processName).Length>0)
				{
					timer.Enabled = false;
					Console.WriteLine("Process was stopped outside of monitor app.");
					StopProcess();

				}
			}

		}
		public void StopProcess()
		{
			foreach (Process process in Process.GetProcessesByName(processName))
			{
				try
				{
					process.Kill();
				}
				catch (Win32Exception ex)
				{
					if (ex.NativeErrorCode == 5)
						Console.WriteLine("The process requires this app to be ran with administrator/system access for process kill");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Could not kill one of sub-processes, because of unknown exception. Exception: " + ex.Message);
				}

			}
			Environment.Exit(0);
		}

	}
}
