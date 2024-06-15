using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Forms;
using System.ComponentModel;

namespace TaskMonitor
{
	public class MonitorableProcess
	{
		private Process[] innerProcess;
		private System.Timers.Timer timer;
		public string processName;
		public Nullable<DateTime> terminationDate = null;
		public bool monitored = false;
		public bool dead = false;
		TaskMonitor monitor;
		public MonitorableProcess(Process[] innerProcess, string processName)
		{
			this.innerProcess = innerProcess;
			this.processName = processName;
		}
		public void StartMonitoring(DateTime terminationDate, decimal checkInterval, TaskMonitor monitor)
		{
			this.terminationDate = terminationDate;
			timer = new System.Timers.Timer((double)(checkInterval * 60 * 1000));
			timer.Elapsed += CheckProcess;
			timer.Enabled = true;
			this.monitor = monitor;
			this.monitored = true;
			monitor.RefreshTable();
		}
		private void CheckProcess(object sender, ElapsedEventArgs e)
		{
			this.innerProcess = Process.GetProcessesByName(processName);
			if (this.innerProcess.Length < 1)
			{
				timer.Enabled = false;
				dead = true;
				monitor.RefreshTable();
			}
			else if(DateTime.Now >= terminationDate)
			{
				foreach (Process process in innerProcess) {
					try
					{
						process.Kill();
					}
					catch(Win32Exception ex)
					{
						if(ex.NativeErrorCode == 5)
							MessageBox.Show("The process requires this app to be ran with administrator/system access for process kill");
					}
					catch (Exception ex) {
						MessageBox.Show("Could not kill one of sub-processes of " + processName +", because of unknown exception. Exceptions: "+ ex.Message);
					}
				}
				dead = true;
				timer.Enabled = false;
				monitor.RefreshTable();
			}
		}

		public static List<MonitorableProcess> GetMonitorableProcesses()
		{
			List<MonitorableProcess> result = new List<MonitorableProcess>();
			var ProcessNames = from p in Process.GetProcesses()
							   group p by p.ProcessName into c
							   select c.Key;
			foreach (var processName in ProcessNames)
			{
				result.Add(new MonitorableProcess(Process.GetProcessesByName(processName), processName));
			}
			result = (from p in result
					  orderby p.monitored == true, p.processName ascending
					  select p)
						  .ToList();
			return result;
		}
	}
}
