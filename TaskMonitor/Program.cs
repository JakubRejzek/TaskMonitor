using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.ComponentModel;

namespace TaskMonitor
{
	public class Program
	{
		public static void Main(string[] args)
		{
			bool correctInput = ValidateSingleLineMonitoring(args,out Nullable<DateTime> terminationDate,out Nullable<double> checkTime);
			if(!correctInput)
			{
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
					args = new string[] { "", "", "" };
					Console.WriteLine("Write task name for monitoring");
					args[0] = Console.ReadLine();
					Console.WriteLine("Write task termination time in format [0-9]*,[0-9] for monitoring");
					args[1] = Console.ReadLine();
					Console.WriteLine("Write task check time in format [0-9]*,[0-9] for monitoring");
					args[2] = Console.ReadLine();
					if (!ValidateSingleLineMonitoring(args, out Nullable<DateTime> terminationDate1, out Nullable<double> checkTime1))
					{
						Console.WriteLine("To try again press any key. To exit press q");
						if (Console.ReadKey(true).KeyChar == 'q')
							key = 'q';
						Console.Clear();
					}
					else
					{
						new SimpleMonitorableProcess(terminationDate1.Value, checkTime1.Value, args[0]);
						Console.WriteLine("Process is being monitored. Press q key to end the app");
						var key1 = ' ';
						do
						{
							key1 = Console.ReadKey(true).KeyChar;
						} while (key1 != 'q');
						key = ' ';
					}
				}
				if (key == 'q' || key == 'Q')
					Environment.Exit(0);
				if (key == 'f' || key == 'F')
				{
					Console.WriteLine("Console will be closed after you finish in TaskMonitor form");
					Application.Run(new TaskMonitor());
					return;
				}
			}
			else
			{
				new SimpleMonitorableProcess(terminationDate.Value, checkTime.Value, args[0]);
                Console.WriteLine("Process is being monitored. Press q key to end the app");
				var key = ' ';
				do
				{
					key = Console.ReadKey(true).KeyChar;
				} while (key != 'q');
            }
			
		}
		public static bool ValidateSingleLineMonitoring(string[] args, out Nullable<DateTime> terminationTime, out Nullable<double> checktime) 
		{
			terminationTime = null;
			checktime = null;
			if (args.Length != 3) {
                Console.WriteLine("Incorrect amount of arguments for monitoring.");
                return false;
			}
			if (!Double.TryParse(args[1], out double terminationTimeC))
			{
                Console.WriteLine("Incorrect format for double in second argument. 1.0 expected got " + args[1]);
                return false;
			}
				
			if (!Double.TryParse(args[2], out double checkTimeC))
			{
				Console.WriteLine("Incorrect format for double in third argument. 1.0 expected got " + args[1]);
				return false;
			}
			if (checkTimeC < 0.05)
				checkTimeC = 0.05;
			terminationTime = DateTime.Now.AddMinutes(terminationTimeC);
			checktime = checkTimeC;
			return ProcessExists(args[0]);
		}
		public static bool ProcessExists(string processName)
		{
			Process[] allProcs = Process.GetProcessesByName(processName);
			bool wasFound = allProcs.Length >= 1;
			if (!wasFound)
			{
				Console.WriteLine("Process with this name does not exist");
			}
			return wasFound;
		}
	}
}
