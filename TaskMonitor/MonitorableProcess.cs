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
		public string processName { get; set; }
		public Nullable<DateTime> terminationDate { get; set; } = null;
		public bool monitored { get; set; } = false;
		public bool dead { get; set; } = false;
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
			monitor.lastMonitoredProcess = processName;
			monitor.RefreshTable();
		}
		private void CheckProcess(object sender, ElapsedEventArgs e)
		{
			this.innerProcess = Process.GetProcessesByName(processName);
			if (this.innerProcess.Length < 1)
			{
				timer.Enabled = false;
				dead = true;
				monitor.lockedMonitoring = false;
				MessageBox.Show("Process ended without intervention");
			}
			else if(DateTime.Now >= terminationDate)
			{
				bool withErrors = false;
				foreach (Process process in innerProcess) {
					try
					{
						process.Kill();
					}
					catch(Win32Exception ex)
					{
						withErrors = true;
						if(ex.NativeErrorCode == 5)
							MessageBox.Show("The process requires this app to be ran with administrator/system access for process kill");
					}
					catch (Exception ex) {
						withErrors = true;
						MessageBox.Show("Could not kill one of sub-processes of " + processName +", because of unknown exception. Exceptions: "+ ex.Message);
					}
				}
				dead = !withErrors;
				if (withErrors)
					MessageBox.Show("Some of sub-processes were not killed successfully.");
				else
					MessageBox.Show("Process was killed successfully");
				timer.Enabled = false;
				monitor.lockedMonitoring=false;
			}
		}

		public static BindingList<MonitorableProcess> GetMonitorableProcesses()
		{
			BindingList<MonitorableProcess> result = new BindingList<MonitorableProcess>();
			var ProcessNames = from p in Process.GetProcesses()
							   group p by p.ProcessName into c
							   select c.Key;
			foreach (var processName in ProcessNames)
			{
				result.Add(new MonitorableProcess(Process.GetProcessesByName(processName), processName));
			}
			result = new BindingList<MonitorableProcess>
					(
						(
							from p in result
							orderby p.monitored == true, p.processName ascending
							select p
						)
					.ToList()
					);
			return result;
		}
	}
}
