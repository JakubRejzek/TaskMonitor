using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskMonitor
{
	public partial class TaskMonitor : Form
	{
		public Process[] allProcs { get; private set; }
		public TaskMonitor()
		{
			InitializeComponent();
		}

		private void TaskMonitor_Load(object sender, EventArgs e)
		{
			allProcs = Process.GetProcesses();
		}
	}
}
