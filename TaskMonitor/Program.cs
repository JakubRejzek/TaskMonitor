using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace TaskMonitor
{
	internal class Program
	{
		static double terminationTime {  get; set; }
		static DateTime terminationDate { get; set; }
		static double checkTime { get; set; }
		static Process[] monitoredProcesses { get; set; }
		static void Main(string[] args)
		{
			bool correctInput = ValidateSingleLineMonitoring(args);
			if(!correctInput)
				Console.WriteLine("This utility requires 3 arguments { processName (string), termination time (1,1 in minutes), check time (1,1 in minutes)}");
			char key = ' ';
			do
			{
				if (key != ' ')
					Console.Clear();
				Console.WriteLine("Do you wish to correct your arguments [press c], or close this application [press q], or enter Task monitor form for more precise control [press f]?");
				key = Console.ReadKey(true).KeyChar;
			} while (!((key == 'C' || key == 'c') || (key == 'Q' || key == 'q') || (key == 'F' || key == 'f')));
			while (key == 'C' || key == 'c') 
			{
				args = new string[]{"","",""};
                Console.WriteLine("Write task name for monitoring");
				args[0] = Console.ReadLine();
                Console.WriteLine("Write task termination time in format [0-9]*,[0-9] for monitoring");
				args[1] = Console.ReadLine();
				Console.WriteLine("Write task check time in format [0-9]*,[0-9] for monitoring");
				args[2] = Console.ReadLine();
				if (!ValidateSingleLineMonitoring(args))
				{
					Console.WriteLine("To try again press any key. To exit press q");
					if (Console.ReadKey(true).KeyChar == 'q')
						key = 'q';
					Console.Clear();
				}
				else
					key = ' ';
			}
			if(key == 'q' || key == 'Q')
				Environment.Exit(0);
			if (key == 'f' || key == 'F')
			{
                Console.WriteLine("Console will be closed after you finish in TaskMonitor form");
                Application.Run(new TaskMonitor());
				return;	
			}
			MonitorProcess(terminationTime, checkTime);
		}
		static bool ValidateSingleLineMonitoring(string[] args) 
		{
			if (args.Length != 3) {
                Console.WriteLine("Incorrect amount of arguments for monitoring.");
                return false;
			}
			if (!Double.TryParse(args[1], out double terminationTime))
			{
                Console.WriteLine("Incorrect format for double in second argument. 1.0 expected got " + args[1]);
                return false;
			}
				
			if (!Double.TryParse(args[2], out double checkTime))
			{
				Console.WriteLine("Incorrect format for double in third argument. 1.0 expected got " + args[1]);
				return false;
			}
				
			Program.terminationTime = terminationTime;
			Program.checkTime = checkTime;

			return ProcessExists(args[0]);
		}
		static bool ProcessExists(string processName)
		{
			Process[] allProcs = Process.GetProcesses()
				.Where((process) => process.ProcessName == processName)
				.ToArray();
			bool wasFound = allProcs.Length >= 1;
			if (wasFound)
			{
				monitoredProcesses = allProcs;
			}
			else
			{
				Console.WriteLine("Process with this name does not exist");
			}
			return wasFound;
		}
		static void MonitorProcess(double terminationTime, double checktime)
		{
			System.Timers.Timer timer = new System.Timers.Timer(ReturnTimerInterval(checktime));
			terminationDate = DateTime.Now.AddMinutes(terminationTime);
            timer.Enabled = true;
			timer.Elapsed += CheckProcess;
            Console.WriteLine("Stop monitoring by pressing q");
			char key = ' ';
			while (key != 'q')
			{
				key = Console.ReadKey(true).KeyChar;
			}
             
        }
		static int ReturnTimerInterval(double minutes)
		{
			if (minutes == 0)
				return 1000;
			return (int)(minutes * 60 * 1000);
		}
		private static void CheckProcess(object sender, ElapsedEventArgs e)
		{
			if (DateTime.Now > terminationDate)
			{
				System.Timers.Timer timer = (System.Timers.Timer)sender;
				timer.Enabled = false;
				StopProcess();
			}
				
		}
		static void StopProcess()
		{
			foreach (Process process in monitoredProcesses) {
			process.Kill();
			}
			Environment.Exit(0);
		}
	}
}
